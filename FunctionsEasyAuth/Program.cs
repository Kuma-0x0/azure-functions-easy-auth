using FunctionsEasyAuth;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

builder.UseWhen<SampleHttpMiddleware>(context =>
{
    return context.FunctionDefinition.InputBindings.Values
        .First(data => data.Type.EndsWith("Trigger")).Type == "httpTrigger";
});

builder.Build().Run();
