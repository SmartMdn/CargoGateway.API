using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CargoGateway.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SearchEntities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    From = table.Column<string>(type: "text", nullable: false),
                    To = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchEntities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShipmentEntities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SearchId = table.Column<Guid>(type: "uuid", nullable: false),
                    CarrierCode = table.Column<string>(type: "text", nullable: false),
                    FlightNumber = table.Column<string>(type: "text", nullable: false),
                    CargoType = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShipmentEntities_SearchEntities_SearchId",
                        column: x => x.SearchId,
                        principalTable: "SearchEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LegEntities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ShipmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartureLocation = table.Column<string>(type: "text", nullable: false),
                    ArrivalLocation = table.Column<string>(type: "text", nullable: false),
                    DepartureDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DepartureTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    ArrivalDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ArrivalTime = table.Column<TimeOnly>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LegEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LegEntities_ShipmentEntities_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "ShipmentEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LegEntities_ShipmentId",
                table: "LegEntities",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentEntities_SearchId",
                table: "ShipmentEntities",
                column: "SearchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LegEntities");

            migrationBuilder.DropTable(
                name: "ShipmentEntities");

            migrationBuilder.DropTable(
                name: "SearchEntities");
        }
    }
}
