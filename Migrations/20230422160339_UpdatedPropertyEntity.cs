using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropertyBase.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedPropertyEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Properties",
                newName: "PropertyType");

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishedDate",
                table: "Properties",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublishedDate",
                table: "Properties");

            migrationBuilder.RenameColumn(
                name: "PropertyType",
                table: "Properties",
                newName: "Type");
        }
    }
}
