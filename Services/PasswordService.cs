using System;
using System.Security.Cryptography;

namespace secureBankingSystem.Services
{
    public class PasswordService
    {
        private const int Iterations = 600_000;
        private const int LegacyIterations = 100_000;
        public string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);

            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                32
            );

            return $"{Iterations}:{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
        }

        public bool VerifyPassword(string password, string storedPassword)
        {
            string[] parts = storedPassword.Split(':');

            try
            {
                int iterations;
                string saltString;
                string hashString;

                if (parts.Length == 2)
                {
                    // Old storage format: salt:hash
                    iterations = LegacyIterations;
                    saltString = parts[0];
                    hashString = parts[1];
                }
                else if (parts.Length == 3)
                {
                    // New storage format: iterations:salt:hash
                    if (!int.TryParse(parts[0], out iterations))
                        return false;

                    saltString = parts[1];
                    hashString = parts[2];
                }
                else
                {
                    return false;
                }

                // Only accept iteration counts used by this application
                if (iterations != LegacyIterations && iterations != Iterations)
                    return false;

                byte[] salt = Convert.FromBase64String(saltString);
                byte[] expectedHash = Convert.FromBase64String(hashString);

                if (salt.Length != 16 || expectedHash.Length != 32)
                    return false;

                byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
                    password,
                    salt,
                    iterations,
                    HashAlgorithmName.SHA256,
                    32
                );

                return CryptographicOperations.FixedTimeEquals(
                    actualHash,
                    expectedHash
                );
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public bool NeedsRehash(string storedPassword)
        {
            string[] parts = storedPassword.Split(':');

            if (parts.Length == 2)
                return true;

            if (parts.Length != 3)
                return false;

            if (!int.TryParse(parts[0], out int iterations))
                return false;

            return iterations != Iterations;
        }
    }
}
