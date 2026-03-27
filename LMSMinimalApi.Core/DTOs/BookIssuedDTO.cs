namespace LMSMinimalApi.Core.DTOs;

public sealed class BookIssuedDTO(
    int ID,
    int BookId,
    string BookName,
    int UserId,
    string UserName,
    DateOnly IssueDate,
    DateOnly? RenewDate,
    bool RenewCount,
    DateOnly? ReturnDate
)
{
    public int ID { get; } = ID;
    public int BookId { get; } = BookId;
    public string BookName { get; } = BookName;
    public int UserId { get; } = UserId;
    public string UserName { get; } = UserName;
    public DateOnly IssueDate { get; } = IssueDate;
    public DateOnly? RenewDate { get; } = RenewDate;

    public bool RenewCount { get; } = RenewCount;
    public DateOnly? ReturnDate { get; } = ReturnDate;
}
