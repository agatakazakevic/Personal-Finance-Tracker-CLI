using System.Collections.Generic;

public class StoreData
{
    public int NextId { get; set; } = 1;
    public List<Transaction> Transactions { get; set; } = new List<Transaction>();
}