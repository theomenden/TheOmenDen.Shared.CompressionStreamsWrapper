using Microsoft.Extensions.DependencyInjection;
using Microsoft.IO;
using TheOmenDen.Shared.CompressionStreamsWrapper.Services;

namespace TheOmenDen.Shared.CompressionStreamsWrapper;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// <para>Adds <see cref="CompressionService" /> as a Singleton lifetime service to the <see cref="IServiceCollection"/></para>
    /// <para>Also adds a default <see cref="RecyclableMemoryStreamManager"/></para>
    /// </summary>
    /// <param name="services">The collection of service descriptors</param>
    /// <returns>The provided <see cref="IServiceCollection"/> for further chaining</returns>
    public static IServiceCollection AddSingletonCompressionService(this IServiceCollection services)
    {
        services.AddSingleton<ICompressionService, CompressionService>();
        services.AddSingleton<RecyclableMemoryStreamManager>();
        return services;
    }

    /// <summary>
    /// <para>Adds <see cref="CompressionService" /> as a Singleton lifetime service to the <see cref="IServiceCollection"/></para>
    /// <para>Adds your self-defined <see cref="RecyclableMemoryStreamManager"/> to the <see cref="IServiceCollection"/></para>
    /// </summary>
    /// <param name="services">The collection of service descriptors</param>
    /// <param name="recyclableMemoryStreamManager">Your self-defined <see cref="RecyclableMemoryStreamManager"/></param>
    /// <returns>The provided <see cref="IServiceCollection"/> for further chaining</returns>
    public static IServiceCollection AddSingletonCompressionService(this IServiceCollection services, RecyclableMemoryStreamManager recyclableMemoryStreamManager)
    {
        services.AddSingleton<ICompressionService, CompressionService>();
        services.AddSingleton(recyclableMemoryStreamManager);
        return services;
    }

    /// <summary>
    /// <para>Adds <see cref="CompressionService" /> as a Singleton lifetime service to the <see cref="IServiceCollection"/></para>
    /// <para>Allows you to configureRecyclableMemoryStreamManagerOptions the <see cref="RecyclableMemoryStreamManager"/> we're attempting to register with the provided <paramref name="configureRecyclableMemoryStreamManagerOptions"/></para>
    /// </summary>
    /// <param name="services">The collection of service descriptors</param>
    /// <param name="configureRecyclableMemoryStreamManagerOptions">The options delegate allowing you to configure the <see cref="RecyclableMemoryStreamManager"/></param>
    /// <returns></returns>
    public static IServiceCollection AddSingletonCompressionService(this IServiceCollection services, Action<RecyclableMemoryStreamManager.Options> configureRecyclableMemoryStreamManagerOptions)
    {
        var recyclableMemoryStreamManager = new RecyclableMemoryStreamManager.Options();
        configureRecyclableMemoryStreamManagerOptions(recyclableMemoryStreamManager);
        services.AddSingleton<ICompressionService, CompressionService>();
        services.AddSingleton(recyclableMemoryStreamManager);
        return services;
    }

    /// <summary>
    /// Adds <see cref="CompressionService" /> as a Singleton lifetime service to the <see cref="IServiceCollection"/>
    /// </summary>
    /// <param name="services">The collection of service descriptors</param>
    /// <returns>The provided <see cref="IServiceCollection"/> for further chaining</returns>
    /// <remarks>This does NOT register a <see cref="RecyclableMemoryStreamManager"/> and will require you to register it yourself</remarks>
    public static IServiceCollection AddSingletonCompressionServiceWithoutRecyclableStreamManager(this IServiceCollection services)
    {
        services.AddSingleton<ICompressionService, CompressionService>();
        return services;
    }

    /// <summary>
    /// <para>Registers the <see cref="CompressionService" /> as a Scoped lifetime service to the <see cref="IServiceCollection"/></para>
    /// <para>Also registers <see cref="RecyclableMemoryStreamManager"/> with default configuration as a <see cref="ServiceLifetime.Singleton"/></para>
    /// </summary>
    /// <param name="services">The collection of service descriptors</param>
    /// <returns>The provided <see cref="IServiceCollection"/> for further chaining</returns>
    public static IServiceCollection AddScopedCompressionService(this IServiceCollection services)
    {
        services.AddScoped<ICompressionService, CompressionService>();
        services.AddSingleton<RecyclableMemoryStreamManager>();
        return services;
    }

    /// <summary>
    /// <para>Registers the <see cref="CompressionService"/> as a Scoped lifetime service to the <see cref="IServiceCollection"/></para>
    /// <para>Also registers your provided <see cref="RecyclableMemoryStreamManager"/> implementation as a <see cref="ServiceLifetime.Singleton"/></para>
    /// </summary>
    /// <param name="services">The collection of service descriptors</param>
    /// <param name="recyclableMemoryStreamManager">Your provided <see cref="RecyclableMemoryStreamManager"/></param>
    /// <returns>The provided <see cref="IServiceCollection"/> for further chaining</returns>
    public static IServiceCollection AddScopedCompressionService(this IServiceCollection services, RecyclableMemoryStreamManager recyclableMemoryStreamManager)
    {
        services.AddScoped<ICompressionService, CompressionService>();
        services.AddSingleton(recyclableMemoryStreamManager);
        return services;
    }

    /// <summary>
    /// <para>Registers the <see cref="CompressionService"/> as a Scoped lifetime service to the <see cref="IServiceCollection"/></para>
    /// <para>Also will register a <see cref="RecyclableMemoryStreamManager"/> with the provided <paramref name="configureRecyclableMemoryStreamManagerOptions"/> as a <see cref="ServiceLifetime.Singleton"/></para>
    /// </summary>
    /// <param name="services">The collection of service descriptors</param>
    /// <param name="configureRecyclableMemoryStreamManagerOptions">A configuration delegate for <see cref="RecyclableMemoryStreamManager"/></param>
    /// <returns>The provided <see cref="IServiceCollection"/> for further chaining</returns>
    public static IServiceCollection AddScopedCompressionService(this IServiceCollection services, Action<RecyclableMemoryStreamManager.Options> configureRecyclableMemoryStreamManagerOptions)
    {
        var recyclableMemoryStreamManager = new RecyclableMemoryStreamManager.Options();
        configureRecyclableMemoryStreamManagerOptions(recyclableMemoryStreamManager);
        services.AddScoped<ICompressionService, CompressionService>();
        services.AddSingleton(recyclableMemoryStreamManager);
        return services;
    }

    /// <summary>
    /// Registers the <see cref="CompressionService"/> as a Scoped lifetime service to the <see cref="IServiceCollection"/>
    /// </summary>
    /// <param name="services">The collection of service descriptors</param>
    /// <returns>The provided <see cref="IServiceCollection"/> for further chaining</returns>
    /// <remarks>This method will NOT register a <see cref="RecyclableMemoryStreamManager"/> and you will be responsible for registering it in your DI container</remarks>
    public static IServiceCollection AddScopedCompressionServiceWithoutRecyclableStreamManager(this IServiceCollection services)
    {
        services.AddScoped<ICompressionService, CompressionService>();
        return services;
    }
}
