using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assignment_Server.Data.migrations
{
    /// <inheritdoc />
    public partial class editFood : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Foods");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Foods",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
