var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
/**
 * Wrapper method for the CompressionStream API's compress method.
 * @param data The data to compress.
 * @param algorithm The compression algorithm to use.
 * @returns A promise that resolves to the compressed data.
 */
export function compress(data_1) {
    return __awaiter(this, arguments, void 0, function* (data, algorithm = 'gzip') {
        const encoder = new TextEncoder();
        const stream = new CompressionStream(algorithm);
        const writer = stream.writable.getWriter();
        const encoded = encoder.encode(data);
        yield writer.write(encoded);
        yield writer.close();
        const reader = stream.readable.getReader();
        let result = new Uint8Array(0);
        let done = false, value;
        while (!done) {
            const readResult = yield reader.read();
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
    });
}
/**
 * Wrapper method for the DecompressionStream API's decompress method.
 * @param data The data to decompress.
 * @param algorithm The compression algorithm to use.
 * @returns A promise that resolves to the decompressed data.
 */
export function decompress(data_1) {
    return __awaiter(this, arguments, void 0, function* (data, algorithm = 'gzip') {
        const stream = new DecompressionStream(algorithm);
        const writer = stream.writable.getWriter();
        yield writer.write(data);
        yield writer.close();
        const reader = stream.readable.getReader();
        let result = '';
        let done, value;
        while (true) {
            const readResult = yield reader.read();
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
    });
}
//# sourceMappingURL=compression.js.map