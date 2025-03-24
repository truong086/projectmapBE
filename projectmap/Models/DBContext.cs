using Microsoft.EntityFrameworkCore;

namespace projectmap.Models
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options) { }

        #region DBset
        public DbSet<User> users { get; set; }
        public DbSet<TrafficEquipment> trafficEquipments { get; set; }
        public DbSet<RepairDetails> repairDetails { get; set; }
        public DbSet<ImageRepair> imageRepairs { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(p => p.RepairDetails)
                .WithOne(u => u.user)
                .HasForeignKey(m => m.MaintenanceEngineer)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TrafficEquipment>()
                .HasMany(r => r.RepairDetails)
                .WithOne(t => t.trafficEquipment)
                .HasForeignKey(te => te.TE_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RepairDetails>()
                .HasMany(i => i.ImageRepairs)
                .WithOne(r => r.repairDetails)
                .HasForeignKey(rid => rid.Repair_id);
        }
    }
}
