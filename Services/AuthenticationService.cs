
using System.Collections.Generic;
using secureBankingSystem.Models;
using secureBankingSystem.Data;

namespace secureBankingSystem.Services
{
    public class AuthenticationService
    {
        private readonly List<User> users;
        private readonly DatabaseService database;
        private readonly PasswordService passwordService = new PasswordService();

        public AuthenticationService(
            List<User> users,
            DatabaseService database)
        {
            this.users = users;
            this.database = database;
        }

        public User? Login(string username, string password)
        {
            foreach (User user in users)
            {
                if (user.Username == username
                    && passwordService.VerifyPassword(password, user.PasswordHash))
                {
                    if (passwordService.NeedsRehash(user.PasswordHash))
                    {
                        string newPasswordHash =
                            passwordService.HashPassword(password);

                        database.UpdatePasswordHash(
                            user.Username,
                            newPasswordHash);

                        user.PasswordHash = newPasswordHash;
                    }

                    return user;
                }
            }

            return null;
        }
    }
}
