namespace LMSMinimalApi.Core.DTOs
{
    public sealed class CategoryDTO(
        int ID,
        string CategoryName
    )
    {
        public int ID { get; } = ID;
        public string CategoryName { get; } = CategoryName;
    }
}
