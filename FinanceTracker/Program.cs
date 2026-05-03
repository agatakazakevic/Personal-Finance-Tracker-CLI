
using System.Text.Json;

public class Program
{
    public static decimal ValidateAmount(string? amount)
    {
        if (string.IsNullOrWhiteSpace(amount))
        {
            throw new ArgumentException("Amount is required");
        }
        if (!decimal.TryParse(amount, out decimal parsedAmount))
        {
            throw new ArgumentException("Invalid amount: must be a number");
        }
        if (parsedAmount <= 0)
        {
            throw new ArgumentException("Invalid amount: must be greater than zero");
        }
        return parsedAmount;
    }
    public static string ValidateDescription(string? desc)
    {
        if (string.IsNullOrWhiteSpace(desc))
        {
            throw new ArgumentException("Description cannot be empty");
        }
        return desc;
    }
    private static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Please write the command or type help");

            return;
        }
        string command = args[0].ToLower();
        string dataFile = "transactions.json";
        List<Transaction> transactions;
        if (File.Exists(dataFile))
        {
            string json = File.ReadAllText(dataFile);
            var result = JsonSerializer.Deserialize<List<Transaction>>(json);
            if (result != null)
                transactions = result;
            else
                transactions = new List<Transaction>();
        }
        else
        {
            transactions = new List<Transaction>();
        }
        switch (command)
        {
            
            case "add":

                string? amount = GetFlagValue(args, "--amount");
                string? category = GetFlagValue(args, "--category");
                string? desc = GetFlagValue(args, "--desc");
                string? type = GetFlagValue(args, "--type") ?? "expense";
                string? date = GetFlagValue(args, "--date") ?? DateTime.Now.ToString("yyyy-MM-dd");
                
                try
                {
                    decimal parsedAmount = ValidateAmount(amount);
                    string validDesc = ValidateDescription(desc);
                    if (string.IsNullOrWhiteSpace(category))
                    {
                        Console.Error.WriteLine("Category is required");
                        Environment.Exit(1);
                    }

                    TransactionType parsedType;
                    if (type == "income")
                    {
                        parsedType = TransactionType.Income;
                    }
                    else
                    {
                        parsedType = TransactionType.Expense;
                    }
                    var transaction = new Transaction(parsedAmount, category, validDesc, parsedType, date);
                    transaction.Id = transactions.Count + 1;
                    transactions.Add(transaction);

                    var options = new JsonSerializerOptions { WriteIndented = true };
                    string updatedJson = JsonSerializer.Serialize(transactions, options);
                    File.WriteAllText(dataFile, updatedJson);
                    Console.WriteLine($"Amount: {parsedAmount} | Category: {category} | Desc: {validDesc} | Type: {type} | Date: {date}");
                }
                catch (ArgumentException ex)
                {
                    Console.Error.WriteLine(ex.Message);
                    Environment.Exit(1);
                }
                
                break;
            case "list":
                Console.WriteLine("  ID    Date        Type     Category    Amount    Description");
                Console.WriteLine("  --    ----------  -------  ----------  ------    -----------");

                decimal totalIncome = 0;
                decimal totalExpenses = 0;

                foreach (var t in transactions)
                {
                    string prefix;
                    
                    if (t.Type == TransactionType.Income)
                    {
                        prefix = "+";
                    }
                    else
                    {
                        prefix = "-";
                    }


                    string amountStr = $"{prefix}{t.Amount:F2}";

                    Console.WriteLine("  " + t.Id.ToString().PadLeft(2) + "    "
                    + t.Date.PadRight(12)
                    + t.Type.ToString().PadRight(9)
                    + t.Category.PadRight(12)
                    + amountStr.PadLeft(8) + "  "
                    + t.Description);

                    if (t.Type == TransactionType.Income)
                        totalIncome += t.Amount;
                    else
                        totalExpenses += t.Amount;
                }

                decimal net = totalIncome - totalExpenses;
                string netSign;
                if (net >= 0)
                {
                    netSign = "+";
                }
                else
                {
                    netSign = "-";
                }
                string netStr = netSign + "$" + Math.Abs(net).ToString("F2");
                Console.WriteLine();
                Console.WriteLine($"  {transactions.Count} transactions | Income: ${totalIncome:F2} | Expenses: ${totalExpenses:F2} | Net: {netStr}");
                break;
            case "help":
                Console.WriteLine("Usage: finance <command> [options]");
                Console.WriteLine();
                Console.WriteLine("Commands:");
                Console.WriteLine("  add     Add a transaction");
                Console.WriteLine("  --amount    Amount (required)");
                Console.WriteLine("  --category  Category (required)");
                Console.WriteLine("  --desc      Description (required)");
                Console.WriteLine("  --type      income or expense (default: expense)");
                Console.WriteLine("  --date      Date YYYY-MM-DD (default: today)");
                Console.WriteLine("  list    List all transactions");
                Console.WriteLine("  help    Show this help message");
                break;
            default:
                Console.Error.WriteLine($"Unknown command: '{command}'. Run 'help' for usage.");
                Environment.Exit(1);
                break;

        }
        static string? GetFlagValue(string[] args, string flag)
        {  
            for(int i=0; i<args.Length; i++)
            {
                if (args[i] == flag && i+1<args.Length)
                {
                    return args[i+1];
                }
            }
            return null;

        }
    }
}