
using System.Text.Json;
internal class Program
{
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
                if (amount ==null || category==null || desc==null)
                {
                    Console.Error.WriteLine("Missing required flag: --amount, --category, and --desc are required");
                    Environment.Exit(1);
                }
                if (!decimal.TryParse(amount, out decimal parsedAmount))
                {
                    Console.Error.WriteLine("Invalid amount, must be a number");
                    Environment.Exit(1);
                }
                if (string.IsNullOrWhiteSpace(desc) )
                {
                    Console.Error.WriteLine("Description cannot be empty");
                    Environment.Exit(1);
                }
                if (parsedAmount <= 0)
                {
                    Console.Error.WriteLine("Invalid amount, must be greater than zero");
                    Environment.Exit(1);
                }  
                
                var transaction = new Transaction{
                        Id = transactions.Count + 1,
                        Amount = parsedAmount,
                        Category = category,
                        Description = desc,
                        Type = type,
                        Date = date
                    };
                transactions.Add(transaction);

                var options = new JsonSerializerOptions { WriteIndented = true };
                string updatedJson = JsonSerializer.Serialize(transactions, options);
                File.WriteAllText(dataFile, updatedJson);
                Console.WriteLine($"Amount: {parsedAmount} | Category: {category} | Desc: {desc} | Type: {type} | Date: {date}");
                break;

            case "list":
                Console.WriteLine($"  {"ID",4}  {"Date",-10}  {"Type",-7}  {"Category",-10}  {"Amount",8}  {"Description"}");
                Console.WriteLine($"  {"--",4}  {"----------",-10}  {"-------",-7}  {"----------",-10}  {"------",8}  {"-----------"}");

                decimal totalIncome = 0;
                decimal totalExpenses = 0;

                foreach (var t in transactions)
                {
                    string prefix;
                    if (t.Type == "income")
                    {
                        prefix = "+";
                    }
                    else
                    {
                        prefix = "-";
}
                    string amountStr = $"{prefix}{t.Amount:F2}";

                    Console.WriteLine($"  {t.Id,4}  {t.Date,-10}  {t.Type,-7}  {t.Category,-10}  {amountStr,8}  {t.Description}");

                    if (t.Type == "income")
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