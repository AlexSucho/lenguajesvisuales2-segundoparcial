using System.ComponentModel.DataAnnotations;

namespace GestorClientesApi.Models
{
    public class LogApi
    {
        [Key]
        public int IdLog { get; set; }

        [Required]
        public DateTime DateTime { get; set; } = System.DateTime.UtcNow;

        [MaxLength(50)]
        public string? TipoLog { get; set; } // Info, Error, Warning

        public string? RequestBody { get; set; }
        public string? ResponseBody { get; set; }

        [MaxLength(300)]
        public string? UrlEndpoint { get; set; }

        [MaxLength(10)]
        public string? MetodoHttp { get; set; }

        [MaxLength(50)]
        public string? DireccionIp { get; set; }

        public string? Detalle { get; set; }
    }
}
