using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace RostalConsoleHelpers.Models.Local
{
    public partial class RostalDbContext : DbContext
    {
        public RostalDbContext()
        {
        }

        public RostalDbContext(DbContextOptions<RostalDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TbookAuthor> TbookAuthor { get; set; }
        public virtual DbSet<TbookTitle> TbookTitle { get; set; }
        public virtual DbSet<Tbooks> Tbooks { get; set; }
        public virtual DbSet<Tlibrary> Tlibrary { get; set; }
        public virtual DbSet<TlibraryBookConnector> TlibraryBookConnector { get; set; }
        public virtual DbSet<TlibraryCategorie> TlibraryCategorie { get; set; }
        public virtual DbSet<TlibrarySubCategorie> TlibrarySubCategorie { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlite("Data Source=Sql/RostalDB.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TbookAuthor>(entity =>
            {
                entity.ToTable("TBookAuthor");

                entity.HasIndex(e => e.Id)
                    .IsUnique();

                entity.HasIndex(e => e.Name)
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).IsRequired();

                entity.HasOne(d => d.IdBookNavigation)
                    .WithMany(p => p.TbookAuthor)
                    .HasForeignKey(d => d.IdBook);
            });

            modelBuilder.Entity<TbookTitle>(entity =>
            {
                entity.ToTable("TBookTitle");

                entity.HasIndex(e => e.Id)
                    .IsUnique();

                entity.HasIndex(e => e.Name)
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).IsRequired();

                entity.HasOne(d => d.IdBookNavigation)
                    .WithMany(p => p.TbookTitle)
                    .HasForeignKey(d => d.IdBook);
            });

            modelBuilder.Entity<Tbooks>(entity =>
            {
                entity.ToTable("TBooks");

                entity.HasIndex(e => e.Cotation)
                    .IsUnique();

                entity.HasIndex(e => e.Guid)
                    .IsUnique();

                entity.HasIndex(e => e.Id)
                    .IsUnique();

                entity.HasIndex(e => e.Isbn)
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.DateAjout).IsRequired();

                entity.Property(e => e.Guid).IsRequired();

                entity.Property(e => e.Isbn).HasColumnName("ISBN");
            });

            modelBuilder.Entity<Tlibrary>(entity =>
            {
                entity.ToTable("TLibrary");

                entity.HasIndex(e => e.Guid)
                    .IsUnique();

                entity.HasIndex(e => e.Id)
                    .IsUnique();

                entity.HasIndex(e => e.Name)
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.DateAjout).IsRequired();

                entity.Property(e => e.Guid).IsRequired();

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<TlibraryBookConnector>(entity =>
            {
                entity.ToTable("TLibraryBookConnector");

                entity.HasIndex(e => e.Id)
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.IdBookNavigation)
                    .WithMany(p => p.TlibraryBookConnector)
                    .HasForeignKey(d => d.IdBook);

                entity.HasOne(d => d.IdCategorieNavigation)
                    .WithMany(p => p.TlibraryBookConnector)
                    .HasForeignKey(d => d.IdCategorie)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.IdLibraryNavigation)
                    .WithMany(p => p.TlibraryBookConnector)
                    .HasForeignKey(d => d.IdLibrary);

                entity.HasOne(d => d.IdSubCategorieNavigation)
                    .WithMany(p => p.TlibraryBookConnector)
                    .HasForeignKey(d => d.IdSubCategorie)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TlibraryCategorie>(entity =>
            {
                entity.ToTable("TLibraryCategorie");

                entity.HasIndex(e => e.Id)
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).IsRequired();

                entity.HasOne(d => d.IdLibraryNavigation)
                    .WithMany(p => p.TlibraryCategorie)
                    .HasForeignKey(d => d.IdLibrary);
            });

            modelBuilder.Entity<TlibrarySubCategorie>(entity =>
            {
                entity.ToTable("TLibrarySubCategorie");

                entity.HasIndex(e => e.Id)
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).IsRequired();

                entity.HasOne(d => d.IdCategorieNavigation)
                    .WithMany(p => p.TlibrarySubCategorie)
                    .HasForeignKey(d => d.IdCategorie);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
