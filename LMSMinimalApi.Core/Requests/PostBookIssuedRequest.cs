namespace LMSMinimalApi.Core.Requests
{
    public sealed class PostBookIssuedRequest
    {
        public int ID { get; init; }
        public required int BookID { get; init; }
        public required int UserID { get; init; }
        public DateOnly IssueDate { get; init; }
        public DateOnly RenewDate { get; init; }

        public bool RenewCount { get; init; }
        public DateOnly? ReturnDate { get; init; }
        public required decimal BookPrice { get; init; }
    }
}
