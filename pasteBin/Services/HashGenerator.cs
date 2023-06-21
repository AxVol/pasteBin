using System.Security.Cryptography;
using System.Text;

namespace pasteBin.Services
{
    public class HashGenerator : IHashGenerator
    {
        public string HashForURL()
        {
            string hash = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString())).Remove(8);

            return hash;
        }

        public string PasswordHash(string password)
        {
            SHA256 hash = SHA256.Create();

            return Convert.ToHexString(hash.ComputeHash(Encoding.ASCII.GetBytes(password)));
        }
    }
}
