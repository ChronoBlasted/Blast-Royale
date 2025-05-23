// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("HIBgj4L6bjqXoMG7CpTLrAYC3g3a3AnzUI3DfqsyNhfMeBviZdTf4oThWKlE5Q32q4/HKnhaeaoJA8AW1W+bROAt2rRyOGH/5gOVubQi5GFUjSS4xjVp5+Z1aiyNn5uRAlkjFqNDOLGcqEYCf7EesNocspALDOJ7DGScN2kj65vA3iLQhDw1Ryjr1Y3/g/s5DS7XJJJwwflQ/4wChz4PkmrYW3hqV1xTcNwS3K1XW1tbX1pZ8m7FPodM6zjiqwWCbuIHwXwG+2axoYoSATbLcNkcxEx3kr3A4e8fbNhbVVpq2FtQWNhbW1rfDXY5zk/udScdVYKpV1W4CWubAK/JXUKWXKXgA0P2gAdhIxMzIePAYGh/RaJXCvXp8djbxdcvf1hZW1pb");
        private static int[] order = new int[] { 10,8,12,6,7,6,6,9,10,9,12,12,12,13,14 };
        private static int key = 90;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
