using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DbConsoleHelpers.Models.Local
{
    public partial class LibraryDbContext : DbContext
    {
        public LibraryDbContext()
        {
        }

        public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Tbook> Tbook { get; set; }
        public virtual DbSet<TbookAuthorConnector> TbookAuthorConnector { get; set; }
        public virtual DbSet<TbookClassification> TbookClassification { get; set; }
        public virtual DbSet<TbookCollectionConnector> TbookCollectionConnector { get; set; }
        public virtual DbSet<TbookEditeurConnector> TbookEditeurConnector { get; set; }
        public virtual DbSet<TbookEtat> TbookEtat { get; set; }
        public virtual DbSet<TbookExemplary> TbookExemplary { get; set; }
        public virtual DbSet<TbookFormat> TbookFormat { get; set; }
        public virtual DbSet<TbookIdentification> TbookIdentification { get; set; }
        public virtual DbSet<TbookOtherTitle> TbookOtherTitle { get; set; }
        public virtual DbSet<TbookPret> TbookPret { get; set; }
        public virtual DbSet<Tcollection> Tcollection { get; set; }
        public virtual DbSet<Tcontact> Tcontact { get; set; }
        public virtual DbSet<Tlibrary> Tlibrary { get; set; }
        public virtual DbSet<TlibraryCategorie> TlibraryCategorie { get; set; }
        public virtual DbSet<TlibrarySubCategorie> TlibrarySubCategorie { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlite("Data Source=Sql/LibraryDB.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tbook>(entity =>
            {
                entity.ToTable("TBook");

                entity.HasIndex(e => e.Guid)
                    .IsUnique();

                entity.HasIndex(e => e.Id)
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.DateAjout).IsRequired();

                entity.Property(e => e.Guid).IsRequired();

                entity.Property(e => e.MainTitle).IsRequired();

                entity.HasOne(d => d.IdCategorieNavigation)
                    .WithMany(p => p.Tbook)
                    .HasForeignKey(d => d.IdCategorie)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.IdLibraryNavigation)
                    .WithMany(p => p.Tbook)
                    .HasForeignKey(d => d.IdLibrary);

                entity.HasOne(d => d.IdSubCategorieNavigation)
                    .WithMany(p => p.Tbook)
                    .HasForeignKey(d => d.IdSubCategorie)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TbookAuthorConnector>(entity =>
            {
                entity.ToTable("TBookAuthorConnector");

                entity.HasIndex(e => e.Id)
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.IdAuthorNavigation)
                    .WithMany(p => p.TbookAuthorConnector)
                    .HasForeignKey(d => d.IdAuthor);

                entity.HasOne(d => d.IdBookNavigation)
                    .WithMany(p => p.TbookAuthorConnector)
                    .HasForeignKey(d => d.IdBook);
            });

            modelBuilder.Entity<TbookClassification>(entity =>
            {
                entity.ToTable("TBookClassification");

                entity.HasIndex(e => e.Id)
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.AtelAge).HasColumnName("ATelAge");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.TbookClassification)
                    .HasForeignKey<TbookClassification>(d => d.Id);
            });

            modelBuilder.Entity<TbookCollectionConnector>(entity =>
            {
                entity.ToTable("TBookCollectionConnector");

                entity.HasIndex(e => e.Id)
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.IdBookNavigation)
                    .WithMany(p => p.TbookCollectionConnector)
                    .HasForeignKey(d => d.IdBook);

                entity.HasOne(d => d.IdCollectionNavigation)
                    .WithMany(p => p.TbookCollectionConnector)
                    .HasForeignKey(d => d.IdCollection);

                entity.HasOne(d => d.IdLibraryNavigation)
                    .WithMany(p => p.TbookCollectionConnector)
                    .HasForeignKey(d => d.IdLibrary);
            });

            modelBuilder.Entity<TbookEditeurConnector>(entity =>
            {
                entity.ToTable("TBookEditeurConnector");

                entity.HasIndex(e => e.Id)
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.IdBookNavigation)
                    .WithMany(p => p.TbookEditeurConnector)
                    .HasForeignKey(d => d.IdBook);

                entity.HasOne(d => d.IdEditeurNavigation)
                    .WithMany(p => p.TbookEditeurConnector)
                    .HasForeignKey(d => d.IdEditeur);
            });

            modelBuilder.Entity<TbookEtat>(entity =>
            {
                entity.ToTable("TBookEtat");

                entity.HasIndex(e => e.Id)
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.DateAjout).IsRequired();

                entity.Property(e => e.Etat).IsRequired();

                entity.HasOne(d => d.IdBookExemplaryNavigation)
                    .WithMany(p => p.TbookEtat)
                    .HasForeignKey(d => d.IdBookExemplary);
            });

            modelBuilder.Entity<TbookExemplary>(entity =>
            {
                entity.ToTable("TBookExemplary");

                entity.HasIndex(e => e.Id)
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.DateAjout).IsRequired();

                entity.Property(e => e.IsVisible).HasDefaultValueSql("1");

                entity.Property(e => e.TypeAcquisition).IsRequired();

                entity.HasOne(d => d.IdBookNavigation)
                    .WithMany(p => p.TbookExemplary)
                    .HasForeignKey(d => d.IdBook);

                entity.HasOne(d => d.IdContactSourceNavigation)
                    .WithMany(p => p.TbookExemplary)
                    .HasForeignKey(d => d.IdContactSource)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TbookFormat>(entity =>
            {
                entity.ToTable("TBookFormat");

                entity.HasIndex(e => e.Id)
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.TbookFormat)
                    .HasForeignKey<TbookFormat>(d => d.Id);
            });

            modelBuilder.Entity<TbookIdentification>(entity =>
            {
                entity.ToTable("TBookIdentification");

                entity.HasIndex(e => e.Id)
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Asin).HasColumnName("ASIN");

                entity.Property(e => e.Isbn).HasColumnName("ISBN");

                entity.Property(e => e.Isbn10).HasColumnName("ISBN10");

                entity.Property(e => e.Isbn13).HasColumnName("ISBN13");

                entity.Property(e => e.Issn).HasColumnName("ISSN");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.TbookIdentification)
                    .HasForeignKey<TbookIdentification>(d => d.Id);
            });

            modelBuilder.Entity<TbookOtherTitle>(entity =>
            {
                entity.ToTable("TBookOtherTitle");

                entity.HasIndex(e => e.Id)
                    .IsUnique();

                entity.HasIndex(e => e.Title)
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Title).IsRequired();

                entity.HasOne(d => d.IdBookNavigation)
                    .WithMany(p => p.TbookOtherTitle)
                    .HasForeignKey(d => d.IdBook);
            });

            modelBuilder.Entity<TbookPret>(entity =>
            {
                entity.ToTable("TBookPret");

                entity.HasIndex(e => e.Id)
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.DatePret).IsRequired();

                entity.HasOne(d => d.IdBookExemplaryNavigation)
                    .WithMany(p => p.TbookPret)
                    .HasForeignKey(d => d.IdBookExemplary);

                entity.HasOne(d => d.IdContactNavigation)
                    .WithMany(p => p.TbookPret)
                    .HasForeignKey(d => d.IdContact);

                entity.HasOne(d => d.IdEtatAfterNavigation)
                    .WithMany(p => p.TbookPretIdEtatAfterNavigation)
                    .HasForeignKey(d => d.IdEtatAfter)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.IdEtatBeforeNavigation)
                    .WithMany(p => p.TbookPretIdEtatBeforeNavigation)
                    .HasForeignKey(d => d.IdEtatBefore);
            });

            modelBuilder.Entity<Tcollection>(entity =>
            {
                entity.ToTable("TCollection");

                entity.HasIndex(e => e.Id)
                    .IsUnique();

                entity.HasIndex(e => e.Name)
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).IsRequired();

                entity.HasOne(d => d.IdLibraryNavigation)
                    .WithMany(p => p.Tcollection)
                    .HasForeignKey(d => d.IdLibrary);
            });

            modelBuilder.Entity<Tcontact>(entity =>
            {
                entity.ToTable("TContact");

                entity.HasIndex(e => e.Guid)
                    .IsUnique();

                entity.HasIndex(e => e.Id)
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.DateAjout).IsRequired();

                entity.Property(e => e.Guid).IsRequired();
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
