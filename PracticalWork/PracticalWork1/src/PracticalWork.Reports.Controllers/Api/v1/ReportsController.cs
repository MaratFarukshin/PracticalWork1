using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PracticalWork.Reports.Application;
using PracticalWork.Reports.Application.Commands;
using PracticalWork.Reports.Application.Queries;
using PracticalWork.Reports.Contracts.v1.Reports.Request;
using PracticalWork.Reports.Contracts.v1.Reports.Response;
using PracticalWork.Reports.Controllers.Mappers.v1;
using PracticalWork.Reports.Models;
using EventType = PracticalWork.Reports.Models.EventType;

namespace PracticalWork.Reports.Controllers.Api.v1;

/// <summary>
/// Контроллер для работы с отчетами
/// </summary>
[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/reports")]
public sealed class ReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Генерация отчета в формате CSV
    /// </summary>
    [HttpPost("generate")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(GenerateReportResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GenerateReport([FromBody] GenerateReportRequest request)
    {
        var command = new GenerateReportCommand(
            request.PeriodFrom,
            request.PeriodTo,
            request.EventType != null ? (EventType)request.EventType : null
        );

        var report = await _mediator.SendAsync(command);
        return Ok(report.ToGenerateReportResponse());
    }

    /// <summary>
    /// Получение логов активности
    /// </summary>
    [HttpGet("activity")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(GetActivityLogsResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetActivityLogs([FromQuery] GetActivityLogsRequest request)
    {
        var query = new GetActivityLogsQuery(
            request.FromDate,
            request.ToDate,
            request.EventType != null ? (EventType)request.EventType : null,
            request.PageNumber,
            request.PageSize
        );

        var result = await _mediator.SendAsync(query);
        return Ok(result.ToGetActivityLogsResponse());
    }

    /// <summary>
    /// Получение списка отчетов
    /// </summary>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(GetReportsResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetReports()
    {
        var query = new GetReportsQuery();
        var reports = await _mediator.SendAsync(query);
        return Ok(reports.ToGetReportsResponse());
    }

    /// <summary>
    /// Получение URL для скачивания отчета
    /// </summary>
    [HttpGet("{reportName}/download")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(GetReportDownloadResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetReportDownload(string reportName)
    {
        var query = new GetReportDownloadQuery(reportName);
        var downloadUrl = await _mediator.SendAsync(query);
        return Ok(new GetReportDownloadResponse(downloadUrl));
    }
}

