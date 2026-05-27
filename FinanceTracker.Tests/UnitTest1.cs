namespace FinanceTracker.Tests;
using System.Linq;
public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        Assert.Pass();
    }
    [Test]
    public void ValidateAmount_RejectsNegative()
    {
        Assert.Throws<ArgumentException>(() => Program.ValidateAmount("-5"));
    }

    [Test]
    public void ValidateAmount_RejectsZero()
    {
        Assert.Throws<ArgumentException>(() => Program.ValidateAmount("0"));
    }

    [Test]
    public void ValidateAmount_RejectsNonNumber()
    {
        Assert.Throws<ArgumentException>(() => Program.ValidateAmount("abc"));
    }

    [Test]
    public void ValidateAmount_RejectsNull()
    {
        Assert.Throws<ArgumentException>(() => Program.ValidateAmount(null));
    }

    [Test]
    public void ValidateAmount_AcceptsValid()
    {
        decimal result = Program.ValidateAmount("45.50");
        Assert.That(result, Is.EqualTo(45.50m));
    }

    [Test]
    public void ValidateDescription_RejectsEmpty()
    {
        Assert.Throws<ArgumentException>(() => Program.ValidateDescription(""));
    }

    [Test]
    public void ValidateDescription_RejectsWhitespace()
    {
        Assert.Throws<ArgumentException>(() => Program.ValidateDescription("   "));
    }

    [Test]
    public void ValidateDescription_RejectsNull()
    {
        Assert.Throws<ArgumentException>(() => Program.ValidateDescription(null));
    }

    [Test]
    public void ValidateDescription_AcceptsValid()
    {
        string result = Program.ValidateDescription("Groceries");
        Assert.That(result, Is.EqualTo("Groceries"));
    }

    [Test]
    public void Transaction_RejectsNegativeAmount()
    {
        Assert.Throws<ArgumentException>(() => new Transaction(-5, "Food", "Test", TransactionType.Expense, "2026-05-04"));
    }
    [Test]
    public void Transaction_RejectsZeroAmount()
    {
        Assert.Throws<ArgumentException>(() => new Transaction(0, "Food", "Test", TransactionType.Expense, "2026-05-04"));
    }

    [Test]
    public void Transaction_RejectsEmptyDescription()
    {
        Assert.Throws<ArgumentException>(() => new Transaction(10, "Food", "", TransactionType.Expense, "2026-05-04"));
    }

    [Test]
    public void Transaction_RejectsWhitespaceDescription()
    {
        Assert.Throws<ArgumentException>(() => new Transaction(10, "Food", "   ", TransactionType.Expense, "2026-05-04"));
    }

    [Test]
    public void Transaction_RejectsEmptyCategory()
    {
        Assert.Throws<ArgumentException>(() => new Transaction(10, "", "Test", TransactionType.Expense, "2026-05-04"));
    }

    [Test]
    public void Transaction_AcceptsValidInput()
    {
        var t = new Transaction(45.50m, "Food", "Groceries", TransactionType.Expense, "2026-05-04");
        Assert.That(t.Amount, Is.EqualTo(45.50m));
        Assert.That(t.Category, Is.EqualTo("Food"));
        Assert.That(t.Description, Is.EqualTo("Groceries"));
        Assert.That(t.Type, Is.EqualTo(TransactionType.Expense));
        Assert.That(t.Date, Is.EqualTo("2026-05-04"));
    }

    [Test]
    public void Transaction_IncomeTypeWorks()
    {
        var t = new Transaction(2500, "Salary", "Monthly pay", TransactionType.Income, "2026-05-04");
        Assert.That(t.Type, Is.EqualTo(TransactionType.Income));
    }

    [Test]
    public void AddAndGetAll()
    {
        string tempFile = Path.GetTempFileName();
        var store = new TransactionStore(tempFile);
        store.Add(new Transaction(50, "Food", "Groceries", TransactionType.Expense, "2026-05-06"));

        Assert.That(store.GetAll().Count, Is.EqualTo(1));
        File.Delete(tempFile);
    }
    [Test]
    public void Store_FindById_ReturnsCorrect()
    {
        string tempFile = Path.GetTempFileName();
        var store = new TransactionStore(tempFile);
        store.Add(new Transaction(10, "Food", "First", TransactionType.Expense, "2026-05-06"));
        store.Add(new Transaction(20, "Food", "Second", TransactionType.Expense, "2026-05-06"));

        var found = store.FindById(2);
        Assert.That(found, Is.Not.Null);
        Assert.That(found.Description, Is.EqualTo("Second"));
        File.Delete(tempFile);
    }
    [Test]
    public void Store_FindById_ReturnsNullWhenNotFound()
    {
        string tempFile = Path.GetTempFileName();
        var store = new TransactionStore(tempFile);

        Assert.That(store.FindById(99), Is.Null);
        File.Delete(tempFile);
    }

    [Test]
    public void Store_Delete_RemovesTransaction()
    {
        string tempFile = Path.GetTempFileName();
        var store = new TransactionStore(tempFile);
        store.Add(new Transaction(10, "Food", "Test", TransactionType.Expense, "2026-05-06"));

        store.Delete(1);
        Assert.That(store.GetAll().Count, Is.EqualTo(0));
        File.Delete(tempFile);
    }

    [Test]
    public void Store_DeletedId_NeverReused()
    {
        string tempFile = Path.GetTempFileName();
        var store = new TransactionStore(tempFile);
        store.Add(new Transaction(10, "Food", "First", TransactionType.Expense, "2026-05-06"));
        store.Add(new Transaction(20, "Food", "Second", TransactionType.Expense, "2026-05-06"));
        store.Delete(2);
        store.Add(new Transaction(30, "Food", "Third", TransactionType.Expense, "2026-05-06"));

        var third = store.FindById(3);
        Assert.That(third, Is.Not.Null);
        Assert.That(third.Description, Is.EqualTo("Third"));
        File.Delete(tempFile);
    }

    [Test]
    public void Store_MissingFile_StartsEmpty()
    {
        string tempFile = Path.Combine(Path.GetTempPath(), "nonexistent.json");
        if (File.Exists(tempFile)) File.Delete(tempFile);

        var store = new TransactionStore(tempFile);
        Assert.That(store.GetAll().Count, Is.EqualTo(0));
    }

    [Test]
    public void Store_EmptyFile_StartsEmpty()
    {
        string tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, "");

        var store = new TransactionStore(tempFile);
        Assert.That(store.GetAll().Count, Is.EqualTo(0));
        File.Delete(tempFile);
    }

    [Test]
    public void Store_CorruptJson_StartsEmpty()
    {
        string tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, "this is not json!!!");

        var store = new TransactionStore(tempFile);
        Assert.That(store.GetAll().Count, Is.EqualTo(0));
        File.Delete(tempFile);
    }
    [Test]
    public void Store_FilterByCategory()
    {
        string tempFile = Path.GetTempFileName();
        var store = new TransactionStore(tempFile);
        store.Add(new Transaction(10, "Food", "Lunch", TransactionType.Expense, "2026-05-01"));
        store.Add(new Transaction(20, "Transport", "Bus", TransactionType.Expense, "2026-05-01"));
        store.Add(new Transaction(30, "Food", "Dinner", TransactionType.Expense, "2026-05-01"));

        var filtered = store.GetAll()
            .Where(t => t.Category.ToLower() == "food")
            .ToList();

        Assert.That(filtered.Count, Is.EqualTo(2));
        File.Delete(tempFile);
    }

    [Test]
    public void Store_FilterByDateRange()
    {
        string tempFile = Path.GetTempFileName();
        var store = new TransactionStore(tempFile);
        store.Add(new Transaction(10, "Food", "Jan", TransactionType.Expense, "2026-01-15"));
        store.Add(new Transaction(20, "Food", "May", TransactionType.Expense, "2026-05-10"));
        store.Add(new Transaction(30, "Food", "Dec", TransactionType.Expense, "2026-12-01"));

        var filtered = store.GetAll()
            .Where(t => string.Compare(t.Date, "2026-05-01") >= 0)
            .Where(t => string.Compare(t.Date, "2026-05-31") <= 0)
            .ToList();

        Assert.That(filtered.Count, Is.EqualTo(1));
        Assert.That(filtered[0].Description, Is.EqualTo("May"));
        File.Delete(tempFile);
    }

    [Test]
    public void Store_FilterByType()
    {
        string tempFile = Path.GetTempFileName();
        var store = new TransactionStore(tempFile);
        store.Add(new Transaction(100, "Food", "Lunch", TransactionType.Expense, "2026-05-01"));
        store.Add(new Transaction(2500, "Salary", "Pay", TransactionType.Income, "2026-05-01"));

        var incomeOnly = store.GetAll()
            .Where(t => t.Type == TransactionType.Income)
            .ToList();

        Assert.That(incomeOnly.Count, Is.EqualTo(1));
        Assert.That(incomeOnly[0].Category, Is.EqualTo("Salary"));
        File.Delete(tempFile);
    }

    [Test]
    public void Summary_CalculatesTotalsCorrectly()
    {
        string tempFile = Path.GetTempFileName();
        var store = new TransactionStore(tempFile);
        store.Add(new Transaction(2500, "Salary", "Pay", TransactionType.Income, "2026-05-01"));
        store.Add(new Transaction(45, "Food", "Groceries", TransactionType.Expense, "2026-05-01"));
        store.Add(new Transaction(12, "Transport", "Bus", TransactionType.Expense, "2026-05-01"));

        var all = store.GetAll();
        decimal income = all.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
        decimal expenses = all.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);

        Assert.That(income, Is.EqualTo(2500));
        Assert.That(expenses, Is.EqualTo(57));
        Assert.That(income - expenses, Is.EqualTo(2443));
        File.Delete(tempFile);
    }

    [Test]
    public void Config_RejectsUnknownCategory()
    {
        var categories = new List<string> { "Food", "Transport", "Utilities" };
        Assert.That(categories.Contains("Hobbies"), Is.False);
        Assert.That(categories.Contains("Food"), Is.True);
    }

    [Test]
    public void Budget_CalculatesPercentCorrectly()
    {
        decimal budget = 100m;
        decimal spent = 75m;
        decimal percent = spent / budget;
        Assert.That(percent, Is.EqualTo(0.75m));
    }

    [Test]
    public void Budget_DetectsOverBudget()
    {
        decimal budget = 100m;
        decimal spent = 103m;
        decimal percent = spent / budget;
        Assert.That(percent > 1, Is.True);
    }

    [Test]
    public void Budget_DetectsNearLimit()
    {
        decimal budget = 100m;
        decimal spent = 78m;
        decimal threshold = 0.75m;
        decimal percent = spent / budget;
        Assert.That(percent >= threshold, Is.True);
        Assert.That(percent <= 1, Is.True);
    }

    [Test]
    public void Budget_UnderThresholdNoWarning()
    {
        decimal budget = 100m;
        decimal spent = 20m;
        decimal threshold = 0.75m;
        decimal percent = spent / budget;
        Assert.That(percent < threshold, Is.True);
    }

    [Test]

    public void Export_CsvFormat()
    {
        string tempFile=Path.GetTempFileName();
        var store = new TransactionStore(tempFile);
        store.Add(new Transaction(50, "Food", "Groceries", TransactionType.Expense, "2026-05-27"));
        store.Add(new Transaction(2500, "Salary", "Pay", TransactionType.Income, "2026-05-27"));

        var csv = new System.Text.StringBuilder();
        csv.AppendLine("Id,Date,Type,Category,Amount,Description");
        foreach (var t in store.GetAll())
        {
            csv.AppendLine($"{t.Id},{t.Date},{t.Type},{t.Category},{t.Amount},{t.Description}");
        }

        string result = csv.ToString();
        Assert.That(result, Does.Contain("Id,Date,Type,Category,Amount,Description"));
        Assert.That(result, Does.Contain("Food"));
        Assert.That(result, Does.Contain("Salary"));
        File.Delete(tempFile);

    }
    [Test]
    public void Export_CsvEscapesCommas()
    {
        string tempFile = Path.GetTempFileName();
        var store = new TransactionStore(tempFile);
        store.Add(new Transaction(50, "Food", "Rice, beans, and chicken", TransactionType.Expense, "2026-05-19"));

        var t = store.GetAll()[0];
        string desc = t.Description.Contains(',') ? $"\"{t.Description}\"" : t.Description;

        Assert.That(desc, Is.EqualTo("\"Rice, beans, and chicken\""));
        File.Delete(tempFile);
    }

    [Test]
    public void Export_JsonFormat()
    {
        string tempFile = Path.GetTempFileName();
        var store = new TransactionStore(tempFile);
        store.Add(new Transaction(50, "Food", "Groceries", TransactionType.Expense, "2026-05-19"));

        var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
        string json = System.Text.Json.JsonSerializer.Serialize(store.GetAll(), options);

        Assert.That(json, Does.Contain("Food"));
        Assert.That(json, Does.Contain("Groceries"));
        Assert.That(json, Does.Contain("50"));
        File.Delete(tempFile);
    }

    [Test]
    public void Search_FiltersByDescription()
    {
        string tempFile = Path.GetTempFileName();
        var store = new TransactionStore(tempFile);
        store.Add(new Transaction(50, "Food", "Weekly shop", TransactionType.Expense, "2026-05-19"));
        store.Add(new Transaction(25, "Food", "Takeout", TransactionType.Expense, "2026-05-19"));

        var results = store.GetAll()
            .Where(t => t.Description.ToLower().Contains("shop"))
            .ToList();

        Assert.That(results.Count, Is.EqualTo(1));
        Assert.That(results[0].Description, Is.EqualTo("Weekly shop"));
        File.Delete(tempFile);
    }

    [Test]
    public void Search_CaseInsensitive()
    {
        string tempFile = Path.GetTempFileName();
        var store = new TransactionStore(tempFile);
        store.Add(new Transaction(50, "Food", "Weekly Shop", TransactionType.Expense, "2026-05-19"));

        var results = store.GetAll()
            .Where(t => t.Description.ToLower().Contains("shop"))
            .ToList();

        Assert.That(results.Count, Is.EqualTo(1));
        File.Delete(tempFile);
    }
}

