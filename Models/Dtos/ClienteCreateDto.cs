using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GestorClientesApi.Models.Dtos
{
    public class ClienteCreateDto
    {
        [Required, MaxLength(20)]
        public string CI { get; set; } = default!;

        [Required, MaxLength(120)]
        public string Nombres { get; set; } = default!;

        [Required, MaxLength(200)]
        public string Direccion { get; set; } = default!;

        [Required, MaxLength(30)]
        public string Telefono { get; set; } = default!;

        // Tres fotos opcionales/obligatorias según tu criterio de validación
        public IFormFile? FotoCasa1 { get; set; }
        public IFormFile? FotoCasa2 { get; set; }
        public IFormFile? FotoCasa3 { get; set; }
    }
}
