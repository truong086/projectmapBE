using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projectmap.Migrations
{
    public partial class update_db : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_imageRepairs_repairDetails_Repair_id",
                table: "imageRepairs");

            migrationBuilder.DropForeignKey(
                name: "FK_repairDetails_trafficEquipments_TE_id",
                table: "repairDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_repairRecords_repairDetails_RD_id",
                table: "repairRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_repairRecords_trafficEquipments_TE_id",
                table: "repairRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_repairRecords_users_Engineer_id",
                table: "repairRecords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_trafficEquipments",
                table: "trafficEquipments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_repairRecords",
                table: "repairRecords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_repairDetails",
                table: "repairDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_imageRepairs",
                table: "imageRepairs");

            migrationBuilder.RenameTable(
                name: "trafficEquipments",
                newName: "trafficequipments");

            migrationBuilder.RenameTable(
                name: "repairRecords",
                newName: "repairrecords");

            migrationBuilder.RenameTable(
                name: "repairDetails",
                newName: "repairdetails");

            migrationBuilder.RenameTable(
                name: "imageRepairs",
                newName: "imagerepairs");

            migrationBuilder.RenameIndex(
                name: "IX_repairRecords_TE_id",
                table: "repairrecords",
                newName: "IX_repairrecords_TE_id");

            migrationBuilder.RenameIndex(
                name: "IX_repairRecords_RD_id",
                table: "repairrecords",
                newName: "IX_repairrecords_RD_id");

            migrationBuilder.RenameIndex(
                name: "IX_repairRecords_Engineer_id",
                table: "repairrecords",
                newName: "IX_repairrecords_Engineer_id");

            migrationBuilder.RenameIndex(
                name: "IX_repairDetails_TE_id",
                table: "repairdetails",
                newName: "IX_repairdetails_TE_id");

            migrationBuilder.RenameIndex(
                name: "IX_imageRepairs_Repair_id",
                table: "imagerepairs",
                newName: "IX_imagerepairs_Repair_id");

            migrationBuilder.AddColumn<string>(
                name: "Picture",
                table: "repairrecords",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "repairrecords",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_trafficequipments",
                table: "trafficequipments",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_repairrecords",
                table: "repairrecords",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_repairdetails",
                table: "repairdetails",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_imagerepairs",
                table: "imagerepairs",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_imagerepairs_repairdetails_Repair_id",
                table: "imagerepairs",
                column: "Repair_id",
                principalTable: "repairdetails",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_repairdetails_trafficequipments_TE_id",
                table: "repairdetails",
                column: "TE_id",
                principalTable: "trafficequipments",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_repairrecords_repairdetails_RD_id",
                table: "repairrecords",
                column: "RD_id",
                principalTable: "repairdetails",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_repairrecords_trafficequipments_TE_id",
                table: "repairrecords",
                column: "TE_id",
                principalTable: "trafficequipments",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_repairrecords_users_Engineer_id",
                table: "repairrecords",
                column: "Engineer_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_imagerepairs_repairdetails_Repair_id",
                table: "imagerepairs");

            migrationBuilder.DropForeignKey(
                name: "FK_repairdetails_trafficequipments_TE_id",
                table: "repairdetails");

            migrationBuilder.DropForeignKey(
                name: "FK_repairrecords_repairdetails_RD_id",
                table: "repairrecords");

            migrationBuilder.DropForeignKey(
                name: "FK_repairrecords_trafficequipments_TE_id",
                table: "repairrecords");

            migrationBuilder.DropForeignKey(
                name: "FK_repairrecords_users_Engineer_id",
                table: "repairrecords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_trafficequipments",
                table: "trafficequipments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_repairrecords",
                table: "repairrecords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_repairdetails",
                table: "repairdetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_imagerepairs",
                table: "imagerepairs");

            migrationBuilder.DropColumn(
                name: "Picture",
                table: "repairrecords");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "repairrecords");

            migrationBuilder.RenameTable(
                name: "trafficequipments",
                newName: "trafficEquipments");

            migrationBuilder.RenameTable(
                name: "repairrecords",
                newName: "repairRecords");

            migrationBuilder.RenameTable(
                name: "repairdetails",
                newName: "repairDetails");

            migrationBuilder.RenameTable(
                name: "imagerepairs",
                newName: "imageRepairs");

            migrationBuilder.RenameIndex(
                name: "IX_repairrecords_TE_id",
                table: "repairRecords",
                newName: "IX_repairRecords_TE_id");

            migrationBuilder.RenameIndex(
                name: "IX_repairrecords_RD_id",
                table: "repairRecords",
                newName: "IX_repairRecords_RD_id");

            migrationBuilder.RenameIndex(
                name: "IX_repairrecords_Engineer_id",
                table: "repairRecords",
                newName: "IX_repairRecords_Engineer_id");

            migrationBuilder.RenameIndex(
                name: "IX_repairdetails_TE_id",
                table: "repairDetails",
                newName: "IX_repairDetails_TE_id");

            migrationBuilder.RenameIndex(
                name: "IX_imagerepairs_Repair_id",
                table: "imageRepairs",
                newName: "IX_imageRepairs_Repair_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_trafficEquipments",
                table: "trafficEquipments",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_repairRecords",
                table: "repairRecords",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_repairDetails",
                table: "repairDetails",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_imageRepairs",
                table: "imageRepairs",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_imageRepairs_repairDetails_Repair_id",
                table: "imageRepairs",
                column: "Repair_id",
                principalTable: "repairDetails",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_repairDetails_trafficEquipments_TE_id",
                table: "repairDetails",
                column: "TE_id",
                principalTable: "trafficEquipments",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_repairRecords_repairDetails_RD_id",
                table: "repairRecords",
                column: "RD_id",
                principalTable: "repairDetails",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_repairRecords_trafficEquipments_TE_id",
                table: "repairRecords",
                column: "TE_id",
                principalTable: "trafficEquipments",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_repairRecords_users_Engineer_id",
                table: "repairRecords",
                column: "Engineer_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
