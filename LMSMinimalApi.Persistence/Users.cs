namespace LMSMinimalApi.Persistence;

public sealed class Users
{
    public int ID { get; set; }
    public string Name { get; set; }
    public int UserTypeID { get; set; } // Foreign Key

    public bool IsActive { get; set; } // New property to indicate if the user is active or not
    public UserTypes UserType { get; set; } // Navigation property

    public IList<BookIssued> BookIssueds { get; init; }
}
