using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using PracticalWork.Reports.Abstractions.Storage;

namespace PracticalWork.Reports.Data.PostgreSql;

public static class Entry
{
    public static IServiceCollection AddPostgreSqlStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ReportsDbContext>(options =>
        {
            var connectionString = configuration["App:DbConnectionString"];
            var npgsqlDataSource = new NpgsqlDataSourceBuilder(connectionString)
                .EnableDynamicJson()
                .Build();

            options.UseNpgsql(npgsqlDataSource);
        });

        services.AddScoped<IActivityLogRepository, Repositories.ActivityLogRepository>();
        services.AddScoped<IReportRepository, Repositories.ReportRepository>();

        return services;
    }
}

