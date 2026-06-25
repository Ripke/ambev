using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ambev.DeveloperEvaluation.ORM.Migrations
{
    /// <inheritdoc />
    public partial class AddSales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "sales_numbers");

            migrationBuilder.CreateTable(
                name: "Sales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    SaleNumber = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "nextval('sales_numbers')"),
                    Version = table.Column<Guid>(type: "uuid", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MovementDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FinishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Subtotal = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    AdditionalAmountTotal = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DiscountAmountTotal = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PaymentAmountTotal = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ChangeAmountTotal = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    StatusChangedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsCanceled = table.Column<bool>(type: "boolean", nullable: false),
                    CancellationAuthorizerId = table.Column<Guid>(type: "uuid", nullable: true),
                    CancellationAuthorizerName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CancellationReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sales", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sales_CompanyId",
                table: "Sales",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_CustomerId",
                table: "Sales",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_SaleNumber",
                table: "Sales",
                column: "SaleNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sales_Status",
                table: "Sales",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sales");

            migrationBuilder.DropSequence(
                name: "sales_numbers");
        }
    }
}
