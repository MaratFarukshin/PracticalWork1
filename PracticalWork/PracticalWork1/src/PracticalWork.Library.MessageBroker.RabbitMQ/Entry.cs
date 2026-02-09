using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.MessageBroker.RabbitMQ.Services;

namespace PracticalWork.Library.MessageBroker.RabbitMQ;

public static class Entry
{
    public static IServiceCollection AddRabbitMqMessageBroker(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["App:RabbitMQ:ConnectionString"];
        
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("RabbitMQ connection string is not configured");
        }

        services.AddSingleton<IMessagePublisher>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<RabbitMqMessagePublisher>>();
            return new RabbitMqMessagePublisher(connectionString, logger);
        });

        return services;
    }
}

