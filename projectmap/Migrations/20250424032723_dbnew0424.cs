using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projectmap.Migrations
{
    public partial class dbnew0424 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaintenanceEngineer",
                table: "repairdetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_repairdetails_MaintenanceEngineer",
                table: "repairdetails",
                column: "MaintenanceEngineer");

            migrationBuilder.AddForeignKey(
                name: "FK_repairdetails_users_MaintenanceEngineer",
                table: "repairdetails",
                column: "MaintenanceEngineer",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_repairdetails_users_MaintenanceEngineer",
                table: "repairdetails");

            migrationBuilder.DropIndex(
                name: "IX_repairdetails_MaintenanceEngineer",
                table: "repairdetails");

            migrationBuilder.DropColumn(
                name: "MaintenanceEngineer",
                table: "repairdetails");
        }
    }
}
