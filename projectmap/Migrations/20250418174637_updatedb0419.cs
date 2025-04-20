using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projectmap.Migrations
{
    public partial class updatedb0419 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Area_Level3_1",
                table: "trafficequipments",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Area_Level3_2",
                table: "trafficequipments",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "District_1",
                table: "trafficequipments",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "District_2",
                table: "trafficequipments",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Road_1",
                table: "trafficequipments",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Road_2",
                table: "trafficequipments",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "SpecialPOI",
                table: "trafficequipments",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Area_Level3_1",
                table: "trafficequipments");

            migrationBuilder.DropColumn(
                name: "Area_Level3_2",
                table: "trafficequipments");

            migrationBuilder.DropColumn(
                name: "District_1",
                table: "trafficequipments");

            migrationBuilder.DropColumn(
                name: "District_2",
                table: "trafficequipments");

            migrationBuilder.DropColumn(
                name: "Road_1",
                table: "trafficequipments");

            migrationBuilder.DropColumn(
                name: "Road_2",
                table: "trafficequipments");

            migrationBuilder.DropColumn(
                name: "SpecialPOI",
                table: "trafficequipments");
        }
    }
}
