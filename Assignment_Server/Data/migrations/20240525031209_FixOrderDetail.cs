using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Assignment_Server.Data.migrations
{
    /// <inheritdoc />
    public partial class FixOrderDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "80ecde0b-a457-4c32-abbd-fd4b4c0e0eb3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ebb8bdf5-cbf9-4adb-9dcc-3f239b6210a0");

            migrationBuilder.RenameColumn(
                name: "Discount",
                table: "OrderDetails",
                newName: "Quantity");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "39b674c9-723d-484d-a624-74a5cf8fdd34", null, "admin", "ADMIN" },
                    { "65058ab0-1bef-445d-abbd-df73c25b0e87", null, "customer", "CUSTOMER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "39b674c9-723d-484d-a624-74a5cf8fdd34");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "65058ab0-1bef-445d-abbd-df73c25b0e87");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "OrderDetails",
                newName: "Discount");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "80ecde0b-a457-4c32-abbd-fd4b4c0e0eb3", null, "admin", "ADMIN" },
                    { "ebb8bdf5-cbf9-4adb-9dcc-3f239b6210a0", null, "customer", "CUSTOMER" }
                });
        }
    }
}
