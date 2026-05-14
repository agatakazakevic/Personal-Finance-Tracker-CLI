using System.Text.Json;
using System.Linq;

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
        //load config.json
        string configFile="config.json";
        Config config;

        if (File.Exists(configFile))
        {
            string configJson=File.ReadAllText(configFile); //raw json file
            config=JsonSerializer.Deserialize<Config>(configJson)?? new Config();//turn into config file
        }
        else
        {
            config = new Config();
        }



        var store = new TransactionStore(config.DataFilePath);
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
                    store.Add(transaction);
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
                var transactions = store.GetAll();


                string? fromDate = GetFlagValue(args, "--from");
                string? toDate = GetFlagValue(args, "--to");
                string? monthFilter = GetFlagValue(args, "--month");
                string? yearFilter = GetFlagValue(args, "--year");
                bool thisMonth = args.Contains("--this-month");
                bool thisYear = args.Contains("--this-year");
                if (thisMonth)
                {
                    var now=DateTime.Now;
                    fromDate=new DateTime(now.Year, now.Month, 1).ToString("yyyy-MM-dd");
                    toDate=new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month)).ToString("yyyy-MM-dd");

                }
                if (thisYear)
                {
                    var now=DateTime.Now;
                    fromDate=new DateTime(now.Year, 1, 1).ToString("yyyy-MM-dd");
                    toDate=new DateTime(now.Year, 12, DateTime.DaysInMonth(now.Year, 12)).ToString("yyyy-mm-dd");

                }
                if (monthFilter != null)
                {
                    var parts = monthFilter.Split('-');
                    int year = int.Parse(parts[0]);
                    int month = int.Parse(parts[1]);
                    fromDate = new DateTime(year, month, 1).ToString("yyyy-MM-dd");
                    toDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)).ToString("yyyy-MM-dd");
                }
                if (yearFilter != null)
                {
                    int year = int.Parse(yearFilter);
                    fromDate = new DateTime(year, 1, 1).ToString("yyyy-MM-dd");
                    toDate = new DateTime(year, 12, 31).ToString("yyyy-MM-dd");
                }
                //need to filter by fromdate and todate; keep only transactions where the date is on or after fromdate
                if (fromDate != null)
                {
                    transactions=transactions.Where(t=>String.Compare(t.Date, fromDate)>=0).ToList();
                }
                if (toDate != null)
                {
                    transactions=transactions.Where(t=>String.Compare(t.Date, toDate)<=0).ToList();
                }

                string? typeFilter = GetFlagValue(args, "--type");

                if (typeFilter != null)
                {
                    if(Enum.TryParse<TransactionType>(typeFilter, true, out TransactionType parsedtype )){

                    transactions=transactions.Where(t=>t.Type==parsedtype).ToList();

                    }
                    
                }

                string? filterCategory = GetFlagValue(args, "--category");
                if (filterCategory!=null)
                {
                    transactions=transactions.Where(t => t.Category.ToLower()==filterCategory.ToLower()).ToList();
                }

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
            case "delete":
                string? id = GetFlagValue(args, "--id");
                if (!int.TryParse(id, out int parsedId))
                {
                    Console.Error.WriteLine("Invalid or missing ID");
                    Environment.Exit(1);
                }

                var found = store.FindById(parsedId);
                 if (found == null)
                {
                    Console.Error.WriteLine($"Transaction with ID {parsedId} not found");
                    Environment.Exit(1);
                }
                store.Delete(parsedId);
                Console.WriteLine($"Deleted transaction ID {parsedId}");
                break;

            case "edit":
                string? editId = GetFlagValue(args, "--id");
                if (!int.TryParse(editId, out int editParsedId))
                {
                    Console.Error.WriteLine("Invalid or missing ID");
                    Environment.Exit(1);
                }
                var editfound=store.FindById(editParsedId);
                if (editfound==null)
                {
                    Console.Error.WriteLine($"Transaction with ID {editParsedId} not found");
                    Environment.Exit(1);
                }
                else 
                    {
                        
                        string? newAmount = GetFlagValue(args, "--amount");
                        string? newCategory = GetFlagValue(args, "--category");
                        string? newDesc = GetFlagValue(args, "--desc");
                        string? newType = GetFlagValue(args, "--type");
                        string? newDate = GetFlagValue(args, "--date");

                        if (newAmount != null)
                        {
                            if (!decimal.TryParse(newAmount, out decimal newAmountParsed))
                                {
                                    Console.Error.WriteLine("Invalid amount: must be a number");
                                    Environment.Exit(1);
                                }
                            else
                            {
                                editfound.Amount=newAmountParsed;
                            }

                        }
                        if (newDesc != null)
                        {
                                editfound.Description=newDesc;
                            
                        }
                        if (newCategory != null)
                        {
                                editfound.Category=newCategory;
                        }
                        if (newType != null)
                        {
                            if (Enum.TryParse<TransactionType>(newType, true, out TransactionType parsedNewType))
                            {
                                editfound.Type = parsedNewType;
                            }
                            else
                            {
                                Console.Error.WriteLine("Invalid type: must be 'income' or 'expense'");
                                Environment.Exit(1);
                            }
                        }
                        if (newDate != null)
                        {
                                editfound.Date=newDate;
                        }

                        Console.WriteLine($"Edited transaction ID {editParsedId}");
                        
                    }
                    store.Save();
                    break;
            case "summary":
                var summaryTransactions = store.GetAll();
                string? summaryFrom = GetFlagValue(args, "--from");
                string? summaryTo = GetFlagValue(args, "--to");
                string? summaryMonth = GetFlagValue(args, "--month");
                string? summaryYear = GetFlagValue(args, "--year");
                bool summaryThisMonth = args.Contains("--this-month");
                bool summaryThisYear = args.Contains("--this-year");

                if (summaryThisMonth)
                {
                    var now = DateTime.Now;
                    summaryFrom = new DateTime(now.Year, now.Month, 1).ToString("yyyy-MM-dd");
                    summaryTo = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month)).ToString("yyyy-MM-dd");
                }
                if (summaryThisYear)
                {
                    var now = DateTime.Now;
                    summaryFrom = new DateTime(now.Year, 1, 1).ToString("yyyy-MM-dd");
                    summaryTo = new DateTime(now.Year, 12, 31).ToString("yyyy-MM-dd");
                }
                if (summaryMonth != null)
                {
                    var parts = summaryMonth.Split('-');
                    int year = int.Parse(parts[0]);
                    int month = int.Parse(parts[1]);
                    summaryFrom = new DateTime(year, month, 1).ToString("yyyy-MM-dd");
                    summaryTo = new DateTime(year, month, DateTime.DaysInMonth(year, month)).ToString("yyyy-MM-dd");
                }
                if (summaryYear != null)
                {
                    int year = int.Parse(summaryYear);
                    summaryFrom = new DateTime(year, 1, 1).ToString("yyyy-MM-dd");
                    summaryTo = new DateTime(year, 12, 31).ToString("yyyy-MM-dd");
                }
                if (summaryFrom != null)
                {
                    summaryTransactions = summaryTransactions
                        .Where(t => string.Compare(t.Date, summaryFrom) >= 0)
                        .ToList();
                }
                if (summaryTo != null)
                {
                    summaryTransactions = summaryTransactions
                        .Where(t => string.Compare(t.Date, summaryTo) <= 0)
                        .ToList();
                }
                decimal summaryIncome=0;
                decimal summaryExpences=0;

                foreach(var t in summaryTransactions)
                {
                    if (t.Type == TransactionType.Income)
                        summaryIncome += t.Amount;
                    else
                        summaryExpences += t.Amount;
                }
                decimal Total= 0;
                Total=summaryIncome-summaryExpences;
                string summarySign = Total >= 0 ? "+" : "-";
                Console.WriteLine();
                Console.WriteLine("  Summary");
                Console.WriteLine("  ==================");
                Console.WriteLine($"  Income:    ${summaryIncome:F2}");
                Console.WriteLine($"  Expenses:  ${summaryExpences:F2}");
                Console.WriteLine($"  Net:       {summarySign}${Math.Abs(Total):F2}");

                //groupby gives key, then we sleect by that key
                var groupbycategory=summaryTransactions.Where(t=> t.Type==TransactionType.Expense).
                GroupBy(t=>t.Category).
                Select(g=> new{Category=g.Key, Total=g.Sum(t=> t.Amount)}).
                OrderByDescending(g=> g.Total).ToList();
            
                //f1-one decimal place
                //f2-two decimal place
                    if (groupbycategory.Count > 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("  Top Spending Categories:");
                    foreach (var g in groupbycategory)
                    {
                        decimal percent = (summaryExpences > 0) ? (g.Total / summaryExpences * 100) : 0;
                        Console.WriteLine($"    {g.Category.PadRight(12)} ${g.Total:F2}   {percent:F1}%");
                    }
                }

    Console.WriteLine();
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