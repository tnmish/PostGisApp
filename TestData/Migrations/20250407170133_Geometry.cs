using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace TestData.Migrations
{
    /// <inheritdoc />
    public partial class Geometry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ActivityType",
                table: "Warehouse",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "Director",
                table: "Warehouse",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Geometry>(
                name: "Geometry",
                table: "Warehouse",
                type: "geometry",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Director",
                table: "Warehouse");

            migrationBuilder.DropColumn(
                name: "Geometry",
                table: "Warehouse");

            migrationBuilder.AlterColumn<int>(
                name: "ActivityType",
                table: "Warehouse",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
