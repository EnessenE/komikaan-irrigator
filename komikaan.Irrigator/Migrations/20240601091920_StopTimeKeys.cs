using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace komikaan.Irrigator.Migrations
{
    /// <inheritdoc />
    public partial class StopTimeKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_stop_times",
                table: "stop_times");

            migrationBuilder.DropColumn(
                name: "id",
                table: "stop_times");

            migrationBuilder.AlterColumn<string>(
                name: "trip_id",
                table: "stop_times",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "stop_id",
                table: "stop_times",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "pk_stop_times",
                table: "stop_times",
                columns: new[] { "data_origin", "trip_id", "stop_id", "stop_sequence" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_stop_times",
                table: "stop_times");

            migrationBuilder.AlterColumn<string>(
                name: "stop_id",
                table: "stop_times",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "trip_id",
                table: "stop_times",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "id",
                table: "stop_times",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "pk_stop_times",
                table: "stop_times",
                columns: new[] { "data_origin", "id" });
        }
    }
}
