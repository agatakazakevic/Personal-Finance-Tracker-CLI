using System.Text.Json;
using System.Text.Json.Serialization;
public class TransactionStore
{
    private List<Transaction> transactions = new List<Transaction>();//never null, gets overwritten with load()
    private string dataFile;
    private int nextId;

    public TransactionStore(string dataFile)
    {
        this.dataFile=dataFile;
        Load();
    }

    public void Add(Transaction transaction)
    {
        transaction.Id = nextId;
        nextId++;
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

    public void Save()
    {
        string json = JsonSerializer.Serialize(transactions, jsonOptions);
        File.WriteAllText(dataFile, json);

    }

    private void Load()
    {
        if (File.Exists(dataFile))
        {
            string json = File.ReadAllText(dataFile);
            var result = JsonSerializer.Deserialize<List<Transaction>>(json, jsonOptions);
            if (result != null)
                transactions = result;
            else
                transactions = new List<Transaction>();
        }
        else
        {
            transactions = new List<Transaction>();
        }
        if (transactions.Count > 0)
        {
            int maxId = 0;
            foreach (var t in transactions)
            {
                if (t.Id > maxId)
                    maxId = t.Id;
            }
            nextId = maxId + 1;
        }
        else
        {
            nextId = 1;
        }
    }
    private JsonSerializerOptions jsonOptions = new JsonSerializerOptions 
    { 
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };



    

}
