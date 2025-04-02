using Microsoft.EntityFrameworkCore;

namespace projectmap.Models
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options) { }

        #region DBset
        public DbSet<User> users { get; set; }
        public DbSet<TrafficEquipment> trafficequipments { get; set; }
        public DbSet<RepairDetails> repairdetails { get; set; }
        public DbSet<ImageRepair> imagerepairs { get; set; }
        public DbSet<RepairRecord> repairrecords { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(p => p.RepairRecords)
                .WithOne(u => u.user)
                .HasForeignKey(m => m.Engineer_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TrafficEquipment>()
                .HasMany(r => r.RepairDetails)
                .WithOne(t => t.trafficEquipment)
                .HasForeignKey(te => te.TE_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TrafficEquipment>()
                .HasMany(r => r.RepairRecords)
                .WithOne(t => t.trafficEquipment)
                .HasForeignKey(te => te.TE_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RepairDetails>()
                .HasMany(i => i.ImageRepairs)
                .WithOne(r => r.repairDetails)
                .HasForeignKey(rid => rid.Repair_id);

            modelBuilder.Entity<RepairDetails>()
                .HasMany(i => i.RepairRecords)
                .WithOne(r => r.repairDetails)
                .HasForeignKey(rid => rid.RD_id);
        }
    }
}
