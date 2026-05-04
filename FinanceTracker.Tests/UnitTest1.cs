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

}

