using LMSMinimalApi.Core.DTOs;
using LMSMinimalApi.Core.Requests;
using LMSMinimalApi.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace LMSMinimalApi.Services;

public sealed class BookIssuedServices
{
    private readonly AppDbContext _DbContext;
    private readonly ILogger<BookIssuedServices> _logger;

    public BookIssuedServices(AppDbContext dbContext, ILogger<BookIssuedServices> logger)
    {
        _DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger;
    }

    public IEnumerable<BookIssuedDTO> GetBookIssued()
    {
        IReadOnlyList<BookIssuedDTO> issuedBooks = _DbContext.BookIssued
            .Include(bi => bi.Book)
            .Include(bi => bi.User)
            .Select(bi => new BookIssuedDTO
            (
                bi.ID,
                bi.Book.BookName,
                bi.User.Name,
                bi.IssueDate,
                bi.RenewDate,
                bi.RenewCount,
                bi.ReturnDate,
                bi.BookPrice
            ))
            .ToList();

        return issuedBooks;
    }

    public IEnumerable<BookIssuedDTO> GetBookIssuedBySeach(string? user)
    {
        var query = _DbContext.BookIssued.AsQueryable();

        if (!string.IsNullOrWhiteSpace(user)) query = query.Where(b => b.User.Name.Contains(user));

        var result = query
            .Include(u => u.User)
            .Include(b => b.Book)
            .Select(bi => new BookIssuedDTO(
                bi.ID,
                bi.Book.BookName,
                bi.User.Name,
                bi.IssueDate,
                bi.RenewDate,
                bi.RenewCount,
                bi.ReturnDate,
                bi.BookPrice
            )).ToList();

        return new ReadOnlyCollection<BookIssuedDTO>(result);
    }

    public IEnumerable<BookIssuedDTO> GetBookIssuedByUserId(int userId)
    {
        var issuedBooks = _DbContext.BookIssued
            .Where(b => b.UserID == userId)
            .ToList();

        if (!issuedBooks.Any())
            throw new Exception($"No books found for UserID {userId}");

        var result = issuedBooks.Select(issuedBook => new BookIssuedDTO(
            issuedBook.ID,
            _DbContext.Books
                .Where(b => b.ID == issuedBook.BookID)
                .Select(b => b.BookName)
                .FirstOrDefault() ?? string.Empty,
            _DbContext.Users
                .Where(u => u.ID == issuedBook.UserID)
                .Select(u => u.Name)
                .FirstOrDefault() ?? string.Empty,
            issuedBook.IssueDate,
            issuedBook.RenewDate,
            issuedBook.RenewCount,
            issuedBook.ReturnDate,
            issuedBook.BookPrice
        ));

        return result;
    }

    public BookIssuedDTO? CreateBookIssueRequest(PostBookIssuedRequest request)
    {
        try
        {
            var user = _DbContext.Users
            .Include(u => u.UserType)
            .FirstOrDefault(u => u.ID == request.UserID);

            if (user == null)
                throw new ConflictException($"User with ID {request.UserID} not found.");

            var book = _DbContext.Books
                .FirstOrDefault(b => b.ID == request.BookID);

            if (book == null)
                throw new ConflictException($"Book with ID {request.BookID} does not exist.");

            bool alreadyIssued = _DbContext.BookIssued
                .Any(b => b.BookID == request.BookID &&
                          b.UserID == request.UserID);

            if (alreadyIssued)
                throw new ConflictException("This user already has this book issued.");

            int issuedBooksCount = _DbContext.BookIssued
                .Count(b => b.UserID == request.UserID && b.ReturnDate != null);

            if (issuedBooksCount >= user.UserType.MaxBooks)
                throw new ConflictException(
                    $"User already issued maximum books allowed ({user.UserType.MaxBooks}).");

            var bookIssue = new BookIssued
            {
                BookID = request.BookID,
                UserID = request.UserID,
                IssueDate = request.IssueDate,
                RenewDate = request.RenewDate,
                RenewCount = request.RenewCount,
                ReturnDate = request.ReturnDate,
                BookPrice = request.BookPrice
            };

            if (bookIssue == null)
                throw new Exception("Failed to create a Book issue from the provided request.");

            _DbContext.BookIssued.Add(bookIssue);
            _DbContext.SaveChanges();

            var CreatedIssuedBook = new BookIssuedDTO(
                bookIssue.ID,
                _DbContext.Books
                    .Where(b => b.ID == bookIssue.BookID)
                    .Select(b => b.BookName)
                    .FirstOrDefault() ?? string.Empty,
                _DbContext.Users
                    .Where(u => u.ID == bookIssue.UserID)
                    .Select(u => u.Name)
                    .FirstOrDefault() ?? string.Empty,
                bookIssue.IssueDate,
                bookIssue.RenewDate,
                bookIssue.RenewCount,
                bookIssue.ReturnDate,
                bookIssue.BookPrice
            );

            return CreatedIssuedBook;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex,
                "Error while creating Book Issue for UserID {UserID} and BookID {BookID}.",
                request.UserID, request.BookID);
        }
        catch (Exception e)
        {
            _logger.LogError(e,
                "Error while creating Book Issue.");
        }

        return null;
    }

    public BookIssuedDTO? UpdateBookIssueRequest(int id, PostBookIssuedRequest request)
    {
        try
        {
            var BookIssue = _DbContext.BookIssued.Find(id);

            if (BookIssue == null) return null;

            BookIssue.BookID = request.BookID;
            BookIssue.UserID = request.UserID;
            BookIssue.IssueDate = request.IssueDate;
            BookIssue.RenewDate = request.RenewDate;
            BookIssue.RenewCount = request.RenewCount;
            BookIssue.ReturnDate = request.ReturnDate;
            BookIssue.BookPrice = request.BookPrice;

            _DbContext.SaveChanges();

            var updatedBookIssueDTO = new BookIssuedDTO(
                BookIssue.ID,
                _DbContext.Books
                    .Where(b => b.ID == BookIssue.BookID)
                    .Select(b => b.BookName)
                    .FirstOrDefault() ?? string.Empty,
                _DbContext.Users
                    .Where(u => u.ID == BookIssue.UserID)
                    .Select(u => u.Name)
                    .FirstOrDefault() ?? string.Empty,
                BookIssue.IssueDate,
                BookIssue.RenewDate,
                BookIssue.RenewCount,
                BookIssue.ReturnDate,
                BookIssue.BookPrice
            );
            return updatedBookIssueDTO;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex,
                "Error while updating a Book issue with ID {ID}.", id);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while updating a Book issue with ID {ID} and data {@Request}.", id, request);
        }

        return null;
    }
    public bool DeleteBookIssue(int ID)
    {
        try
        {
            var bookIssue = _DbContext.BookIssued
                .FirstOrDefault(b => b.ID == ID);

            if (bookIssue == null)
                throw new ConflictException($"Book Issue with ID {ID} not found.");

            _DbContext.BookIssued.Remove(bookIssue);
            _DbContext.SaveChanges();

            return true;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error while deleting Book Issue with ID {ID}", ID);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while deleting Book Issue.");
        }

        return false;
    }
}