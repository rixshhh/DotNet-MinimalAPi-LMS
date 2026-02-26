namespace LMSMinimalApi.Persistence
{
    public sealed class BookIssued
    {
        public int ID { get; set; }

        public int BookID { get; set; }
        public int UserID { get; set; }

        public DateOnly IssueDate { get; set; }
        public DateOnly RenewDate { get; set; }

        public bool RenewCount { get; set; }
        public DateOnly? ReturnDate { get; set; }
        public string UserName { get; set; }
        public decimal BookPrice { get; set; }

        public Books Book { get; set; }
        public Users User { get; set; }
    }
}
