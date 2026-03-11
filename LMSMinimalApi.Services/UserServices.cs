using LMSMinimalApi.Core.DTOs;
using LMSMinimalApi.Core.Requests;
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
                u.UserType.MaxBooks,
                u.IsActive
            )).ToList();

        return users;
    }

    public UsersDTO? GetUserByID(int ID)
    {
        UsersDTO? user = _DbContext.Users
            .Where(u => u.ID == ID)
            .Select(s => new UsersDTO(
                s.ID,
                s.Name,
                s.UserType.TypeName,
                s.UserType.MaxBooks,
                s.IsActive
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
                u.UserType.MaxBooks,
                u.IsActive
            )).ToList();
        return users;
    }

    public UsersDTO? CreateUserRequest(PostUserRequest request)
    {
        try
        {
            bool existingUser = _DbContext.Users
                .Any(u => u.Name == request.Name);

            if (existingUser)
            {
                _logger.LogWarning("User with name {Name} already exists.", request.Name);
                return null;
            }

            bool existingUserType = _DbContext.UserTypes
                .Any(ut => ut.ID == request.UserTypeID);

            if (!existingUserType)
            {
                _logger.LogWarning("UserType with ID {UserTypeID} does not exist.", request.UserTypeID);
                return null;
            }

            Users user = new() { Name = request.Name, UserTypeID = request.UserTypeID };

            _DbContext.Users.Add(user);
            _DbContext.SaveChanges();

            UsersDTO result = new(
                user.ID,
                user.Name,
                _DbContext.UserTypes
                    .Where(u => u.ID == user.UserTypeID)
                    .Select(u => u.TypeName)
                    .FirstOrDefault() ?? string.Empty,
                _DbContext.UserTypes
                    .Where(u => u.ID == user.UserTypeID)
                    .Select(u => u.MaxBooks)
                    .FirstOrDefault(),
                user.IsActive
            );

            return result;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex,
                "Database Error while creating a User.");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while creating a User with name {Name}.", request);
        }

        return null;
    }

    public UsersDTO? UpdateUser(int Id, PostUserRequest request)
    {
        try
        {
            Users? user = _DbContext.Users.Find(Id);
            if (user == null)
            {
                _logger.LogWarning("User with ID {Id} not found for update.", Id);
                return null;
            }

            user.Name = request.Name;
            user.UserTypeID = request.UserTypeID;
            user.IsActive = request.IsActive;

            _DbContext.SaveChanges();

            UsersDTO result = new(
                user.ID,
                user.Name,
                _DbContext.UserTypes
                    .Where(u => u.ID == user.UserTypeID)
                    .Select(u => u.TypeName)
                    .FirstOrDefault() ?? string.Empty,
                _DbContext.UserTypes
                    .Where(u => u.ID == user.UserTypeID)
                    .Select(u => u.MaxBooks)
                    .FirstOrDefault(),
                user.IsActive
            );

            return result;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex,
                "Error while updating a User with ID {Id}.", Id);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while updating a User with ID {Id} and name {@Name}.", Id, request);
        }

        return null;
    }

    public UsersDTO? PatchUser(int Id, PatchUserIsActiveRequest request)
    {
        try
        {
            Users? user = _DbContext.Users.Find(Id);

            if (user == null)
            {
                _logger.LogWarning("User with ID {Id} not found for patching.", Id);
                return null;
            }

            _DbContext.Entry(user).CurrentValues.SetValues(request);

            _DbContext.SaveChanges();

            UsersDTO result = new(
                user.ID,
                user.Name,
                _DbContext.UserTypes
                    .Where(u => u.ID == user.UserTypeID)
                    .Select(u => u.TypeName)
                    .FirstOrDefault() ?? string.Empty,
                _DbContext.UserTypes
                    .Where(u => u.ID == user.UserTypeID)
                    .Select(u => u.MaxBooks)
                    .FirstOrDefault(),
                user.IsActive
            );

            return result;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex,
                "Error while patching a User with ID {Id}.", Id);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while patching a User with ID {Id} and IsActive status.", Id);
        }

        return null;
    }
}
