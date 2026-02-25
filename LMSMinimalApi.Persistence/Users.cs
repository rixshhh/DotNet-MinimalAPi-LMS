namespace LMSMinimalApi.Persistence
{
    public sealed class Users
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int UserTypeID { get; set; }   // Foreign Key
        public required UserTypes UserType { get; set; } // Navigation property
    }
}
