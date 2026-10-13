using System;
using System.Collections.Generic;
using secureBankingSystem.Models;

namespace secureBankingSystem.Services
{
    class Bank
        {

        private List<User> users;
        private AuthenticationService authenticationService;

        // Constructor sets up the system and adds a dummy user for testing
        public Bank()
        {
            users = new List<User>();
            PasswordService passwordService = new PasswordService();
            users.Add(new User("Joe.Doe", "joe.doe@email.com", 30, "0414824718", passwordService.HashPassword("Password123")));
            authenticationService = new AuthenticationService(users);
        }
        // Returns the logged-in user if credentials match; otherwise returns null

        // Allows the logged-in user to deposit money into their balance
        public void Deposit(User user)
        {
            Console.Write("\nEnter amount to deposit: ");
            string input = Console.ReadLine() ?? "";
            double amount;

            while (!double.TryParse(input, out amount))
            {
                Console.WriteLine("Only numerical data can be entered for deposit.");
                Console.Write("Enter amount to deposit: ");
                input = Console.ReadLine() ?? "";
            }

            if (amount <= 0)
            {
                Console.WriteLine("Deposit amount must be greater than zero.");
                return;
            }

            user.Balance += amount;
            // Save transaction
            user.Transactions.Add("Deposited $" + amount);

            Console.WriteLine("Deposit successful.");
            Console.WriteLine("Updated balance: $" + user.Balance);
        }

        // Allows the logged-in user to withdraw money if sufficient funds are available
        public void Withdraw(User user)
        {
            Console.Write("\nEnter amount to withdraw: ");
            string input = Console.ReadLine() ?? "";
            double amount;

            while (!double.TryParse(input, out amount))
            {
                Console.WriteLine("Only numerical data to be entered for withdrawal.");
                Console.Write("Enter amount to withdraw: ");
                input = Console.ReadLine() ?? "";
            }

            if (amount <= 0)
            {
                Console.WriteLine("Withdrawal amount must be greater than zero.");
                return;
            }

            if (amount > user.Balance)
            {
                Console.WriteLine("Not sufficient fund available.");

                // Save failed attempt
                user.Transactions.Add("Failed withdrawal of $" + amount + " (insufficient funds)");

                return;

            }

            user.Balance -= amount;

            // Save transaction
            user.Transactions.Add("Withdrew $" + amount);

            Console.WriteLine("Withdrawal successful.");
            Console.WriteLine("Updated balance: $" + user.Balance);
        }

        // Allows the user to attempt login up to 3 times
        public void StartLogin()
        {
            int attempts = 0;
            const int maxAttempts = 3;

            while (attempts < maxAttempts)
            {
                Console.Write("\nEnter username: ");
                string username = Console.ReadLine() ?? "";

                while (string.IsNullOrWhiteSpace(username))
                {
                    Console.WriteLine("Username cannot be empty. Please try again.");
                    Console.Write("Enter username: ");
                    username = Console.ReadLine() ?? "";
                }

                Console.Write("Enter password: ");
                string password = Console.ReadLine() ?? "";

                while (string.IsNullOrWhiteSpace(password))
                {
                    Console.WriteLine("Password cannot be empty. Please try again.");
                    Console.Write("Enter password: ");
                    password = Console.ReadLine() ?? "";
                }

                User? loggedInUser = authenticationService.Login(username, password);

                if (loggedInUser != null)
                {
                    Console.WriteLine("\nLogin successful.");
                    Console.WriteLine("Welcome to the bank system.");

                    // Opens the banking menu for the logged-in user
                    BankingMenu(loggedInUser);
                    return;
                }

                attempts++;
                Console.WriteLine($"\nInvalid username or password. Attempts remaining: {maxAttempts - attempts}");
            }

            Console.WriteLine("\nToo many failed attempts. Access denied.");
        }

