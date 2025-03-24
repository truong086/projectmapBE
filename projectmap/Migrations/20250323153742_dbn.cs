using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projectmap.Migrations
{
    public partial class dbn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "trafficEquipments",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CategoryCode = table.Column<int>(type: "int", nullable: true),
                    IdentificationCode = table.Column<double>(type: "double", nullable: true),
                    ManagementUnit = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    JobClassification = table.Column<int>(type: "int", nullable: true),
                    timePosition = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    SignalNumber = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TypesOfSignal = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SignalInstallation = table.Column<int>(type: "int", nullable: true),
                    UseStatus = table.Column<int>(type: "int", nullable: true),
                    DataStatus = table.Column<int>(type: "int", nullable: true),
                    UpdateTime = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    Remark = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Length = table.Column<double>(type: "double", nullable: true),
                    Longitude = table.Column<double>(type: "double", nullable: true),
                    Latitude = table.Column<double>(type: "double", nullable: true),
                    deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trafficEquipments", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Identity = table.Column<int>(type: "int", nullable: true),
                    UserStatus = table.Column<int>(type: "int", nullable: true),
                    deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "repairDetails",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TE_id = table.Column<int>(type: "int", nullable: true),
                    FaultCodes = table.Column<int>(type: "int", nullable: true),
                    RepairStatus = table.Column<int>(type: "int", nullable: true),
                    MaintenanceEngineer = table.Column<int>(type: "int", nullable: true),
                    Remark = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FaultReportingTime = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    RepairCompletionTime = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_repairDetails", x => x.id);
                    table.ForeignKey(
                        name: "FK_repairDetails_trafficEquipments_TE_id",
                        column: x => x.TE_id,
                        principalTable: "trafficEquipments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_repairDetails_users_MaintenanceEngineer",
                        column: x => x.MaintenanceEngineer,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "imageRepairs",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Repair_id = table.Column<int>(type: "int", nullable: true),
                    image = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    publicId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_imageRepairs", x => x.id);
                    table.ForeignKey(
                        name: "FK_imageRepairs_repairDetails_Repair_id",
                        column: x => x.Repair_id,
                        principalTable: "repairDetails",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_imageRepairs_Repair_id",
                table: "imageRepairs",
                column: "Repair_id");

            migrationBuilder.CreateIndex(
                name: "IX_repairDetails_MaintenanceEngineer",
                table: "repairDetails",
                column: "MaintenanceEngineer");

            migrationBuilder.CreateIndex(
                name: "IX_repairDetails_TE_id",
                table: "repairDetails",
                column: "TE_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "imageRepairs");

            migrationBuilder.DropTable(
                name: "repairDetails");

            migrationBuilder.DropTable(
                name: "trafficEquipments");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
