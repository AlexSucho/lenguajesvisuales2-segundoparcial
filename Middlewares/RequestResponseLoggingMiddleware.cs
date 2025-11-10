using System.Text;
using Microsoft.EntityFrameworkCore;
using GestorClientesApi.Data;
using GestorClientesApi.Models;

namespace GestorClientesApi.Middlewares
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        // Límites para no saturar la DB con payloads gigantes
        private const int MaxBodyToLogBytes = 1024 * 1024; // 1 MB

        public RequestResponseLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestResponseLoggingMiddleware> logger,
            IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public async Task Invoke(HttpContext context)
        {
            var request = context.Request;
            var originalBodyStream = context.Response.Body;

            // Habilitar buffering del request para poder leerlo sin consumir el stream
            request.EnableBuffering();

            string requestBody = await ReadStreamLimitedAsync(request.Body, leaveOpen: true);
            request.Body.Position = 0;

            // Capturar el response
            await using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            string? responseBodyText = null;
            string tipoLog = "Info";
            string detalleError = null;

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                tipoLog = "Error";
                detalleError = ex.ToString();
                // Devolvemos un 500 estandarizado
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                var problem = new
                {
                    status = 500,
                    title = "Error interno en el servidor",
                    traceId = context.TraceIdentifier
                };
                var json = System.Text.Json.JsonSerializer.Serialize(problem);
                await using var writer = new StreamWriter(context.Response.Body, leaveOpen: true);
                await writer.WriteAsync(json);
                await writer.FlushAsync();
            }
            finally
            {
                // Leer el response capturado
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                responseBodyText = await ReadStreamLimitedAsync(context.Response.Body, leaveOpen: true);
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                // Persistir en DB (en un scope nuevo)
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var log = new LogApi
                    {
                        DateTime = DateTime.UtcNow,
                        TipoLog = tipoLog,
                        RequestBody = requestBody,
                        ResponseBody = responseBodyText,
                        UrlEndpoint = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}",
                        MetodoHttp = request.Method,
                        DireccionIp = context.Connection.RemoteIpAddress?.ToString(),
                        Detalle = detalleError
                    };

                    db.LogsApi.Add(log);
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    // Si fallara el log, al menos lo registramos en consola
                    _logger.LogError(e, "Fallo al guardar LogApi");
                }

                // Devolver el response original al cliente
                await responseBody.CopyToAsync(originalBodyStream);
                context.Response.Body = originalBodyStream;
            }
        }

        private static async Task<string> ReadStreamLimitedAsync(Stream stream, bool leaveOpen)
        {
            stream.Seek(0, SeekOrigin.Begin);
            using var ms = new MemoryStream();
            var buffer = new byte[16 * 1024]; // 16KB
            int total = 0, read;
            while ((read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                var toWrite = Math.Min(read, Math.Max(0, MaxBodyToLogBytes - total));
                if (toWrite > 0)
                {
                    await ms.WriteAsync(buffer, 0, toWrite);
                    total += toWrite;
                }
                if (total >= MaxBodyToLogBytes) break;
            }
            if (!leaveOpen) stream.Dispose();
            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }

    // Extensión para registrar con app.UseRequestResponseLogging();
    public static class RequestResponseLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder app)
            => app.UseMiddleware<RequestResponseLoggingMiddleware>();
    }
}
