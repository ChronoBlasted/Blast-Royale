// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("HBrPNZZLBbht9PDRCr7dJKMSGSRCJ55vgiPLMG1JAey+nL9sz8UG0JJL4n4A868hILOs6ktZXVfEn+XQZYX+d1pugMS5d9h2HNp0Vs3KJL2z4duTRG+Rk37PrV3GaQ+bhFCaYzSoA/hBii3+JG3DRKgkwQe6wD2gOUU9/8voEeJUtgc/ljlKxEH4yVTaRqZJRDyo/FFmB33MUg1qwMQYyybFhTBGwafl1fXnJQamrrmDZJHMHp2TnKwenZaeHp2dnBnLsP8IiSh3Z0zUx/ANth/aAoqxVHsGJynZqsqiWvGv5S1dBhjkFkL684HuLRNLrB6dvqyRmpW2GtQaa5GdnZ2ZnJ8TqV2CJusccrT+pzkgxVN/cuQipzMvNx4dAxHpuZ6fnZyd");
        private static int[] order = new int[] { 8,12,7,3,11,11,9,10,13,12,10,11,13,13,14 };
        private static int key = 156;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
