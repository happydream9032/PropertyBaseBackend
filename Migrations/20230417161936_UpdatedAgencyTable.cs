using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropertyBase.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedAgencyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "Agencies",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Street",
                table: "Agencies");
        }
    }
}
