using Microsoft.Extensions.DependencyInjection;
using PracticalWork.Reports.Application;
using PracticalWork.Reports.Application.Abstractions;
using PracticalWork.Reports.Application.Commands;
using PracticalWork.Reports.Application.Handlers.Commands;
using PracticalWork.Reports.Application.Handlers.Queries;
using PracticalWork.Reports.Application.Queries;

namespace PracticalWork.Reports;

public static class Entry
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        // Register Mediator
        services.AddSingleton<IMediator, Mediator>();

        // Register Command Handlers
        services.AddScoped<ICommandHandler<GenerateReportCommand, Models.Report>, GenerateReportCommandHandler>();

        // Register Query Handlers
        services.AddScoped<IQueryHandler<GetActivityLogsQuery, Models.PagedResult<Models.ActivityLog>>, GetActivityLogsQueryHandler>();
        services.AddScoped<IQueryHandler<GetReportsQuery, IReadOnlyList<Application.Queries.ReportInfo>>, GetReportsQueryHandler>();
        services.AddScoped<IQueryHandler<GetReportDownloadQuery, string>, GetReportDownloadQueryHandler>();

        return services;
    }
}

