using System;
using NSec.Cryptography;
using Xunit;

namespace NSec.Tests.Rfc
{
    public static class HkdfSha256Tests
    {
        public static readonly TheoryData<string, string, string, string, string> Rfc5869TestVectors = new()
        {
            // Appendix A.1
            {
                "0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b",
                "000102030405060708090a0b0c",
                "f0f1f2f3f4f5f6f7f8f9",
                "077709362c2e32df0ddc3f0dc47bba6390b6c73bb50f9c3122ec844ad7c2b3e5",
                "3cb25f25faacd57a90434f64d0362f2a2d2d0a90cf1a5a4c5db02d56ecc4c5bf34007208d5b887185865"
            },
            // Appendix A.2
            {
                "000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f202122232425262728292a2b2c2d2e2f303132333435363738393a3b3c3d3e3f404142434445464748494a4b4c4d4e4f",
                "606162636465666768696a6b6c6d6e6f707172737475767778797a7b7c7d7e7f808182838485868788898a8b8c8d8e8f909192939495969798999a9b9c9d9e9fa0a1a2a3a4a5a6a7a8a9aaabacadaeaf",
                "b0b1b2b3b4b5b6b7b8b9babbbcbdbebfc0c1c2c3c4c5c6c7c8c9cacbcccdcecfd0d1d2d3d4d5d6d7d8d9dadbdcdddedfe0e1e2e3e4e5e6e7e8e9eaebecedeeeff0f1f2f3f4f5f6f7f8f9fafbfcfdfeff",
                "06a6b88c5853361a06104c9ceb35b45cef760014904671014a193f40c15fc244",
                "b11e398dc80327a1c8e7f78c596a49344f012eda2d4efad8a050cc4c19afa97c59045a99cac7827271cb41c65e590e09da3275600c2f09b8367793a9aca3db71cc30c58179ec3e87c14c01d5c1f3434f1d87"
            },
            // Appendix A.3
            {
                "0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b",
                "",
                "",
                "19ef24a32c717b167f33a91d6f648bdf96596776afdb6377ac434c1c293ccb04",
                "8da4e775a563c18f715f802a063c5a31b8a11f5c5ee1879ec3454e5f3c738d2d9d201395faa4b61a96c8"
            },
        };

        [Theory]
        [MemberData(nameof(Rfc5869TestVectors))]
        public static void TestOneStepSpan(string ikm, string salt, string info, string expectedPrk, string expectedOkm)
        {
            var a = KeyDerivationAlgorithm.HkdfSha256;

            var actualOkm = a.DeriveBytes(ikm.DecodeHex(), salt.DecodeHex(), info.DecodeHex(), expectedOkm.DecodeHex().Length);

            Assert.Equal(expectedOkm.DecodeHex(), actualOkm);
        }

        [Theory]
        [MemberData(nameof(Rfc5869TestVectors))]
        public static void TestOneStep(string ikm, string salt, string info, string expectedPrk, string expectedOkm)
        {
            var a = KeyDerivationAlgorithm.HkdfSha256;

            using var s = SharedSecret.Import(ikm.DecodeHex(), SharedSecretBlobFormat.RawSharedSecret);

            var actualOkm = a.DeriveBytes(s, salt.DecodeHex(), info.DecodeHex(), expectedOkm.DecodeHex().Length);

            Assert.Equal(expectedOkm.DecodeHex(), actualOkm);
        }

        [Theory]
        [MemberData(nameof(Rfc5869TestVectors))]
        public static void TestTwoStep(string ikm, string salt, string info, string expectedPrk, string expectedOkm)
        {
            var a = KeyDerivationAlgorithm.HkdfSha256;

            using var s = SharedSecret.Import(ikm.DecodeHex(), SharedSecretBlobFormat.RawSharedSecret);

            var actualPrk = a.Extract(s, salt.DecodeHex());
            Assert.Equal(expectedPrk.DecodeHex(), actualPrk);

            var actualOkm = a.Expand(actualPrk, info.DecodeHex(), expectedOkm.DecodeHex().Length);
            Assert.Equal(expectedOkm.DecodeHex(), actualOkm);
        }
    }
}
