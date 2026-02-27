namespace LMSMinimalApi.Core.Requests
{
    public sealed class PostUserRequest
    {
        public int ID { get; init; }
        public string Name { get; init; }
        public int UserTypeID { get; init; } // Foreign Key

        public bool IsActive { get; init; }
    }
}
