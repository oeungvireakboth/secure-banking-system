using System;
using System.Collections.Generic;
using secureBankingSystem.Models;

namespace secureBankingSystem.Models
{
    // Represents one bank user and stores personal information
    public class User
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public string Phone { get; set; }
        public string PasswordHash { get; set; }
        public decimal Balance { get; set; }
        public List<string> Transactions { get; set; }

        // Constructor creates a user object with all required details
        public User(string username, string email, int age, string phone, string password)
        {
            Username = username;
            Email = email;
            Age = age;
            Phone = phone;
            PasswordHash = password;

            // New users start with a zero balance
            Balance = 0;

            // Initialize transaction history list
            Transactions = new List<string>();
        }
    }
}