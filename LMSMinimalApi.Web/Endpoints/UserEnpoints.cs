using LMSMinimalApi.Core.DTOs;
using LMSMinimalApi.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LMSMinimalApi.Web.Endpoints;

public static class UserEnpoints
{
    public static IEndpointRouteBuilder MapUserGroup(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapGroup("Users");
    }

    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var userGroup = endpoints.MapUserGroup();

        userGroup.MapGet("", GetUsers);
        userGroup.MapGet("{ID:int}", GetUser);
        userGroup.MapGet("ByType/{ID:int}", GetUsersByType);

        return endpoints;
    }

    private static Ok<IEnumerable<UsersDTO>> GetUsers(UserServices userServices)
    {
        var users = userServices.GetUsersList();
        return TypedResults.Ok(users);
    }

    private static IResult GetUser(UserServices userServices, int ID)
    {
        var user = userServices.GetUserByID(ID);
        return user == null ? TypedResults.NotFound() : TypedResults.Ok(user);
    }

    private static IResult GetUsersByType(UserServices userServices, int ID)
    {
        var users = userServices.GetUsersByType(ID);
        return TypedResults.Ok(users);
    }
}