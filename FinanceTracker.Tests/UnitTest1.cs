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
}

