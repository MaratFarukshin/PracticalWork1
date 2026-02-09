using Microsoft.Extensions.DependencyInjection;

namespace PracticalWork.Library.Application;

/// <summary>
/// Реализация медиатора для отправки команд и запросов
/// </summary>
public sealed class Mediator : Abstractions.IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResult> SendAsync<TResult>(Abstractions.ICommand<TResult> command, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(Abstractions.ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResult));
        var handler = _serviceProvider.GetRequiredService(handlerType);
        var handleMethod = handlerType.GetMethod(nameof(Abstractions.ICommandHandler<Abstractions.ICommand<TResult>, TResult>.HandleAsync));
        
        if (handleMethod == null)
            throw new InvalidOperationException($"Handler for command {command.GetType().Name} not found.");

        var result = handleMethod.Invoke(handler, new object[] { command, cancellationToken });
        return await (Task<TResult>)result!;
    }

    public async Task SendAsync(Abstractions.ICommand command, CancellationToken cancellationToken = default)
    {
        var commandType = command.GetType();
        var commandInterface = commandType.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(Abstractions.ICommand<>));
        
        if (commandInterface != null)
        {
            var resultType = commandInterface.GetGenericArguments()[0];
            var handlerType = typeof(Abstractions.ICommandHandler<,>).MakeGenericType(commandType, resultType);
            var handler = _serviceProvider.GetRequiredService(handlerType);
            var handleMethod = handlerType.GetMethod(nameof(Abstractions.ICommandHandler<Abstractions.ICommand<object>, object>.HandleAsync));
            
            if (handleMethod == null)
                throw new InvalidOperationException($"Handler for command {commandType.Name} not found.");

            await (Task)handleMethod.Invoke(handler, new object[] { command, cancellationToken })!;
        }
        else
        {
            var handlerType = typeof(Abstractions.ICommandHandler<>).MakeGenericType(commandType);
            var handler = _serviceProvider.GetRequiredService(handlerType);
            var handleMethod = handlerType.GetMethod(nameof(Abstractions.ICommandHandler<Abstractions.ICommand>.HandleAsync));
            
            if (handleMethod == null)
                throw new InvalidOperationException($"Handler for command {commandType.Name} not found.");

            await (Task)handleMethod.Invoke(handler, new object[] { command, cancellationToken })!;
        }
    }

    public async Task<TResult> SendAsync<TResult>(Abstractions.IQuery<TResult> query, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(Abstractions.IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
        var handler = _serviceProvider.GetRequiredService(handlerType);
        var handleMethod = handlerType.GetMethod(nameof(Abstractions.IQueryHandler<Abstractions.IQuery<TResult>, TResult>.HandleAsync));
        
        if (handleMethod == null)
            throw new InvalidOperationException($"Handler for query {query.GetType().Name} not found.");

        var result = handleMethod.Invoke(handler, new object[] { query, cancellationToken });
        return await (Task<TResult>)result!;
    }
}

