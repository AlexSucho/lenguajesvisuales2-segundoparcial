using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GestorClientesApi.Models.Dtos
{
    public class UploadZipDto
    {
        [Required, MaxLength(20)]
        public string CICliente { get; set; } = default!;

        [Required]
        public IFormFile Zip { get; set; } = default!;
    }
}
