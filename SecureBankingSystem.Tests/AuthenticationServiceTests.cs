using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using secureBankingSystem.Data;
using secureBankingSystem.Models;
using secureBankingSystem.Services;
using Xunit;

namespace SecureBankingSystem.Tests
{
    public class AuthenticationServiceTests
    {
        private const int LegacyIterations = 100_000;

        [Fact]
        public void Login_ValidCredentials_ReturnsUser()
        {
            var (path, database) = CreateDatabase();

            try
            {
                var passwordService = new PasswordService();
                string password = "TestPassword123!";
                string hash = passwordService.HashPassword(password);

                var user = CreateUser(hash);
                SaveUser(database, user);

                var auth = new AuthenticationService(
                    new List<User> { user }, database);

                var result = auth.Login(user.Username, password);

                Assert.Same(user, result);
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Fact]
        public void Login_IncorrectPassword_ReturnsNull()
        {
            var (path, database) = CreateDatabase();

            try
            {
                string hash = new PasswordService()
                    .HashPassword("CorrectPassword123!");

                var user = CreateUser(hash);
                SaveUser(database, user);

                var auth = new AuthenticationService(
                    new List<User> { user }, database);

                var result = auth.Login(
                    user.Username, "WrongPassword123!");

                Assert.Null(result);
                Assert.Equal(hash, user.PasswordHash);
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Fact]
        public void Login_LegacyPasswordHash_UpgradesAndSavesHash()
        {
            var (path, database) = CreateDatabase();

            try
            {
                string password = "LegacyPassword123!";
                string legacyHash = CreateLegacyHash(password);

                var user = CreateUser(legacyHash);
                SaveUser(database, user);

                var auth = new AuthenticationService(
                    new List<User> { user }, database);

                var result = auth.Login(user.Username, password);

                Assert.Same(user, result);
                Assert.NotEqual(legacyHash, user.PasswordHash);

                string[] parts = user.PasswordHash.Split(':');

                Assert.Equal(3, parts.Length);
                Assert.Equal("600000", parts[0]);

                var savedUser = Assert.Single(database.LoadUsers());

                Assert.Equal(user.PasswordHash, savedUser.PasswordHash);
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Fact]
        public void Login_WrongPasswordWithLegacyHash_DoesNotUpgradeHash()
        {
            var (path, database) = CreateDatabase();

            try
            {
                string legacyHash = CreateLegacyHash("CorrectPassword123!");

                var user = CreateUser(legacyHash);
                SaveUser(database, user);

                var auth = new AuthenticationService(
                    new List<User> { user }, database);

                var result = auth.Login(
                    user.Username, "WrongPassword123!");

                Assert.Null(result);
                Assert.Equal(legacyHash, user.PasswordHash);

                var savedUser = Assert.Single(database.LoadUsers());

                Assert.Equal(legacyHash, savedUser.PasswordHash);
            }
            finally
            {
                File.Delete(path);
            }
        }

        private static (string Path, DatabaseService Database)
            CreateDatabase()
        {
            string path = Path.Combine(
                Path.GetTempPath(),
                $"SecureBanking_{Guid.NewGuid():N}.db");

            var database = new DatabaseService(
                $"Data Source={path};Pooling=False");

            database.InitializeDatabase();

            return (path, database);
        }

        private static User CreateUser(string passwordHash)
        {
            return new User(
                "testuser",
                "test@example.com",
                25,
                "0400000000",
                passwordHash);
        }

        private static void SaveUser(DatabaseService database, User user)
        {
            database.SaveUser(
                user.Username,
                user.Email,
                user.Age,
                user.Phone,
                user.PasswordHash,
                user.Balance);
        }

        private static string CreateLegacyHash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);

            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                LegacyIterations,
                HashAlgorithmName.SHA256,
                32);

            return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
        }
    }
}
