using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace komikaan.Irrigator.Migrations
{
    /// <inheritdoc />
    public partial class InternalId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "internal_id",
                table: "stops",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_stops_internal_id",
                table: "stops",
                column: "internal_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_stops_internal_id",
                table: "stops");

            migrationBuilder.DropColumn(
                name: "internal_id",
                table: "stops");
        }
    }
}
