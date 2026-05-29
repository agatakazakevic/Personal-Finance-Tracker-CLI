# Personal Finance Tracker CLI

A command-line application for tracking personal income and expenses, built with C# and .NET. Users can log transactions, categorize them, set monthly budgets, filter and generate summary reports, and export data to CSV or JSON.

## Features

- **Add, edit, and delete transactions** with validation
- **List transactions** with filtering by category, type, date range, and search
- **Monthly summaries** with income, expenses, net, and top spending categories
- **Budget tracking** with configurable limits and warnings at 75% and 100%
- **Export** to CSV or JSON formats
- **Persistent storage** using JSON with auto-incrementing IDs that are never reused
- **Config file** for categories, budgets, and data file path

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) (or compatible version)

### Build and Run

```bash
cd FinanceTracker
dotnet build
dotnet run -- help
```

### Run Tests

```bash
# From the root (solution) folder
dotnet test
```

## Usage

### Add a transaction

```bash
# Expense (default type)
dotnet run -- add --amount 45.50 --category Food --desc "Groceries"

# Income
dotnet run -- add --amount 2500 --category Salary --desc "Monthly salary" --type income --date 2026-05-01
```

### List transactions

```bash
# All transactions
dotnet run -- list

# Filter by category
dotnet run -- list --category Food

# Filter by type
dotnet run -- list --type income

# Filter by date
dotnet run -- list --this-month
dotnet run -- list --month 2026-05
dotnet run -- list --year 2026
dotnet run -- list --from 2026-05-01 --to 2026-05-31

# Search by description
dotnet run -- list --search "groceries"
```

### Edit a transaction

```bash
# Only updates the fields you provide
dotnet run -- edit --id 1 --amount 50.00
dotnet run -- edit --id 1 --desc "Updated groceries" --category Transport
```

### Delete a transaction

```bash
dotnet run -- delete --id 3
```

### Monthly summary

```bash
dotnet run -- summary --this-month
dotnet run -- summary --month 2026-05
```

### Budget overview

```bash
# All categories
dotnet run -- budget

# Single category drilldown
dotnet run -- budget --category Food
```

### Export data

```bash
dotnet run -- export --format csv --output transactions.csv
dotnet run -- export --format json --output transactions.json
```

## Sample Output

### List

```
  ID    Date        Type     Category    Amount    Description
  --    ----------  -------  ----------  ------    -----------
   1    2026-05-01  Income   Salary      +2500.00  Monthly salary
   2    2026-05-03  Expense  Food          -45.50  Groceries
   3    2026-05-05  Expense  Transport     -12.00  Bus pass

  3 transactions | Income: $2500.00 | Expenses: $57.50 | Net: +$2442.50
```

### Budget

```
  Budgets
  ==================
  Food         $82.80 / $100.00  ████████████████░░░░  83% ⚠ Near limit!
  Transport    $12.00 / $50.00   ████░░░░░░░░░░░░░░░░  24%
  Utilities    $85.00 / $90.00   ██████████████████░░  94% ⚠ Near limit!
```

## Configuration

Edit `config.json` to customize categories, budgets, and settings:

```json
{
  "categories": {
    "expense": ["Food", "Transport", "Utilities", "Entertainment", "Health"],
    "income": ["Salary", "Freelance", "Investments"]
  },
  "budgets": {
    "Food": 100.00,
    "Transport": 50.00,
    "Utilities": 90.00,
    "Entertainment": 75.00,
    "Health": 60.00
  },
  "budgetWarningThreshold": 0.75,
  "dataFilePath": "./transactions.json"
}
```

## Project Structure

```
Finance tracker/
├── FinanceTracker.sln
├── FinanceTracker/
│   ├── Program.cs              # CLI parsing and commands
│   ├── Transaction.cs          # Domain model with validation
│   ├── TransactionType.cs      # Income/Expense enum
│   ├── TransactionStore.cs     # CRUD operations and file I/O
│   ├── StoreData.cs            # JSON wrapper (persists nextId)
│   ├── Config.cs               # Categories and budget settings
│   ├── config.json             # User configuration
│   └── transactions.json       # Persisted transaction data
└── FinanceTracker.Tests/
    └── UnitTest1.cs            # 39 NUnit tests
```

## Technologies Used

- C# / .NET
- System.Text.Json for serialization
- LINQ for filtering and aggregation
- NUnit for testing
