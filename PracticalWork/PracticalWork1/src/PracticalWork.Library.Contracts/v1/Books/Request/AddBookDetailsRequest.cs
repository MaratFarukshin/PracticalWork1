using Microsoft.AspNetCore.Http;

namespace PracticalWork.Library.Contracts.v1.Books.Request;

/// <summary>
/// Запрос на добавления деталей по книге
/// </summary>
//// <param name="Description">Краткое описание книги</param>
//public sealed record AddBookDetailsRequest(string Description);

public class AddBookDetailsRequest
{
    public string Description { get; set; }

    public IFormFile CoverImage { get; set; }
}
