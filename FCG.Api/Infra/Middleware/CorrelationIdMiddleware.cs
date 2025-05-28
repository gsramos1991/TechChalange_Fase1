using Microsoft.Extensions.Primitives;

namespace FCG.Api.Infra.Middleware;

public class CorrelationIdMiddleware 
{
    private readonly RequestDelegate _next;
    private const string _correlationIdHeaderName = "x-correlation-id";

    public CorrelationIdMiddleware(RequestDelegate next) => _next = next;


    public async Task Invoke(HttpContext context, ICorrelationIdGenerator correlationIdGenerator)
    {
        StringValues correlationIdValue = GetCorrelationId(context, correlationIdGenerator);
        AddCorrelationIdHeader(context, correlationIdValue);

        await _next(context);
    }

    private static StringValues GetCorrelationId(HttpContext context, ICorrelationIdGenerator correlationIdGenerator)
    {
        if (context.Request.Headers.TryGetValue(_correlationIdHeaderName, out StringValues correlationId))
        {
            correlationIdGenerator.Set(correlationId);
            return correlationId;
        }
        else
        {
            correlationId = Guid.NewGuid().ToString();
            correlationIdGenerator.Set(correlationId);
            return correlationId;
        }
    }

    private static void AddCorrelationIdHeader(HttpContext context, StringValues correlationId)
    {
        // Define o método que será executado antes de a resposta ser enviada
        Task AddHeaders()
        {
            context.Response.Headers[_correlationIdHeaderName] = correlationId;
            context.Request.Headers[_correlationIdHeaderName] = correlationId;
            return Task.CompletedTask;
        }

        context.Response.OnStarting(AddHeaders);
    }

}
