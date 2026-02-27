namespace LMSMinimalApi.Core.DTOs;

public sealed class UsersDTO(
    int ID,
    string Name,
    string Type,
    int MaxBooks,
    bool IsActive
)
{
    public int ID { get; } = ID;
    public string Name { get; } = Name;

    public bool IsActive { get; } = IsActive;
    public string Type { get; } = Type;
    public int MaxBooks { get; } = MaxBooks;
}