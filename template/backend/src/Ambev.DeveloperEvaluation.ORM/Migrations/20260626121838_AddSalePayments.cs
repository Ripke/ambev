using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ambev.DeveloperEvaluation.ORM.Migrations
{
    /// <inheritdoc />
    public partial class AddSalePayments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SalesChanges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    IdSales = table.Column<Guid>(type: "uuid", nullable: false),
                    TypePayment = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesChanges_Sales_IdSales",
                        column: x => x.IdSales,
                        principalTable: "Sales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    IdSales = table.Column<Guid>(type: "uuid", nullable: false),
                    TypePayment = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesPayments_Sales_IdSales",
                        column: x => x.IdSales,
                        principalTable: "Sales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalesChanges_IdSales",
                table: "SalesChanges",
                column: "IdSales");

            migrationBuilder.CreateIndex(
                name: "IX_SalesChanges_IdSales_ChangedAt",
                table: "SalesChanges",
                columns: new[] { "IdSales", "ChangedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesPayments_IdSales",
                table: "SalesPayments",
                column: "IdSales");

            migrationBuilder.CreateIndex(
                name: "IX_SalesPayments_IdSales_PaidAt",
                table: "SalesPayments",
                columns: new[] { "IdSales", "PaidAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SalesChanges");

            migrationBuilder.DropTable(
                name: "SalesPayments");
        }
    }
}
