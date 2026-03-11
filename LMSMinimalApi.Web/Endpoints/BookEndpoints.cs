using LMSMinimalApi.Core.DTOs;
using LMSMinimalApi.Core.Requests;
using LMSMinimalApi.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LMSMinimalApi.Web.Endpoints;

public static class BookEndpoints
{
    public static IEndpointRouteBuilder MapBookGroup(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapGroup("Books");
    }

    public static IEndpointRouteBuilder MapBookEndpoints(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        IEndpointRouteBuilder booksGroup = endpoints.MapBookGroup();

        booksGroup.MapGet("", GetBooks);
        booksGroup.MapGet("{ID:int}", GetBook);
        booksGroup.MapGet("Search", Search);
        booksGroup.MapPost("", PostBookRequest);
        booksGroup.MapPut("{ID:int}", Update);
        booksGroup.MapDelete("{ID:int}", Delete);

        return endpoints;
    }

    private static Ok<IEnumerable<BooksDTO>> GetBooks(BookServices bookServices)
    {
        IEnumerable<BooksDTO> books = bookServices.GetBooksList();

        return TypedResults.Ok(books);
    }

    private static IResult GetBook(BookServices bookServices, int ID)
    {
        BooksDTO? book = bookServices.GetBookById(ID);

        return book == null ? TypedResults.NotFound() : TypedResults.Ok(book);
    }

    private static IResult Search(BookServices bookServices, string BookName)
    {
        IEnumerable<BooksDTO>? books = bookServices.GetBookBySearch(BookName);

        return books == null ? TypedResults.NotFound() : TypedResults.Ok(books);
    }

    private static IResult PostBookRequest(BookServices bookServices, PostBookRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.BookName))
        {
            return TypedResults.BadRequest("BookName is required.");
        }

        BooksDTO? result = bookServices.PostBookRequest(request);
        return result is null
            ? TypedResults.Problem("There was some problem. See log for more details.")
            : TypedResults.Ok(result);
    }

    private static IResult Update(BookServices bookServices, PostBookRequest requests, int ID)
    {
        BooksDTO? result = bookServices.UpdateBook(ID, requests);

        return result is null
            ? TypedResults.Problem("There was some problem. See log for more details.")
            : TypedResults.Ok(result);
    }

    private static IResult Delete(BookServices bookServices, int ID)
    {
        BooksDTO? result = bookServices.DeleteBook(ID);

        return result is null
            ? TypedResults.Problem("There was some problem. See log for more details.")
            : TypedResults.Ok(result);
    }
}
