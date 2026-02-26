using LMSMinimalApi.Core.DTOs;
using LMSMinimalApi.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace LMSMinimalApi.Services
{
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

            if (!string.IsNullOrWhiteSpace(user))
            {
                query = query.Where(b => b.UserName.Contains(user));
            }

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

        public BookIssuedDTO? GetBookIssuedByUserId(int UserID)
        {
            var issuedBook = _DbContext.BookIssued
                .Where(bi => bi.UserID == UserID)
                .Select(bi => new BookIssuedDTO(
                    bi.ID,
                        bi.Book.BookName,
                        bi.User.Name,
                        bi.IssueDate,
                        bi.RenewDate,
                        bi.RenewCount,
                        bi.ReturnDate,
                        bi.BookPrice
                ))
                .FirstOrDefault();

            return issuedBook;


        }
    }
}
