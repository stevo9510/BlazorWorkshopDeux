using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace BandBookerData.Models
{
    public partial class BandBookerContext : DbContext
    {
        public BandBookerContext()
        {
        }

        public BandBookerContext(DbContextOptions<BandBookerContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Band> Band { get; set; }
        public virtual DbSet<BandMusician> BandMusician { get; set; }
        public virtual DbSet<Booking> Booking { get; set; }
        public virtual DbSet<BookingInstrument> BookingInstrument { get; set; }
        public virtual DbSet<BookingMusicianInstrument> BookingMusicianInstrument { get; set; }
        public virtual DbSet<Instrument> Instrument { get; set; }
        public virtual DbSet<Musician> Musician { get; set; }
        public virtual DbSet<MusicianInstrument> MusicianInstrument { get; set; }
        public virtual DbSet<Venue> Venue { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var builder = new ConfigurationBuilder()
                    .AddJsonFile("data_appsettings.json", 
                    optional: true, reloadOnChange: true);

                var Configuration = builder.Build();
                string connstr = Configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connstr);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BandMusician>(entity =>
            {
                entity.HasOne(d => d.Band)
                    .WithMany(p => p.BandMusician)
                    .HasForeignKey(d => d.BandId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BandMusician_Band");

                entity.HasOne(d => d.Musician)
                    .WithMany(p => p.BandMusician)
                    .HasForeignKey(d => d.MusicianId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BandMusician_ToMusician");
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasOne(d => d.Band)
                    .WithMany(p => p.Booking)
                    .HasForeignKey(d => d.BandId)
                    .HasConstraintName("FK_Booking_ToBand");

                entity.HasOne(d => d.Venue)
                    .WithMany(p => p.Booking)
                    .HasForeignKey(d => d.VenueId)
                    .HasConstraintName("FK_Booking_ToVenue");
            });

            modelBuilder.Entity<BookingInstrument>(entity =>
            {
                entity.HasOne(d => d.Booking)
                    .WithMany(p => p.BookingInstrument)
                    .HasForeignKey(d => d.BookingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BookingInstrument_Booking");

                entity.HasOne(d => d.Instrument)
                    .WithMany(p => p.BookingInstrument)
                    .HasForeignKey(d => d.InstrumentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BookingInstrument_Instrument");
            });

            modelBuilder.Entity<BookingMusicianInstrument>(entity =>
            {
                entity.Property(e => e.ResponseDate).HasColumnType("datetime");

                entity.HasOne(d => d.Booking)
                    .WithMany(p => p.BookingMusicianInstrument)
                    .HasForeignKey(d => d.BookingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BookingMusicianInstrument_ToBooking");

                entity.HasOne(d => d.Instrument)
                    .WithMany(p => p.BookingMusicianInstrument)
                    .HasForeignKey(d => d.InstrumentId)
                    .HasConstraintName("FK_BookingMusicianInstrument_ToInstrument");

                entity.HasOne(d => d.Musician)
                    .WithMany(p => p.BookingMusicianInstrument)
                    .HasForeignKey(d => d.MusicianId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BookingMusicianInstrument_ToMusician");
            });

            modelBuilder.Entity<Instrument>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Musician>(entity =>
            {
                entity.Property(e => e.Bio).HasMaxLength(4000);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.MobilePhone).HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.PhotoUrl).HasMaxLength(255);
            });

            modelBuilder.Entity<MusicianInstrument>(entity =>
            {
                entity.HasOne(d => d.Instrument)
                    .WithMany(p => p.MusicianInstrument)
                    .HasForeignKey(d => d.InstrumentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MusicianInstrument_Instrument");

                entity.HasOne(d => d.Musician)
                    .WithMany(p => p.MusicianInstrument)
                    .HasForeignKey(d => d.MusicianId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MusicianInstrument_ToMusician");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
