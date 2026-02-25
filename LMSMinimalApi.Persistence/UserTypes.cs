namespace LMSMinimalApi.Persistence
{
    public sealed class UserTypes
    {
        public int ID { get; set; }
        public required string TypeName { get; set; }
        public int MaxBooks { get; set; }
        public IList<Users> Users { get; set; }
    }
}
