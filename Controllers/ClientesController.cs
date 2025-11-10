using GestorClientesApi.Data;
using GestorClientesApi.Models;
using GestorClientesApi.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestorClientesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public ClientesController(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Crear([FromForm] ClienteCreateDto dto)
        {
            // Validaciones mínimas del enunciado
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Evitar duplicados por CI
            var existe = await _db.Clientes.AnyAsync(c => c.CI == dto.CI);
            if (existe) return Conflict($"Ya existe un cliente con CI {dto.CI}");

            // Carpeta destino: wwwroot/uploads/clientes/{CI}
            var basePath = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "clientes", dto.CI);
            Directory.CreateDirectory(basePath);

            string? foto1 = await GuardarArchivo(dto.FotoCasa1, basePath, "foto1");
            string? foto2 = await GuardarArchivo(dto.FotoCasa2, basePath, "foto2");
            string? foto3 = await GuardarArchivo(dto.FotoCasa3, basePath, "foto3");

            // Rutas relativas para devolver como URL
            string? ToUrl(string? fullPath) =>
                fullPath is null ? null :
                fullPath.Replace(_env.WebRootPath ?? "wwwroot", "").Replace("\\", "/").Insert(0, "");

            var cliente = new Cliente
            {
                CI = dto.CI,
                Nombres = dto.Nombres,
                Direccion = dto.Direccion,
                Telefono = dto.Telefono,
                FotoCasa1Url = ToUrl(foto1),
                FotoCasa2Url = ToUrl(foto2),
                FotoCasa3Url = ToUrl(foto3)
            };

            _db.Clientes.Add(cliente);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(ObtenerPorCI), new { ci = cliente.CI }, cliente);
        }

        [HttpGet("{ci}")]
        public async Task<IActionResult> ObtenerPorCI(string ci)
        {
            var cliente = await _db.Clientes.FindAsync(ci);
            return cliente is null ? NotFound() : Ok(cliente);
        }

        private static async Task<string?> GuardarArchivo(IFormFile? file, string basePath, string nombreBase)
        {
            if (file is null || file.Length == 0) return null;
            var ext = Path.GetExtension(file.FileName);
            var nombre = $"{nombreBase}_{DateTime.UtcNow:yyyyMMddHHmmssfff}{ext}";
            var fullPath = Path.Combine(basePath, nombre);
            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);
            return fullPath;
        }
    }
}
