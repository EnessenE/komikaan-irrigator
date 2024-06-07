using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace komikaan.Irrigator.Migrations
{
    /// <inheritdoc />
    public partial class GeoLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.AddColumn<Point>(
                name: "geo_location",
                table: "stops",
                type: "geometry",
                nullable: false);

            migrationBuilder.AddColumn<Point>(
                name: "geo_location",
                table: "shapes",
                type: "geometry",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "geo_location",
                table: "stops");

            migrationBuilder.DropColumn(
                name: "geo_location",
                table: "shapes");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:postgis", ",,");
        }
    }
}
