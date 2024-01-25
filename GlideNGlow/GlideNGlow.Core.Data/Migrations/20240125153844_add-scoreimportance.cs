using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GlideNGlow.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class addscoreimportance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ScoreImportance",
                table: "Games",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScoreImportance",
                table: "Games");
        }
    }
}
