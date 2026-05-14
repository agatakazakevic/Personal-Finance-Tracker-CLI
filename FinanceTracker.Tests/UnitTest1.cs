namespace FinanceTracker.Tests;

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
}

