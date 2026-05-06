using System.Text.Json;
using System.Text.Json.Serialization;
public class TransactionStore
{
    private List<Transaction> transactions = new List<Transaction>();//never null, gets overwritten with load()
    private string dataFile;
    private int nextId = 1;

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
        var data = new StoreData
        {
            NextId = nextId,
            Transactions = transactions
        };
        string json = JsonSerializer.Serialize(data, jsonOptions);
        File.WriteAllText(dataFile, json);

    }

    private void Load()
    {
        if (File.Exists(dataFile))
        {
            string json = File.ReadAllText(dataFile);
            var result = JsonSerializer.Deserialize<StoreData>(json, jsonOptions);
            if (result != null){
                transactions = result.Transactions;
                nextId = result.NextId;
            }
        }
    }
    private JsonSerializerOptions jsonOptions = new JsonSerializerOptions 
    { 
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };



    

}
