using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace komikaan.Irrigator.Migrations
{
    /// <inheritdoc />
    public partial class GeneralDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "agencies",
                columns: table => new
                {
                    data_origin = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    url = table.Column<string>(type: "text", nullable: true),
                    timezone = table.Column<string>(type: "text", nullable: true),
                    language_code = table.Column<string>(type: "text", nullable: true),
                    phone = table.Column<string>(type: "text", nullable: true),
                    fare_url = table.Column<string>(type: "text", nullable: true),
                    email = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_agencies", x => new { x.data_origin, x.id });
                });

            migrationBuilder.CreateTable(
                name: "calendar_dates",
                columns: table => new
                {
                    data_origin = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    id = table.Column<string>(type: "text", nullable: false),
                    service_id = table.Column<string>(type: "text", nullable: true),
                    date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    exception_type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_calendar_dates", x => new { x.data_origin, x.id });
                });

            migrationBuilder.CreateTable(
                name: "calenders",
                columns: table => new
                {
                    data_origin = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    service_id = table.Column<string>(type: "text", nullable: false),
                    mask = table.Column<byte>(type: "smallint", nullable: false),
                    monday = table.Column<bool>(type: "boolean", nullable: false),
                    tuesday = table.Column<bool>(type: "boolean", nullable: false),
                    wednesday = table.Column<bool>(type: "boolean", nullable: false),
                    thursday = table.Column<bool>(type: "boolean", nullable: false),
                    friday = table.Column<bool>(type: "boolean", nullable: false),
                    saturday = table.Column<bool>(type: "boolean", nullable: false),
                    sunday = table.Column<bool>(type: "boolean", nullable: false),
                    start_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    end_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_calenders", x => new { x.data_origin, x.service_id });
                });

            migrationBuilder.CreateTable(
                name: "frequencies",
                columns: table => new
                {
                    data_origin = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    trip_id = table.Column<string>(type: "text", nullable: false),
                    start_time = table.Column<string>(type: "text", nullable: false),
                    end_time = table.Column<string>(type: "text", nullable: false),
                    headway_secs = table.Column<string>(type: "text", nullable: true),
                    exact_times = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_frequencies", x => new { x.data_origin, x.trip_id, x.start_time, x.end_time });
                });

            migrationBuilder.CreateTable(
                name: "pathway",
                columns: table => new
                {
                    data_origin = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    id = table.Column<string>(type: "text", nullable: false),
                    from_stop_id = table.Column<string>(type: "text", nullable: true),
                    to_stop_id = table.Column<string>(type: "text", nullable: true),
                    pathway_mode = table.Column<int>(type: "integer", nullable: false),
                    is_bidirectional = table.Column<int>(type: "integer", nullable: false),
                    length = table.Column<double>(type: "double precision", nullable: true),
                    traversal_time = table.Column<int>(type: "integer", nullable: true),
                    stair_count = table.Column<int>(type: "integer", nullable: true),
                    max_slope = table.Column<double>(type: "double precision", nullable: true),
                    min_width = table.Column<double>(type: "double precision", nullable: true),
                    signposted_as = table.Column<string>(type: "text", nullable: true),
                    reversed_signposted_as = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pathway", x => new { x.data_origin, x.id });
                });

            migrationBuilder.CreateTable(
                name: "routes",
                columns: table => new
                {
                    data_origin = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    id = table.Column<string>(type: "text", nullable: false),
                    agency_id = table.Column<string>(type: "text", nullable: true),
                    short_name = table.Column<string>(type: "text", nullable: true),
                    long_name = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: false),
                    url = table.Column<string>(type: "text", nullable: true),
                    color = table.Column<string>(type: "text", nullable: true),
                    text_color = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_routes", x => new { x.data_origin, x.id });
                });

            migrationBuilder.CreateTable(
                name: "shapes",
                columns: table => new
                {
                    data_origin = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    id = table.Column<string>(type: "text", nullable: false),
                    sequence = table.Column<long>(type: "bigint", nullable: false),
                    internal_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    latitude = table.Column<double>(type: "double precision", nullable: false),
                    longitude = table.Column<double>(type: "double precision", nullable: false),
                    distance_travelled = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shapes", x => new { x.data_origin, x.id, x.sequence });
                });

            migrationBuilder.CreateTable(
                name: "stop_times",
                columns: table => new
                {
                    data_origin = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    id = table.Column<string>(type: "text", nullable: false),
                    trip_id = table.Column<string>(type: "text", nullable: true),
                    arrival_time = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    departure_time = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    stop_id = table.Column<string>(type: "text", nullable: true),
                    stop_sequence = table.Column<long>(type: "bigint", nullable: false),
                    stop_headsign = table.Column<string>(type: "text", nullable: true),
                    pickup_type = table.Column<int>(type: "integer", nullable: true),
                    drop_off_type = table.Column<int>(type: "integer", nullable: true),
                    shape_dist_travelled = table.Column<double>(type: "double precision", nullable: true),
                    timepoint_type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_stop_times", x => new { x.data_origin, x.id });
                });

            migrationBuilder.CreateTable(
                name: "stops",
                columns: table => new
                {
                    data_origin = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    id = table.Column<string>(type: "text", nullable: false),
                    code = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    latitude = table.Column<double>(type: "double precision", nullable: false),
                    longitude = table.Column<double>(type: "double precision", nullable: false),
                    zone = table.Column<string>(type: "text", nullable: true),
                    url = table.Column<string>(type: "text", nullable: true),
                    location_type = table.Column<int>(type: "integer", nullable: true),
                    parent_station = table.Column<string>(type: "text", nullable: true),
                    timezone = table.Column<string>(type: "text", nullable: true),
                    wheelchair_boarding = table.Column<string>(type: "text", nullable: true),
                    level_id = table.Column<string>(type: "text", nullable: true),
                    platform_code = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_stops", x => new { x.data_origin, x.id });
                });

            migrationBuilder.CreateTable(
                name: "transfers",
                columns: table => new
                {
                    data_origin = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    id = table.Column<string>(type: "text", nullable: false),
                    from_stop_id = table.Column<string>(type: "text", nullable: true),
                    to_stop_id = table.Column<string>(type: "text", nullable: true),
                    transfer_type = table.Column<int>(type: "integer", nullable: false),
                    minimum_transfer_time = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_transfers", x => new { x.data_origin, x.id });
                });

            migrationBuilder.CreateTable(
                name: "trips",
                columns: table => new
                {
                    data_origin = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    id = table.Column<string>(type: "text", nullable: false),
                    route_id = table.Column<string>(type: "text", nullable: true),
                    service_id = table.Column<string>(type: "text", nullable: true),
                    headsign = table.Column<string>(type: "text", nullable: true),
                    short_name = table.Column<string>(type: "text", nullable: true),
                    direction = table.Column<int>(type: "integer", nullable: true),
                    block_id = table.Column<string>(type: "text", nullable: true),
                    shape_id = table.Column<string>(type: "text", nullable: true),
                    accessibility_type = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trips", x => new { x.data_origin, x.id });
                });

            migrationBuilder.CreateIndex(
                name: "ix_agencies_data_origin",
                table: "agencies",
                column: "data_origin");

            migrationBuilder.CreateIndex(
                name: "ix_agencies_id_name",
                table: "agencies",
                columns: new[] { "id", "name" });

            migrationBuilder.CreateIndex(
                name: "ix_calendar_dates_data_origin",
                table: "calendar_dates",
                column: "data_origin");

            migrationBuilder.CreateIndex(
                name: "ix_calendar_dates_service_id",
                table: "calendar_dates",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "ix_calenders_data_origin",
                table: "calenders",
                column: "data_origin");

            migrationBuilder.CreateIndex(
                name: "ix_calenders_service_id",
                table: "calenders",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "ix_frequencies_data_origin",
                table: "frequencies",
                column: "data_origin");

            migrationBuilder.CreateIndex(
                name: "ix_pathway_data_origin",
                table: "pathway",
                column: "data_origin");

            migrationBuilder.CreateIndex(
                name: "ix_routes_agency_id",
                table: "routes",
                column: "agency_id");

            migrationBuilder.CreateIndex(
                name: "ix_routes_data_origin",
                table: "routes",
                column: "data_origin");

            migrationBuilder.CreateIndex(
                name: "ix_routes_long_name",
                table: "routes",
                column: "long_name");

            migrationBuilder.CreateIndex(
                name: "ix_routes_short_name",
                table: "routes",
                column: "short_name");

            migrationBuilder.CreateIndex(
                name: "ix_shapes_data_origin",
                table: "shapes",
                column: "data_origin");

            migrationBuilder.CreateIndex(
                name: "ix_shapes_id",
                table: "shapes",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_stop_times_data_origin",
                table: "stop_times",
                column: "data_origin");

            migrationBuilder.CreateIndex(
                name: "ix_stop_times_stop_id",
                table: "stop_times",
                column: "stop_id");

            migrationBuilder.CreateIndex(
                name: "ix_stop_times_trip_id",
                table: "stop_times",
                column: "trip_id");

            migrationBuilder.CreateIndex(
                name: "ix_stops_data_origin",
                table: "stops",
                column: "data_origin");

            migrationBuilder.CreateIndex(
                name: "ix_stops_name",
                table: "stops",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_stops_name_parent_station",
                table: "stops",
                columns: new[] { "name", "parent_station" });

            migrationBuilder.CreateIndex(
                name: "ix_stops_parent_station",
                table: "stops",
                column: "parent_station");

            migrationBuilder.CreateIndex(
                name: "ix_transfers_data_origin",
                table: "transfers",
                column: "data_origin");

            migrationBuilder.CreateIndex(
                name: "ix_trips_data_origin",
                table: "trips",
                column: "data_origin");

            migrationBuilder.CreateIndex(
                name: "ix_trips_route_id",
                table: "trips",
                column: "route_id");

            migrationBuilder.CreateIndex(
                name: "ix_trips_service_id",
                table: "trips",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "ix_trips_shape_id",
                table: "trips",
                column: "shape_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "agencies");

            migrationBuilder.DropTable(
                name: "calendar_dates");

            migrationBuilder.DropTable(
                name: "calenders");

            migrationBuilder.DropTable(
                name: "frequencies");

            migrationBuilder.DropTable(
                name: "pathway");

            migrationBuilder.DropTable(
                name: "routes");

            migrationBuilder.DropTable(
                name: "shapes");

            migrationBuilder.DropTable(
                name: "stop_times");

            migrationBuilder.DropTable(
                name: "stops");

            migrationBuilder.DropTable(
                name: "transfers");

            migrationBuilder.DropTable(
                name: "trips");
        }
    }
}
