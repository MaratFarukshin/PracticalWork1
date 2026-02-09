using Microsoft.Extensions.DependencyInjection;
using PracticalWork.Reports.Application.Abstractions;

namespace PracticalWork.Reports.Application;

/// <summary>
/// Простой медиатор для отправки команд и запросов.
/// Резолвит scoped-обработчики через IServiceScopeFactory, т.к. сам зарегистрирован как Singleton.
/// </summary>
public sealed class Mediator : IMediator
{
    private readonly IServiceScopeFactory _scopeFactory;

    public Mediator(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
    {
        using var scope = _scopeFactory.CreateScope();
        var provider = scope.ServiceProvider;
        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResult));
        var handler = provider.GetRequiredService(handlerType);
        var handleMethod = handlerType.GetMethod(nameof(ICommandHandler<ICommand<TResult>, TResult>.HandleAsync));
        
        if (handleMethod == null)
            throw new InvalidOperationException($"Handler for command {command.GetType().Name} not found.");

        var result = handleMethod.Invoke(handler, new object[] { command, cancellationToken });
        return await (Task<TResult>)result!;
    }

    public async Task SendAsync(ICommand command, CancellationToken cancellationToken = default)
    {
        using var scope = _scopeFactory.CreateScope();
        var provider = scope.ServiceProvider;
        var commandType = command.GetType();
        var commandInterface = commandType.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>));
        
        if (commandInterface != null)
        {
            var resultType = commandInterface.GetGenericArguments()[0];
            var handlerType = typeof(ICommandHandler<,>).MakeGenericType(commandType, resultType);
            var handler = provider.GetRequiredService(handlerType);
            var handleMethod = handlerType.GetMethod(nameof(ICommandHandler<ICommand<object>, object>.HandleAsync));
            
            if (handleMethod == null)
                throw new InvalidOperationException($"Handler for command {commandType.Name} not found.");

            await (Task)handleMethod.Invoke(handler, new object[] { command, cancellationToken })!;
        }
        else
        {
            var handlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);
            var handler = provider.GetRequiredService(handlerType);
            var handleMethod = handlerType.GetMethod(nameof(ICommandHandler<ICommand>.HandleAsync));
            
            if (handleMethod == null)
                throw new InvalidOperationException($"Handler for command {commandType.Name} not found.");

            await (Task)handleMethod.Invoke(handler, new object[] { command, cancellationToken })!;
        }
    }

    public async Task<TResult> SendAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
    {
        using var scope = _scopeFactory.CreateScope();
        var provider = scope.ServiceProvider;
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
        var handler = provider.GetRequiredService(handlerType);
        var handleMethod = handlerType.GetMethod(nameof(IQueryHandler<IQuery<TResult>, TResult>.HandleAsync));
        
        if (handleMethod == null)
            throw new InvalidOperationException($"Handler for query {query.GetType().Name} not found.");

        var result = handleMethod.Invoke(handler, new object[] { query, cancellationToken });
        return await (Task<TResult>)result!;
    }
}


/// <summary>
/// Интерфейс медиатора
/// </summary>
public interface IMediator
{
    Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default);
    Task SendAsync(ICommand command, CancellationToken cancellationToken = default);
    Task<TResult> SendAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
}
