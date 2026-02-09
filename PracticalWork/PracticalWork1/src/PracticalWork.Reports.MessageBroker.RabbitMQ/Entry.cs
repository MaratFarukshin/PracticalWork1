using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PracticalWork.Reports.Abstractions.Services;
using PracticalWork.Reports.Abstractions.Storage;
using PracticalWork.Reports.MessageBroker.RabbitMQ.Services;

namespace PracticalWork.Reports.MessageBroker.RabbitMQ;

public static class Entry
{
    public static IServiceCollection AddRabbitMqMessageConsumer(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration["App:RabbitMQ:ConnectionString"];

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("RabbitMQ connection string is not configured");
        }

        services.AddSingleton<IHostedService>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<RabbitMqMessageConsumer>>();
            var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
            return new RabbitMqMessageConsumer(connectionString, logger, scopeFactory);
        });

        return services;
    }
}

