using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceSports.Migrations
{
    /// <inheritdoc />
    public partial class CupoRemove : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transacoes_Cupons_CupomId",
                table: "Transacoes");

            migrationBuilder.DropIndex(
                name: "IX_Transacoes_CupomId",
                table: "Transacoes");

            migrationBuilder.DropColumn(
                name: "CupomId",
                table: "Transacoes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CupomId",
                table: "Transacoes",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transacoes_CupomId",
                table: "Transacoes",
                column: "CupomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transacoes_Cupons_CupomId",
                table: "Transacoes",
                column: "CupomId",
                principalTable: "Cupons",
                principalColumn: "Id");
        }
    }
}
