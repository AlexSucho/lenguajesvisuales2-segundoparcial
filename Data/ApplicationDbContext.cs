using Microsoft.EntityFrameworkCore;
using GestorClientesApi.Models;

namespace GestorClientesApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<ArchivoCliente> ArchivosCliente { get; set; }
        public DbSet<LogApi> LogsApi { get; set; }
    }
}
