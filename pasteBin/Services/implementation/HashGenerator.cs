using System.Security.Cryptography;
using System.Text;
using pasteBin.Services.Interfaces;

namespace pasteBin.Services.implementation
{
    public class HashGenerator : IHashGenerator
    {
        // Метод для генерации уникальных и коротких хешей для ссылок к постам
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
