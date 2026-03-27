using System.Text.Json.Serialization;

namespace LMSMinimalApi.Core.Requests;

public sealed class PostBookIssuedRequest
{
    public int ID { get; init; }

    [JsonPropertyName("bookId")]
    public int BookId { get; init; }

    [JsonPropertyName("userId")]
    public int UserId { get; init; }
    public DateOnly IssueDate { get; init; }
    public DateOnly? RenewDate { get; init; }

    public bool RenewCount { get; init; }
    public DateOnly? ReturnDate { get; init; }
    public decimal BookPrice { get; init; }
}
