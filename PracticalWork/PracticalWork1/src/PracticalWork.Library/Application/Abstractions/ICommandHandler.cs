namespace PracticalWork.Library.Application.Abstractions;

/// <summary>
/// Обработчик команды
/// </summary>
/// <typeparam name="TCommand">Тип команды</typeparam>
/// <typeparam name="TResult">Тип результата</typeparam>
public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand<TResult>
{
    Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

/// <summary>
/// Обработчик команды без результата
/// </summary>
/// <typeparam name="TCommand">Тип команды</typeparam>
public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

/// <summary>
/// Базовый интерфейс команды
/// </summary>
/// <typeparam name="TResult">Тип результата</typeparam>
public interface ICommand<out TResult>
{
}

/// <summary>
/// Базовый интерфейс команды без результата
/// </summary>
public interface ICommand
{
}

