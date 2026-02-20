using Microsoft.EntityFrameworkCore;
using TscLoanManagement.TSCDB.Application.DTOs;
using TscLoanManagement.TSCDB.Core.Domain;
using TscLoanManagement.TSCDB.Core.Domain.Authentication;
using TscLoanManagement.TSCDB.Core.Domain.DDR;
using TscLoanManagement.TSCDB.Core.Domain.Dealer;
using TscLoanManagement.TSCDB.Core.Domain.Loan;
using TscLoanManagement.TSCDB.Core.Domain.LoanDocuments;
using TscLoanManagement.TSCDB.Core.Domain.LoanInstallment;
using TscLoanManagement.TSCDB.Core.Domain.Masters;
using static TscLoanManagement.TSCDB.Core.Domain.Location;

namespace TscLoanManagement.TSCDB.Infrastructure.Data.Context
{
    // OOP (Encapsulation): centralizes DB schema mapping and relationship rules.
    // SOLID (SRP): this class only handles EF Core persistence configuration.
    // Depends on: Core domain entities and EF Core runtime.
    public class TSCDbContext : DbContext
    {
        public TSCDbContext(DbContextOptions<TSCDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<OtpVerification> OtpVerifications { get; set; }
        public DbSet<Dealer> Dealers { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<VehicleInfo> VehicleInfos { get; set; }
        public DbSet<BuyerInfo> BuyerInfos { get; set; }
        public DbSet<LoanAttachment> LoanAttachments { get; set; }
        public DbSet<LoanPayment> LoanPayments { get; set; }
        public DbSet<LoanDetail> LoanDetails { get; set; }
        public DbSet<LoanFee> LoanFees { get; set; }
        public DbSet<LoanInstallment> LoanInstallments { get; set; }
        public DbSet<LoanInterest> LoanInterests { get; set; }
        public DbSet<Waiver> Waivers { get; set; }
        public DbSet<DDR> DDRs { get; set; }
        public DbSet<LoanDocumentUpload> LoanDocumentUploads { get; set; }

        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<EntityType> EntityTypes { get; set; }
        public DbSet<BusinessCategory> BusinessCategories { get; set; }
        public DbSet<BusinessType> BusinessTypes { get; set; }
        public DbSet<AddressStatus> AddressStatuses { get; set; }
        public DbSet<PersonType> PersonTypes { get; set; }
        public DbSet<BankDetail> BankDetails { get; set; }
        public DbSet<PurchaseSource> PurchaseSources { get; set; }
        public DbSet<Make> Makes { get; set; }
        public DbSet<ChequeLocation> ChequeLocations { get; set; }

        public DbSet<Location.State> States { get; set; }
        public DbSet<Location.City> Cities { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }

        public DbSet<PurchaseSourceDocument1> PurchaseSourceDocuments1 { get; set; }
        public DbSet<DDRDocument> DDRDocuments { get; set; }
        public DbSet<PendingInstallment> PendingInstallments { get; set; }

        public DbSet<DealerDashboardSummaryDto> DealerDashboardSummary { get; set; }
        public DbSet<LoanDocumentMaster> LoanDocumentMasters { get; set; }
        public DbSet<LoanDocumentActivity> LoanDocumentActivities { get; set; }

        public DbSet<Designation> Designations { get; set; }
        public DbSet<LoanAccrual> LoanAccrual { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SecurityDepositDetails>()
                .ToTable(tb => tb.HasTrigger("trg_UpdateDealersFromSecurityDeposit"));
            base.OnModelCreating(modelBuilder);
 

            modelBuilder.Entity<Dealer>()
                .HasMany(d => d.BorrowerDetails)
                .WithOne(b => b.Dealer)
                .HasForeignKey(b => b.DealerId);

            modelBuilder.Entity<Dealer>()
                .HasMany(d => d.GuarantorDetails)
                .WithOne(g => g.Dealer)
                .HasForeignKey(g => g.DealerId);

            modelBuilder.Entity<Dealer>()
                .HasMany(d => d.ChequeDetails)
                .WithOne(c => c.Dealer)
                .HasForeignKey(c => c.DealerId);

            modelBuilder.Entity<Dealer>()
                .HasMany(d => d.SecurityDepositDetails)
                .WithOne(s => s.Dealer)
                .HasForeignKey(s => s.DealerId);

            modelBuilder.Entity<DocumentUpload>()
                .HasOne(d => d.Dealer)
                .WithMany(d => d.DocumentUploads)
                .HasForeignKey(d => d.DealerId)
                .OnDelete(DeleteBehavior.Restrict); // prevent cascade

            modelBuilder.Entity<Dealer>()
                .HasOne(d => d.RelationshipManager)
                .WithMany()
                .HasForeignKey(d => d.RelationshipManagerId)
                .OnDelete(DeleteBehavior.Restrict); // optional: prevents cascade delete

            modelBuilder.Entity<Dealer>().Ignore(d => d.RelationshipManagerId);
            modelBuilder.Entity<Dealer>().Ignore(d => d.RelationshipManager);








            modelBuilder.Entity<LoanPayment>()
                .HasOne(lp => lp.Loan)
                .WithMany(l => l.LoanPayments)
                .HasForeignKey(lp => lp.LoanId);

            // Ensure BuyerInfo & VehicleInfo are properly mapped
            //modelBuilder.Entity<VehicleInfo>()
            //    .HasOne(v => v.Loan)
            //    .WithOne(l => l.VehicleInfo)
            //    .HasForeignKey<VehicleInfo>(v => v.LoanId);

            modelBuilder.Entity<BuyerInfo>()
                .HasOne(b => b.Loan)
                .WithOne(l => l.BuyerInfo)
                .HasForeignKey<BuyerInfo>(b => b.LoanId);

            modelBuilder.Entity<LoanAttachment>()
                .HasOne<Loan>()
                .WithMany(l => l.Attachments)
                .HasForeignKey(la => la.LoanId);



            // LoanDetail

            modelBuilder.Entity<LoanDetail>()
                .HasKey(ld => ld.LoanId);

            modelBuilder.Entity<LoanDetail>()
                .Property(ld => ld.LoanId)
                .ValueGeneratedOnAdd(); // Ensure auto-increment

            // Configure one-to-one relationship between Loan and LoanDetail
            modelBuilder.Entity<Loan>()
                .HasOne(l => l.LoanDetail)
                .WithOne()
                .HasForeignKey<Loan>(l => l.LoanDetailId)
                .IsRequired(false);

            modelBuilder.Entity<LoanDetail>()
                .HasKey(ld => ld.LoanId);

            modelBuilder.Entity<LoanDetail>()
                .HasOne(ld => ld.LoanFee)
                .WithOne(fee => fee.Loan)
                .HasForeignKey<LoanFee>(fee => fee.LoanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LoanDetail>()
                .HasMany(ld => ld.Installments)
                .WithOne(inst => inst.Loan)
                .HasForeignKey(inst => inst.LoanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LoanDetail>()
                .HasMany(ld => ld.Interests)
                .WithOne(interest => interest.Loan)
                .HasForeignKey(interest => interest.LoanId)
                .OnDelete(DeleteBehavior.Cascade);

            // LoanFee
            modelBuilder.Entity<LoanFee>()
                .HasKey(f => f.LoanFeeId);

            // LoanInstallment
            modelBuilder.Entity<LoanInstallment>()
                .HasKey(i => i.InstallmentId);

            // LoanInterest
            modelBuilder.Entity<LoanInterest>()
                .HasKey(i => i.InterestId);

            // Waiver
            modelBuilder.Entity<Waiver>()
                .HasKey(w => w.WaiverId);

            modelBuilder.Entity<Waiver>()
                .HasOne(w => w.Loan)
                .WithMany()
                .HasForeignKey(w => w.LoanId)
                .OnDelete(DeleteBehavior.Cascade);

            // Optional: Waiver linked to Installment (if needed in the future)
            // modelBuilder.Entity<Waiver>()
            //     .HasOne<LoanInstallment>()
            //     .WithMany()
            //     .HasForeignKey(w => w.InstallmentId)
            //     .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<DDR>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.DDRNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Amount)
                    .HasPrecision(18, 2);

                entity.Property(e => e.InterestRate)
                    .HasPrecision(5, 2);

                entity.Property(e => e.ProcessingFee)
                    .HasPrecision(18, 2);

                entity.Property(e => e.GSTOnProcessingFee)
                    .HasPrecision(18, 2);

                entity.Property(e => e.InterestWaiver)
                    .HasPrecision(18, 2);

                entity.Property(e => e.DelayedROI)
                    .HasPrecision(18, 2);

                entity.Property(e => e.UtrNumber)
                    .HasMaxLength(50);

                entity.Property(e => e.PurchaseSource)
                    .HasMaxLength(100);

                entity.Property(e => e.InvoiceNumber)
                    .HasMaxLength(50);

                entity.Property(e => e.HoldReason)
                    .HasMaxLength(500);

                entity.Property(e => e.HoldBy)
                    .HasMaxLength(100);

                entity.Property(e => e.RejectedReason)
                    .HasMaxLength(500);

                entity.Property(e => e.RejectedBy)
                    .HasMaxLength(100);

                // Configure relationship with Dealer
                entity.HasOne(d => d.Dealer)
                    .WithMany()
                    .HasForeignKey(d => d.DealerId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Configure relationship with VehicleInfo (One-to-One)
                entity.HasOne(d => d.VehicleInfo)
                    .WithOne()
                    .HasForeignKey<VehicleInfo>(v => v.DDRId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Configure relationship with BuyerInfo (One-to-One)
                //entity.HasOne(d => d.BuyerInfo)
                //    .WithOne()
                //    .HasForeignKey<BuyerInfo>(b => b.DDRId)
                //    .OnDelete(DeleteBehavior.Cascade);

                // Configure relationship with LoanAttachments
                //entity.HasMany(d => d.Attachments)
                //    .WithOne()
                //    .HasForeignKey("DDRId")
                //    .OnDelete(DeleteBehavior.Cascade);
                modelBuilder.Entity<LoanDocumentUpload>(entity =>
                {
                    entity.HasKey(e => e.Id);

                    entity.Property(e => e.DocumentType).HasColumnType("nvarchar(100)").IsRequired();
                    entity.Property(e => e.FileName).HasColumnType("nvarchar(255)").IsRequired();
                    entity.Property(e => e.FilePath).HasColumnType("nvarchar(500)").IsRequired();
                    entity.Property(e => e.UploadedOn).HasColumnType("datetime2");

                    entity.HasOne(e => e.Loan)
                          .WithMany(l => l.LoanDocumentUploads)
                          .HasForeignKey(e => e.LoanId)
                          .OnDelete(DeleteBehavior.Cascade);
                });

                modelBuilder.Entity<RolePermission>()
                    .HasKey(rp => new { rp.RoleId, rp.PermissionId });

                modelBuilder.Entity<RolePermission>()
                    .HasOne(rp => rp.Role)
                    .WithMany(r => r.RolePermissions)
                    .HasForeignKey(rp => rp.RoleId);

                modelBuilder.Entity<RolePermission>()
                    .HasOne(rp => rp.Permission)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(rp => rp.PermissionId);

                modelBuilder.Entity<EntityType>(entity =>
                {
                    entity.ToTable("EntityTypes");
                    entity.HasKey(e => e.Id);
                    entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                });

                modelBuilder.Entity<BusinessCategory>(entity =>
                {
                    entity.ToTable("BusinessCategories");
                    entity.HasKey(e => e.Id);
                    entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                });

                modelBuilder.Entity<BusinessType>(entity =>
                {
                    entity.ToTable("BusinessTypes");
                    entity.HasKey(e => e.Id);
                    entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                });

                modelBuilder.Entity<AddressStatus>(entity =>
                {
                    entity.ToTable("AddressStatuses");
                    entity.HasKey(e => e.Id);
                    entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                });

                modelBuilder.Entity<PersonType>(entity =>
                {
                    entity.ToTable("PersonTypes");
                    entity.HasKey(e => e.Id);
                    entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                });

                modelBuilder.Entity<BankDetail>(entity =>
                {
                    entity.ToTable("BankDetails");
                    entity.HasKey(e => e.Id);
                    entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                });

                modelBuilder.Entity<PurchaseSource>(entity =>
                {
                    entity.ToTable("PurchaseSources");
                    entity.HasKey(e => e.Id);
                    entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                });

                modelBuilder.Entity<Make>(entity =>
                {
                    entity.ToTable("Makes");
                    entity.HasKey(e => e.Id);
                    entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                });

                modelBuilder.Entity<ChequeLocation>(entity =>
                {
                    entity.ToTable("ChequeLocations");
                    entity.HasKey(e => e.Id);
                    entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                });

                modelBuilder.Entity<Location.State>().ToTable("States");
                modelBuilder.Entity<Location.City>().ToTable("Cities");

                modelBuilder.Entity<Location.State>()
                    .HasMany(s => s.Cities)
                    .WithOne(c => c.State)
                    .HasForeignKey(c => c.StateId)
                    .OnDelete(DeleteBehavior.Cascade);
                //Purchase source document type
                modelBuilder.Entity<DocumentType>(entity =>
                {
                    entity.ToTable("DocumentTypes");
                    entity.HasKey(e => e.Id);
                    entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                });

                modelBuilder.Entity<PurchaseSourceDocument1>()
                    .HasOne(p => p.PurchaseSource)
                    .WithMany()
                    .HasForeignKey(p => p.PurchaseSourceId)
                    .OnDelete(DeleteBehavior.Cascade);

                modelBuilder.Entity<DDRDocument>(entity =>
                {
                    entity.HasKey(e => e.Id);
                    entity.Property(e => e.DocumentType).IsRequired().HasMaxLength(100);
                    entity.Property(e => e.FilePath).HasMaxLength(500);
                });

                modelBuilder.Entity<DealerDashboardSummaryDto>().HasNoKey().ToView(null);
                modelBuilder.Entity<LoanAccrual>(entity =>
                {
                    entity.ToTable("LoanAccrual");   // create table
                    entity.HasKey(e => e.Id);        // primary key
                });

            });
        }
    }
}