        // Collects user details, validates them, and stores the new user
        public void SignUp()
        {
            Console.WriteLine("\n==================================");
            Console.WriteLine("             SIGN UP");
            Console.WriteLine("==================================");

            Console.Write("Enter Username: ");
            string username = Console.ReadLine() ?? "";

            while (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("Username cannot be empty. Please enter all details correctly.");
                Console.Write("Enter Username: ");
                username = Console.ReadLine() ?? "";
            }

            while (UsernameExists(username))
            {
                Console.WriteLine("That username already exists. Please choose a different username.");
                Console.Write("Enter Username: ");
                username = Console.ReadLine() ?? "";
            }

            Console.Write("Enter Email: ");
            string email = Console.ReadLine() ?? "";

            while (string.IsNullOrWhiteSpace(email) || !email.Contains("@") || !email.Contains(".com"))
            {
                Console.WriteLine("Invalid email. Please enter a valid email address.");
                Console.Write("Enter Email: ");
                email = Console.ReadLine() ?? "";
            }

            Console.Write("Enter Age: ");
            string ageInput = Console.ReadLine() ?? "";
            int age;

            while (!int.TryParse(ageInput, out age) || age < 18 || age > 120)
            {
                Console.WriteLine("Invalid age. Age must be between 18 and 120.");
                Console.Write("Enter Age: ");
                ageInput = Console.ReadLine() ?? "";
            }

            Console.Write("Enter Phone (10 digits): ");
            string phone = Console.ReadLine() ?? "";

            while (string.IsNullOrWhiteSpace(phone) || phone.Length != 10 || !IsDigitsOnly(phone))
            {
                Console.WriteLine("Phone number must be exactly 10 digits.");
                Console.Write("Enter Phone (10 digits): ");
                phone = Console.ReadLine() ?? "";
            }

            Console.Write("Enter Password (minimum 6 characters): ");
            string password = Console.ReadLine() ?? "";

            while (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                Console.WriteLine("Password must be at least 6 characters long.");
                Console.Write("Enter Password (minimum 6 characters): ");
                password = Console.ReadLine() ?? "";
            }

            User newUser = new User(username, email, age, phone, password);
            users.Add(newUser);

            Console.WriteLine("\nAccount created successfully.");
            Console.WriteLine("You can now log in using your new username and password.");
        }

        // Displays banking options after successful login
        public void BankingMenu(User loggedInUser)
        {
            bool banking = true;

            while (banking)
            {
                Console.WriteLine("\n==================================");
                Console.WriteLine("          BANKING MENU");
                Console.WriteLine("==================================");
                Console.WriteLine("1. Deposit");
                Console.WriteLine("2. Withdraw");
                Console.WriteLine("3. View Balance");
                Console.WriteLine("4. View Transaction History");
                Console.WriteLine("5. Logout");

                Console.Write("Choose an option: ");

                string choice = Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1":
                        Deposit(loggedInUser);
                        break;

                    case "2":
                        Withdraw(loggedInUser);
                        break;

                    case "3":
                        ViewBalance(loggedInUser);
                        break;


                    case "4":
                        ViewTransactionHistory(loggedInUser);
                        break;


                    case "5":
                        banking = false;
                        Console.WriteLine("Logged out successfully.");
                        break;

                    default:
                        Console.WriteLine("Invalid option. Please choose 1, 2, 3, or 4.");
                        break;
                }
            }
        }

        // Displays the current balance of the logged-in user
        public void ViewBalance(User user)
        {
            Console.WriteLine("\n==================================");
            Console.WriteLine("            BALANCE");
            Console.WriteLine("==================================");
            Console.WriteLine("Current balance: $" + user.Balance);
        }

        // Displays all users currently stored in the system
        public void ViewRegisteredUsers()
        {
            Console.WriteLine("\n==================================");
            Console.WriteLine("        REGISTERED USERS");
            Console.WriteLine("==================================");

            for (int i = 0; i < users.Count; i++)
            {
                Console.WriteLine($"User {i + 1}");
                Console.WriteLine($"Username: {users[i].Username}");
                Console.WriteLine($"Email: {users[i].Email}");
                Console.WriteLine($"Age: {users[i].Age}");
                Console.WriteLine($"Phone: {users[i].Phone}");
                Console.WriteLine("----------------------------------");
            }
        }

        // Checks whether a username already exists
        private bool UsernameExists(string username)
        {
            foreach (User user in users)
            {
                if (user.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        // Checks whether the phone input contains numbers only
        private bool IsDigitsOnly(string text)
        {
            foreach (char c in text)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }

            return true;
        }

        // Displays all transactions for the logged-in user
        public void ViewTransactionHistory(User user)
        {
            Console.WriteLine("\n==================================");
            Console.WriteLine("      TRANSACTION HISTORY");
            Console.WriteLine("==================================");

            if (user.Transactions.Count == 0)
            {
                Console.WriteLine("No transactions yet.");
                return;
            }

            for (int i = 0; i < user.Transactions.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {user.Transactions[i]}");
            }
        }
    }
}