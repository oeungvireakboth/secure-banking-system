# Secure Banking System

A security-focused banking application built with **C# and .NET 10**, designed to demonstrate secure authentication, transaction processing, data persistence, input validation, and automated testing.

This project is being developed as a personal software engineering and cybersecurity portfolio project.

---

## Overview

Secure Banking System is a console-based banking application that allows users to create accounts, authenticate, manage their balances, and review transaction history.

The main focus is not just implementing banking functionality, but understanding how to handle sensitive operations safely, protect user credentials, and maintain data consistency when something goes wrong.

## Features

### Account Management
- User registration and login.
- Account information stored in SQLite.
- Persistent account balances across application restarts.

### Authentication and Security
- Password hashing using PBKDF2 with SHA-256.
- Random salt generation for password protection.
- Password verification without storing plaintext passwords.
- Login attempt restrictions after repeated failed attempts.
- Input validation for account and transaction operations.

### Banking Operations
- Deposit funds.
- Withdraw funds.
- Balance validation and insufficient-funds protection.
- Transaction history tracking.
- Protection against invalid amounts and decimal overflow.

### Database and Data Integrity
- SQLite database integration.
- Persistent user and transaction records.
- Atomic balance updates and transaction recording.
- Database rollback protection when transaction processing fails.
- Error handling for database failures.

### Automated Testing
- Unit testing with xUnit.
- Password hashing and verification tests.
- Input validation and balance calculation tests.
- SQLite integration tests.
- Transaction rollback and database failure tests.

---

## Technology Stack

| Technology | Purpose |
|---|---|
| C# | Application development |
| .NET 10 | Application framework |
| SQLite | Persistent data storage |
| Microsoft.Data.Sqlite | Database connectivity |
| xUnit | Automated testing |
| Git & GitHub | Version control |

---

## Security Implementations

Security is a central part of this project.

Some of the key implementations include:

**Password Protection**

Passwords are hashed using PBKDF2 with SHA-256 and individual random salts. Plaintext passwords are not stored in the database.

**Transaction Integrity**

Balance changes and transaction records are committed together using SQLite transactions. This helps prevent situations where a balance changes without the corresponding transaction being recorded.

**Input and Balance Validation**

The application rejects invalid transaction amounts, prevents withdrawals beyond the available balance, and handles decimal overflow during deposit calculations.

**Failure Handling**

Database failures during deposits and withdrawals are handled without incorrectly updating the in-memory account balance.

---

## Project Structure

```text
SecureBankingSystem/
│
├── Data/
│   └── DatabaseService.cs
│
├── Models/
│   └── User.cs
│
├── Services/
│   ├── Bank.cs
│   ├── AuthenticationService.cs
│   ├── PasswordService.cs
│   ├── AmountValidator.cs
│   ├── WithdrawalValidator.cs
│   └── BalanceCalculator.cs
│
├── SecureBankingSystem.Tests/
│   ├── UnitTest1.cs
│   └── DatabaseServiceTests.cs
│
├── Program.cs
├── banking-system.csproj
└── README.md
```

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Git
- Visual Studio or Visual Studio Code (recommended)

### Clone the Repository

```bash
git clone https://github.com/oeungvireakboth/secure-banking-system.git
```

Navigate into the project directory:

```bash
cd secure-banking-system
```

### Restore Dependencies

```bash
dotnet restore
```

### Run the Application

```bash
dotnet run
```

### Run Automated Tests

```bash
dotnet test SecureBankingSystem.Tests
```

---

## Current Development Status

**Status: In Development**

The core banking functionality, password hashing, SQLite persistence, transaction integrity, and initial automated test coverage have been implemented.

Current development focuses on improving exception handling, expanding security tests, and reviewing the overall application architecture.

---

## Future Improvements

- Improve database failure handling across all operations.
- Expand authentication and security test coverage.
- Improve input handling and application architecture.
- Introduce stronger database integrity protections.
- Explore concurrent transaction handling.
- Improve logging and error diagnostics.

---

## Disclaimer

This project is an educational portfolio application developed to demonstrate software engineering and cybersecurity concepts.

It is not a production banking application and should not be used to store or manage real financial information.

---

## Author

**Vireakboth Oeung**

Cybersecurity Student | Aspiring Security Professional

- [GitHub Profile](https://github.com/oeungvireakboth)
- [Portfolio Website](https://github.com/oeungvireakboth/portfolio)

---

*Built with C#, .NET 10, SQLite, and a focus on secure software development.*
