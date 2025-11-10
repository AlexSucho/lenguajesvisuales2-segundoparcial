using System.IO.Compression;
using GestorClientesApi.Data;
using GestorClientesApi.Models;
using GestorClientesApi.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestorClientesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArchivosController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public ArchivosController(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        /// <summary>
        /// Sube un .zip asociado a un cliente, descomprime, guarda en wwwroot y registra cada archivo en BD.
        /// </summary>
        [HttpPost("upload-zip")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> SubirZip([FromForm] UploadZipDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Validar que el cliente exista
            var clienteExiste = await _db.Clientes.AnyAsync(c => c.CI == dto.CICliente);
            if (!clienteExiste) return NotFound($"No existe cliente con CI {dto.CICliente}");

            if (dto.Zip == null || dto.Zip.Length == 0) return BadRequest("Archivo ZIP vacío.");
            if (!Path.GetExtension(dto.Zip.FileName).Equals(".zip", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Debe adjuntar un archivo .zip");

            // Carpeta base: wwwroot/uploads/archivos/{CI}/{timestamp}
            var webRoot = _env.WebRootPath ?? "wwwroot";
            var lote = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var destinoLote = Path.Combine(webRoot, "uploads", "archivos", dto.CICliente, lote);
            Directory.CreateDirectory(destinoLote);

            // Guardar el ZIP temporalmente para descomprimir
            var zipTempPath = Path.Combine(destinoLote, "lote.zip");
            using (var fs = new FileStream(zipTempPath, FileMode.Create))
            {
                await dto.Zip.CopyToAsync(fs);
            }

            // Descomprimir
            var extraerEn = Path.Combine(destinoLote, "files");
            Directory.CreateDirectory(extraerEn);
            ZipFile.ExtractToDirectory(zipTempPath, extraerEn);
            System.IO.File.Delete(zipTempPath);

            // Recorrer archivos extraídos y registrar
            var archivosRegistrados = new List<ArchivoCliente>();
            foreach (var filePath in Directory.GetFiles(extraerEn, "*", SearchOption.AllDirectories))
            {
                // Ignorar carpetas vacías; sólo archivos
                var nombre = Path.GetFileName(filePath);

                // URL relativa
                var urlRelativa = filePath.Replace(webRoot, "").Replace("\\", "/");
                if (!urlRelativa.StartsWith("/")) urlRelativa = "/" + urlRelativa;

                var entidad = new ArchivoCliente
                {
                    CICliente = dto.CICliente,
                    NombreArchivo = nombre,
                    UrlArchivo = urlRelativa,
                    FechaSubida = DateTime.UtcNow
                };
                archivosRegistrados.Add(entidad);
                _db.ArchivosCliente.Add(entidad);
            }

            await _db.SaveChangesAsync();

            return Ok(new
            {
                Mensaje = "ZIP procesado correctamente",
                Cliente = dto.CICliente,
                CantidadArchivos = archivosRegistrados.Count,
                Archivos = archivosRegistrados.Select(a => new { a.IdArchivo, a.NombreArchivo, a.UrlArchivo })
            });
        }

        /// <summary>
        /// Lista los archivos asociados a un cliente por CI.
        /// </summary>
        [HttpGet("{ciCliente}")]
        public async Task<IActionResult> ListarPorCliente(string ciCliente)
        {
            var lista = await _db.ArchivosCliente
                                 .Where(a => a.CICliente == ciCliente)
                                 .OrderByDescending(a => a.FechaSubida)
                                 .ToListAsync();
            return Ok(lista);
        }
    }
}
