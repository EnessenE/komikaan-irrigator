﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using komikaan.Irrigator.Contexts;

#nullable disable

namespace komikaan.Irrigator.Migrations
{
    [DbContext(typeof(GTFSContext))]
    [Migration("20240601162538_Nullability3")]
    partial class Nullability3
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("GTFS.Entities.Agency", b =>
                {
                    b.Property<string>("DataOrigin")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("data_origin");

                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<string>("Email")
                        .HasColumnType("text")
                        .HasColumnName("email");

                    b.Property<string>("FareURL")
                        .HasColumnType("text")
                        .HasColumnName("fare_url");

                    b.Property<string>("LanguageCode")
                        .HasColumnType("text")
                        .HasColumnName("language_code");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("Phone")
                        .HasColumnType("text")
                        .HasColumnName("phone");

                    b.Property<string>("Timezone")
                        .HasColumnType("text")
                        .HasColumnName("timezone");

                    b.Property<string>("URL")
                        .HasColumnType("text")
                        .HasColumnName("url");

                    b.HasKey("DataOrigin", "Id")
                        .HasName("pk_agencies");

                    b.HasIndex("DataOrigin")
                        .HasDatabaseName("ix_agencies_data_origin");

                    b.HasIndex("Id", "Name")
                        .HasDatabaseName("ix_agencies_id_name");

                    b.ToTable("agencies", (string)null);
                });

