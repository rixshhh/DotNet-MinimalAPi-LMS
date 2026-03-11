namespace LMSMinimalApi.Core.DTOs;

public sealed class BookIssuedDTO(
    int ID,
    string BookName,
    string UserName,
    DateOnly IssueDate,
    DateOnly? RenewDate,
    bool RenewCount,
    DateOnly? ReturnDate
)
{
    public int ID { get; } = ID;
    public string BookName { get; } = BookName;
    public string UserName { get; } = UserName;
    public DateOnly IssueDate { get; } = IssueDate;
    public DateOnly? RenewDate { get; } = RenewDate;

    public bool RenewCount { get; } = RenewCount;
    public DateOnly? ReturnDate { get; } = ReturnDate;
}
