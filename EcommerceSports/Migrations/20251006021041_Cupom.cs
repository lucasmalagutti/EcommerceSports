using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceSports.Migrations
{
    /// <inheritdoc />
    public partial class Cupom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transacoes_Cupons_CupomId",
                table: "Transacoes");

            migrationBuilder.AddForeignKey(
                name: "FK_Transacoes_Cupons_CupomId",
                table: "Transacoes",
                column: "CupomId",
                principalTable: "Cupons",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transacoes_Cupons_CupomId",
                table: "Transacoes");

            migrationBuilder.AddForeignKey(
                name: "FK_Transacoes_Cupons_CupomId",
                table: "Transacoes",
                column: "CupomId",
                principalTable: "Cupons",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
