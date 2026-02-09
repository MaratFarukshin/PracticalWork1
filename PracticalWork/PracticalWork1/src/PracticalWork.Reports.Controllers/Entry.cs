using Asp.Versioning;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

namespace PracticalWork.Reports.Controllers;

public static class Entry
{
    public static IMvcBuilder AddApi(this IMvcBuilder builder)
    {
        builder.Services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1.0);
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

        builder.AddApplicationPart(typeof(Api.v1.ReportsController).Assembly);

        return builder;
    }
}

