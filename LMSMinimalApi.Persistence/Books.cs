namespace LMSMinimalApi.Persistence;

public sealed class Books
{
    public int ID { get; set; }
    public required string BookName { get; set; }
    public required string Author { get; set; }
    public required string Publisher { get; set; }
    public decimal Price { get; set; }
    public int CategoryID { get; set; } // Foreign Key
    public Categories Categories { get; set; }

    public IList<BookIssued> BookIssueds { get; init; } = [];

}