using PracticalWork.Reports.Application.Abstractions;
using PracticalWork.Reports.Models;

namespace PracticalWork.Reports.Application.Commands;

/// <summary>
/// Команда генерации отчета
/// </summary>
public sealed record GenerateReportCommand(
    DateOnly PeriodFrom,
    DateOnly PeriodTo,
    EventType? EventType = null
) : ICommand<Report>;

