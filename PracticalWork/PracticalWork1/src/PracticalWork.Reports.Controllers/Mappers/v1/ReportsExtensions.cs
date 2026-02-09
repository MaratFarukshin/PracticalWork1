using PracticalWork.Reports.Application.Queries;
using PracticalWork.Reports.Contracts.v1.Reports.Response;
using PracticalWork.Reports.Models;
using ReportStatus = PracticalWork.Reports.Contracts.v1.Reports.Response.ReportStatus;

namespace PracticalWork.Reports.Controllers.Mappers.v1;

public static class ReportsExtensions
{
    public static GenerateReportResponse ToGenerateReportResponse(this Report report) =>
        new(
            report.Id,
            report.Name,
            report.GeneratedAt,
            report.PeriodFrom,
            report.PeriodTo,
            (ReportStatus)report.Status
        );

    public static GetActivityLogsResponse ToGetActivityLogsResponse(this PagedResult<ActivityLog> pagedResult) =>
        new(
            pagedResult.Items.Select(log => new ActivityLogItemResponse(
                log.Id,
                log.ExternalBookId,
                log.ExternalReaderId,
                log.EventType,
                log.EventDate,
                log.Metadata
            )).ToList(),
            pagedResult.TotalCount,
            pagedResult.PageNumber,
            pagedResult.PageSize,
            pagedResult.TotalPages
        );

    public static GetReportsResponse ToGetReportsResponse(this IReadOnlyList<ReportInfo> reports) =>
        new(
            reports.Select(r => new ReportInfoResponse(
                r.Id,
                r.Name,
                r.GeneratedAt,
                r.PeriodFrom,
                r.PeriodTo,
                (ReportStatus)r.Status
            )).ToList()
        );
}

