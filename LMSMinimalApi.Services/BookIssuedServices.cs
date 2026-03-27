using System.Collections.ObjectModel;
using LMSMinimalApi.Core.DTOs;
using LMSMinimalApi.Core.Requests;
using LMSMinimalApi.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
                bi.Book.ID,
                bi.Book.BookName,
                bi.User.ID,
                bi.User.Name,
                bi.IssueDate,
                bi.RenewDate,
                bi.RenewCount,
                bi.ReturnDate
            ))
            .ToList();

        return issuedBooks;
    }

    public BookIssuedDTO? GetBookIssuedById(int id)
    {
        var issueBook = _DbContext.BookIssued
            .Where(b => b.ID == id)
            .Select(b => new BookIssuedDTO(
                b.ID,
                b.Book.ID,
                b.Book.BookName,
                b.User.ID,
                b.User.Name,
                b.IssueDate,
                b.RenewDate,
                b.RenewCount,
                b.ReturnDate
            ))
            .FirstOrDefault();

        if (issueBook == null)
        {
            throw new ConflictException($"No IssuedBook found with this Id {id}");
        }

        return issueBook;
    }

    public IEnumerable<BookIssuedDTO> GetBookIssuedBySearch(string? user)
    {
        IQueryable<BookIssued> query = _DbContext.BookIssued.AsQueryable();

        if (!string.IsNullOrWhiteSpace(user))
        {
            query = query.Where(b => b.User.Name.Contains(user));
        }

        List<BookIssuedDTO> result = query
            .Include(u => u.User)
            .Include(b => b.Book)
            .Select(bi => new BookIssuedDTO(
                bi.ID,
                bi.Book.ID,
                bi.Book.BookName,
                bi.User.ID,
                bi.User.Name,
                bi.IssueDate,
                bi.RenewDate,
                bi.RenewCount,
                bi.ReturnDate
            )).ToList();

        return new ReadOnlyCollection<BookIssuedDTO>(result);
    }

    public IEnumerable<BookIssuedDTO> GetBookIssuedByDate(DateOnly? fromDate, DateOnly? toDate)
    {
        IQueryable<BookIssued> query = _DbContext.BookIssued.AsQueryable();

        if (fromDate.HasValue)
        {
            query = query.Where(b => b.IssueDate >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(b => b.IssueDate <= toDate.Value);
        }

        return query
            .Include(u => u.User)
            .Include(b => b.Book)
            .Select(bi => new BookIssuedDTO(
                bi.ID,
                bi.Book.ID,
                bi.Book.BookName,
                bi.User.ID,
                bi.User.Name,
                bi.IssueDate,
                bi.RenewDate,
                bi.RenewCount,
                bi.ReturnDate
            ))
            .ToList();
    }

    public IEnumerable<BookIssuedDTO> GetBookIssuedByUserId(int userId)
    {
        List<BookIssuedDTO> issuedBooks = _DbContext.BookIssued
            .Include(b => b.Book)
            .Include(b => b.User)
            .Where(b => b.UserID == userId)
            .Select(b => new BookIssuedDTO(
                b.ID,
                b.Book.ID,
                b.Book.BookName,
                b.User.ID,
                b.User.Name,
                b.IssueDate,
                b.RenewDate,
                b.RenewCount,
                b.ReturnDate
            ))
            .ToList();

        if (!issuedBooks.Any())
        {
            throw new ConflictException($"No books found for UserID {userId}");
        }

        return issuedBooks;
    }

    public BookIssuedDTO? CreateBookIssueRequest(PostBookIssuedRequest request)
    {
        try
        {
            Users? user = _DbContext.Users
                .Include(u => u.UserType)
                .FirstOrDefault(u => u.ID == request.UserId);

            if (user == null)
            {
                throw new ConflictException($"User with ID {request.UserId} not found.");
            }

            Books? book = _DbContext.Books
                .FirstOrDefault(b => b.ID == request.BookId);

            if (book == null)
            {
                throw new ConflictException($"Book with ID {request.BookId} does not exist.");
            }

            bool alreadyIssued = _DbContext.BookIssued
                .Any(b => b.BookID == request.BookId &&
                          b.UserID == request.UserId);

            if (alreadyIssued)
            {
                throw new ConflictException("This user already has this book issued.");
            }

            int issuedBooksCount = _DbContext.BookIssued
                .Count(b => b.UserID == request.UserId && b.ReturnDate != null);

            if (issuedBooksCount >= user.UserType.MaxBooks)
            {
                throw new ConflictException(
                    $"User already issued maximum books allowed ({user.UserType.MaxBooks}).");
            }

            BookIssued bookIssue = new()
            {
                BookID = request.BookId,
                UserID = request.UserId,
                IssueDate = DateOnly.FromDateTime(DateTime.Today),
                RenewDate = null,
                RenewCount = false,
                ReturnDate = DateOnly.FromDateTime(DateTime.Today).AddDays(15)
            };

            _DbContext.BookIssued.Add(bookIssue);
            _DbContext.SaveChanges();

            BookIssuedDTO CreatedIssuedBook = new(
                bookIssue.ID,
                book.ID,
                book.BookName,
                user.ID,
                user.Name,
                bookIssue.IssueDate,
                bookIssue.RenewDate,
                bookIssue.RenewCount,
                bookIssue.ReturnDate
            );

            return CreatedIssuedBook;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex,
                "Error while creating Book Issue for UserID {UserID} and BookID {BookID}.",
                request.UserId, request.BookId);
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
            BookIssued? BookIssue = _DbContext.BookIssued.Find(id);

            if (BookIssue == null)
            {
                return null;
            }

            BookIssue.BookID = request.BookId;
            BookIssue.UserID = request.UserId;
            BookIssue.IssueDate = request.IssueDate;
            BookIssue.RenewDate = request.RenewDate;
            BookIssue.RenewCount = request.RenewCount;
            BookIssue.ReturnDate = request.ReturnDate;

            _DbContext.SaveChanges();

            BookIssuedDTO updatedBookIssueDTO = new(
                BookIssue.ID,
                BookIssue.BookID,
                _DbContext.Books
                    .Where(b => b.ID == BookIssue.BookID)
                    .Select(b => b.BookName)
                    .FirstOrDefault() ?? string.Empty,
                BookIssue.UserID,
                _DbContext.Users
                    .Where(u => u.ID == BookIssue.UserID)
                    .Select(u => u.Name)
                    .FirstOrDefault() ?? string.Empty,
                BookIssue.IssueDate,
                BookIssue.RenewDate,
                BookIssue.RenewCount,
                BookIssue.ReturnDate
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
            BookIssued? bookIssue = _DbContext.BookIssued
                .FirstOrDefault(b => b.ID == ID);

            if (bookIssue == null)
            {
                throw new ConflictException($"Book Issue with ID {ID} not found.");
            }

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
