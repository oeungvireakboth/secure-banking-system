// BANKING SYSTEM
using System;
using System.Collections.Generic;
using secureBankingSystem.Services;
using secureBankingSystem.Models;
using secureBankingSystem.Data;

namespace secureBankingSystem
{
    // This class is defined in the UserManagement.Models namespace

    // Handles signup, login, balance viewing and future banking operations

    class Program
    {
        static void Main(string[] args)
        {
            DatabaseService database = new DatabaseService("Data Source=banking_system.db");
            database.InitializeDatabase();

            Console.WriteLine("Database initialized successfully.");

            Bank bank = new Bank(database);
            bool running = true;

            while (running)
            {
                Console.WriteLine("\n==================================");
                Console.WriteLine("         BANKING SYSTEM");
                Console.WriteLine("==================================");
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Sign Up");
                Console.WriteLine("3. View Registered Users");
                Console.WriteLine("4. Exit");
                Console.Write("Choose an option: ");

                string choice = Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1":
                        bank.StartLogin();
                        break;

                    case "2":
                        bank.SignUp();
                        break;

                    case "3":
                        bank.ViewRegisteredUsers();
                        break;

                    case "4":
                        running = false;
                        Console.WriteLine("Thank you for using the banking system.");
                        break;

                    default:
                        Console.WriteLine("Invalid option. Please choose 1, 2, 3, or 4.");
                        break;
                }
            }
        }
    }
}