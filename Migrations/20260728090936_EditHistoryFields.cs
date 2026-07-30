using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IslamicCompanion.Migrations
{
    /// <inheritdoc />
    public partial class EditHistoryFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Category",
                table: "Activities",
                newName: "TaskName");

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "Activities",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TaskId",
                table: "Activities",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "TaskId",
                table: "Activities");

            migrationBuilder.RenameColumn(
                name: "TaskName",
                table: "Activities",
                newName: "Category");
        }
    }
}
