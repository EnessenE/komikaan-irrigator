using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace komikaan.Irrigator.Migrations
{
    /// <inheritdoc />
    public partial class AdditionalIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_calendar_dates",
                table: "calendar_dates");

            migrationBuilder.DropColumn(
                name: "id",
                table: "calendar_dates");

            migrationBuilder.AddPrimaryKey(
                name: "pk_calendar_dates",
                table: "calendar_dates",
                columns: new[] { "data_origin", "service_id" });

            migrationBuilder.CreateIndex(
                name: "ix_stops_id_parent_station",
                table: "stops",
                columns: new[] { "id", "parent_station" });

            migrationBuilder.CreateIndex(
                name: "ix_stop_times_arrival_time",
                table: "stop_times",
                column: "arrival_time");

            migrationBuilder.CreateIndex(
                name: "ix_stop_times_arrival_time_departure_time",
                table: "stop_times",
                columns: new[] { "arrival_time", "departure_time" });

            migrationBuilder.CreateIndex(
                name: "ix_stop_times_departure_time",
                table: "stop_times",
                column: "departure_time");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_stops_id_parent_station",
                table: "stops");

            migrationBuilder.DropIndex(
                name: "ix_stop_times_arrival_time",
                table: "stop_times");

            migrationBuilder.DropIndex(
                name: "ix_stop_times_arrival_time_departure_time",
                table: "stop_times");

            migrationBuilder.DropIndex(
                name: "ix_stop_times_departure_time",
                table: "stop_times");

            migrationBuilder.DropPrimaryKey(
                name: "pk_calendar_dates",
                table: "calendar_dates");

            migrationBuilder.AddColumn<string>(
                name: "id",
                table: "calendar_dates",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "pk_calendar_dates",
                table: "calendar_dates",
                columns: new[] { "data_origin", "id" });
        }
    }
}
