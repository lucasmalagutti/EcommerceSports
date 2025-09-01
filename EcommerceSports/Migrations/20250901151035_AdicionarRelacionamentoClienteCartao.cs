using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceSports.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarRelacionamentoClienteCartao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Cartoes_ClienteId",
                table: "Cartoes",
                column: "ClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cartoes_Clientes_ClienteId",
                table: "Cartoes",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cartoes_Clientes_ClienteId",
                table: "Cartoes");

            migrationBuilder.DropIndex(
                name: "IX_Cartoes_ClienteId",
                table: "Cartoes");
        }
    }
}
