using LMSMinimalApi.Core.DTOs;
using LMSMinimalApi.Core.Requests;
using LMSMinimalApi.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LMSMinimalApi.Web.Endpoints;

public static class BookIssuedEndpoints
{
    public static IEndpointRouteBuilder MapBookIssuedGroup(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapGroup("BookIssued");
    }

    public static IEndpointRouteBuilder MapBookIssuedEndpoints(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var bookIssuedGroup = endpoints.MapBookIssuedGroup();

        bookIssuedGroup.MapGet("", GetBookIssued);
        bookIssuedGroup.MapGet("Search", GetBookIssuedByUserName);
        bookIssuedGroup.MapGet("user/{UserID:int}/bookIssued", GetBookIssuedByUserID);
        bookIssuedGroup.MapPost("", CreateIssueBook);
        bookIssuedGroup.MapPut("Renew/{ID:int}", UpdateIssueBook);


        return endpoints;
    }

    private static Ok<IEnumerable<BookIssuedDTO>> GetBookIssued(BookIssuedServices bookIssuedServices)
    {
        var books = bookIssuedServices.GetBookIssued();

        return TypedResults.Ok(books);
    }

    private static IResult GetBookIssuedByUserName(BookIssuedServices bookIssuedServices, string user)
    {
        var bookIssued = bookIssuedServices.GetBookIssuedBySeach(user);

        return bookIssued is null ? TypedResults.NotFound("UserName Not Found") : TypedResults.Ok(bookIssued);
    }

    private static IResult GetBookIssuedByUserID(BookIssuedServices bookIssuedServices, int UserID)
    {
        var bookIssued = bookIssuedServices.GetBookIssuedByUserId(UserID);

        return bookIssued is null ? TypedResults.NotFound("UserID Not Found") : TypedResults.Ok(bookIssued);
    }

    private static IResult CreateIssueBook(BookIssuedServices bookIssuedServices, PostBookIssuedRequest request)
    {
        var bookIssued = bookIssuedServices.CreateBookIssueRequest(request);

        return bookIssued is null
            ? TypedResults.BadRequest("Unable to Create Book Issue Request")
            : TypedResults.Ok(bookIssued);
    }

    private static IResult UpdateIssueBook(BookIssuedServices bookIssuedServices, int id, PostBookIssuedRequest request)
    {
        var bookIssued = bookIssuedServices.UpdateBookIssueRequest(id, request);

        return bookIssued is null
            ? TypedResults.BadRequest("Unable to Update Book Issue Request")
            : TypedResults.Ok(bookIssued);
    }
}