﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using projectmap.Models;

#nullable disable

namespace projectmap.Migrations
{
    [DbContext(typeof(DBContext))]
    [Migration("20250323153742_dbn")]
    partial class dbn
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.20")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("projectmap.Models.ImageRepair", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("Repair_id")
                        .HasColumnType("int");

                    b.Property<string>("cretoredit")
                        .HasColumnType("longtext");

                    b.Property<bool>("deleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("image")
                        .HasColumnType("longtext");

                    b.Property<string>("publicId")
                        .HasColumnType("longtext");

                    b.HasKey("id");

                    b.HasIndex("Repair_id");

                    b.ToTable("imageRepairs");
                });

            modelBuilder.Entity("projectmap.Models.RepairDetails", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("FaultCodes")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset?>("FaultReportingTime")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("MaintenanceEngineer")
                        .HasColumnType("int");

                    b.Property<string>("Remark")
                        .HasColumnType("longtext");

                    b.Property<DateTimeOffset?>("RepairCompletionTime")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("RepairStatus")
                        .HasColumnType("int");

                    b.Property<int?>("TE_id")
                        .HasColumnType("int");

                    b.Property<string>("cretoredit")
                        .HasColumnType("longtext");

                    b.Property<bool>("deleted")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("id");

                    b.HasIndex("MaintenanceEngineer");

                    b.HasIndex("TE_id");

                    b.ToTable("repairDetails");
                });

            modelBuilder.Entity("projectmap.Models.TrafficEquipment", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("CategoryCode")
                        .HasColumnType("int");

                    b.Property<int?>("DataStatus")
                        .HasColumnType("int");

                    b.Property<double?>("IdentificationCode")
                        .HasColumnType("double");

                    b.Property<int?>("JobClassification")
                        .HasColumnType("int");

                    b.Property<double?>("Latitude")
                        .HasColumnType("double");

                    b.Property<double?>("Length")
                        .HasColumnType("double");

                    b.Property<double?>("Longitude")
                        .HasColumnType("double");

                    b.Property<string>("ManagementUnit")
                        .HasColumnType("longtext");

                    b.Property<string>("Remark")
                        .HasColumnType("longtext");

                    b.Property<int?>("SignalInstallation")
                        .HasColumnType("int");

                    b.Property<string>("SignalNumber")
                        .HasColumnType("longtext");

                    b.Property<string>("TypesOfSignal")
                        .HasColumnType("longtext");

                    b.Property<DateTimeOffset>("UpdateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("UseStatus")
                        .HasColumnType("int");

                    b.Property<string>("cretoredit")
                        .HasColumnType("longtext");

                    b.Property<bool>("deleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTimeOffset?>("timePosition")
                        .HasColumnType("datetime(6)");

                    b.HasKey("id");

                    b.ToTable("trafficEquipments");
                });

            modelBuilder.Entity("projectmap.Models.User", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("Identity")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<string>("Password")
                        .HasColumnType("longtext");

                    b.Property<int?>("UserStatus")
                        .HasColumnType("int");

                    b.Property<string>("cretoredit")
                        .HasColumnType("longtext");

                    b.Property<bool>("deleted")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("id");

                    b.ToTable("users");
                });

            modelBuilder.Entity("projectmap.Models.ImageRepair", b =>
                {
                    b.HasOne("projectmap.Models.RepairDetails", "repairDetails")
                        .WithMany("ImageRepairs")
                        .HasForeignKey("Repair_id");

                    b.Navigation("repairDetails");
                });

            modelBuilder.Entity("projectmap.Models.RepairDetails", b =>
                {
                    b.HasOne("projectmap.Models.User", "user")
                        .WithMany("RepairDetails")
                        .HasForeignKey("MaintenanceEngineer")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("projectmap.Models.TrafficEquipment", "trafficEquipment")
                        .WithMany("RepairDetails")
                        .HasForeignKey("TE_id")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("trafficEquipment");

                    b.Navigation("user");
                });

            modelBuilder.Entity("projectmap.Models.RepairDetails", b =>
                {
                    b.Navigation("ImageRepairs");
                });

            modelBuilder.Entity("projectmap.Models.TrafficEquipment", b =>
                {
                    b.Navigation("RepairDetails");
                });

            modelBuilder.Entity("projectmap.Models.User", b =>
                {
                    b.Navigation("RepairDetails");
                });
#pragma warning restore 612, 618
        }
    }
}
