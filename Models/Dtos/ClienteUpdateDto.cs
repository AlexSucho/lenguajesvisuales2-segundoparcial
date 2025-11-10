using System.ComponentModel.DataAnnotations;

namespace GestorClientesApi.Models.Dtos
{
    public class ClienteUpdateDto
    {
        [Required, MaxLength(120)]
        public string Nombres { get; set; } = default!;

        [Required, MaxLength(200)]
        public string Direccion { get; set; } = default!;

        [Required, MaxLength(30)]
        public string Telefono { get; set; } = default!;

        // Mantengo campos opcionales por si luego quieres permitir actualizar URLs
        public string? FotoCasa1Url { get; set; }
        public string? FotoCasa2Url { get; set; }
        public string? FotoCasa3Url { get; set; }
    }
}
