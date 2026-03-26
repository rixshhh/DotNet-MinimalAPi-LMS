namespace LMSMinimalApi.Core.Requests;

public sealed class UpdateUserRequest
{
    public string Name { get; init; }
    public string Type { get; init; }

    public int UserTypeId { get; init; } // Foreign Key
    public int MaxBooks { get; init; }

    public bool IsActive { get; init; }
}
