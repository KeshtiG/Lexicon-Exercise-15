using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tournament.Data.Migrations
{
    /// <inheritdoc />
    public partial class Changeofcolumnname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Time",
                table: "Games",
                newName: "StartDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Games",
                newName: "Time");
        }
    }
}
