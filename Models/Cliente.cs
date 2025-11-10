using System.ComponentModel.DataAnnotations;

namespace GestorClientesApi.Models
{
    public class Cliente
    {
        [Key]
        [Required]
        [MaxLength(20)]
        public string CI { get; set; } = default!;

        [Required, MaxLength(120)]
        public string Nombres { get; set; } = default!;

        [Required, MaxLength(200)]
        public string Direccion { get; set; } = default!;

        [Required, MaxLength(30)]
        public string Telefono { get; set; } = default!;

        // Guardamos rutas relativas (URL) a archivos en wwwroot
        public string? FotoCasa1Url { get; set; }
        public string? FotoCasa2Url { get; set; }
        public string? FotoCasa3Url { get; set; }
    }
}
