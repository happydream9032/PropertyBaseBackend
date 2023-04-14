using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropertyBase.Migrations
{
    /// <inheritdoc />
    public partial class AddedImageIdColumnToUserEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageFileId",
                table: "AspNetUsers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageFileId",
                table: "AspNetUsers");
        }
    }
}
