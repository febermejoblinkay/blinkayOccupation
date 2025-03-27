using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlinkayOccupation.Domain.Migrations
{
    /// <inheritdoc />
    public partial class NewTablesForStays : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "preprod");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:fuzzystrmatch", ",,");

            migrationBuilder.CreateTable(
                name: "occupations",
                schema: "preprod",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    InstallationId = table.Column<string>(type: "text", nullable: true),
                    ZoneId = table.Column<string>(type: "text", nullable: true),
                    TariffId = table.Column<string>(type: "text", nullable: true),
                    Paid_Real_Occupation = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    Unpaid_Real_Occupation = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    Paid_Occupation = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    Total = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "'-infinity'::timestamp with time zone"),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_occupations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_occupations_Installations_InstallationId",
                        column: x => x.InstallationId,
                        principalTable: "Installations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_occupations_Tariffs_TariffId",
                        column: x => x.TariffId,
                        principalTable: "Tariffs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_occupations_Zones_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "Zones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "stays",
                schema: "preprod",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    EntryEventId = table.Column<string>(type: "text", nullable: true),
                    ExitEventId = table.Column<string>(type: "text", nullable: true),
                    EntryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExitDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    InstallationId = table.Column<string>(type: "text", nullable: true),
                    ZoneId = table.Column<string>(type: "text", nullable: false, defaultValueSql: "''::text"),
                    CaseId = table.Column<int>(type: "integer", nullable: true),
                    InitPaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndPaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    InitPaymentProcessed = table.Column<bool>(type: "boolean", nullable: true),
                    EndPaymentProcessed = table.Column<bool>(type: "boolean", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "'-infinity'::timestamp with time zone"),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_stays_Installations_InstallationId",
                        column: x => x.InstallationId,
                        principalTable: "Installations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_stays_ParkingEvents_EntryEventId",
                        column: x => x.EntryEventId,
                        principalTable: "ParkingEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_stays_ParkingEvents_ExitEventId",
                        column: x => x.ExitEventId,
                        principalTable: "ParkingEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_stays_Zones_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "Zones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "stays_parking_rights",
                schema: "preprod",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    StayId = table.Column<string>(type: "text", nullable: false, defaultValueSql: "''::text"),
                    ParkingRightId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "'-infinity'::timestamp with time zone"),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parking_right_stays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_parking_right_stays_StayId",
                        column: x => x.StayId,
                        principalSchema: "preprod",
                        principalTable: "stays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_parking_rights_ParkingRightId",
                        column: x => x.ParkingRightId,
                        principalTable: "ParkingRights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_occupations_Date",
                schema: "preprod",
                table: "occupations",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_occupations_InstallationId",
                schema: "preprod",
                table: "occupations",
                column: "InstallationId");

            migrationBuilder.CreateIndex(
                name: "IX_occupations_TariffId",
                schema: "preprod",
                table: "occupations",
                column: "TariffId");

            migrationBuilder.CreateIndex(
                name: "IX_occupations_Updated",
                schema: "preprod",
                table: "occupations",
                column: "Updated");

            migrationBuilder.CreateIndex(
                name: "IX_occupations_ZoneId",
                schema: "preprod",
                table: "occupations",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_stays_CaseId",
                schema: "preprod",
                table: "stays",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_stays_EntryEventId",
                schema: "preprod",
                table: "stays",
                column: "EntryEventId");

            migrationBuilder.CreateIndex(
                name: "IX_stays_ExitEventId",
                schema: "preprod",
                table: "stays",
                column: "ExitEventId");

            migrationBuilder.CreateIndex(
                name: "IX_stays_InstallationId",
                schema: "preprod",
                table: "stays",
                column: "InstallationId");

            migrationBuilder.CreateIndex(
                name: "IX_stays_Updated",
                schema: "preprod",
                table: "stays",
                column: "Updated");

            migrationBuilder.CreateIndex(
                name: "IX_stays_ZoneId",
                schema: "preprod",
                table: "stays",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_stays_parking_rights_ParkingRightId",
                schema: "preprod",
                table: "stays_parking_rights",
                column: "ParkingRightId");

            migrationBuilder.CreateIndex(
                name: "IX_stays_parking_rights_StayId",
                schema: "preprod",
                table: "stays_parking_rights",
                column: "StayId");

            migrationBuilder.CreateIndex(
                name: "IX_stays_parking_rights_Updated",
                schema: "preprod",
                table: "stays_parking_rights",
                column: "Updated");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "occupations",
                schema: "preprod");

            migrationBuilder.DropTable(
                name: "stays_parking_rights",
                schema: "preprod");

            migrationBuilder.DropTable(
                name: "stays",
                schema: "preprod");
        }
    }
}
