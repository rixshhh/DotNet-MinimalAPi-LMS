using LMSMinimalApi.Core.DTOs;
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

        var booksGroup = endpoints.MapBookGroup();

        booksGroup.MapGet("", GetBooks);
        booksGroup.MapGet("{ID:int}", GetBook);
        booksGroup.MapGet("Search", Search);

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

    private static IResult Search(BookServices bookServices, string BookName)
    {
        IEnumerable<BooksDTO> books = bookServices.GetBookBySearch(BookName);

        return books == null ? TypedResults.NotFound() : TypedResults.Ok(books);

    }
}