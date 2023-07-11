using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropertyBase.Migrations
{
    /// <inheritdoc />
    public partial class updatedPropertyImageTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileId",
                table: "PropertyImages",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileId",
                table: "PropertyImages");
        }
    }
}
