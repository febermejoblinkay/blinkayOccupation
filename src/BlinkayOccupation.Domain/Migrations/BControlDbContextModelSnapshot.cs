﻿// <auto-generated />
using System;
using System.Collections.Generic;
using BlinkayOccupation.Domain.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BlinkayOccupation.Domain.Migrations
{
    [DbContext(typeof(BControlDbContext))]
    partial class BControlDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "fuzzystrmatch");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("BlinkayOccupation.Domain.Models.Occupations", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<DateTime>("Created")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("'-infinity'::timestamp with time zone");

                    b.Property<DateTime?>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("Deleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false);

                    b.Property<string>("InstallationId")
                        .HasColumnType("text");

                    b.Property<int?>("PaidOccupation")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0)
                        .HasColumnName("Paid_Occupation");

                    b.Property<int?>("PaidRealOccupation")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0)
                        .HasColumnName("Paid_Real_Occupation");

                    b.Property<string>("TariffId")
                        .HasColumnType("text");

                    b.Property<int?>("Total")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0);

                    b.Property<int?>("UnpaidRealOccupation")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0)
                        .HasColumnName("Unpaid_Real_Occupation");

                    b.Property<DateTime>("Updated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("ZoneId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("TariffId");

                    b.HasIndex(new[] { "Date" }, "IX_occupations_Date");

                    b.HasIndex(new[] { "InstallationId" }, "IX_occupations_InstallationId");

                    b.HasIndex(new[] { "Updated" }, "IX_occupations_Updated");

                    b.HasIndex(new[] { "ZoneId" }, "IX_occupations_ZoneId");

                    b.ToTable("occupations", "preprod");
                });

            modelBuilder.Entity("BlinkayOccupation.Domain.Models.Stays", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<int?>("CaseId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Created")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("'-infinity'::timestamp with time zone");

                    b.Property<bool>("Deleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("EndPaymentDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool?>("EndPaymentProcessed")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("EntryDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("EntryEventId")
                        .HasColumnType("text");

                    b.Property<DateTime?>("ExitDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("ExitEventId")
                        .HasColumnType("text");

                    b.Property<DateTime?>("InitPaymentDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool?>("InitPaymentProcessed")
                        .HasColumnType("boolean");

                    b.Property<string>("InstallationId")
                        .HasColumnType("text");

                    b.Property<DateTime>("Updated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("ZoneId")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text")
                        .HasDefaultValueSql("''::text");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "CaseId" }, "IX_stays_CaseId");

                    b.HasIndex(new[] { "EntryEventId" }, "IX_stays_EntryEventId");

                    b.HasIndex(new[] { "ExitEventId" }, "IX_stays_ExitEventId");

                    b.HasIndex(new[] { "InstallationId" }, "IX_stays_InstallationId");

                    b.HasIndex(new[] { "Updated" }, "IX_stays_Updated");

                    b.HasIndex(new[] { "ZoneId" }, "IX_stays_ZoneId");

                    b.ToTable("stays", "preprod");
                });

            modelBuilder.Entity("BlinkayOccupation.Domain.Models.StaysParkingRights", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<DateTime>("Created")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("'-infinity'::timestamp with time zone");

                    b.Property<bool>("Deleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false);

                    b.Property<string>("ParkingRightId")
                        .HasColumnType("text");

                    b.Property<string>("StayId")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text")
                        .HasDefaultValueSql("''::text");

                    b.Property<DateTime>("Updated")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id")
                        .HasName("PK_parking_right_stays");

                    b.HasIndex(new[] { "ParkingRightId" }, "IX_stays_parking_rights_ParkingRightId");

                    b.HasIndex(new[] { "StayId" }, "IX_stays_parking_rights_StayId");

                    b.HasIndex(new[] { "Updated" }, "IX_stays_parking_rights_Updated");

                    b.ToTable("stays_parking_rights", "preprod");
                });

            modelBuilder.Entity("BlinkayOccupation.Domain.Models.Occupations", b =>
                {
                    b.HasOne("BlinkayOccupation.Domain.Models.Installations", "Installation")
                        .WithMany("Occupations")
                        .HasForeignKey("InstallationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BlinkayOccupation.Domain.Models.Tariffs", "Tariff")
                        .WithMany("Occupations")
                        .HasForeignKey("TariffId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BlinkayOccupation.Domain.Models.Zones", "Zone")
                        .WithMany("Occupations")
                        .HasForeignKey("ZoneId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Installation");

                    b.Navigation("Tariff");

                    b.Navigation("Zone");
                });

            modelBuilder.Entity("BlinkayOccupation.Domain.Models.Stays", b =>
                {
                    b.HasOne("BlinkayOccupation.Domain.Models.ParkingEvents", "EntryEvent")
                        .WithMany("StaysEntryEvent")
                        .HasForeignKey("EntryEventId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BlinkayOccupation.Domain.Models.ParkingEvents", "ExitEvent")
                        .WithMany("StaysExitEvent")
                        .HasForeignKey("ExitEventId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BlinkayOccupation.Domain.Models.Installations", "Installation")
                        .WithMany("Stays")
                        .HasForeignKey("InstallationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BlinkayOccupation.Domain.Models.Zones", "Zone")
                        .WithMany("Stays")
                        .HasForeignKey("ZoneId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EntryEvent");

                    b.Navigation("ExitEvent");

                    b.Navigation("Installation");

                    b.Navigation("Zone");
                });

            modelBuilder.Entity("BlinkayOccupation.Domain.Models.StaysParkingRights", b =>
                {
                    b.HasOne("BlinkayOccupation.Domain.Models.ParkingRights", "ParkingRight")
                        .WithMany("StaysParkingRights")
                        .HasForeignKey("ParkingRightId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_parking_rights_ParkingRightId");

                    b.HasOne("BlinkayOccupation.Domain.Models.Stays", "Stay")
                        .WithMany("StaysParkingRights")
                        .HasForeignKey("StayId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_parking_right_stays_StayId");

                    b.Navigation("ParkingRight");

                    b.Navigation("Stay");
                });

            modelBuilder.Entity("BlinkayOccupation.Domain.Models.Installations", b =>
                {
                    b.Navigation("Occupations");

                    b.Navigation("Stays");
                });

            modelBuilder.Entity("BlinkayOccupation.Domain.Models.ParkingEvents", b =>
                {
                    b.Navigation("StaysEntryEvent");

                    b.Navigation("StaysExitEvent");
                });

            modelBuilder.Entity("BlinkayOccupation.Domain.Models.ParkingRights", b =>
                {
                    b.Navigation("StaysParkingRights");
                });

            modelBuilder.Entity("BlinkayOccupation.Domain.Models.Stays", b =>
                {
                    b.Navigation("StaysParkingRights");
                });

            modelBuilder.Entity("BlinkayOccupation.Domain.Models.Tariffs", b =>
                {
                    b.Navigation("Occupations");
                });

            modelBuilder.Entity("BlinkayOccupation.Domain.Models.Zones", b =>
                {
                    b.Navigation("Occupations");

                    b.Navigation("Stays");
                });
#pragma warning restore 612, 618
        }
    }
}
