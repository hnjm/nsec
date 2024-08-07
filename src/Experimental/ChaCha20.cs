using System;
using System.Diagnostics;
using System.Threading;
using NSec.Cryptography;
using NSec.Cryptography.Formatting;
using static Interop.Libsodium;

namespace NSec.Experimental
{
    public class ChaCha20 : StreamCipherAlgorithm
    {
        private static int s_selfTest;

        public ChaCha20() : base(
            keySize: crypto_stream_chacha20_ietf_KEYBYTES,
            nonceSize: crypto_stream_chacha20_ietf_NONCEBYTES)
        {
            if (s_selfTest == 0)
            {
                SelfTest();
                Interlocked.Exchange(ref s_selfTest, 1);
            }
        }

        internal override void CreateKey(
            ReadOnlySpan<byte> seed,
            out SecureMemoryHandle keyHandle,
            out PublicKey? publicKey)
        {
            Debug.Assert(seed.Length == crypto_stream_chacha20_ietf_KEYBYTES);

            publicKey = null;
            keyHandle = SecureMemoryHandle.CreateFrom(seed);
        }

        internal override int GetSeedSize()
        {
            return crypto_stream_chacha20_ietf_KEYBYTES;
        }

        private protected override void GeneratePseudoRandomStreamCore(
            SecureMemoryHandle keyHandle,
            ReadOnlySpan<byte> nonce,
            Span<byte> bytes)
        {
            Debug.Assert(keyHandle.Size == crypto_stream_chacha20_ietf_KEYBYTES);
            Debug.Assert(nonce.Length == crypto_stream_chacha20_ietf_NONCEBYTES);

            int error = crypto_stream_chacha20_ietf(
                bytes,
                (ulong)bytes.Length,
                nonce,
                keyHandle);

            Debug.Assert(error == 0);
        }

        private protected override void XOrCore(
            SecureMemoryHandle keyHandle,
            ReadOnlySpan<byte> nonce,
            ReadOnlySpan<byte> input,
            Span<byte> output)
        {
            Debug.Assert(keyHandle.Size == crypto_stream_chacha20_ietf_KEYBYTES);
            Debug.Assert(nonce.Length == crypto_stream_chacha20_ietf_NONCEBYTES);
            Debug.Assert(output.Length == input.Length);

            int error = crypto_stream_chacha20_ietf_xor(
                output,
                input,
                (ulong)input.Length,
                nonce,
                keyHandle);

            Debug.Assert(error == 0);
        }

        private protected override void XOrICCore(
            SecureMemoryHandle keyHandle,
            ReadOnlySpan<byte> nonce,
            ReadOnlySpan<byte> input,
            uint ic,
            Span<byte> output)
        {
            Debug.Assert(keyHandle.Size == crypto_stream_chacha20_ietf_KEYBYTES);
            Debug.Assert(nonce.Length == crypto_stream_chacha20_ietf_NONCEBYTES);
            Debug.Assert(output.Length == input.Length);

            int error = crypto_stream_chacha20_ietf_xor_ic(
                output,
                input,
                (ulong)input.Length,
                nonce,
                ic,
                keyHandle);

            Debug.Assert(error == 0);
        }

        internal override bool TryExportKey(
            SecureMemoryHandle keyHandle,
            KeyBlobFormat format,
            Span<byte> blob,
            out int blobSize)
        {
            return format switch
            {
                KeyBlobFormat.RawSymmetricKey => RawKeyFormatter.TryExport(keyHandle, blob, out blobSize),
                _ => throw Error.Argument_FormatNotSupported(nameof(format), format.ToString()),
            };
        }

        internal override bool TryImportKey(
            ReadOnlySpan<byte> blob,
            KeyBlobFormat format,
            out SecureMemoryHandle? keyHandle,
            out PublicKey? publicKey)
        {
            publicKey = null;

            return format switch
            {
                KeyBlobFormat.RawSymmetricKey => RawKeyFormatter.TryImport(crypto_stream_chacha20_ietf_KEYBYTES, blob, out keyHandle),
                _ => throw Error.Argument_FormatNotSupported(nameof(format), format.ToString()),
            };
        }

        private static void SelfTest()
        {
            if ((crypto_stream_chacha20_ietf_keybytes() != crypto_stream_chacha20_ietf_KEYBYTES) ||
                (crypto_stream_chacha20_ietf_noncebytes() != crypto_stream_chacha20_ietf_NONCEBYTES))
            {
                throw Error.InvalidOperation_InitializationFailed();
            }
        }
    }
}
