using GestorClientesApi.Data;
using GestorClientesApi.Middlewares;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(); // <- Importante
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<FormOptions>(o =>
{
    o.MultipartBodyLengthLimit = 512L * 1024L * 1024L; // 512 MB
});

var app = builder.Build();

var webRoot = app.Environment.WebRootPath ?? "wwwroot";
Directory.CreateDirectory(Path.Combine(webRoot, "uploads", "clientes"));
Directory.CreateDirectory(Path.Combine(webRoot, "uploads", "archivos"));



app.UseSwagger();
app.UseSwaggerUI();

// Servir archivos estáticos (wwwroot)
app.UseStaticFiles();

app.UseRequestResponseLogging();

app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();   // crea la DB si no existe
    try { db.Database.Migrate(); } // aplica migraciones pendientes
    catch { /* si no hay migraciones nuevas, no falla */ }
}


app.Run();
