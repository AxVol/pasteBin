using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pasteBin.Migrations
{
    /// <inheritdoc />
    public partial class Migration_004 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Hash",
                table: "pasts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hash",
                table: "pasts");
        }
    }
}
