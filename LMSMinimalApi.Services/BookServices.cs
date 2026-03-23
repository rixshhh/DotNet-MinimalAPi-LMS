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
        BooksDTO? bookDto = _DbContext.Books
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
        IQueryable<Books> query = _DbContext.Books.AsQueryable();

        if (!string.IsNullOrWhiteSpace(BookName))
        {
            query = query.Where(b => b.BookName.Contains(BookName));
        }

        List<BooksDTO> result = query
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
            Books book = new()
            {
                BookName = request.BookName,
                Author = request.Author,
                Publisher = request.Publisher,
                Price = request.Price,
                CategoryID = request.CategoryID
            };

            _DbContext.Books.Add(book);
            _DbContext.SaveChanges();

            BooksDTO bookDto = new(
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

    public BooksDTO? UpdateBook(int Id, PostBookRequest requests)
    {
        try
        {
            Books? book = _DbContext.Books.Find(Id);

            if (book is null)
            {
                return null;
            }

            book.BookName = requests.BookName;
            book.Author = requests.Author;
            book.Publisher = requests.Publisher;
            book.Price = requests.Price;
            book.CategoryID = requests.CategoryID;

            _DbContext.SaveChanges();

            return new BooksDTO(
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
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex,
                "Error while creating a Book.");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while Updating a Book with name {@BookName}.", requests);
        }

        return null;
    }

    public BooksDTO? DeleteBook(int ID)
    {
        //try
        //{
        //    Books? book = _DbContext.Books.FirstOrDefault(b => b.ID == ID);

        //    if (book is null)
        //    {
        //        throw new ConflictException($"Cannot find this Id {ID}");
        //    }

        //    _DbContext.Books.Remove(book);

        //    _DbContext.SaveChanges();

        //    return new BooksDTO(
        //        book.ID,
        //        book.BookName,
        //        book.Author,
        //        book.Publisher,
        //        book.Price,
        //        _DbContext.Categories
        //            .Where(c => c.ID == book.CategoryID)
        //            .Select(c => c.CategoryName)
        //            .FirstOrDefault() ?? string.Empty
        //    );
        //}
        //catch (ConflictException ex)
        //{
        //    _logger.LogError(ex, "Error while creating a state with BookId {Id}. Some conflicts occured.",
        //        ID);
        //}
        //catch (DbUpdateException ex)
        //{
        //    _logger.LogError(ex,
        //        "Error while Deleting a Book.");
        //}
        //catch (Exception e)
        //{
        //    _logger.LogError(e, "Error while Deleting a Book with name {@BookName}.", ID);
        //}

        return null;
    }
}
