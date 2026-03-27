using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyAPP.Migrations
{
    /// <inheritdoc />
    public partial class AddAlertBroadcastFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Alerts",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Alerts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBroadcast",
                table: "Alerts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "Alerts",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_CreatedByUserId",
                table: "Alerts",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Alerts_AspNetUsers_CreatedByUserId",
                table: "Alerts",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alerts_AspNetUsers_CreatedByUserId",
                table: "Alerts");

            migrationBuilder.DropIndex(
                name: "IX_Alerts_CreatedByUserId",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "IsBroadcast",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "Alerts");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Alerts",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
