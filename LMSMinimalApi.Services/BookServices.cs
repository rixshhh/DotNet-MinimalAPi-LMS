using LMSMinimalApi.Core.DTOs;
using LMSMinimalApi.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LMSMinimalApi.Services;

public sealed class BookServices
{
    private readonly AppDbContext _DbContext;
    private readonly ILogger<BookServices> _logger;

    public BookServices(AppDbContext dbContext, ILogger<BookServices> logger)
    {
        _DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger;
    }

    public IEnumerable<BooksDTO> GetBooksList()
    {
        IReadOnlyList<BooksDTO> books = _DbContext.Books
            .Include(b => b.Categories)
            .Select(b => new BooksDTO(
                b.ID,
                b.BookName,
                b.Author,
                b.Publisher,
                b.Price,
                b.Categories.CategoryName
            ))
            .ToList();

        return books;
    }

    public BooksDTO? GetBookById(int ID)
    {
        var bookDto = _DbContext.Books
            .Where(s => s.ID == ID)
            .Select(b => new BooksDTO(
                b.ID,
                b.BookName,
                b.Author,
                b.Publisher,
                b.Price,
                b.Categories.CategoryName
            ))
            .FirstOrDefault();

        return bookDto;

        // Option B (alternative): fetch entity then map (uncomment if you prefer)
        /*
        var book = _DbContext.Books
            .Include(b => b.Categories)
            .FirstOrDefault(s => s.ID == ID);

        if (book == null) return null;

        return new BooksDTO(
            book.ID,
            book.BookName,
            book.Author,
            book.Publisher,
            book.Price,
            book.Categories.CategoryName
        );
        */
    }
}