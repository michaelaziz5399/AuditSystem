using AuditServer.DataBase.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace AuditServer.DataBase.DBContext
{
    public class AppDbContext(DbContextOptions<AppDbContext> options)
        : IdentityDbContext<ApplicationUser>(options)
    {
        #region DbSets
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<Region> Regions => Set<Region>();
        public DbSet<Governorate> Governorates => Set<Governorate>();
        public DbSet<Branch> Branches => Set<Branch>();
        public DbSet<BranchOperationsOfficer> BranchOperationsOfficers => Set<BranchOperationsOfficer>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<RegionalDirective> RegionalDirectives => Set<RegionalDirective>();
        public DbSet<AuditMission> AuditMissions => Set<AuditMission>();
        public DbSet<MissionAuditor> MissionAuditors => Set<MissionAuditor>();
        public DbSet<MissionOfficerAssignment> MissionOfficerAssignments => Set<MissionOfficerAssignment>();
        public DbSet<MissionClientSample> MissionClientSamples => Set<MissionClientSample>();
        public DbSet<AuditFinding> AuditFindings => Set<AuditFinding>();
        public DbSet<AuditAttachment> AuditAttachments => Set<AuditAttachment>();
        public DbSet<AuditorDelegation> AuditorDelegations => Set<AuditorDelegation>();
        public DbSet<DailyAttendance> DailyAttendances => Set<DailyAttendance>();
        public DbSet<AttendanceLocationLog> AttendanceLocationLogs => Set<AttendanceLocationLog>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        #endregion

        #region OnModelCreating
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // =========================================================================
            // 1. Department (Self-Referencing & Hierarchy)
            // =========================================================================
            builder.Entity<Department>(entity =>
            {
                entity.HasOne(d => d.ParentDepartment)
                      .WithMany(d => d.SubDepartments)
                      .HasForeignKey(d => d.ParentDepartmentId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // =========================================================================
            // 2. ApplicationUser Relationships
            // =========================================================================
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.HasOne(u => u.Department)
                      .WithMany(d => d.Users)
                      .HasForeignKey(u => u.DepartmentId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.ToTable("Auditors");
            });

            // =========================================================================
            // 3. Region, Governorate & Branch Geofencing
            // =========================================================================
            builder.Entity<Governorate>(entity =>
            {
                entity.HasOne(g => g.Region)
                      .WithMany(r => r.Governorates)
                      .HasForeignKey(g => g.RegionId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Branch>(entity =>
            {
                entity.HasIndex(b => b.BranchCode).IsUnique();

                entity.HasOne(b => b.Governorate)
                      .WithMany(g => g.Branches)
                      .HasForeignKey(b => b.GovernorateId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(b => b.Latitude).HasPrecision(18, 8);
                entity.Property(b => b.Longitude).HasPrecision(18, 8);
            });

            // =========================================================================
            // 4. Operations Officer & Customer
            // =========================================================================
            builder.Entity<BranchOperationsOfficer>(entity =>
            {
                entity.HasOne(boo => boo.Branch)
                      .WithMany(b => b.OperationsOfficers)
                      .HasForeignKey(boo => boo.BranchId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Customer>(entity =>
            {
                entity.HasIndex(c => c.AccountNumber).IsUnique();

                entity.HasOne(c => c.OperationsOfficer)
                      .WithMany(oo => oo.Customers)
                      .HasForeignKey(c => c.OperationsOfficerId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // =========================================================================
            // 5. Regional Directive & Audit Mission
            // =========================================================================
            builder.Entity<RegionalDirective>(entity =>
            {
                entity.HasOne(rd => rd.Region)
                      .WithMany(r => r.RegionalDirectives)
                      .HasForeignKey(rd => rd.RegionId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(rd => rd.Status).HasConversion<int>();
            });

            builder.Entity<AuditMission>(entity =>
            {
                entity.HasOne(m => m.Branch)
                      .WithMany(b => b.AuditMissions)
                      .HasForeignKey(m => m.BranchId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.RegionalDirective)
                      .WithMany(rd => rd.Missions)
                      .HasForeignKey(m => m.RegionalDirectiveId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.Property(m => m.Type).HasConversion<int>();
                entity.Property(m => m.Status).HasConversion<int>();
            });

            // =========================================================================
            // 6. Join Tables (MissionAuditor & MissionOfficerAssignment)
            // =========================================================================
            builder.Entity<MissionAuditor>(entity =>
            {
                entity.HasKey(ma => new { ma.MissionId, ma.AuditorId });

                entity.HasOne(ma => ma.Mission)
                      .WithMany(m => m.MissionAuditors)
                      .HasForeignKey(ma => ma.MissionId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ma => ma.Auditor)
                      .WithMany(u => u.MissionAuditors)
                      .HasForeignKey(ma => ma.AuditorId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<MissionOfficerAssignment>(entity =>
            {
                entity.HasOne(moa => moa.Mission)
                      .WithMany(m => m.OfficerAssignments)
                      .HasForeignKey(moa => moa.MissionId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(moa => moa.OperationsOfficer)
                      .WithMany(oo => oo.MissionAssignments)
                      .HasForeignKey(moa => moa.OperationsOfficerId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(moa => moa.AssignedAuditor)
                      .WithMany(u => u.MissionOfficerAssignments)
                      .HasForeignKey(moa => moa.AssignedAuditorId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // =========================================================================
            // 7. MissionClientSample
            // =========================================================================
            builder.Entity<MissionClientSample>(entity =>
            {
                entity.HasOne(mcs => mcs.Mission)
                      .WithMany(m => m.ClientSamples)
                      .HasForeignKey(mcs => mcs.MissionId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(mcs => mcs.Customer)
                      .WithMany(c => c.MissionSamples)
                      .HasForeignKey(mcs => mcs.CustomerId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(mcs => mcs.AssignedAuditor)
                      .WithMany(u => u.AssignedClientSamples)
                      .HasForeignKey(mcs => mcs.AssignedAuditorId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.Property(mcs => mcs.Status).HasConversion<int>();
                entity.Property(mcs => mcs.VisitLatitude).HasPrecision(18, 8);
                entity.Property(mcs => mcs.VisitLongitude).HasPrecision(18, 8);
            });

            // =========================================================================
            // 8. AuditFinding & AuditAttachment (دعم مهام الخزينة والعملاء)
            // =========================================================================
            builder.Entity<AuditFinding>(entity =>
            {
                // ربط إجباري بالمهمة مباشرة (لجميع أنواع المهام بما فيها الخزينة)
                entity.HasOne(af => af.Mission)
                      .WithMany()
                      .HasForeignKey(af => af.MissionId)
                      .OnDelete(DeleteBehavior.Cascade);

                // ربط اختياري بعينة العميل (يكون null في حالة جرد الخزينة)
                entity.HasOne(af => af.MissionClientSample)
                      .WithMany(mcs => mcs.Findings)
                      .HasForeignKey(af => af.MissionClientSampleId)
                      .OnDelete(DeleteBehavior.Restrict); // Restrict لتجنب الـ Multiple Cascade Paths

                entity.HasOne(af => af.CreatedByAuditor)
                      .WithMany(u => u.CreatedFindings)
                      .HasForeignKey(af => af.CreatedByAuditorId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Enums Conversions
                entity.Property(af => af.Category).HasConversion<int>();
                entity.Property(af => af.FieldViolation).HasConversion<int>();
                entity.Property(af => af.FinancialViolation).HasConversion<int>();
                entity.Property(af => af.DocumentaryViolation).HasConversion<int>();
                entity.Property(af => af.Severity).HasConversion<int>();
                entity.Property(af => af.Status).HasConversion<int>();
            });

            builder.Entity<AuditAttachment>(entity =>
            {
                entity.HasOne(a => a.AuditFinding)
                      .WithMany(af => af.Attachments)
                      .HasForeignKey(a => a.AuditFindingId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // =========================================================================
            // 9. AuditorDelegation
            // =========================================================================
            builder.Entity<AuditorDelegation>(entity =>
            {
                entity.HasOne(d => d.Delegator)
                      .WithMany(u => u.DelegationsGiven)
                      .HasForeignKey(d => d.DelegatorId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Delegatee)
                      .WithMany(u => u.DelegationsReceived)
                      .HasForeignKey(d => d.DelegateeId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(d => d.Status).HasConversion<int>();
            });

            // =========================================================================
            // 10. Attendance & Tracking Logs
            // =========================================================================
            builder.Entity<DailyAttendance>(entity =>
            {
                entity.HasIndex(da => new { da.AuditorId, da.AttendanceDate });

                entity.HasOne(da => da.Auditor)
                      .WithMany(u => u.Attendances)
                      .HasForeignKey(da => da.AuditorId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(da => da.Mission)
                      .WithMany()
                      .HasForeignKey(da => da.MissionId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(da => da.CheckInLatitude).HasPrecision(18, 8);
                entity.Property(da => da.CheckInLongitude).HasPrecision(18, 8);
            });

            builder.Entity<AttendanceLocationLog>(entity =>
            {
                entity.HasOne(log => log.DailyAttendance)
                      .WithMany(da => da.LocationLogs)
                      .HasForeignKey(log => log.DailyAttendanceId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(log => log.Latitude).HasPrecision(18, 8);
                entity.Property(log => log.Longitude).HasPrecision(18, 8);
            });

            // =========================================================================
            // 11.UserRegion (Many-to-Many Relationship)
            // =========================================================================
            builder.Entity<UserRegion>()
                .HasKey(ur => new { ur.UserId, ur.RegionId });
        }
        #endregion
    }
}