using LMSMinimalApi.Core.DTOs;
using LMSMinimalApi.Core.Requests;
using LMSMinimalApi.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LMSMinimalApi.Web.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserGroup(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapGroup("Users");
    }

    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        IEndpointRouteBuilder userGroup = endpoints.MapUserGroup();

        userGroup.MapGet("", GetUsers);
        userGroup.MapGet("{ID:int}", GetUser);
        userGroup.MapGet("ByType/{ID:int}", GetUsersByType);

        userGroup.MapPost("", CreateUser);
        userGroup.MapPut("{ID:int}", Update);
        userGroup.MapPatch("{ID:int}", Patch);

        return endpoints;
    }

    private static Ok<IEnumerable<UsersDTO>> GetUsers(UserServices userServices)
    {
        IEnumerable<UsersDTO> users = userServices.GetUsersList();
        return TypedResults.Ok(users);
    }

    private static IResult GetUser(UserServices userServices, int ID)
    {
        UsersDTO? user = userServices.GetUserByID(ID);
        return user == null ? TypedResults.NotFound() : TypedResults.Ok(user);
    }

    private static IResult GetUsersByType(UserServices userServices, int ID)
    {
        IEnumerable<UsersDTO> users = userServices.GetUsersByType(ID);
        return TypedResults.Ok(users);
    }

    private static IResult CreateUser(UserServices userServices, PostUserRequest request)
    {
        UsersDTO? result = userServices.CreateUserRequest(request);
        return result is null
            ? TypedResults.Problem("There was some problem. See log for more details.")
            : TypedResults.Ok(result);
    }

    private static IResult Update(UserServices userServices, int ID, PostUserRequest request)
    {
        UsersDTO? result = userServices.UpdateUser(ID, request);
        return result is null
            ? TypedResults.Problem("There was some problem. See log for more details.")
            : TypedResults.Ok(result);
    }

    private static IResult Patch(UserServices userServices, int Id, PatchUserIsActiveRequest request)
    {
        UsersDTO? result = userServices.PatchUser(Id, request);
        return result is null
            ? TypedResults.Problem("There was some problem. See log for more details.")
            : TypedResults.Ok(result);
    }
}
