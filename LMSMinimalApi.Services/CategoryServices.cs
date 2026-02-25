using System.Collections.ObjectModel;
using LMSMinimalApi.Core.DTOs;
using LMSMinimalApi.Persistence;
using Microsoft.Extensions.Logging;

namespace LMSMinimalApi.Services;

public sealed class CategoryServices
{
    private readonly AppDbContext _DbContext;
    private readonly ILogger<CategoryServices> _logger;

    public CategoryServices(AppDbContext DbContext, ILogger<CategoryServices> logger)
    {
        _DbContext = DbContext ?? throw new ArgumentNullException(nameof(DbContext));
        _logger = logger;
    }

    public IEnumerable<CategoryDTO> GetCategoriesList()
    {
        IList<CategoryDTO> categories = _DbContext.Categories
            .Select(c => new CategoryDTO(
                c.ID,
                c.CategoryName
            ))
            .ToList();
        return new ReadOnlyCollection<CategoryDTO>(categories);
    }

    public CategoryDTO? GetCategoryByID(int ID)
    {
        var category = _DbContext.Categories
            .Where(c => c.ID == ID)
            .Select(c => new CategoryDTO(
                c.ID,
                c.CategoryName
            ))
            .FirstOrDefault();

        if (category == null) _logger.LogWarning("Category with ID {ID} not found.", ID);

        return category;
    }
}