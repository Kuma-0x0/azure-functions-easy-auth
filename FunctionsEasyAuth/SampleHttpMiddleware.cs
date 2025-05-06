using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using System.IdentityModel.Tokens.Jwt;

namespace FunctionsEasyAuth;

public class SampleHttpMiddleware : IFunctionsWorkerMiddleware
{
    private static readonly JwtSecurityTokenHandler _tokenHandler = new();

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        var headerValue = await GetAuthorizationHeaderValueAsync(context);
        var token = GetToken(headerValue);
        var claims = _tokenHandler.ReadJwtToken(token);
        var id = claims.Claims.FirstOrDefault(c => c.Type == "oid")?.Value;

        if (string.IsNullOrWhiteSpace(id))
        {
            throw new UnauthorizedAccessException("OIDC claim is missing.");
        }

        Console.WriteLine($"User ID: {id}");

        await next(context);
    }

    private static async Task<string> GetAuthorizationHeaderValueAsync(FunctionContext context)
    {
        var httpRequest = await context.GetHttpRequestDataAsync();
        if (httpRequest is null
            || !httpRequest.Headers.TryGetValues("Authorization", out var values)
            || values is null
            || !values.Any())
        {
            throw new UnauthorizedAccessException("Authorization header is missing.");
        }

        return values.First();
    }

    private static string GetToken(string authorizationHeader)
    {
        var parts = authorizationHeader.Split(" ");
        if (parts is not { Length: 2 })
        {
            throw new UnauthorizedAccessException("Authorization header is malformed.");
        }

        if (!parts[0].Equals("bearer", StringComparison.CurrentCultureIgnoreCase))
        {
            throw new UnauthorizedAccessException("Authorization header is malformed.");
        }

        return parts[1];
    }
}
