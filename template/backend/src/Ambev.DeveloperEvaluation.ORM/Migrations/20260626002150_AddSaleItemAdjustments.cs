using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ambev.DeveloperEvaluation.ORM.Migrations
{
    /// <inheritdoc />
    public partial class AddSaleItemAdjustments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SALES_ITEMS_additions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    id_sales_item = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo = table.Column<int>(type: "integer", nullable: false),
                    valor = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    autorizador_id = table.Column<Guid>(type: "uuid", nullable: true),
                    autorizador_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    motivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    datahora = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SALES_ITEMS_additions", x => x.id);
                    table.ForeignKey(
                        name: "FK_SALES_ITEMS_additions_SalesItems_id_sales_item",
                        column: x => x.id_sales_item,
                        principalTable: "SalesItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SALES_ITEMS_discount",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    id_sales_item = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo_desconto = table.Column<int>(type: "integer", nullable: false),
                    valor = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    autorizador_id = table.Column<Guid>(type: "uuid", nullable: true),
                    autorizador_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    motivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    datahora = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SALES_ITEMS_discount", x => x.id);
                    table.ForeignKey(
                        name: "FK_SALES_ITEMS_discount_SalesItems_id_sales_item",
                        column: x => x.id_sales_item,
                        principalTable: "SalesItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SALES_ITEMS_additions_id_sales_item",
                table: "SALES_ITEMS_additions",
                column: "id_sales_item");

            migrationBuilder.CreateIndex(
                name: "IX_SALES_ITEMS_discount_id_sales_item",
                table: "SALES_ITEMS_discount",
                column: "id_sales_item");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SALES_ITEMS_additions");

            migrationBuilder.DropTable(
                name: "SALES_ITEMS_discount");
        }
    }
}
