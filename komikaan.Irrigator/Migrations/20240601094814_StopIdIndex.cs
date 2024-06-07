using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace komikaan.Irrigator.Migrations
{
    /// <inheritdoc />
    public partial class StopIdIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_stops_id",
                table: "stops",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_stops_id",
                table: "stops");
        }
    }
}
