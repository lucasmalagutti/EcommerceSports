using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EcommerceSports.Migrations
{
    /// <inheritdoc />
    public partial class AjustandoFluxo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Transacoes");

            migrationBuilder.DropColumn(
                name: "Valor",
                table: "Transacoes");

            migrationBuilder.AddColumn<int>(
                name: "CartaoId",
                table: "Transacoes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CupomId",
                table: "Transacoes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EnderecoId",
                table: "Transacoes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StatusTransacao",
                table: "Transacoes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "ValorFrete",
                table: "Transacoes",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorTotal",
                table: "Transacoes",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "Cupons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Desconto = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cupons", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transacoes_CartaoId",
                table: "Transacoes",
                column: "CartaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Transacoes_CupomId",
                table: "Transacoes",
                column: "CupomId");

            migrationBuilder.CreateIndex(
                name: "IX_Transacoes_EnderecoId",
                table: "Transacoes",
                column: "EnderecoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transacoes_Cartoes_CartaoId",
                table: "Transacoes",
                column: "CartaoId",
                principalTable: "Cartoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transacoes_Cupons_CupomId",
                table: "Transacoes",
                column: "CupomId",
                principalTable: "Cupons",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Transacoes_Enderecos_EnderecoId",
                table: "Transacoes",
                column: "EnderecoId",
                principalTable: "Enderecos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transacoes_Cartoes_CartaoId",
                table: "Transacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Transacoes_Cupons_CupomId",
                table: "Transacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Transacoes_Enderecos_EnderecoId",
                table: "Transacoes");

            migrationBuilder.DropTable(
                name: "Cupons");

            migrationBuilder.DropIndex(
                name: "IX_Transacoes_CartaoId",
                table: "Transacoes");

            migrationBuilder.DropIndex(
                name: "IX_Transacoes_CupomId",
                table: "Transacoes");

            migrationBuilder.DropIndex(
                name: "IX_Transacoes_EnderecoId",
                table: "Transacoes");

            migrationBuilder.DropColumn(
                name: "CartaoId",
                table: "Transacoes");

            migrationBuilder.DropColumn(
                name: "CupomId",
                table: "Transacoes");

            migrationBuilder.DropColumn(
                name: "EnderecoId",
                table: "Transacoes");

            migrationBuilder.DropColumn(
                name: "StatusTransacao",
                table: "Transacoes");

            migrationBuilder.DropColumn(
                name: "ValorFrete",
                table: "Transacoes");

            migrationBuilder.DropColumn(
                name: "ValorTotal",
                table: "Transacoes");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Transacoes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Valor",
                table: "Transacoes",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
