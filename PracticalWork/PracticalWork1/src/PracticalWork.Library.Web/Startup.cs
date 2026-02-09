using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using PracticalWork.Library.Cache.Redis;
using PracticalWork.Library.Controllers;
using PracticalWork.Library.Data.Minio;
using PracticalWork.Library.Data.PostgreSql;
using PracticalWork.Library.Domain.Exceptions;
using PracticalWork.Library.MessageBroker.RabbitMQ;
using PracticalWork.Library.Web.Configuration;
using PracticalWork.Library.Web.Middleware;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;

namespace PracticalWork.Library.Web;

public class Startup
{
    private static string _basePath;
    private IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;

        _basePath = string.IsNullOrWhiteSpace(Configuration["GlobalPrefix"]) ? "" : $"/{Configuration["GlobalPrefix"].Trim('/')}";
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddPostgreSqlStorage(cfg =>
        {
            var npgsqlDataSource = new NpgsqlDataSourceBuilder(Configuration["App:DbConnectionString"])
                .EnableDynamicJson()
                .Build();

            cfg.UseNpgsql(npgsqlDataSource);
        });

        services.AddMvc(opt =>
            {
                opt.Filters.Add<DomainExceptionFilter<DomainException>>();
            })
            .AddApi()
            .AddControllersAsServices()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });

        services.AddSwaggerGen(c =>
        {
            // Описываем документ Swagger для версии v1
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "PracticalWork Library API",
                Version = "v1"
            });

            c.UseOneOfForPolymorphism();
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "PracticalWork.Library.Contracts.xml"));
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "PracticalWork.Library.Controllers.xml"));
        });

        services.AddDomain();
        services.AddCache(Configuration);
        services.AddMinioFileStorage(Configuration);
        services.AddRabbitMqMessageBroker(Configuration);
    }

    [UsedImplicitly]
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime,
        ILogger logger, IServiceProvider serviceProvider)
    {
        app.UsePathBase(new PathString(_basePath));

        // Middleware для логирования запросов
        app.UseMiddleware<RequestLoggingMiddleware>();

        app.UseRouting();

        // Подключение Swagger и Swagger UI до Endpoints
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            // В проекте используется только v1, поэтому настраиваем один endpoint
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "V1");
        });

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
