using System;
using NSec.Cryptography;
using Xunit;

namespace NSec.Tests.Algorithms
{
    public static class Sha512Tests
    {
        private const string s_hashOfEmpty = "cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e";

        #region Properties

        [Fact]
        public static void Properties()
        {
            var a = new Sha512();

            Assert.Equal(32, Sha512.MinHashSize);
            Assert.Equal(64, Sha512.MaxHashSize);

            Assert.Equal(64, a.HashSize);

            Assert.Equal(32, HashAlgorithm.Sha512_256.HashSize);

            Assert.Equal(64, HashAlgorithm.Sha512.HashSize);
        }

        #endregion

        #region Hash #1

        [Fact]
        public static void HashEmpty()
        {
            var a = HashAlgorithm.Sha512;

            var expected = Convert.FromHexString(s_hashOfEmpty);
            var actual = a.Hash([]);

            Assert.Equal(a.HashSize, actual.Length);
            Assert.Equal(expected, actual);
        }

        #endregion

        #region Hash #3

        [Fact]
        public static void HashEmptyWithSpan()
        {
            var a = HashAlgorithm.Sha512;

            var expected = Convert.FromHexString(s_hashOfEmpty);
            var actual = new byte[expected.Length];

            a.Hash([], actual);
            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
