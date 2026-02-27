namespace LMSMinimalApi.Core.Requests;

public sealed class PostBookRequest
{
    public required string BookName { get; init; }
    public required string Author { get; init; }
    public required string Publisher { get; init; }
    public decimal Price { get; init; }
    public int CategoryID { get; init; } // Foreign Key
}