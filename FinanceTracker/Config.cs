
public class Config
{
    public Dictionary<string, List<string>> Categories { get; set; } = new();
    public Dictionary<string, decimal> Budgets { get; set; } = new();
    public decimal BudgetWarningThreshold { get; set; } = 0.75m;
    public string DataFilePath { get; set; } = "./transactions.json";
}