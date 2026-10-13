using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace secureBankingSystem.Data
{
    public class DatabaseService
    {
        private readonly string connectionString;

        public DatabaseService(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void InitializeDatabase()
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            using var command = connection.CreateCommand();

            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    Email TEXT NOT NULL,
                    Age INTEGER NOT NULL,
                    Phone TEXT NOT NULL,
                    PasswordHash TEXT NOT NULL,
                    Balance TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS Transactions (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL,
                    Description TEXT NOT NULL,
                    CreatedAt TEXT NOT NULL
                );
            ";

            command.ExecuteNonQuery();
        }
        
        public void SaveUser(
            string username,
            string email,
            int age,
            string phone,
            string passwordHash,
            decimal balance)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            using var command = connection.CreateCommand();

            command.CommandText = @"
            INSERT INTO Users
            (Username, Email, Age, Phone, PasswordHash, Balance)
            VALUES
            ($username, $email, $age, $phone, $passwordHash, $balance);
            ";

            command.Parameters.AddWithValue("$username", username);
            command.Parameters.AddWithValue("$email", email);
            command.Parameters.AddWithValue("$age", age);
            command.Parameters.AddWithValue("$phone", phone);
            command.Parameters.AddWithValue("$passwordHash", passwordHash);
            command.Parameters.AddWithValue(
            "$balance",
            balance.ToString(System.Globalization.CultureInfo.InvariantCulture));

            command.ExecuteNonQuery();
        }
        
        public void UpdateUserBalance(string username, decimal balance)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            using var command = connection.CreateCommand();

            command.CommandText = @"
            UPDATE Users
            SET Balance = $balance
            WHERE Username = $username;
            ";

            command.Parameters.AddWithValue(
            "$balance",
            balance.ToString(System.Globalization.CultureInfo.InvariantCulture));

            command.Parameters.AddWithValue("$username", username);

            command.ExecuteNonQuery();
        }
         
        public void SaveTransaction(string username, string description)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            using var command = connection.CreateCommand();

            command.CommandText = @"
            INSERT INTO Transactions (Username, Description, CreatedAt)
            VALUES ($username, $description, $createdAt);
            ";

            command.Parameters.AddWithValue("$username", username);
            command.Parameters.AddWithValue("$description", description);
            command.Parameters.AddWithValue(
            "$createdAt",
            DateTime.UtcNow.ToString("O"));

            command.ExecuteNonQuery();
        }
        
        public List<string> LoadTransactions(string username)
        {
            var transactions = new List<string>();

            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            using var command = connection.CreateCommand();

            command.CommandText = @"
                SELECT Description
                FROM Transactions
                WHERE Username = $username
                ORDER BY Id;
            ";

            command.Parameters.AddWithValue("$username", username);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                transactions.Add(reader.GetString(0));
            }

            return transactions;
        }

        public List<secureBankingSystem.Models.User> LoadUsers()
        {
            var users = new List<secureBankingSystem.Models.User>();

            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            using var command = connection.CreateCommand();

            command.CommandText = @"
            SELECT Username, Email, Age, Phone, PasswordHash, Balance
            FROM Users;
            ";

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var user = new secureBankingSystem.Models.User(
                    reader.GetString(0),
                    reader.GetString(1),
                    reader.GetInt32(2),
                    reader.GetString(3),
                    reader.GetString(4)
                );

                user.Balance = decimal.Parse(
                    reader.GetString(5),
                    System.Globalization.CultureInfo.InvariantCulture
                );

                users.Add(user);
            }

            return users;
        }
    }
}
