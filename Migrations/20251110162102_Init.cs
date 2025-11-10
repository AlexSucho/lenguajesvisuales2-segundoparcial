using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestorClientesApi.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    CI = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nombres = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    FotoCasa1Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FotoCasa2Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FotoCasa3Url = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.CI);
                });

            migrationBuilder.CreateTable(
                name: "LogsApi",
                columns: table => new
                {
                    IdLog = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoLog = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RequestBody = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponseBody = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UrlEndpoint = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    MetodoHttp = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    DireccionIp = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Detalle = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogsApi", x => x.IdLog);
                });

            migrationBuilder.CreateTable(
                name: "ArchivosCliente",
                columns: table => new
                {
                    IdArchivo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CICliente = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NombreArchivo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UrlArchivo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FechaSubida = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchivosCliente", x => x.IdArchivo);
                    table.ForeignKey(
                        name: "FK_ArchivosCliente_Clientes_CICliente",
                        column: x => x.CICliente,
                        principalTable: "Clientes",
                        principalColumn: "CI",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArchivosCliente_CICliente",
                table: "ArchivosCliente",
                column: "CICliente");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArchivosCliente");

            migrationBuilder.DropTable(
                name: "LogsApi");

            migrationBuilder.DropTable(
                name: "Clientes");
        }
    }
}
