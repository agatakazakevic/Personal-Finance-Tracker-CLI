# Personal Finance Tracker CLI — Plan

A command-line application for tracking personal income and expenses. Users can log transactions, categorize them, set monthly budgets, filter and generate summary reports, and export data to CSV or JSON.

## General Rules

- **Commit after every task**, not at the end of a milestone
- **Read the compiler error before googling** — most C# errors are descriptive
- **Don't copy-paste solutions** — type everything, even from examples
- **Refactor as you go** — if something feels messy after a milestone, clean it up before starting the next one
- **Do not use LLMs to generate code** use them to guide you and explain things you do not understand. They should not write the code - you should.

> **Note:** The usage examples and sample outputs below show the **final state** of the completed project. You will build toward this incrementally across all milestones — not everything shown here applies from the start.

### Usage Examples

```
$ finance add --amount 45.50 --category Food --desc "Groceries"  # --date defaults to today
$ finance add --amount 2500 --category Salary --desc "Monthly salary" --type income --date 2026-04-01
$ finance list --this-month
$ finance list --category Food --from 2026-04-01 --to 2026-04-30
$ finance edit --id 3 --amount 50.00
$ finance delete --id 7
$ finance summary --month 2026-04
$ finance budget
$ finance list --search "groceries"
$ finance export --format csv --output april.csv
```

### Sample Output — List

```
$ finance list --this-month

  ID  Date        Type     Category    Amount  Description
  --  ----------  -------  ----------  ------  -----------
   1  2026-04-01  Income   Salary     +2500.00  Monthly salary from work
   2  2026-04-03  Expense  Food         -45.50  Groceries
   3  2026-04-05  Expense  Transport    -12.00  Bus pass
   4  2026-04-10  Expense  Food         -32.80  Restaurant
   5  2026-04-15  Expense  Utilities    -85.00  Electricity bill

  5 transactions | Income: $2500.00 | Expenses: $175.30 | Net: +$2324.70
```

### Sample Output — Summary

```
$ finance summary --month 2026-04

  April 2026 Summary
  ==================
  Income:    $2500.00
  Expenses:   $175.30
  Net:       +$2324.70

  Top Spending Categories:
    Utilities   $85.00  ████████████████░░░░  48.5%
    Food        $78.30  ██████████████░░░░░░  44.7%
    Transport   $12.00  ██░░░░░░░░░░░░░░░░░░   6.8%

  Budget Alerts:
    Food        $78.30 / $100.00  (78% used) ⚠ Near limit!
    Utilities   $85.00 / $90.00   (94% used) ⚠ Near limit!
```

---

## Milestone 1 — CLI Skeleton & Add Command

**Goal:** A working CLI app that accepts subcommands, persists data to a JSON file, and can add and list transactions.

| Task | Hints |
|---|---|
| Project setup & git repo | `dotnet new console`, add `.gitignore` |
| Set up a test project alongside the main project | `dotnet new nunit`, add a project reference to the main project |
| Parse subcommands from `args[]` (`add`, `list`, `help`) | `args[0]` is the subcommand, remaining elements are flags and values |
| Implement `add` with flags: `--amount`, `--category`, `--desc`, `--type`, `--date` | Parse flags from `args`, default `--type` to `expense`, default `--date` to today |
| Validate inputs (no negatives, no empty descriptions) | Print a clear error and exit with non-zero code |
| Save transactions to a JSON file after each `add` | `System.Text.Json`, `JsonSerializer`, write to a file next to the executable for now |
| Load transactions from the JSON file on startup | If the file doesn't exist yet, start with an empty list |
| Implement `list` — print all transactions as a formatted table | String interpolation, padding with `PadRight`/`PadLeft` |
| Implement `help` — print available commands and flags | Also print help when no arguments are given |
| Handle unknown subcommands/flags with a useful error | Don't just crash silently |
| Write tests for input validation (rejects negatives, empty descriptions) | `Assert.Throws<ArgumentException>`, test both valid and invalid cases |

**Done when:** `finance add --amount 45.50 --category Food --desc "Groceries"` followed by `finance list` works across separate invocations. `dotnet test` passes.

---

## Milestone 2 — Domain Modeling

**Goal:** Refactor into proper classes. Add edit and delete subcommands.

| Task | Hints |
|---|---|
| Model a transaction as a class | Properties, constructor with validation |
| Distinguish income from expenses | Use an `enum` for `TransactionType` |
| Centralize transaction logic in the `Transaction` class | Keep list management in `Program.cs` for now, Milestone 3 will extract storage into its own class |
| Assign auto-incrementing IDs to transactions | Each transaction gets the next available integer ID |
| Implement `delete` subcommand with `--id` flag | Handle "not found" gracefully, exit with non-zero code. Deleted IDs should never be reused for new transactions |
| Implement `edit` subcommand with `--id` and optional override flags | Only update fields that are provided, keep the rest |
| Write tests for the `Transaction` class (validation, enum, property behavior) | Test the class directly, not through the CLI |

**Done when:** Transaction logic lives in domain classes. `Program.cs` still manages the list but delegates to `Transaction` for validation and modeling. `dotnet test` passes.

---

## Milestone 3 — Robust Persistence

**Goal:** Refactor the basic JSON read/write from Milestone 1 into something production-worthy.

