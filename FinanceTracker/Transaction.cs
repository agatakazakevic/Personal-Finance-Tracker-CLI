public class Transaction
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string Category { get; set; } = "";
    public string Description { get; set; } = "";
    public string Type { get; set; } = "expense";
    public string Date { get; set; } = "";
}