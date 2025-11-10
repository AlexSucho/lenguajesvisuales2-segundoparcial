using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestorClientesApi.Data;

namespace GestorClientesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public LogsController(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Consulta de logs con filtros opcionales: tipo (Info/Error), fecha desde/hasta, método y búsqueda simple.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] string? tipo = null,
            [FromQuery] DateTime? fromUtc = null,
            [FromQuery] DateTime? toUtc = null,
            [FromQuery] string? metodo = null,
            [FromQuery] string? q = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            var query = _db.LogsApi.AsQueryable();

            if (!string.IsNullOrWhiteSpace(tipo))
                query = query.Where(l => l.TipoLog == tipo);

            if (fromUtc.HasValue)
                query = query.Where(l => l.DateTime >= fromUtc.Value);

            if (toUtc.HasValue)
                query = query.Where(l => l.DateTime <= toUtc.Value);

            if (!string.IsNullOrWhiteSpace(metodo))
                query = query.Where(l => l.MetodoHttp == metodo);

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(l =>
                    (l.UrlEndpoint != null && l.UrlEndpoint.Contains(q)) ||
                    (l.RequestBody != null && l.RequestBody.Contains(q)) ||
                    (l.ResponseBody != null && l.ResponseBody.Contains(q)) ||
                    (l.Detalle != null && l.Detalle.Contains(q)));

            var total = await query.CountAsync();

            var data = await query
                .OrderByDescending(l => l.DateTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(l => new {
                    l.IdLog,
                    l.DateTime,
                    l.TipoLog,
                    l.UrlEndpoint,
                    l.MetodoHttp,
                    l.DireccionIp,
                    // Para listar rápido, no devolvemos cuerpos completos (pueden ser grandes)
                    RequestPreview = l.RequestBody != null && l.RequestBody.Length > 256
                        ? l.RequestBody.Substring(0, 256) + "..."
                        : l.RequestBody,
                    ResponsePreview = l.ResponseBody != null && l.ResponseBody.Length > 256
                        ? l.ResponseBody.Substring(0, 256) + "..."
                        : l.ResponseBody
                })
                .ToListAsync();

            return Ok(new { total, page, pageSize, data });
        }
    }
}