| Task | Hints |
|---|---|
| Extract a `TransactionStore` class that owns the list and handles file I/O | `Program.cs` should not manage the list or touch files directly |
| Handle corrupt/invalid JSON gracefully | `try`/`catch` around deserialization, print a clear error, don't lose the file |
| Handle file system errors (permissions, disk full) | Catch `IOException`, give actionable feedback |
| Ensure auto-incrementing IDs work correctly across restarts | New transactions should never reuse or collide with existing IDs |
| Write tests for `TransactionStore` CRUD operations (add, get, delete, not-found) | Test the store class directly |
| Write tests for persistence edge cases (missing file, empty file, corrupt JSON) | Create temp files in tests, assert expected behavior |

**Done when:** Storage logic is in its own class. The app handles a missing, empty, or corrupt data file without crashing. `dotnet test` passes.

---

## Milestone 4 — Filtering & Reporting

**Goal:** Answer questions about the data using LINQ. Add flags to `list` and a new `summary` subcommand.

| Task | Hints |
|---|---|
| Add `--category` flag to `list` | LINQ `Where` |
| Add shared date flags to `list` and `summary` (see table below) | `--month`/`--year`/`--this-month`/`--this-year` translate to `--from`/`--to` internally |
| Add `--type` flag to `list` (income/expense) | Combine multiple filters |
| Implement `summary` subcommand with the flags below | `Sum`, `GroupBy` |
| Top N spending categories in summary output | `GroupBy`, `OrderByDescending`, `Take` |
| Print a summary report to the console | Format as a readable block, not raw numbers |

**Shared date flags for `list` and `summary`:**

| Flag | Description | Example |
|---|---|---|
| `--from` | Start date (inclusive) | `finance list --from 2026-01-01` |
| `--to` | End date (inclusive) | `finance list --to 2026-03-31` |
| `--month` | Filter by a specific month | `finance summary --month 2026-04` |
| `--year` | Filter by a specific year | `finance summary --year 2026` |
| `--this-month` | Shortcut for the current month | `finance list --this-month` |
| `--this-year` | Shortcut for the current year | `finance summary --this-year` |

| Task | Hints |
|---|---|
| Write tests for filtering and summary logic | Build a known list of transactions, assert filtered results and computed totals |

**Done when:** `finance summary --month 2026-04` shows income, expenses, net, and top categories. `dotnet test` passes.

---

## Milestone 5 — Categories & Budgets

**Goal:** Introduce a config file and budget tracking.

**Example `config.json`:**

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

| Task | Hints |
|---|---|
| Define a list of known categories in a config file | JSON config, loaded at startup |
| Allow configuring the data file path via config | Fall back to a default if not set |
| Reject unknown categories with an error on `add` and `edit` | Validate against the config list, print known categories and point to config.json |
| Set a monthly budget per category in config | `Dictionary<string, decimal>` |
| Warn when a category approaches or exceeds its budget | Check after every `add` and `edit`, warn at threshold and again if over 100% |
| `budget` subcommand — show spent vs. limit per category | A simple bar or percentage. Supports shared date flags and `--category` for single-category drilldown. Defaults to current month |
| Write tests for budget calculations and category validation | Set a budget, add transactions, assert warnings trigger at the right thresholds |

**Example usage:**

```
$ finance add --amount 78.30 --category Food --desc "Weekly shop"
  Added: $78.30 — Weekly shop (Food) on 2026-04-20
  ⚠ Budget warning: Food is at 78% ($78.30 / $100.00)

$ finance add --amount 25.00 --category Food --desc "Takeout"
  Added: $25.00 — Takeout (Food) on 2026-04-22
  ⚠ Budget exceeded: Food is at 103% ($103.30 / $100.00)

$ finance add --amount 50.00 --category Hobbies --desc "Book"
  Unknown category "Hobbies" for type expense. Known categories: Food, Transport, Utilities, Entertainment, Health.
  Add it to config.json to use it.

$ finance budget
  April 2026 Budgets
  ==================
  Food        $103.30 / $100.00  ████████████████████  103% ⚠ OVER
  Utilities    $85.00 / $90.00   ██████████████████░░   94% ⚠ Near limit!
  Transport    $12.00 / $50.00   ████░░░░░░░░░░░░░░░░   24%

$ finance budget --category Food
  Food — April 2026
  ==================
  Budget:  $100.00
  Spent:   $103.30
  Status:  ⚠ Over by $3.30

  Transactions:
    2026-04-20  $78.30  Weekly shop
    2026-04-22  $25.00  Takeout
```

**Done when:** Adding a transaction that pushes Food over budget prints a warning. `dotnet test` passes.

---

## Milestone 6 — Export & Polish

**Goal:** Output data in other formats, handle edge cases, clean up.

| Task | Hints |
|---|---|
| Implement `export` subcommand with `--format` and `--output` flags | Support `csv` and `json` formats |
| CSV export | `StringBuilder` or `StreamWriter`, escape commas in descriptions |
| JSON export | Reuse `System.Text.Json`, pretty-print with `JsonSerializerOptions { WriteIndented = true }` |
| Add `--search` flag to `list` — filters transactions where description contains the search term | Case-insensitive `Contains`, e.g. `finance list --search "groceries"` |
| Handle all error paths — bad files, bad input, empty state | `try`/`catch` at boundaries, friendly messages everywhere |
| Write tests for CSV and JSON export output | Assert the output strings match expected format, test edge cases like commas in descriptions |

**Done when:** App feels solid — no way to crash it with bad input, exports work, output looks good. `dotnet test` passes.


