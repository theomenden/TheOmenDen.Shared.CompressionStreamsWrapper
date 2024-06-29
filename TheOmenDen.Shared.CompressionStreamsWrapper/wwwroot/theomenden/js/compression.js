/**
 * Wrapper method for the CompressionStream API's compress method.
 * @param data The data to compress.
 * @param algorithm The compression algorithm to use.
 * @returns A promise that resolves to the compressed data.
 */
export async function compress(data, algorithm = 'gzip') {
    const encoder = new TextEncoder();
    const stream = new CompressionStream(algorithm);
    const writer = stream.writable.getWriter();
    const encoded = encoder.encode(data);
    await writer.write(encoded);
    await writer.close();
    const reader = stream.readable.getReader();
    let result = new Uint8Array(0);
    let done = false, value;
    while (!done) {
        const readResult = await reader.read();
        done = readResult.done;
        value = readResult.value;
        if (value) {
            const newResult = new Uint8Array(result.length + value.length);
            newResult.set(result);
            newResult.set(value, result.length);
            result = newResult;
        }
    }
    return result;
}
/**
 * Wrapper method for the DecompressionStream API's decompress method.
 * @param data The data to decompress.
 * @param algorithm The compression algorithm to use.
 * @returns A promise that resolves to the decompressed data.
 */
export async function decompress(data, algorithm = 'gzip') {
    const stream = new DecompressionStream(algorithm);
    const writer = stream.writable.getWriter();
    await writer.write(data);
    await writer.close();
    const reader = stream.readable.getReader();
    let result = '';
    let done, value;
    while (true) {
        const readResult = await reader.read();
        done = readResult.done;
        value = readResult.value;
        if (done)
            break;
        if (value) {
            const decoder = new TextDecoder();
            result += decoder.decode(value, { stream: true });
        }
    }
    return result;
}
