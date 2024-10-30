using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;

namespace UserMS.Logic.ServiceLayer.Helpers
{
    public static class PasswordHelper
    {
        public static string Hash(string salt, string password)
        {
            byte[] staticSalt = Encoding.UTF8.GetBytes(salt);

            string encryptedPassw = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: staticSalt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
            ));

            return encryptedPassw;

        }
    }
}
