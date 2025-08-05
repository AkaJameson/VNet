using System.Text;

namespace VNet.Utilites
{
    public interface IVNetCryptoProvider
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
    }

    public class VNetCryptoProvider : IVNetCryptoProvider
    {
        private const int Shift = 3; 

        public string Encrypt(string plainText)
        {
            var shifted = new StringBuilder();
            foreach (char c in plainText)
            {
                shifted.Append((char)(c + Shift));
            }

            var reversed = new string(shifted.ToString().Reverse().ToArray());

            var bytes = Encoding.UTF8.GetBytes(reversed);
            return Convert.ToBase64String(bytes);
        }

        public string Decrypt(string cipherText)
        {
            var bytes = Convert.FromBase64String(cipherText);
            var reversed = Encoding.UTF8.GetString(bytes);

            var unshiftedText = new string(reversed.Reverse().ToArray());

            var original = new StringBuilder();
            foreach (char c in unshiftedText)
            {
                original.Append((char)(c - Shift));
            }

            return original.ToString();
        }
    }

}
