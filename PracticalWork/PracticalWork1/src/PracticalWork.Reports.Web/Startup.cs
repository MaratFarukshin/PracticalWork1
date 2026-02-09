using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using PracticalWork.Reports;
using PracticalWork.Reports.Cache.Redis;
using PracticalWork.Reports.Controllers;
using PracticalWork.Reports.Data.Minio;
using PracticalWork.Reports.Data.PostgreSql;
using PracticalWork.Reports.MessageBroker.RabbitMQ;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;

namespace PracticalWork.Reports.Web;

public class Startup
{
    private static string? _basePath;
    private IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        _basePath = string.IsNullOrWhiteSpace(Configuration["GlobalPrefix"]) ? "" : $"/{Configuration["GlobalPrefix"]!.Trim('/')}";
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // PostgreSQL
        services.AddPostgreSqlStorage(Configuration);

        // MVC
        services.AddMvc(opt =>
            {
                opt.Filters.Add<DomainExceptionFilter>();
            })
            .AddApi()
            .AddControllersAsServices()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });

        // Swagger
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "PracticalWork Reports API",
                Version = "v1"
            });

            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "PracticalWork.Reports.Contracts.xml"));
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "PracticalWork.Reports.Controllers.xml"));
        });

        // Domain
        services.AddDomain();

        // Infrastructure
        services.AddCache(Configuration);
        services.AddMinioFileStorage(Configuration);
        services.AddRabbitMqMessageConsumer(Configuration);
    }

    [UsedImplicitly]
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime,
        ILogger logger, IServiceProvider serviceProvider)
    {
        app.UsePathBase(new PathString(_basePath));

        app.UseRouting();

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "V1");
        });

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}

/// <summary>
/// Фильтр для обработки исключений домена
/// </summary>
public sealed class DomainExceptionFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var executedContext = await next();

        if (executedContext.Exception != null)
        {
            TryHandleException(executedContext, executedContext.Exception);
        }
    }

    private static void TryHandleException(ActionExecutedContext context, Exception exception)
    {
        if (exception is ArgumentException argEx)
        {
            context.Result = new ObjectResult(new { error = argEx.Message })
            {
                StatusCode = 400
            };
            context.ExceptionHandled = true;
        }
        else if (exception is FileNotFoundException fileEx)
        {
            context.Result = new ObjectResult(new { error = fileEx.Message })
            {
                StatusCode = 404
            };
            context.ExceptionHandled = true;
        }
        else if (exception is InvalidOperationException)
        {
            context.Result = new ObjectResult(new { error = exception.Message })
            {
                StatusCode = 500
            };
            context.ExceptionHandled = true;
        }
    }
}

