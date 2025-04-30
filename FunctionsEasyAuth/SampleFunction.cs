using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FunctionsEasyAuth;

public class SampleFunction(ILogger<SampleFunction> logger)
{
    [Function("SampleHttpFunction")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "sample")] HttpRequest req)
    {
        logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}
