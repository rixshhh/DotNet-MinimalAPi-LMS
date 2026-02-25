namespace LMSMinimalApi.Persistence
{
    public sealed class Categories
    {
        public int ID { get; set; }
        public required string CategoryName { get; set; }

        public IList<Books> Books { get; set; }
    }
}
