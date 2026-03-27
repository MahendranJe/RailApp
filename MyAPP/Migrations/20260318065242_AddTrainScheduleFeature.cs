using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyAPP.Migrations
{
    /// <inheritdoc />
    public partial class AddTrainScheduleFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "TrainUpdates",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ScheduleType",
                table: "TrainUpdates",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "TrainUpdates",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TrainScheduleDays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrainUpdateId = table.Column<int>(type: "int", nullable: false),
                    DayOfWeek = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainScheduleDays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainScheduleDays_TrainUpdates_TrainUpdateId",
                        column: x => x.TrainUpdateId,
                        principalTable: "TrainUpdates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrainScheduleDays_TrainUpdateId",
                table: "TrainScheduleDays",
                column: "TrainUpdateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrainScheduleDays");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "TrainUpdates");

            migrationBuilder.DropColumn(
                name: "ScheduleType",
                table: "TrainUpdates");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "TrainUpdates");
        }
    }
}
