using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyAPP.Migrations
{
    /// <inheritdoc />
    public partial class AddUtrNumberColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UtrNumber",
                table: "Payments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UtrNumber",
                table: "Payments");
        }
    }
}
