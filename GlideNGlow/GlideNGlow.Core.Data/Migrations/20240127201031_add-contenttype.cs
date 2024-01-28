using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GlideNGlow.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class addcontenttype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContentType",
                table: "Games",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Games");
        }
    }
}
