using LMSMinimalApi.Core.DTOs;
using LMSMinimalApi.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LMSMinimalApi.Services;

public sealed class UserServices
{
    private readonly AppDbContext _DbContext;
    private readonly ILogger<UserServices> _logger;

    public UserServices(AppDbContext dbContext, ILogger<UserServices> logger)
    {
        _DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger;
    }

    public IEnumerable<UsersDTO> GetUsersList()
    {
        IReadOnlyList<UsersDTO> users = _DbContext.Users
            .Include(u => u.UserType)
            .Select(u => new UsersDTO(
                u.ID,
                u.Name,
                u.UserType.TypeName,
                u.UserType.MaxBooks
            )).ToList();

        return users;
    }

    public UsersDTO? GetUserByID(int ID)
    {
        var user = _DbContext.Users
            .Where(u => u.ID == ID)
            .Select(s => new UsersDTO(
                s.ID,
                s.Name,
                s.UserType.TypeName,
                s.UserType.MaxBooks
            )).FirstOrDefault();

        return user;
    }

    public IEnumerable<UsersDTO> GetUsersByType(int ID)
    {
        IReadOnlyList<UsersDTO> users = _DbContext.Users
            .Include(ut => ut.UserType)
            .Where(ut => ut.UserType.ID == ID)
            .Select(u => new UsersDTO(
                u.ID,
                u.Name,
                u.UserType.TypeName,
                u.UserType.MaxBooks
            )).ToList();
        return users;
    }
}