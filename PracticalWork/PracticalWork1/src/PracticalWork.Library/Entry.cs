using Microsoft.Extensions.DependencyInjection;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Application;
using PracticalWork.Library.Application.Abstractions;
using PracticalWork.Library.Application.Handlers.Commands;
using PracticalWork.Library.Application.Handlers.Queries;
using PracticalWork.Library.Domain.Services;
using PracticalWork.Library.Services;

namespace PracticalWork.Library;

public static class Entry
{
    /// <summary>
    /// Регистрация зависимостей уровня бизнес-логики
    /// </summary>
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        // Старые сервисы (оставляем для обратной совместимости, но можно будет удалить)
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IReaderService, ReaderService>();
        services.AddScoped<ILibraryService, LibraryService>();
        services.AddSingleton<IFileValidationService, FileValidationService>();

        // Domain Services
        services.AddScoped<BookAvailabilityService>();
        services.AddScoped<ReaderValidationService>();
        services.AddScoped<ArchiveCheckService>();

        // Application Layer - Commands
        services.AddScoped<ICommandHandler<Application.Commands.Books.CreateBookCommand, Guid>, CreateBookCommandHandler>();
        services.AddScoped<ICommandHandler<Application.Commands.Books.UpdateBookCommand>, UpdateBookCommandHandler>();
        services.AddScoped<ICommandHandler<Application.Commands.Books.ArchiveBookCommand, Models.Book>, ArchiveBookCommandHandler>();
        services.AddScoped<ICommandHandler<Application.Commands.Books.AddBookDetailsCommand, string>, AddBookDetailsCommandHandler>();
        services.AddScoped<ICommandHandler<Application.Commands.Library.BorrowBookCommand, Models.BorrowedBook>, BorrowBookCommandHandler>();
        services.AddScoped<ICommandHandler<Application.Commands.Library.ReturnBookCommand, Models.ReturnedBook>, ReturnBookCommandHandler>();
        services.AddScoped<ICommandHandler<Application.Commands.Readers.CreateReaderCommand, Guid>, CreateReaderCommandHandler>();
        services.AddScoped<ICommandHandler<Application.Commands.Readers.ExtendReaderCommand, DateOnly>, ExtendReaderCommandHandler>();
        services.AddScoped<ICommandHandler<Application.Commands.Readers.CloseReaderCommand>, CloseReaderCommandHandler>();

        // Application Layer - Queries
        services.AddScoped<IQueryHandler<Application.Queries.Books.GetBooksQuery, Models.PagedResult<Models.Book>>, GetBooksQueryHandler>();
        services.AddScoped<IQueryHandler<Application.Queries.Books.GetBookDetailsQuery, Models.Book>, GetBookDetailsQueryHandler>();
        services.AddScoped<IQueryHandler<Application.Queries.Library.GetLibraryBooksQuery, Models.PagedResult<Models.BookWithBorrowInfo>>, GetLibraryBooksQueryHandler>();
        services.AddScoped<IQueryHandler<Application.Queries.Library.GetLibraryBookDetailsQuery, Models.BookDetails>, GetLibraryBookDetailsQueryHandler>();
        services.AddScoped<IQueryHandler<Application.Queries.Readers.GetReaderBooksQuery, IReadOnlyList<Models.BorrowedBook>>, GetReaderBooksQueryHandler>();

        // Mediator
        services.AddScoped<IMediator, Mediator>();

        return services;
    }
}