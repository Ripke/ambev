using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ambev.DeveloperEvaluation.ORM.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCustomersAndRenameSaleUserColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "customer_id",
                table: "sales",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "customer_name",
                table: "sales",
                newName: "user_name");

            migrationBuilder.RenameIndex(
                name: "IX_sales_customer_id",
                table: "sales",
                newName: "IX_sales_user_id");

            migrationBuilder.DropTable(
                name: "customers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "customers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    birth_date = table.Column<DateTime>(type: "date", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    encrypted_cpf = table.Column<string>(type: "text", nullable: false),
                    full_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customers", x => x.id);
                });

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "sales",
                newName: "customer_id");

            migrationBuilder.RenameColumn(
                name: "user_name",
                table: "sales",
                newName: "customer_name");

            migrationBuilder.RenameIndex(
                name: "IX_sales_user_id",
                table: "sales",
                newName: "IX_sales_customer_id");
        }
    }
}
