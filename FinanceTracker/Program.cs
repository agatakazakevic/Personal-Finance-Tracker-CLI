
using System.Text.Json;
internal class Program
{
    private static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Please write the command");
            return;
        }
        string command = args[0].ToLower();
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
                    Console.Error.WriteLine("Invalid amount: must be a number");
                    Environment.Exit(1);
                }
                if (string.IsNullOrWhiteSpace(desc) )
                {
                    Console.Error.WriteLine("Description cannot be empty");
                    Environment.Exit(1);
                }
                if (parsedAmount <= 0)
                {
                    Console.Error.WriteLine("Invalid amount: must be greater than zero");
                    Environment.Exit(1);
                }  
                string dataFile = "transactions.json";
                List<Transaction> transactions;

                if (File.Exists(dataFile))
                {
                    string json = File.ReadAllText(dataFile);
                    transactions = JsonSerializer.Deserialize<List<Transaction>>(json) ?? new List<Transaction>();
                }
                else
                {
                    transactions = new List<Transaction>();
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
                Console.Error.WriteLine("Not yet implemented");
                break;
            case "help":
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