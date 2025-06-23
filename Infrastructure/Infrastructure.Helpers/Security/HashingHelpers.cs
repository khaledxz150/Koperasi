using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Helpers.Security
{
    public static class HashingHelpers
    {
        public static string GenerateSalt()
        {
            var bytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return Convert.ToBase64String(bytes);
        }

        public static string SaltHash(this string value, string salt)
        {
            return Hash(Encoding.UTF8.GetBytes(value), Encoding.UTF8.GetBytes(salt));
        }

        public static string Hash(byte[] value, byte[] salt)
        {
            byte[] saltedValue = value.Concat(salt).ToArray();

            // Compute the hash
            byte[] hashBytes = new SHA256Managed().ComputeHash(saltedValue);

            // Convert the hash to a Base64 string for storage
            return Convert.ToBase64String(hashBytes);
        }
    }
}
