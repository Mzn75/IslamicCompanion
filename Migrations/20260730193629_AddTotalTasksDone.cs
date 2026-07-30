using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IslamicCompanion.Migrations
{
    /// <inheritdoc />
    public partial class AddTotalTasksDone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalDoneTasks",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalDoneTasks",
                table: "Users");
        }
    }
}
