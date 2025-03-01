
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection.PortableExecutable;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using IRepositories.Entity.Accounts;
using IRepositories.Entity.Inventory;
using IRepositories.Entity.Orders;
using IRepositories.Entity.Schedule;
using IRepositories.Entity.Vaccines;

namespace Repositories.Context
{
    public class DatabaseContext : DbContext
    {
        // Constructor with options
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options) { }

        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<VaccineCenter> VaccineCenters { get; set; }
        public DbSet<VaccineBatch> VaccineBatches { get; set; }
        public DbSet<VaccineCategory> VaccineCategories { get; set; }
        public DbSet<Vaccine> Vaccines { get; set; }
        public DbSet<VaccinePackage> VaccinePackages { get; set; }
        public DbSet<VaccinePackageDetail> VaccinePackageDetails { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<ChildrenProfile> ChildrenProfiles { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<OrderVaccineDetails> OrderVaccineDetails { get; set; }
        public DbSet<OrderPackageDetails> OrderPackageDetails { get; set; }
        public DbSet<VaccineHistory> VaccineHistories { get; set; }
        public DbSet<VaccinationSchedule> VaccinationSchedules { get; set; }
        public DbSet<VaccineReaction> VaccineReactions { get; set; }

        // Configure the DbContext to use MySQL
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    // Specify the connection string here (adjust as needed)
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseMySQL("Server=localhost;Database=vaccineschedule;Uid=sa;Pwd=1234567890;");
        //    }
        //}

        // Configuration for relationships and database constraints
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Store RoleEnum as a string in the database
            modelBuilder.Entity<Account>()
                .Property(a => a.Role)
                .HasConversion<string>();

            // VaccineCategory -> ParentCategory relationship
            modelBuilder.Entity<VaccineCategory>()
                .HasOne(v => v.ParentCategory)
                .WithMany()
                .HasForeignKey(v => v.ParentCategoryId);

            // VaccineBatch -> Manufacturer relationship
            modelBuilder.Entity<VaccineBatch>()
                .HasOne(vb => vb.Manufacturer) // VaccineBatch has ONE Manufacturer
                .WithMany(m => m.VaccineBatches) // Manufacturer has MANY VaccineBatches
                .HasForeignKey(vb => vb.ManufacturerId) // Foreign key is ManufacturerId
                .OnDelete(DeleteBehavior.Restrict);

            // VaccineBatch -> VaccineCenter relationship
            modelBuilder.Entity<VaccineBatch>()
                .HasOne(vb => vb.VaccineCenter)  // Updated navigation property name
                .WithMany(vc => vc.VaccineBatches)  // Ensure VaccineCenter has a collection
                .HasForeignKey(vb => vb.VaccineCenterId)  // Updated foreign key property
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Vaccine>()
                    .HasOne(v => v.VaccineCategory)       // A Vaccine has one VaccineCategory
                    .WithMany(vc => vc.Vaccines)          // A VaccineCategory has many Vaccines
                    .HasForeignKey(v => v.VaccineCategoryId);  // The foreign key in Vaccine is VaccineCategoryId

            // ✅ Fix: Vaccine -> VaccineBatch relationship
            modelBuilder.Entity<Vaccine>()
                .HasOne(v => v.Batch)
                .WithMany()
                .HasForeignKey(v => v.BatchId)
                .OnDelete(DeleteBehavior.Restrict);

            // VaccinePackageDetail -> Vaccine relationship
            modelBuilder.Entity<VaccinePackageDetail>()
                .HasOne(vpd => vpd.Vaccine)
                .WithMany()
                .HasForeignKey(vpd => vpd.VaccineId);

            // VaccinePackageDetail -> VaccinePackage relationship
            modelBuilder.Entity<VaccinePackageDetail>()
                .HasOne(vpd => vpd.VaccinePackage)
                .WithMany(vp => vp.PackageDetails)
                .HasForeignKey(vpd => vpd.VaccinePackageId);

            // Account -> VaccineCenter relationship
            modelBuilder.Entity<Account>()
                .HasOne(a => a.VaccineCenter)  // Updated navigation property name
                .WithMany(vc => vc.Accounts)   // Make sure VaccineCenter has a collection of Accounts
                .HasForeignKey(a => a.VaccineCenterId)  // Updated foreign key property
                .OnDelete(DeleteBehavior.Restrict);  // Optional: Define delete behavior

            // ChildrenProfile -> Account relationship
            modelBuilder.Entity<ChildrenProfile>()
                .HasOne(cp => cp.Account)
                .WithMany(a => a.ChildrenProfiles)  // Specify the collection navigation property
                .HasForeignKey(cp => cp.AccountId);

            // Order -> Feedback relationship
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Feedback)
                .WithOne(f => f.Order)
                .HasForeignKey<Feedback>(f => f.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order -> ChildrenProfile relationship
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Profile)
                .WithMany()
                .HasForeignKey(o => o.ProfileId);

            // Payment -> Order relationship
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithMany()
                .HasForeignKey(p => p.OrderId);

            // Configure PayAmount column type
            modelBuilder.Entity<Payment>()
                .Property(p => p.PayAmount)
                .HasColumnType("decimal(18,4)");

            // OrderVaccineDetail -> Order relationship
            modelBuilder.Entity<OrderVaccineDetails>()
                .HasOne(ovd => ovd.Order)
                .WithMany(o => o.OrderVaccineDetails)
                .HasForeignKey(ovd => ovd.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            // OrderVaccineDetail -> Vaccine relationship
            modelBuilder.Entity<OrderVaccineDetails>()
                .HasOne(ovd => ovd.Vaccine)
                .WithMany()
                .HasForeignKey(ovd => ovd.VaccineId)
                .OnDelete(DeleteBehavior.Restrict);

            // VaccineHistory -> Vaccine relationship
            modelBuilder.Entity<VaccineHistory>()
                .HasOne(vh => vh.Vaccine)
                .WithMany()
                .HasForeignKey(vh => vh.VaccineId)
                .OnDelete(DeleteBehavior.Restrict);

            // VaccineHistory -> ChildrenProfile relationship
            modelBuilder.Entity<VaccineHistory>()
                .HasOne(vh => vh.Profile)
                .WithMany()
                .HasForeignKey(vh => vh.ProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            // VaccineHistory -> Account relationship
            modelBuilder.Entity<VaccineHistory>()
                .HasOne(vh => vh.Account)
                .WithMany()
                .HasForeignKey(vh => vh.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // VaccineHistory -> VaccineCenter relationship
            modelBuilder.Entity<VaccineHistory>()
                .HasOne(vh => vh.Center)
                .WithMany()
                .HasForeignKey(vh => vh.CenterId)
                .OnDelete(DeleteBehavior.Restrict);

            // VaccinationSchedule -> ChildrenProfile relationship
            modelBuilder.Entity<VaccinationSchedule>()
                .HasOne(vs => vs.Profile)
                .WithMany(cp => cp.VaccinationSchedules)  // A ChildrenProfile can have many VaccinationSchedules
                .HasForeignKey(vs => vs.ProfileId);

            // VaccinationSchedule -> VaccineCenter relationship
            modelBuilder.Entity<VaccinationSchedule>()
                .HasOne(vs => vs.VaccineCenter)
                .WithMany(vc => vc.VaccinationSchedules)
                .HasForeignKey(vs => vs.VaccineCenterId)
                .OnDelete(DeleteBehavior.Restrict);

            // VaccinationSchedule -> OrderVaccineDetail relationship
            modelBuilder.Entity<VaccinationSchedule>()
                .HasOne(vs => vs.OrderVaccineDetails)
                .WithMany()
                .HasForeignKey(vs => vs.OrderVaccineDetailsId)
                .OnDelete(DeleteBehavior.Restrict);

            // VaccinationSchedule -> OrderPackageDetail relationship
            modelBuilder.Entity<VaccinationSchedule>()
                .HasOne(vs => vs.OrderPackageDetails)
                .WithMany()
                .HasForeignKey(vs => vs.OrderPackageDetailsId)
                .OnDelete(DeleteBehavior.Restrict);

            // VaccineReaction -> VaccinationSchedule relationship
            modelBuilder.Entity<VaccineReaction>()
                .HasOne(vr => vr.VaccinationSchedule)  // Updated navigation property
                .WithMany(vs => vs.VaccineReactions)  // Ensure VaccinationSchedule has a collection
                .HasForeignKey(vr => vr.VaccinationScheduleId);  // Updated foreign key property
        }
    }
}
