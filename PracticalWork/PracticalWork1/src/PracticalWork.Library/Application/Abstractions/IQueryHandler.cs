namespace PracticalWork.Library.Application.Abstractions;

/// <summary>
/// Обработчик запроса
/// </summary>
/// <typeparam name="TQuery">Тип запроса</typeparam>
/// <typeparam name="TResult">Тип результата</typeparam>
public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
{
    Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}

/// <summary>
/// Базовый интерфейс запроса
/// </summary>
/// <typeparam name="TResult">Тип результата</typeparam>
public interface IQuery<out TResult>
{
}

