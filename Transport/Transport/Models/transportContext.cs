using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Transport.ViewModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Transport.Models
{
    public partial class transportContext : IdentityDbContext<ApplicationUser>
    {
        public virtual DbSet<Korisnik> Korisnik { get; set; }
        public virtual DbSet<Lokacija> Lokacija { get; set; }
        public virtual DbSet<Mjesto> Mjesto { get; set; }
        public virtual DbSet<Naplata> Naplata { get; set; }
        public virtual DbSet<Narucitelj> Narucitelj { get; set; }
        public virtual DbSet<PonudaPrijevoza> PonudaPrijevoza { get; set; }
        public virtual DbSet<Prijevoz> Prijevoz { get; set; }
        public virtual DbSet<Prijevoznik> Prijevoznik { get; set; }
        public virtual DbSet<RazlogNaplate> RazlogNaplate { get; set; }
        public virtual DbSet<StatusPonude> StatusPonude { get; set; }
        public virtual DbSet<StatusZahtjeva> StatusZahtjeva { get; set; }
        public virtual DbSet<Vozilo> Vozilo { get; set; }
        public virtual DbSet<VrstaLokacije> VrstaLokacije { get; set; }
        public virtual DbSet<VrstaNaplate> VrstaNaplate { get; set; }
        public virtual DbSet<Zahtjev> Zahtjev { get; set; }
        public virtual DbSet<ZahtjevLokacija> ZahtjevLokacija { get; set; }

        public transportContext(DbContextOptions<transportContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Korisnik>(entity =>
            {
               
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnType("char(100)");

                entity.Property(e => e.Ime)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Lozinka)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.Oib)
                    .IsRequired()
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.Prezime)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.TelBroj)
                    .IsRequired()
                    .HasColumnType("text");
            });

            modelBuilder.Entity<Lokacija>(entity =>
            {
                entity.Property(e => e.Ulica)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.HasOne(d => d.IdMjestoNavigation)
                    .WithMany(p => p.Lokacija)
                    .HasForeignKey(d => d.IdMjesto)
                    .HasConstraintName("Lokacija_Mjesto_FK");

                entity.HasOne(d => d.IdVrstaLokacijeNavigation)
                    .WithMany(p => p.Lokacija)
                    .HasForeignKey(d => d.IdVrstaLokacije)
                    .HasConstraintName("Lokacija_VrstaLokacije_FK");
            });

            modelBuilder.Entity<Mjesto>(entity =>
            {
                entity.HasIndex(e => e.PostanskiBroj)
                    .HasName("UK_Mjesto")
                    .IsUnique();

                entity.Property(e => e.Naziv)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Naplata>(entity =>
            {
                entity.Property(e => e.RokIzvrsenjaNaplate).HasColumnType("datetime");

                entity.HasOne(d => d.IdPrijevozNavigation)
                    .WithMany(p => p.Naplata)
                    .HasForeignKey(d => d.IdPrijevoz)
                    .HasConstraintName("Naplata_Prijevoz_FK");

                entity.HasOne(d => d.IdRazlogNavigation)
                    .WithMany(p => p.Naplata)
                    .HasForeignKey(d => d.IdRazlog)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Naplata_RazlogNaplate_FK");

                entity.HasOne(d => d.IdVrstaNaplateNavigation)
                    .WithMany(p => p.Naplata)
                    .HasForeignKey(d => d.IdVrstaNaplate)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Naplata_VrstaNaplate_FK");
            });

            modelBuilder.Entity<Narucitelj>(entity =>
            {
                entity.HasKey(e => e.IdKorisnik);

                entity.Property(e => e.IdKorisnik).ValueGeneratedNever();

                entity.Property(e => e.ZahtijevaEko)
                    .HasColumnName("zahtijevaEko")
                    .HasColumnType("text");

                entity.HasOne(d => d.IdKorisnikNavigation)
                    .WithOne(p => p.Narucitelj)
                    .HasForeignKey<Narucitelj>(d => d.IdKorisnik)
                    .HasConstraintName("Narucitelj_Korisnik_FK");
            });

            modelBuilder.Entity<PonudaPrijevoza>(entity =>
            {
                entity.Property(e => e.RokIstekaPonude).HasColumnType("datetime");

                entity.Property(e => e.RokOtkazaPonude).HasColumnType("datetime");

                entity.HasOne(d => d.IdPrijevoznikNavigation)
                    .WithMany(p => p.PonudaPrijevoza)
                    .HasForeignKey(d => d.IdPrijevoznik)
                    .HasConstraintName("PonudaPrijevoza_Prijevoznik_FK");

                entity.HasOne(d => d.IdStatusPonudeNavigation)
                    .WithMany(p => p.PonudaPrijevoza)
                    .HasForeignKey(d => d.IdStatusPonude)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PonudaPrijevoza_StatusPonude_FK");

                entity.HasOne(d => d.IdZahtjevNavigation)
                    .WithMany(p => p.PonudaPrijevoza)
                    .HasForeignKey(d => d.IdZahtjev)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PonudaPrijevoza_Zahtjev");
            });

            modelBuilder.Entity<Prijevoz>(entity =>
            {
                entity.Property(e => e.OpisUslugeNarucitelja).HasMaxLength(450);

                entity.Property(e => e.OpisUslugePrijevoznika).HasMaxLength(450);

                entity.HasOne(d => d.IdPonudaPrijevozaNavigation)
                    .WithMany(p => p.Prijevoz)
                    .HasForeignKey(d => d.IdPonudaPrijevoza)
                    .HasConstraintName("Prijevoz_PonudaPrijevoza_FK");
            });

            modelBuilder.Entity<Prijevoznik>(entity =>
            {
                entity.HasKey(e => e.IdKorisnik);

                entity.Property(e => e.IdKorisnik).ValueGeneratedNever();

                entity.Property(e => e.NazivTvrtke)
                    .HasColumnName("nazivTvrtke")
                    .HasMaxLength(150);

                entity.HasOne(d => d.IdKorisnikNavigation)
                    .WithOne(p => p.Prijevoznik)
                    .HasForeignKey<Prijevoznik>(d => d.IdKorisnik)
                    .HasConstraintName("Prijevoznik_Korisnik_FK");
            });

            modelBuilder.Entity<RazlogNaplate>(entity =>
            {
                entity.Property(e => e.Razlog)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<StatusPonude>(entity =>
            {
                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<StatusZahtjeva>(entity =>
            {
                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<Vozilo>(entity =>
            {
                entity.HasIndex(e => e.RegistarskaOznaka)
                    .HasName("UK_Vozilo")
                    .IsUnique();

                entity.Property(e => e.Boja).HasMaxLength(50);

                entity.Property(e => e.Marka)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.RegistarskaOznaka)
                    .IsRequired()
                    .HasColumnType("char(10)");

                entity.Property(e => e.Tip).HasMaxLength(50);

                entity.HasOne(d => d.IdPrijevoznikNavigation)
                    .WithMany(p => p.Vozilo)
                    .HasForeignKey(d => d.IdPrijevoznik)
                    .HasConstraintName("Vozilo_Prijevoznik_FK");
            });

            modelBuilder.Entity<VrstaLokacije>(entity =>
            {
                entity.Property(e => e.Vrsta)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<VrstaNaplate>(entity =>
            {
                entity.Property(e => e.Vrsta)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Zahtjev>(entity =>
            {
                entity.Property(e => e.Opis)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.VrijemePocetka).HasColumnType("datetime");

                entity.Property(e => e.VrijemeZavrsetka).HasColumnType("datetime");

                entity.HasOne(d => d.IdNaruciteljNavigation)
                    .WithMany(p => p.Zahtjev)
                    .HasForeignKey(d => d.IdNarucitelj)
                    .HasConstraintName("Zahtjev_Narucitelj_FK");

                entity.HasOne(d => d.IdStatusZahtjevaNavigation)
                    .WithMany(p => p.Zahtjev)
                    .HasForeignKey(d => d.IdStatusZahtjeva)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Zahtjev_StatusZahtjeva_FK");
            });

            modelBuilder.Entity<ZahtjevLokacija>(entity =>
            {
                entity.HasOne(d => d.IdLokacijaNavigation)
                    .WithMany(p => p.ZahtjevLokacija)
                    .HasForeignKey(d => d.IdLokacija)
                    .HasConstraintName("ZahtjevLokacija_Lokacija_FK");

                entity.HasOne(d => d.IdZahtjevNavigation)
                    .WithMany(p => p.ZahtjevLokacija)
                    .HasForeignKey(d => d.IdZahtjev)
                    .HasConstraintName("ZahtjevLokacija_Zahtjev_FK");
            });
        }

        public DbSet<Transport.ViewModels.ProsireniZahtjevViewModel> ProsireniZahtjev { get; set; }

        public DbSet<Transport.ViewModels.KorisnikovProfilViewModel> KorisnikovProfilViewModel { get; set; }

        public DbSet<Transport.ViewModels.ProsireniPrijevozViewModel> ProsireniPrijevoz { get; set; }

    }
}
