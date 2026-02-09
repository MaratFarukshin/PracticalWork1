using PracticalWork.Library.Application.Abstractions;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Application.Commands.Readers;

/// <summary>
/// Команда создания читателя
/// </summary>
public sealed record CreateReaderCommand(Reader Reader) : ICommand<Guid>;

