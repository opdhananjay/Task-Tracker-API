using System.Security.Cryptography;

namespace devops.Helpers
{
    public class PasswordHelper
    {
        // Hashing Password 
        public static (string PasswordHash,string PasswordSalt) CreatePasswordHashWithSalt(string password)
        {
            var salt = new byte[128 / 8];
            using(var randomNumberGenerator = RandomNumberGenerator.Create())
            {
                randomNumberGenerator.GetBytes(salt);
            }
            var hash = new Rfc2898DeriveBytes(password, salt,1000, HashAlgorithmName.SHA256);
            string saltBase64 = Convert.ToBase64String(salt);
            string hashBase64 = Convert.ToBase64String(hash.GetBytes(32));
            return (PasswordHash:hashBase64, PasswordSalt:saltBase64);
        }

        public static bool VerifyPassword(string enteredPassword,string storedHash,string storedSalt)
        {
            byte[] saltBytes = Convert.FromBase64String(storedSalt);
            using(var pbkdf2 = new Rfc2898DeriveBytes(enteredPassword, saltBytes, 1000, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(32);
                return Convert.ToBase64String(hash) == storedHash;
            }
        }

    }
}
