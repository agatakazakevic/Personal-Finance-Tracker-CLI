using System.Text.Json;

public class TransactionStore
{
    private List<Transaction> transactions;
    private string dataFile;

    public TransactionStore(string dataFile)
    {
        this.dataFile=dataFile;
    }

    public void Add(Transaction transaction)
    {
        //assign next id
        //add to list
        //save to file
        int NextId;
        if (transactions.Count > 0)
            {
                int maxId = 0;
                foreach (var t in transactions)
                {
                    if (t.Id > maxId)
                        {
                            maxId = t.Id;
                        }
                }
                    NextId = maxId + 1;
            }
        else
        {
            NextId = 1;
        }
        transaction.Id = NextId;
        transactions.Add(transaction);
        Save();

        
    }
    public void Delete(int id)
    {
        var found = FindById(id);
        if (found != null)
        {
            transactions.Remove(found);
            Save();
        }
        
    }
    public Transaction? FindById(int id)
    {
        foreach(var item in transactions)
        {
            if (item.Id == id)
            {
                return item;
                            
            }
        }
        return null;
        
    }
    public List<Transaction> GetAll()
    {
        return transactions;
    }

    private void Save()
    {
        var Options = new JsonSerializerOptions { WriteIndented = true };
        string Json = JsonSerializer.Serialize(transactions, Options);
        File.WriteAllText(dataFile, Json);

    }

    private void Load()
    {
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
    }



    

}
