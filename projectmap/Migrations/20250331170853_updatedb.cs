using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projectmap.Migrations
{
    public partial class updatedb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_repairDetails_users_MaintenanceEngineer",
                table: "repairDetails");

            migrationBuilder.DropIndex(
                name: "IX_repairDetails_MaintenanceEngineer",
                table: "repairDetails");

            migrationBuilder.DropColumn(
                name: "MaintenanceEngineer",
                table: "repairDetails");

            migrationBuilder.DropColumn(
                name: "Remark",
                table: "repairDetails");

            migrationBuilder.CreateTable(
                name: "repairRecords",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RD_id = table.Column<int>(type: "int", nullable: true),
                    TE_id = table.Column<int>(type: "int", nullable: true),
                    Engineer_id = table.Column<int>(type: "int", nullable: true),
                    RecordTime = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    NotificationRecord = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_repairRecords", x => x.id);
                    table.ForeignKey(
                        name: "FK_repairRecords_repairDetails_RD_id",
                        column: x => x.RD_id,
                        principalTable: "repairDetails",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_repairRecords_trafficEquipments_TE_id",
                        column: x => x.TE_id,
                        principalTable: "trafficEquipments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_repairRecords_users_Engineer_id",
                        column: x => x.Engineer_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_repairRecords_Engineer_id",
                table: "repairRecords",
                column: "Engineer_id");

            migrationBuilder.CreateIndex(
                name: "IX_repairRecords_RD_id",
                table: "repairRecords",
                column: "RD_id");

            migrationBuilder.CreateIndex(
                name: "IX_repairRecords_TE_id",
                table: "repairRecords",
                column: "TE_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "repairRecords");

            migrationBuilder.AddColumn<int>(
                name: "MaintenanceEngineer",
                table: "repairDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remark",
                table: "repairDetails",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_repairDetails_MaintenanceEngineer",
                table: "repairDetails",
                column: "MaintenanceEngineer");

            migrationBuilder.AddForeignKey(
                name: "FK_repairDetails_users_MaintenanceEngineer",
                table: "repairDetails",
                column: "MaintenanceEngineer",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
