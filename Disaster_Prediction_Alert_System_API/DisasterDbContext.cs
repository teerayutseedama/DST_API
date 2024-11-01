using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Disaster_Prediction_Alert_System_API;

public partial class DisasterDbContext : DbContext
{
    public DisasterDbContext()
    {
    }

    public DisasterDbContext(DbContextOptions<DisasterDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TbAlert> TbAlerts { get; set; }

    public virtual DbSet<TbAlertSetting> TbAlertSettings { get; set; }

    public virtual DbSet<TbRegion> TbRegions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=disaster.database.windows.net;Initial Catalog=disaster_db;User ID=disaster_db_user;Password=P@ssw0rd2*");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TbAlert>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("TbAlerts_PK");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasColumnName("ID");
            entity.Property(e => e.AlertMessage).HasMaxLength(250);
            entity.Property(e => e.DisasterType).HasMaxLength(50);
            entity.Property(e => e.RegionId)
                .HasMaxLength(20)
                .HasColumnName("RegionID");
            entity.Property(e => e.RiskLevel).HasMaxLength(100);
            entity.Property(e => e.Timestamp).HasColumnType("datetime");
        });

        modelBuilder.Entity<TbAlertSetting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("TbAlertSettings_PK");

            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.DisasterType).HasMaxLength(50);
            entity.Property(e => e.RegionId)
                .HasMaxLength(20)
                .HasColumnName("RegionID");
        });

        modelBuilder.Entity<TbRegion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("TbRegions_PK");

            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.DisasterType).HasMaxLength(50);
            entity.Property(e => e.Latitude);
            entity.Property(e => e.Longitude);
            entity.Property(e => e.RegionId)
                .HasMaxLength(20)
                .HasColumnName("RegionID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
