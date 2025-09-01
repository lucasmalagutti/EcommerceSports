using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceSports.Migrations
{
    /// <inheritdoc />
    public partial class RemovendoColunaCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumRanking",
                table: "Clientes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumRanking",
                table: "Clientes",
                type: "integer",
                nullable: true);
        }
    }
}
