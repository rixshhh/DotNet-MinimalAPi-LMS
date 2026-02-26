using LMSMinimalApi.Core.DTOs;
using LMSMinimalApi.Core.Requests;
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
    }

    public IEnumerable<BooksDTO> GetBookBySearch(string? BookName)
    {
        var query = _DbContext.Books.AsQueryable();

        if (!string.IsNullOrWhiteSpace(BookName))
        {
            query = query.Where(b => b.BookName.Contains(BookName));
        }

        var result = query
             .Select(b => new BooksDTO
             (
                 b.ID,
                 b.BookName,
                 b.Author,
                 b.Publisher,
                 b.Price,
                 b.Categories.CategoryName
             )).ToList();

        return result;
    }

    public BooksDTO? PostBookRequest(PostBookRequest request)
    {
        try
        {
            var book = new Books
            {
                BookName = request.BookName,
                Author = request.Author,
                Publisher = request.Publisher,
                Price = request.Price,
                CategoryID = request.CategoryID
            };

            _DbContext.Books.Add(book);
            _DbContext.SaveChanges();

            var bookDto = new BooksDTO(
                book.ID,
                book.BookName,
                book.Author,
                book.Publisher,
                book.Price,
                _DbContext.Categories
                    .Where(c => c.ID == book.CategoryID)
                    .Select(c => c.CategoryName)
                    .FirstOrDefault() ?? string.Empty
            );
            return bookDto;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex,
                "Error while creating a Book.");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while creating a Book with name {@BookName}.", request);
        }

        return null;

    }
}