public class Transaction
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string Category { get; set; } = "";
    public string Description { get; set; } = "";
    public TransactionType Type { get; set; }
    public string Date { get; set; } = "";

    //parametress constructor
    public Transaction() { }

    //contructor with validation
    public Transaction(decimal amount, string category, string description, TransactionType type, string date)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Amount must be greater than zero");
        }
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Description cannot be empty");
        }
        if (string.IsNullOrWhiteSpace(category))
        {
            throw new ArgumentException("Category is required");
        }

        Amount = amount;
        Category = category;
        Description = description;
        Type = type;
        Date = date;
    }

}