            modelBuilder.Entity("GTFS.Entities.Calendar", b =>
                {
                    b.Property<string>("DataOrigin")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("data_origin");

                    b.Property<string>("ServiceId")
                        .HasColumnType("text")
                        .HasColumnName("service_id");

                    b.Property<DateTimeOffset>("EndDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("end_date");

                    b.Property<bool>("Friday")
                        .HasColumnType("boolean")
                        .HasColumnName("friday");

                    b.Property<byte>("Mask")
                        .HasColumnType("smallint")
                        .HasColumnName("mask");

                    b.Property<bool>("Monday")
                        .HasColumnType("boolean")
                        .HasColumnName("monday");

                    b.Property<bool>("Saturday")
                        .HasColumnType("boolean")
                        .HasColumnName("saturday");

                    b.Property<DateTimeOffset>("StartDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("start_date");

                    b.Property<bool>("Sunday")
                        .HasColumnType("boolean")
                        .HasColumnName("sunday");

                    b.Property<bool>("Thursday")
                        .HasColumnType("boolean")
                        .HasColumnName("thursday");

                    b.Property<bool>("Tuesday")
                        .HasColumnType("boolean")
                        .HasColumnName("tuesday");

                    b.Property<bool>("Wednesday")
                        .HasColumnType("boolean")
                        .HasColumnName("wednesday");

                    b.HasKey("DataOrigin", "ServiceId")
                        .HasName("pk_calenders");

                    b.HasIndex("DataOrigin")
                        .HasDatabaseName("ix_calenders_data_origin");

                    b.HasIndex("ServiceId")
                        .HasDatabaseName("ix_calenders_service_id");

                    b.ToTable("calenders", (string)null);
                });

            modelBuilder.Entity("GTFS.Entities.CalendarDate", b =>
                {
                    b.Property<string>("DataOrigin")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("data_origin");

                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<DateTimeOffset>("Date")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date");

                    b.Property<int>("ExceptionType")
                        .HasColumnType("integer")
                        .HasColumnName("exception_type");

                    b.Property<string>("ServiceId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("service_id");

                    b.HasKey("DataOrigin", "Id")
                        .HasName("pk_calendar_dates");

                    b.HasIndex("DataOrigin")
                        .HasDatabaseName("ix_calendar_dates_data_origin");

                    b.HasIndex("ServiceId")
                        .HasDatabaseName("ix_calendar_dates_service_id");

                    b.ToTable("calendar_dates", (string)null);
                });

            modelBuilder.Entity("GTFS.Entities.Frequency", b =>
                {
                    b.Property<string>("DataOrigin")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("data_origin");

                    b.Property<string>("TripId")
                        .HasColumnType("text")
                        .HasColumnName("trip_id");

                    b.Property<string>("StartTime")
                        .HasColumnType("text")
                        .HasColumnName("start_time");

                    b.Property<string>("EndTime")
                        .HasColumnType("text")
                        .HasColumnName("end_time");

                    b.Property<bool?>("ExactTimes")
                        .HasColumnType("boolean")
                        .HasColumnName("exact_times");

                    b.Property<string>("HeadwaySecs")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("headway_secs");

                    b.HasKey("DataOrigin", "TripId", "StartTime", "EndTime")
                        .HasName("pk_frequencies");

                    b.HasIndex("DataOrigin")
                        .HasDatabaseName("ix_frequencies_data_origin");

                    b.ToTable("frequencies", (string)null);
                });

            modelBuilder.Entity("GTFS.Entities.Pathway", b =>
                {
                    b.Property<string>("DataOrigin")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("data_origin");

                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<string>("FromStopId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("from_stop_id");

                    b.Property<int>("IsBidirectional")
                        .HasColumnType("integer")
                        .HasColumnName("is_bidirectional");

                    b.Property<double?>("Length")
                        .HasColumnType("double precision")
                        .HasColumnName("length");

                    b.Property<double?>("MaxSlope")
                        .HasColumnType("double precision")
                        .HasColumnName("max_slope");

                    b.Property<double?>("MinWidth")
                        .HasColumnType("double precision")
                        .HasColumnName("min_width");

                    b.Property<int>("PathwayMode")
                        .HasColumnType("integer")
                        .HasColumnName("pathway_mode");

                    b.Property<string>("ReversedSignpostedAs")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("reversed_signposted_as");

                    b.Property<string>("SignpostedAs")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("signposted_as");

                    b.Property<int?>("StairCount")
                        .HasColumnType("integer")
                        .HasColumnName("stair_count");

                    b.Property<string>("ToStopId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("to_stop_id");

                    b.Property<int?>("TraversalTime")
                        .HasColumnType("integer")
                        .HasColumnName("traversal_time");

                    b.HasKey("DataOrigin", "Id")
                        .HasName("pk_pathway");

                    b.HasIndex("DataOrigin")
                        .HasDatabaseName("ix_pathway_data_origin");

                    b.ToTable("pathway", (string)null);
                });

            modelBuilder.Entity("GTFS.Entities.Route", b =>
                {
                    b.Property<string>("DataOrigin")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("data_origin");

                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<string>("AgencyId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("agency_id");

                    b.Property<string>("Color")
                        .HasColumnType("text")
                        .HasColumnName("color");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<string>("LongName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("long_name");

                    b.Property<string>("ShortName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("short_name");

                    b.Property<string>("TextColor")
                        .HasColumnType("text")
                        .HasColumnName("text_color");

                    b.Property<int>("Type")
                        .HasColumnType("integer")
                        .HasColumnName("type");

                    b.Property<string>("Url")
                        .HasColumnType("text")
                        .HasColumnName("url");

                    b.HasKey("DataOrigin", "Id")
                        .HasName("pk_routes");

                    b.HasIndex("AgencyId")
                        .HasDatabaseName("ix_routes_agency_id");

                    b.HasIndex("DataOrigin")
                        .HasDatabaseName("ix_routes_data_origin");

                    b.HasIndex("LongName")
                        .HasDatabaseName("ix_routes_long_name");

                    b.HasIndex("ShortName")
                        .HasDatabaseName("ix_routes_short_name");

                    b.ToTable("routes", (string)null);
                });

            modelBuilder.Entity("GTFS.Entities.Shape", b =>
                {
                    b.Property<string>("DataOrigin")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("data_origin");

                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<long>("Sequence")
                        .HasColumnType("bigint")
                        .HasColumnName("sequence");

                    b.Property<double?>("DistanceTravelled")
                        .HasColumnType("double precision")
                        .HasColumnName("distance_travelled");

                    b.Property<int>("InternalId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("internal_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("InternalId"));

                    b.Property<double>("Latitude")
                        .HasColumnType("double precision")
                        .HasColumnName("latitude");

                    b.Property<double>("Longitude")
                        .HasColumnType("double precision")
                        .HasColumnName("longitude");

                    b.HasKey("DataOrigin", "Id", "Sequence")
                        .HasName("pk_shapes");

                    b.HasIndex("DataOrigin")
                        .HasDatabaseName("ix_shapes_data_origin");

                    b.HasIndex("Id")
                        .HasDatabaseName("ix_shapes_id");

                    b.ToTable("shapes", (string)null);
                });

            modelBuilder.Entity("GTFS.Entities.Stop", b =>
                {
                    b.Property<string>("DataOrigin")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("data_origin");

                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("code");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<double>("Latitude")
                        .HasColumnType("double precision")
                        .HasColumnName("latitude");

                    b.Property<string>("LevelId")
                        .HasColumnType("text")
                        .HasColumnName("level_id");

                    b.Property<int?>("LocationType")
                        .HasColumnType("integer")
                        .HasColumnName("location_type");

                    b.Property<double>("Longitude")
                        .HasColumnType("double precision")
                        .HasColumnName("longitude");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("ParentStation")
                        .HasColumnType("text")
                        .HasColumnName("parent_station");

                    b.Property<string>("PlatformCode")
                        .HasColumnType("text")
                        .HasColumnName("platform_code");

                    b.Property<string>("Timezone")
                        .HasColumnType("text")
                        .HasColumnName("timezone");

                    b.Property<string>("Url")
                        .HasColumnType("text")
                        .HasColumnName("url");

                    b.Property<string>("WheelchairBoarding")
                        .HasColumnType("text")
                        .HasColumnName("wheelchair_boarding");

                    b.Property<string>("Zone")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("zone");

                    b.HasKey("DataOrigin", "Id")
                        .HasName("pk_stops");

                    b.HasIndex("DataOrigin")
                        .HasDatabaseName("ix_stops_data_origin");

                    b.HasIndex("Id")
                        .HasDatabaseName("ix_stops_id");

                    b.HasIndex("Name")
                        .HasDatabaseName("ix_stops_name");

                    b.HasIndex("ParentStation")
                        .HasDatabaseName("ix_stops_parent_station");

                    b.HasIndex("Name", "ParentStation")
                        .HasDatabaseName("ix_stops_name_parent_station");

                    b.ToTable("stops", (string)null);
                });

            modelBuilder.Entity("GTFS.Entities.StopTime", b =>
                {
                    b.Property<string>("DataOrigin")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("data_origin");

                    b.Property<string>("TripId")
                        .HasColumnType("text")
                        .HasColumnName("trip_id");

                    b.Property<string>("StopId")
                        .HasColumnType("text")
                        .HasColumnName("stop_id");

                    b.Property<long>("StopSequence")
                        .HasColumnType("bigint")
                        .HasColumnName("stop_sequence");

                    b.Property<TimeOnly?>("ArrivalTime")
                        .HasColumnType("time without time zone")
                        .HasColumnName("arrival_time");

                    b.Property<TimeOnly?>("DepartureTime")
                        .HasColumnType("time without time zone")
                        .HasColumnName("departure_time");

                    b.Property<int?>("DropOffType")
                        .HasColumnType("integer")
                        .HasColumnName("drop_off_type");

                    b.Property<int?>("PickupType")
                        .HasColumnType("integer")
                        .HasColumnName("pickup_type");

                    b.Property<double?>("ShapeDistTravelled")
                        .HasColumnType("double precision")
                        .HasColumnName("shape_dist_travelled");

                    b.Property<string>("StopHeadsign")
                        .HasColumnType("text")
                        .HasColumnName("stop_headsign");

                    b.Property<int>("TimepointType")
                        .HasColumnType("integer")
                        .HasColumnName("timepoint_type");

                    b.HasKey("DataOrigin", "TripId", "StopId", "StopSequence")
                        .HasName("pk_stop_times");

                    b.HasIndex("DataOrigin")
                        .HasDatabaseName("ix_stop_times_data_origin");

                    b.HasIndex("StopId")
                        .HasDatabaseName("ix_stop_times_stop_id");

                    b.HasIndex("TripId")
                        .HasDatabaseName("ix_stop_times_trip_id");

                    b.ToTable("stop_times", (string)null);
                });

            modelBuilder.Entity("GTFS.Entities.Transfer", b =>
                {
                    b.Property<string>("DataOrigin")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("data_origin");

                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<string>("FromStopId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("from_stop_id");

                    b.Property<string>("MinimumTransferTime")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("minimum_transfer_time");

                    b.Property<string>("ToStopId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("to_stop_id");

                    b.Property<int>("TransferType")
                        .HasColumnType("integer")
                        .HasColumnName("transfer_type");

                    b.HasKey("DataOrigin", "Id")
                        .HasName("pk_transfers");

                    b.HasIndex("DataOrigin")
                        .HasDatabaseName("ix_transfers_data_origin");

                    b.ToTable("transfers", (string)null);
                });

            modelBuilder.Entity("GTFS.Entities.Trip", b =>
                {
                    b.Property<string>("DataOrigin")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("data_origin");

                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<int?>("AccessibilityType")
                        .HasColumnType("integer")
                        .HasColumnName("accessibility_type");

                    b.Property<string>("BlockId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("block_id");

                    b.Property<int?>("Direction")
                        .HasColumnType("integer")
                        .HasColumnName("direction");

                    b.Property<string>("Headsign")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("headsign");

                    b.Property<string>("RouteId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("route_id");

                    b.Property<string>("ServiceId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("service_id");

                    b.Property<string>("ShapeId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("shape_id");

                    b.Property<string>("ShortName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("short_name");

                    b.HasKey("DataOrigin", "Id")
                        .HasName("pk_trips");

                    b.HasIndex("DataOrigin")
                        .HasDatabaseName("ix_trips_data_origin");

                    b.HasIndex("RouteId")
                        .HasDatabaseName("ix_trips_route_id");

                    b.HasIndex("ServiceId")
                        .HasDatabaseName("ix_trips_service_id");

                    b.HasIndex("ShapeId")
                        .HasDatabaseName("ix_trips_shape_id");

                    b.ToTable("trips", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
