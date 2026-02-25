using LMSMinimalApi.Core.DTOs;
using LMSMinimalApi.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LMSMinimalApi.Web.Endpoints;

public static class BookEndpoints
{
    public static IEndpointRouteBuilder MapBookEndpoints(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        endpoints.MapGet("Books", GetBooks);
        endpoints.MapGet("Books/{ID:int}", GetBook);

        return endpoints;
    }

    private static Ok<IEnumerable<BooksDTO>> GetBooks(BookServices bookServices)
    {
        var books = bookServices.GetBooksList();

        return TypedResults.Ok(books);
    }

    private static IResult GetBook(BookServices bookServices, int ID)
    {
        var book = bookServices.GetBookById(ID);

        return book == null ? TypedResults.NotFound() : TypedResults.Ok(book);
    }
}