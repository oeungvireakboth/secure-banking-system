using Microsoft.Data.Sqlite;
using secureBankingSystem.Data;
using Xunit;

namespace SecureBankingSystem.Tests
{
    public class DatabaseServiceTests
    {
        [Fact]
        public void UpdateBalanceAndSaveTransaction_UpdatesBalanceAndHistory()
        {
            string dbPath = Path.Combine(
                Path.GetTempPath(),
                $"banking_test_{Guid.NewGuid()}.db");

            try
            {
                var database = new DatabaseService(
                $"Data Source={dbPath};Pooling=False");

                database.InitializeDatabase();

                // Create a test account with $100
                database.SaveUser(
                    "TestUser",
                    "test@example.com",
                    25,
                    "1234567890",
                    "test-hash",
                    100m);

                // Deposit $50
                database.UpdateBalanceAndSaveTransaction(
                    "TestUser",
                    150m,
                    "Deposited $50");

                // Verify the updated balance
                var users = database.LoadUsers();

                Assert.Single(users);
                Assert.Equal(150m, users[0].Balance);

                // Verify the transaction history
                var transactions = database.LoadTransactions("TestUser");

                Assert.Single(transactions);
                Assert.Equal("Deposited $50", transactions[0]);
            }
            finally
            {
                if (File.Exists(dbPath))
                {
                    File.Delete(dbPath);
                }
            }
        }

        [Fact]
        public void UpdateBalanceAndSaveTransaction_RollsBackWhenTransactionInsertFails()
        {
            string dbPath = Path.Combine(
                Path.GetTempPath(),
                $"banking_rollback_{Guid.NewGuid()}.db");

            try
            {
                var database = new DatabaseService(
                    $"Data Source={dbPath};Pooling=False");

                database.InitializeDatabase();

                database.SaveUser(
                    "RollbackUser",
                    "rollback@example.com",
                    25,
                    "1234567890",
                    "test-hash",
                    100m);

                // Create a trigger that deliberately makes transaction insertion fail
                using (var connection = new SqliteConnection(
                    $"Data Source={dbPath};Pooling=False"))
                {
                    connection.Open();

                    using var command = connection.CreateCommand();

                    command.CommandText = @"
                CREATE TRIGGER FailTransactionInsert
                BEFORE INSERT ON Transactions
                BEGIN
                    SELECT RAISE(ABORT, 'Simulated database failure');
                END;
            ";

                    command.ExecuteNonQuery();
                }

                // The database operation should fail
                Assert.Throws<SqliteException>(() =>
                    database.UpdateBalanceAndSaveTransaction(
                        "RollbackUser",
                        150m,
                        "Deposited $50"));

                // Verify the original balance remains unchanged
                var users = database.LoadUsers();

                Assert.Equal(100m, users[0].Balance);

                // Verify no transaction was recorded
                var transactions = database.LoadTransactions("RollbackUser");

                Assert.Empty(transactions);
            }
            finally
            {
                if (File.Exists(dbPath))
                {
                    File.Delete(dbPath);
                }
            }
        }
    }
}
