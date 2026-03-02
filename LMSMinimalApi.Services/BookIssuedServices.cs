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
            var bookIssue = new BookIssued
            {
                ID = request.ID,
                BookID = request.BookID,
                UserID = request.UserID,
                IssueDate = request.IssueDate,
                RenewDate = request.RenewDate,
                RenewCount = request.RenewCount,
                ReturnDate = request.ReturnDate,
                BookPrice = request.BookPrice
            };

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
                "Error while creating a Book.");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while creating a BookIssue with name {@BookID}.", request);
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
}