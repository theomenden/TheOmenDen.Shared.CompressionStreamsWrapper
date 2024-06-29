using Microsoft.Extensions.Logging;
using Microsoft.IO;
using Microsoft.JSInterop;
using System.Buffers;
using System.Text.Json;

namespace TheOmenDen.Shared.CompressionStreamsWrapper.Services;
///<summary>
///<para>
///A service that provides compression and decompression of data utilizing JavaScript's built-in compression algorithms.
///</para>
///<list type="bullet">
///<item>
///<term>Javascript Interop</term>
///<description>We make use of the <see cref="IJSRuntime"/> to interact with the JavaScript module that provides the compression and decompression functionality.</description>
///</item>
///<item>
///<term>Memory Streams</term>
///<description>We also make use of the <see cref="RecyclableMemoryStreamManager"/> to manage the memory streams that we use to serialize and deserialize the data.</description>
///</item>
///<item>
///<term>Array Pooling</term>
///<description>We also make use of the <see cref="ArrayPool{T}"/> to manage the byte arrays that we use to store the compressed and decompressed data.</description>
///</item>
///<item>
///<term>Compression Algorithms</term>
///<description>
///The service supports the following compression algorithms:
///<list type="table">
///<listheader>
///<term>Algorithm</term>
///<description>Read More</description>
///</listheader>
///<item>
///<term>gzip</term>
///<description>Read more about the <see href="https://www.rfc-editor.org/rfc/rfc1952">GZip algorithm</see></description>
///</item>
///<item>
///<term>deflate</term>
///<description>Read more about the <see href="https://www.rfc-editor.org/rfc/rfc1950">Deflate algorithm</see></description>
///</item>
///<item>
///<term>deflate-raw</term>
///<description>Read more about the <see href="https://www.rfc-editor.org/rfc/rfc1951">deflate-raw algorithm</see></description>
///</item>
///</list>
///</description>
///</item>
///</list>
///<inheritdoc />
///</summary>
///<remarks>You can learn more about the <see href="https://developer.mozilla.org/en-US/docs/Web/API/Compression_Streams_API">CompressionStreams API here</see></remarks>
public interface ICompressionService : IAsyncDisposable
{
    /// <summary>
    /// Responsible for initializing the <see cref="ICompressionService"/>'s <see cref="IJSObjectReference"/> to the JavaScript module
    /// </summary>
    /// <param name="cancellationToken">Provides <see cref="CancellationToken"/> support</param>
    /// <returns>A completed <see cref="Task"/></returns>
    Task InitializeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Compresses the <paramref name="data"/> using the specified <paramref name="algorithm"/>
    /// </summary>
    /// <typeparam name="T">The type we want to apply the <paramref name="algorithm"/> over</typeparam>
    /// <param name="data">The underlying data</param>
    /// <param name="algorithm">The supplied supported algorithm, defaults to <c>gzip</c></param>
    /// <param name="cancellationToken">Provides <see cref="CancellationToken"/> support</param>
    /// <returns>The compressed data as an <see cref="Array"/> of <see cref="byte"/></returns>
    Task<byte[]> CompressAsync<T>(T data, string algorithm = "gzip", CancellationToken cancellationToken = default);

    /// <summary>
    /// Decompresses the <paramref name="data"/> using the specified <paramref name="algorithm"/>
    /// </summary>
    /// <typeparam name="T">The type we want to deserialize to</typeparam>
    /// <param name="data">The compressed data that we want to decompress</param>
    /// <param name="algorithm">The supplied supported algorithm, defaults to <c>gzip</c></param>
    /// <param name="cancellationToken">Provides <see cref="CancellationToken"/> support</param>
    /// <returns>The deserialized <typeparamref name="T"/> using the <paramref name="algorithm"/></returns>
    /// <remarks>It is recommended to use the same <see cref="algorithm"/> as you did when you applied the <see cref="CompressAsync{T}"/> method</remarks>
    Task<T> DecompressAsync<T>(byte[] data, string algorithm = "gzip", CancellationToken cancellationToken = default);
}

/// <summary>
/// <inheritdoc cref="ICompressionService"/>
/// </summary>
/// <param name="jsRuntime">An injected flavor of <see cref="IJSRuntime"/> or <seealso cref="IJSInProcessRuntime"/></param>
/// <param name="logger">An injected <see cref="Microsoft.Extensions.Logging.ILogger{T}"/></param>
/// <param name="memoryStreamManager">An injected <see cref="RecyclableMemoryStreamManager"/> (preferably already registered as a <see cref="Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton"/>)</param>
internal sealed class CompressionService(
    IJSRuntime jsRuntime,
    ILogger<CompressionService> logger,
    RecyclableMemoryStreamManager memoryStreamManager)
    : ICompressionService
{
    private readonly ArrayPool<byte> _arrayPool = ArrayPool<byte>.Shared;
    private IJSObjectReference _jsModule;

    /// <inheritdoc />
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _jsModule = await jsRuntime.InvokeAsync<IJSObjectReference>("import", cancellationToken, "./compression.js").ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<byte[]> CompressAsync<T>(T data, string algorithm = "gzip", CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Compressing data of type {Type} using {Algorithm}", typeof(T).Name, algorithm);

        try
        {
            await using var memoryStream = memoryStreamManager.GetStream();
            await JsonSerializer.SerializeAsync(memoryStream, data, JsonSerializerOptions.Default, cancellationToken).ConfigureAwait(false);
            var jsonBytes = memoryStream.ToArray();

            return jsRuntime is IJSInProcessRuntime inProcessRuntime
                ? inProcessRuntime.Invoke<byte[]>("compressionInterop.compress", jsonBytes, algorithm)
                : await _jsModule.InvokeAsync<byte[]>("compress", cancellationToken, jsonBytes, algorithm)
                    .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error compressing data of type {Type}", typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<T> DecompressAsync<T>(byte[] data, string algorithm = "gzip", CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Decompressing data to {Type} using {Algorithm}", typeof(T).Name, algorithm);

        var buffer = _arrayPool.Rent(data.Length);
        try
        {
            Array.Copy(data, buffer, data.Length);
            byte[]? decompressedBytes;
            RecyclableMemoryStream? memoryStream;
            if (jsRuntime is IJSInProcessRuntime inProcessRuntime)
            {
                decompressedBytes = inProcessRuntime.Invoke<byte[]>("compressionInterop.decompress", buffer, algorithm);
                memoryStream = memoryStreamManager.GetStream(decompressedBytes);
                return await JsonSerializer.DeserializeAsync<T>(memoryStream, new JsonSerializerOptions(), cancellationToken).ConfigureAwait(false);
            }

            decompressedBytes = await _jsModule.InvokeAsync<byte[]>("decompress", cancellationToken, buffer, algorithm).ConfigureAwait(false);
            memoryStream = memoryStreamManager.GetStream(decompressedBytes);
            return await JsonSerializer.DeserializeAsync<T>(memoryStream, new JsonSerializerOptions(), cancellationToken).ConfigureAwait(false);

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error decompressing data to {Type}", typeof(T).Name);
            throw;
        }
        finally
        {
            _arrayPool.Return(buffer);
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_jsModule is not null)
        {
            await _jsModule.DisposeAsync().ConfigureAwait(false);
        }
    }
}
