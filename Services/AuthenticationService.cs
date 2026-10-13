using System.Collections.Generic;
using secureBankingSystem.Models;

namespace secureBankingSystem.Services
{
    public class AuthenticationService
    {
        private readonly List<User> users;

        public AuthenticationService(List<User> users)
        {
            this.users = users;
        }

        public User? Login(string username, string password)
        {
            foreach (User user in users)
            {
                if (user.Username == username
                && passwordService.VerifyPassword(password, user.PasswordHash))
                {
                    return user;
                }
            }

            return null;
        }
        private readonly PasswordService passwordService = new PasswordService();
    }
}