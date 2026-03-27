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

        IEndpointRouteBuilder bookIssuedGroup = endpoints.MapBookIssuedGroup();

        bookIssuedGroup.MapGet("", GetBookIssued);
        bookIssuedGroup.MapGet("{id:int}", GetIssuedBookById);
        bookIssuedGroup.MapGet("SearchByUserName", SearchBookIssuedByUserName);
        bookIssuedGroup.MapGet("user/{UserID:int}/bookIssued", GetBookIssuedByUserID);
        bookIssuedGroup.MapPost("", CreateIssueBook);
        bookIssuedGroup.MapPut("Renew/{ID:int}", UpdateIssueBook);
        bookIssuedGroup.MapDelete("Delete/{ID:int}", DeleteIssueBook);
        bookIssuedGroup.MapGet("search", SearchBookIssuedByDate);

        return endpoints;
    }

    private static Ok<IEnumerable<BookIssuedDTO>> GetBookIssued(BookIssuedServices bookIssuedServices)
    {
        IEnumerable<BookIssuedDTO> books = bookIssuedServices.GetBookIssued();

        return TypedResults.Ok(books);
    }

    private static IResult GetIssuedBookById(BookIssuedServices bookIssuedServices, int id)
    {
        var bookIssue = bookIssuedServices.GetBookIssuedById(id);

        return bookIssue is null
            ? TypedResults.NotFound("Id Not Found.")
            : TypedResults.Ok(bookIssue);
    }

    private static IResult SearchBookIssuedByUserName(BookIssuedServices bookIssuedServices, string user)
    {
        IEnumerable<BookIssuedDTO>? bookIssued = bookIssuedServices.GetBookIssuedBySearch(user);

        return bookIssued is null
            ? TypedResults.NotFound("UserName Not Found")
            : TypedResults.Ok(bookIssued);
    }

    public static IResult SearchBookIssuedByDate(DateTime? fromDate, DateTime? toDate, BookIssuedServices bookIssuedservice)
    {
        var data = bookIssuedservice.GetBookIssuedByDate(
            fromDate.HasValue ? DateOnly.FromDateTime(fromDate.Value) : null,
            toDate.HasValue ? DateOnly.FromDateTime(toDate.Value) : null
        );

        return Results.Ok(data);
    }

    private static IResult GetBookIssuedByUserID(BookIssuedServices bookIssuedServices, int UserID)
    {
        IEnumerable<BookIssuedDTO>? bookIssued = bookIssuedServices.GetBookIssuedByUserId(UserID);

        return bookIssued is null ? TypedResults.NotFound("UserID Not Found") : TypedResults.Ok(bookIssued);
    }

    private static IResult CreateIssueBook(BookIssuedServices bookIssuedServices, PostBookIssuedRequest request)
    {
        try
        {
            BookIssuedDTO? bookIssued = bookIssuedServices.CreateBookIssueRequest(request);

            return bookIssued is null
                ? TypedResults.BadRequest("Unable to create book issue request. See Logs")
                : TypedResults.Ok(bookIssued);
        }
        catch (Exception ex)
        {
            return TypedResults.Problem(ex.Message);
        }
    }

    private static IResult UpdateIssueBook(BookIssuedServices bookIssuedServices, int id, PostBookIssuedRequest request)
    {
        BookIssuedDTO? bookIssued = bookIssuedServices.UpdateBookIssueRequest(id, request);

        return bookIssued is null
            ? TypedResults.BadRequest("Unable to Update Book Issue Request")
            : TypedResults.Ok(bookIssued);
    }

    private static IResult DeleteIssueBook(BookIssuedServices bookIssuedServices, int id)
    {
        bool isDeleted = bookIssuedServices.DeleteBookIssue(id);
        return isDeleted
            ? TypedResults.Ok("Book Issue Request Deleted Successfully")
            : TypedResults.BadRequest("Unable to Delete Book Issue Request");
    }
}
