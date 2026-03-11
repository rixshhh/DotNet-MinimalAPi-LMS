namespace LMSMinimalApi.Core.DTOs;

public sealed class BooksDTO(
    int ID,
    string BookName,
    string Author,
    string Publisher,
    decimal Price,
    string CategoryName
)
{
    public int ID { get; } = ID;
    public string BookName { get; } = BookName;
    public string Author { get; } = Author;
    public string Publisher { get; } = Publisher;
    public decimal Price { get; } = Price;
    public string CategoryName { get; } = CategoryName;
}
