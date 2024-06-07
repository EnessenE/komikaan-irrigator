using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace komikaan.Irrigator.Migrations
{
    /// <inheritdoc />
    public partial class AdditionalIndexes1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_calendar_dates",
                table: "calendar_dates");

            migrationBuilder.AddPrimaryKey(
                name: "pk_calendar_dates",
                table: "calendar_dates",
                columns: new[] { "data_origin", "date", "service_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_calendar_dates",
                table: "calendar_dates");

            migrationBuilder.AddPrimaryKey(
                name: "pk_calendar_dates",
                table: "calendar_dates",
                columns: new[] { "data_origin", "service_id" });
        }
    }
}
