using System.Text;  
using System.Security.Cryptography;  

namespace TwitterClone.Common
{
    public static class Encryptor
    {
        public static string SHA256Hash(string text)
        {
            SHA256 sha256 = SHA256.Create();
            byte[] hashData = sha256.ComputeHash(Encoding.Default.GetBytes(text));
            StringBuilder returnValue = new StringBuilder();

            for (int i = 0; i < hashData.Length; i++)
            {
                returnValue.Append(hashData[i].ToString());

            }
            return returnValue.ToString();
        }
    }
}