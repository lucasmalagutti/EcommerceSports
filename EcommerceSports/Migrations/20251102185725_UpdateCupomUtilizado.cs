using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceSports.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCupomUtilizado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Desconto",
                table: "Cupons",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataUtilizacao",
                table: "Cupons",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Utilizado",
                table: "Cupons",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataUtilizacao",
                table: "Cupons");

            migrationBuilder.DropColumn(
                name: "Utilizado",
                table: "Cupons");

            migrationBuilder.AlterColumn<float>(
                name: "Desconto",
                table: "Cupons",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");
        }
    }
}
