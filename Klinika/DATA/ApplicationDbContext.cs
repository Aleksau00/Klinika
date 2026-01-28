using Klinika.Models;
using Microsoft.EntityFrameworkCore;

namespace Klinika.DATA
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Worker> Workers { get; set; }
        public DbSet<Guardian> Guardian { get; set; }
        public DbSet<Administrator> Administrators { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Secretary> Secretaries { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Guardian> Guardians { get; set; }
        public DbSet<AppointmentSlot> AppointmentSlots { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Clinic> Clinics { get; set; }
        // ❌ NO ClinicWorker DbSet

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>().ToTable("Persons");
            modelBuilder.Entity<Worker>().ToTable("Workers");
            modelBuilder.Entity<Administrator>().ToTable("Administrators");
            modelBuilder.Entity<Doctor>().ToTable("Doctors");
            modelBuilder.Entity<Secretary>().ToTable("Secretaries");
            modelBuilder.Entity<Guardian>().ToTable("Guardians");
            modelBuilder.Entity<Patient>().ToTable("Patients");
            modelBuilder.Entity<Address>().ToTable("Addresses");
            modelBuilder.Entity<City>().ToTable("Cities");
            modelBuilder.Entity<Clinic>().ToTable("Clinics");

            // Configure Person
            modelBuilder.Entity<Person>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.JMBG).HasMaxLength(20);

                entity.HasOne(e => e.Address)
                      .WithMany()
                      .HasForeignKey(e => e.AddressId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure Address
            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.StreetName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.StreetNumber).IsRequired().HasMaxLength(20);
                entity.Property(e => e.ApartmentNumber).HasMaxLength(20);
                entity.Property(e => e.AdditionalInfo).HasMaxLength(200);

                entity.HasOne(e => e.City)
                      .WithMany(c => c.Addresses)
                      .HasForeignKey(e => e.CityId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure City
            modelBuilder.Entity<City>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PostalCode).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Country).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => new { e.Name, e.Country }).IsUnique();
            });

            // Configure AppointmentSlot
            modelBuilder.Entity<AppointmentSlot>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Doctor)
                      .WithMany()
                      .HasForeignKey(e => e.DoctorId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.DoctorId, e.Date, e.StartTime }).IsUnique();
            });

            // Configure Clinic
            modelBuilder.Entity<Clinic>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Email).IsUnique();

                entity.HasOne(e => e.Address)
                      .WithMany()
                      .HasForeignKey(e => e.AddressId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Worker -> Clinic relationship (One-to-Many)
            modelBuilder.Entity<Worker>(entity =>
            {
                entity.HasOne(e => e.Clinic)
                      .WithMany(c => c.Workers)
                      .HasForeignKey(e => e.ClinicId)
                      .OnDelete(DeleteBehavior.SetNull); // If clinic deleted, worker remains but ClinicId = null
            });

            // Seed Cities (add all 100 from before)
            modelBuilder.Entity<City>().HasData(
                new City { Id = 1, Name = "Beograd", PostalCode = "11000", Country = "Serbia" },
                new City { Id = 2, Name = "Novi Sad", PostalCode = "21000", Country = "Serbia" },
                new City { Id = 3, Name = "Niš", PostalCode = "18000", Country = "Serbia" },
                new City { Id = 4, Name = "Kragujevac", PostalCode = "34000", Country = "Serbia" },
                new City { Id = 5, Name = "Subotica", PostalCode = "24000", Country = "Serbia" },
                new City { Id = 6, Name = "Zrenjanin", PostalCode = "23000", Country = "Serbia" },
                new City { Id = 7, Name = "Pančevo", PostalCode = "26000", Country = "Serbia" },
                new City { Id = 8, Name = "Čačak", PostalCode = "32000", Country = "Serbia" },
                new City { Id = 9, Name = "Kruševac", PostalCode = "37000", Country = "Serbia" },
                new City { Id = 10, Name = "Kraljevo", PostalCode = "36000", Country = "Serbia" },
                new City { Id = 11, Name = "Smederevo", PostalCode = "11300", Country = "Serbia" },
                new City { Id = 12, Name = "Leskovac", PostalCode = "16000", Country = "Serbia" },
                new City { Id = 13, Name = "Užice", PostalCode = "31000", Country = "Serbia" },
                new City { Id = 14, Name = "Vranje", PostalCode = "17500", Country = "Serbia" },
                new City { Id = 15, Name = "Šabac", PostalCode = "15000", Country = "Serbia" },
                new City { Id = 16, Name = "Valjevo", PostalCode = "14000", Country = "Serbia" },
                new City { Id = 17, Name = "Sombor", PostalCode = "25000", Country = "Serbia" },
                new City { Id = 18, Name = "Požarevac", PostalCode = "12000", Country = "Serbia" },
                new City { Id = 19, Name = "Pirot", PostalCode = "18300", Country = "Serbia" },
                new City { Id = 20, Name = "Zaječar", PostalCode = "19000", Country = "Serbia" },
                new City { Id = 21, Name = "Kikinda", PostalCode = "23300", Country = "Serbia" },
                new City { Id = 22, Name = "Sremska Mitrovica", PostalCode = "22000", Country = "Serbia" },
                new City { Id = 23, Name = "Jagodina", PostalCode = "35000", Country = "Serbia" },
                new City { Id = 24, Name = "Vršac", PostalCode = "26300", Country = "Serbia" },
                new City { Id = 25, Name = "Bor", PostalCode = "19210", Country = "Serbia" },
                new City { Id = 26, Name = "Prokuplje", PostalCode = "18400", Country = "Serbia" },
                new City { Id = 27, Name = "Loznica", PostalCode = "15300", Country = "Serbia" },
                new City { Id = 28, Name = "Bečej", PostalCode = "21220", Country = "Serbia" },
                new City { Id = 29, Name = "Aranđelovac", PostalCode = "34300", Country = "Serbia" },
                new City { Id = 30, Name = "Novi Pazar", PostalCode = "36300", Country = "Serbia" },
                new City { Id = 31, Name = "Negotin", PostalCode = "19300", Country = "Serbia" },
                new City { Id = 32, Name = "Paraćin", PostalCode = "35250", Country = "Serbia" },
                new City { Id = 33, Name = "Inđija", PostalCode = "22320", Country = "Serbia" },
                new City { Id = 34, Name = "Lazarevac", PostalCode = "11550", Country = "Serbia" },
                new City { Id = 35, Name = "Ćuprija", PostalCode = "35230", Country = "Serbia" },
                new City { Id = 36, Name = "Mladenovac", PostalCode = "11400", Country = "Serbia" },
                new City { Id = 37, Name = "Ruma", PostalCode = "22400", Country = "Serbia" },
                new City { Id = 38, Name = "Stara Pazova", PostalCode = "22300", Country = "Serbia" },
                new City { Id = 39, Name = "Kovačica", PostalCode = "26210", Country = "Serbia" },
                new City { Id = 40, Name = "Kula", PostalCode = "25230", Country = "Serbia" },
                new City { Id = 41, Name = "Gornji Milanovac", PostalCode = "32300", Country = "Serbia" },
                new City { Id = 42, Name = "Temerin", PostalCode = "21235", Country = "Serbia" },
                new City { Id = 43, Name = "Čoka", PostalCode = "23320", Country = "Serbia" },
                new City { Id = 44, Name = "Aleksinac", PostalCode = "18220", Country = "Serbia" },
                new City { Id = 45, Name = "Novi Bečej", PostalCode = "23272", Country = "Serbia" },
                new City { Id = 46, Name = "Bačka Palanka", PostalCode = "21400", Country = "Serbia" },
                new City { Id = 47, Name = "Svilajnac", PostalCode = "35210", Country = "Serbia" },
                new City { Id = 48, Name = "Petrovac na Mlavi", PostalCode = "12300", Country = "Serbia" },
                new City { Id = 49, Name = "Ada", PostalCode = "24430", Country = "Serbia" },
                new City { Id = 50, Name = "Šid", PostalCode = "22240", Country = "Serbia" },
                new City { Id = 51, Name = "Priboj", PostalCode = "31330", Country = "Serbia" },
                new City { Id = 52, Name = "Srbobran", PostalCode = "21480", Country = "Serbia" },
                new City { Id = 53, Name = "Vlasotince", PostalCode = "16210", Country = "Serbia" },
                new City { Id = 54, Name = "Kučevo", PostalCode = "12240", Country = "Serbia" },
                new City { Id = 55, Name = "Velika Plana", PostalCode = "11320", Country = "Serbia" },
                new City { Id = 56, Name = "Knjaževac", PostalCode = "19350", Country = "Serbia" },
                new City { Id = 57, Name = "Apatin", PostalCode = "25260", Country = "Serbia" },
                new City { Id = 58, Name = "Arilje", PostalCode = "31230", Country = "Serbia" },
                new City { Id = 59, Name = "Žabalj", PostalCode = "21230", Country = "Serbia" },
                new City { Id = 60, Name = "Nova Varoš", PostalCode = "31320", Country = "Serbia" },
                new City { Id = 61, Name = "Senta", PostalCode = "24400", Country = "Serbia" },
                new City { Id = 62, Name = "Kovin", PostalCode = "26220", Country = "Serbia" },
                new City { Id = 63, Name = "Surdulica", PostalCode = "17530", Country = "Serbia" },
                new City { Id = 64, Name = "Žagubica", PostalCode = "12330", Country = "Serbia" },
                new City { Id = 65, Name = "Kladovo", PostalCode = "19320", Country = "Serbia" },
                new City { Id = 66, Name = "Despotovac", PostalCode = "35213", Country = "Serbia" },
                new City { Id = 67, Name = "Beočin", PostalCode = "21300", Country = "Serbia" },
                new City { Id = 68, Name = "Bački Petrovac", PostalCode = "21470", Country = "Serbia" },
                new City { Id = 69, Name = "Rekovac", PostalCode = "35243", Country = "Serbia" },
                new City { Id = 70, Name = "Titel", PostalCode = "21240", Country = "Serbia" },
                new City { Id = 71, Name = "Golubac", PostalCode = "12223", Country = "Serbia" },
                new City { Id = 72, Name = "Barajevo", PostalCode = "11460", Country = "Serbia" },
                new City { Id = 73, Name = "Babušnica", PostalCode = "18330", Country = "Serbia" },
                new City { Id = 74, Name = "Dimitrovgrad", PostalCode = "18320", Country = "Serbia" },
                new City { Id = 75, Name = "Bela Palanka", PostalCode = "18310", Country = "Serbia" },
                new City { Id = 76, Name = "Boljevac", PostalCode = "19370", Country = "Serbia" },
                new City { Id = 77, Name = "Majdanpek", PostalCode = "19250", Country = "Serbia" },
                new City { Id = 78, Name = "Novi Kneževac", PostalCode = "23330", Country = "Serbia" },
                new City { Id = 79, Name = "Sopot", PostalCode = "11450", Country = "Serbia" },
                new City { Id = 80, Name = "Veliko Gradište", PostalCode = "12220", Country = "Serbia" },
                new City { Id = 81, Name = "Žitište", PostalCode = "23210", Country = "Serbia" },
                new City { Id = 82, Name = "Ub", PostalCode = "14210", Country = "Serbia" },
                new City { Id = 83, Name = "Bajina Bašta", PostalCode = "31250", Country = "Serbia" },
                new City { Id = 84, Name = "Kosjerić", PostalCode = "31260", Country = "Serbia" },
                new City { Id = 85, Name = "Ivanjica", PostalCode = "32310", Country = "Serbia" },
                new City { Id = 86, Name = "Tutin", PostalCode = "36320", Country = "Serbia" }, // REPLACED duplicate Negotin
                new City { Id = 87, Name = "Ljubovija", PostalCode = "15320", Country = "Serbia" },
                new City { Id = 88, Name = "Mali Zvornik", PostalCode = "15318", Country = "Serbia" },
                new City { Id = 89, Name = "Crna Trava", PostalCode = "17527", Country = "Serbia" },
                new City { Id = 90, Name = "Gadžin Han", PostalCode = "18205", Country = "Serbia" },
                new City { Id = 91, Name = "Bosilegrad", PostalCode = "17540", Country = "Serbia" },
                new City { Id = 92, Name = "Merošina", PostalCode = "18233", Country = "Serbia" },
                new City { Id = 93, Name = "Žabari", PostalCode = "12260", Country = "Serbia" },
                new City { Id = 94, Name = "Ražanj", PostalCode = "18230", Country = "Serbia" },
                new City { Id = 95, Name = "Malo Crniće", PostalCode = "12306", Country = "Serbia" },
                new City { Id = 96, Name = "Ljig", PostalCode = "14240", Country = "Serbia" },
                new City { Id = 97, Name = "Medveđa", PostalCode = "17516", Country = "Serbia" },
                new City { Id = 98, Name = "Osečina", PostalCode = "14253", Country = "Serbia" },
                new City { Id = 99, Name = "Preševo", PostalCode = "17523", Country = "Serbia" },
                new City { Id = 100, Name = "Bujanovac", PostalCode = "17520", Country = "Serbia" }
            );

            // Seed Addresses
            modelBuilder.Entity<Address>().HasData(
                // Clinic Addresses
                new Address { Id = 1, StreetName = "Kneza Miloša", StreetNumber = "15", CityId = 1 },
                new Address { Id = 2, StreetName = "Bulevar Oslobođenja", StreetNumber = "5", CityId = 2 },
                new Address { Id = 3, StreetName = "Njegoševa", StreetNumber = "10", CityId = 3 },
                new Address { Id = 4, StreetName = "Kralja Aleksandra", StreetNumber = "88", CityId = 4 },
                new Address { Id = 5, StreetName = "Korzo", StreetNumber = "3", CityId = 5 },

                // Administrator Addresses
                new Address { Id = 6, StreetName = "Terazije", StreetNumber = "25", CityId = 1 },
                new Address { Id = 7, StreetName = "Dunavska", StreetNumber = "12", CityId = 2 },
                new Address { Id = 8, StreetName = "Vojvode Tankosića", StreetNumber = "8", CityId = 3 },

                // Doctor Addresses - Beograd
                new Address { Id = 9, StreetName = "Makedonska", StreetNumber = "30", CityId = 1 },
                new Address { Id = 10, StreetName = "Svetozara Markovića", StreetNumber = "12", CityId = 1 },
                new Address { Id = 11, StreetName = "Kralja Milana", StreetNumber = "45", CityId = 1 },
                new Address { Id = 12, StreetName = "Resavska", StreetNumber = "22", CityId = 1 },
                new Address { Id = 13, StreetName = "Nemanjina", StreetNumber = "18", CityId = 1 },

                // Doctor Addresses - Novi Sad
                new Address { Id = 14, StreetName = "Narodnog Fronta", StreetNumber = "8", CityId = 2 },
                new Address { Id = 15, StreetName = "Bulevar Evrope", StreetNumber = "20", CityId = 2 },
                new Address { Id = 16, StreetName = "Jevrejska", StreetNumber = "22", CityId = 2 },
                new Address { Id = 17, StreetName = "Modene", StreetNumber = "5", CityId = 2 },

                // Doctor Addresses - Niš
                new Address { Id = 18, StreetName = "Obrenovićeva", StreetNumber = "18", CityId = 3 },
                new Address { Id = 19, StreetName = "Cara Dušana", StreetNumber = "15", CityId = 3 },
                new Address { Id = 20, StreetName = "Generala Milojka Lešjanina", StreetNumber = "7", CityId = 3 },

                // Doctor Addresses - Kragujevac
                new Address { Id = 21, StreetName = "Svetozara Markovića", StreetNumber = "33", CityId = 4 },
                new Address { Id = 22, StreetName = "Đure Daničića", StreetNumber = "11", CityId = 4 },

                // Doctor Addresses - Subotica
                new Address { Id = 23, StreetName = "Matije Korvina", StreetNumber = "6", CityId = 5 },
                new Address { Id = 24, StreetName = "Segedinski put", StreetNumber = "44", CityId = 5 },

                // Secretary Addresses
                new Address { Id = 25, StreetName = "Bulevar Kralja Aleksandra", StreetNumber = "77", CityId = 1 },
                new Address { Id = 26, StreetName = "Ruzveltova", StreetNumber = "14", CityId = 1 },
                new Address { Id = 27, StreetName = "Janka Čmelika", StreetNumber = "9", CityId = 2 },
                new Address { Id = 28, StreetName = "Laze Nančića", StreetNumber = "16", CityId = 2 },
                new Address { Id = 29, StreetName = "Vožda Karađorđa", StreetNumber = "23", CityId = 3 },
                new Address { Id = 30, StreetName = "Radničke brigade", StreetNumber = "31", CityId = 4 },
                new Address { Id = 31, StreetName = "Đure Cvejića", StreetNumber = "8", CityId = 5 }
            );

            // Seed Clinics
            modelBuilder.Entity<Clinic>().HasData(
                new Clinic
                {
                    Id = 1,
                    Name = "Klinika Beograd Centar",
                    AddressId = 1,
                    PhoneNumber = "011-123-4567",
                    Email = "info@klinika-beograd.rs",
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 15)
                },
                new Clinic
                {
                    Id = 2,
                    Name = "Medicinski Centar Novi Sad",
                    AddressId = 2,
                    PhoneNumber = "021-987-6543",
                    Email = "kontakt@mc-novisad.rs",
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 2, 1)
                },
                new Clinic
                {
                    Id = 3,
                    Name = "Poliklinika Niš",
                    AddressId = 3,
                    PhoneNumber = "018-456-7890",
                    Email = "info@poliklinika-nis.rs",
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 2, 10)
                },
                new Clinic
                {
                    Id = 4,
                    Name = "Dom Zdravlja Kragujevac",
                    AddressId = 4,
                    PhoneNumber = "034-555-1234",
                    Email = "info@dz-kragujevac.rs",
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 3, 1)
                },
                new Clinic
                {
                    Id = 5,
                    Name = "Klinika Subotica",
                    AddressId = 5,
                    PhoneNumber = "024-333-2222",
                    Email = "info@klinika-subotica.rs",
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 3, 15)
                }
            );

            // Seed Administrators
            modelBuilder.Entity<Administrator>().HasData(
                new Administrator
                {
                    Id = 1,
                    Email = "admin@klinika.rs",
                    FirstName = "Marko",
                    LastName = "Administratorović",
                    PhoneNumber = "060-111-0001",
                    JMBG = "0101990800001",
                    Gender = "M",
                    DateOfBirth = new DateTime(1990, 1, 1),
                    AddressId = 6,
                    ClinicId = 1,
                    PasswordHash = "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe", // Admin123
                    SeniorityLevel = "Senior",
                    CreatedAt = new DateTime(2024, 1, 15),
                    IsActive = true
                },
                new Administrator
                {
                    Id = 2,
                    Email = "marija.petrovic@klinika.rs",
                    FirstName = "Marija",
                    LastName = "Petrović",
                    PhoneNumber = "060-111-0002",
                    JMBG = "1503988700002",
                    Gender = "F",
                    DateOfBirth = new DateTime(1987, 3, 15),
                    AddressId = 7,
                    ClinicId = 2,
                    PasswordHash = "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe",
                    SeniorityLevel = "Mid",
                    CreatedAt = new DateTime(2024, 2, 1),
                    IsActive = true
                },
                new Administrator
                {
                    Id = 3,
                    Email = "jovana.stojanovic@klinika.rs",
                    FirstName = "Jovana",
                    LastName = "Stojanović",
                    PhoneNumber = "060-111-0003",
                    JMBG = "2206992800003",
                    Gender = "F",
                    DateOfBirth = new DateTime(1992, 6, 22),
                    AddressId = 8,
                    ClinicId = 3,
                    PasswordHash = "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe",
                    SeniorityLevel = "Junior",
                    CreatedAt = new DateTime(2024, 2, 10),
                    IsActive = true
                }
            );

            // Seed Doctors
            modelBuilder.Entity<Doctor>().HasData(
                // Beograd Doctors
                new Doctor
                {
                    Id = 4,
                    Email = "dr.nikolic@klinika.rs",
                    FirstName = "Nikola",
                    LastName = "Nikolić",
                    PhoneNumber = "060-222-0001",
                    JMBG = "1205985800004",
                    Gender = "M",
                    DateOfBirth = new DateTime(1985, 5, 12),
                    AddressId = 9,
                    ClinicId = 1,
                    PasswordHash = "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe",
                    Specialty = "Cardiology",
                    LicenseNumber = "LIC-2010-001",
                    CreatedAt = new DateTime(2024, 1, 20),
                    IsActive = true
                },
                new Doctor
                {
                    Id = 5,
                    Email = "dr.jovanovic@klinika.rs",
                    FirstName = "Ana",
                    LastName = "Jovanović",
                    PhoneNumber = "060-222-0002",
                    JMBG = "0812987700005",
                    Gender = "F",
                    DateOfBirth = new DateTime(1987, 12, 8),
                    AddressId = 10,
                    ClinicId = 1,
                    PasswordHash = "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe",
                    Specialty = "Pediatrics",
                    LicenseNumber = "LIC-2012-045",
                    CreatedAt = new DateTime(2024, 1, 20),
                    IsActive = true
                },
                new Doctor
                {
                    Id = 6,
                    Email = "dr.stojanovic@klinika.rs",
                    FirstName = "Marko",
                    LastName = "Stojanović",
                    PhoneNumber = "060-222-0003",
                    JMBG = "1108983800006",
                    Gender = "M",
                    DateOfBirth = new DateTime(1983, 8, 11),
                    AddressId = 11,
                    ClinicId = 1,
                    PasswordHash = "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe",
                    Specialty = "General Practice",
                    LicenseNumber = "LIC-2008-123",
                    CreatedAt = new DateTime(2024, 1, 20),
                    IsActive = true
                },
                new Doctor
                {
                    Id = 7,
                    Email = "dr.djordjevic@klinika.rs",
                    FirstName = "Milica",
                    LastName = "Đorđević",
                    PhoneNumber = "060-222-0004",
                    JMBG = "2502990700007",
                    Gender = "F",
                    DateOfBirth = new DateTime(1990, 2, 25),
                    AddressId = 12,
                    ClinicId = 1,
                    PasswordHash = "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe",
                    Specialty = "Dermatology",
                    LicenseNumber = "LIC-2015-078",
                    CreatedAt = new DateTime(2024, 1, 20),
                    IsActive = true
                },
                new Doctor
                {
                    Id = 8,
                    Email = "dr.milosevic@klinika.rs",
                    FirstName = "Stefan",
                    LastName = "Milošević",
                    PhoneNumber = "060-222-0005",
                    JMBG = "1807986700008",
                    Gender = "M",
                    DateOfBirth = new DateTime(1986, 7, 18),
                    AddressId = 13,
                    ClinicId = 1,
                    PasswordHash = "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe",
                    Specialty = "Orthopedics",
                    LicenseNumber = "LIC-2011-092",
                    CreatedAt = new DateTime(2024, 1, 20),
                    IsActive = true
                },

                // Novi Sad Doctors
                new Doctor
                {
                    Id = 9,
                    Email = "dr.popovic@klinika.rs",
                    FirstName = "Jelena",
                    LastName = "Popović",
                    PhoneNumber = "060-222-0006",
                    JMBG = "0602989700009",
                    Gender = "F",
                    DateOfBirth = new DateTime(1989, 2, 6),
                    AddressId = 14,
                    ClinicId = 2,
                    PasswordHash = "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe",
                    Specialty = "Neurology",
                    LicenseNumber = "LIC-2014-156",
                    CreatedAt = new DateTime(2024, 2, 5),
                    IsActive = true
                },
                new Doctor
                {
                    Id = 10,
                    Email = "dr.milic@klinika.rs",
                    FirstName = "Dejan",
                    LastName = "Milić",
                    PhoneNumber = "060-222-0007",
                    JMBG = "2211984700010",
                    Gender = "M",
                    DateOfBirth = new DateTime(1984, 11, 22),
                    AddressId = 15,
                    ClinicId = 2,
                    PasswordHash = "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe",
                    Specialty = "Psychiatry",
                    LicenseNumber = "LIC-2009-201",
                    CreatedAt = new DateTime(2024, 2, 5),
                    IsActive = true
                },
                new Doctor
                {
                    Id = 11,
                    Email = "dr.pavlovic@klinika.rs",
                    FirstName = "Tamara",
                    LastName = "Pavlović",
                    PhoneNumber = "060-222-0008",
                    JMBG = "1404991700011",
                    Gender = "F",
                    DateOfBirth = new DateTime(1991, 4, 14),
                    AddressId = 16,
                    ClinicId = 2,
                    PasswordHash = "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe",
                    Specialty = "Ophthalmology",
                    LicenseNumber = "LIC-2016-234",
                    CreatedAt = new DateTime(2024, 2, 5),
                    IsActive = true
                },
                new Doctor
                {
                    Id = 12,
                    Email = "dr.lazic@klinika.rs",
                    FirstName = "Vladimir",
                    LastName = "Lazić",
                    PhoneNumber = "060-222-0009",
                    JMBG = "0709988700012",
                    Gender = "M",
                    DateOfBirth = new DateTime(1988, 9, 7),
                    AddressId = 17,
                    ClinicId = 2,
                    PasswordHash = "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe",
                    Specialty = "ENT",
                    LicenseNumber = "LIC-2013-189",
                    CreatedAt = new DateTime(2024, 2, 5),
                    IsActive = true
                },

                // Niš Doctors
                new Doctor
                {
                    Id = 13,
                    Email = "dr.ilic@klinika.rs",
                    FirstName = "Aleksandra",
                    LastName = "Ilić",
                    PhoneNumber = "060-222-0010",
                    JMBG = "1910992700013",
                    Gender = "F",
                    DateOfBirth = new DateTime(1992, 10, 19),
                    AddressId = 18,
                    ClinicId = 3,
                    PasswordHash = "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe",
                    Specialty = "Gynecology",
                    LicenseNumber = "LIC-2017-267",
                    CreatedAt = new DateTime(2024, 2, 15),
                    IsActive = true
                },
                new Doctor
                {
                    Id = 14,
                    Email = "dr.djokic@klinika.rs",
                    FirstName = "Nenad",
                    LastName = "Đokić",
                    PhoneNumber = "060-222-0011",
                    JMBG = "2601985700014",
                    Gender = "M",
                    DateOfBirth = new DateTime(1985, 1, 26),
                    AddressId = 19,
                    ClinicId = 3,
                    PasswordHash = "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe",
                    Specialty = "Surgery",
                    LicenseNumber = "LIC-2010-301",
                    CreatedAt = new DateTime(2024, 2, 15),
                    IsActive = true
                },
                new Doctor
                {
                    Id = 15,
                    Email = "dr.ristic@klinika.rs",
                    FirstName = "Dragana",
                    LastName = "Ristić",
                    PhoneNumber = "060-222-0012",
                    JMBG = "0305989700015",
                    Gender = "F",
                    DateOfBirth = new DateTime(1989, 5, 3),
                    AddressId = 20,
                    ClinicId = 3,
                    PasswordHash = "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe",
                    Specialty = "Endocrinology",
                    LicenseNumber = "LIC-2014-345",
                    CreatedAt = new DateTime(2024, 2, 15),
                    IsActive = true
                },

                // Kragujevac Doctors
                new Doctor
                {
                    Id = 16,
                    Email = "dr.markovic@klinika.rs",
                    FirstName = "Milan",
                    LastName = "Marković",
                    PhoneNumber = "060-222-0013",
                    JMBG = "1207983700016",
                    Gender = "M",
                    DateOfBirth = new DateTime(1983, 7, 12),
                    AddressId = 21,
                    ClinicId = 4,
                    PasswordHash = "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe",
                    Specialty = "Urology",
                    LicenseNumber = "LIC-2008-412",
                    CreatedAt = new DateTime(2024, 3, 5),
                    IsActive = true
                },
                new Doctor
                {
                    Id = 17,
                    Email = "dr.simic@klinika.rs",
                    FirstName = "Ivana",
                    LastName = "Simić",
                    PhoneNumber = "060-222-0014",
                    JMBG = "2808991700017",
                    Gender = "F",
                    DateOfBirth = new DateTime(1991, 8, 28),
                    AddressId = 22,
                    ClinicId = 4,
                    PasswordHash = "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe",
                    Specialty = "Pulmonology",
                    LicenseNumber = "LIC-2016-478",
                    CreatedAt = new DateTime(2024, 3, 5),
                    IsActive = true
                },

                // Subotica Doctors
                new Doctor
                {
                    Id = 18,
                    Email = "dr.kostic@klinika.rs",
                    FirstName = "Aleksandar",
                    LastName = "Kostić",
                    PhoneNumber = "060-222-0015",
                    JMBG = "0411987700018",
                    Gender = "M",
                    DateOfBirth = new DateTime(1987, 11, 4),
                    AddressId = 23,
                    ClinicId = 5,
                    PasswordHash = "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe",
                    Specialty = "Rheumatology",
                    LicenseNumber = "LIC-2012-523",
                    CreatedAt = new DateTime(2024, 3, 20),
                    IsActive = true
                },
                new Doctor
                {
                    Id = 19,
                    Email = "dr.tomic@klinika.rs",
                    FirstName = "Bojana",
                    LastName = "Tomić",
                    PhoneNumber = "060-222-0016",
                    JMBG = "1502993700019",
                    Gender = "F",
                    DateOfBirth = new DateTime(1993, 2, 15),
                    AddressId = 24,
                    ClinicId = 5,
                    PasswordHash = "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe",
                    Specialty = "Gastroenterology",
                    LicenseNumber = "LIC-2018-589",
                    CreatedAt = new DateTime(2024, 3, 20),
                    IsActive = true
                }
            );

            // Seed Secretaries
            modelBuilder.Entity<Secretary>().HasData(
                // Beograd Secretaries
                new Secretary
                {
                    Id = 20,
                    Email = "jovana.simic@klinika.rs",
                    FirstName = "Jovana",
                    LastName = "Simić",
                    PhoneNumber = "060-333-0001",
                    JMBG = "1004992800020",
                    Gender = "F",
                    DateOfBirth = new DateTime(1992, 4, 10),
                    AddressId = 25,
                    ClinicId = 1,
                    PasswordHash = "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe",
                    Qualification = "Medical Administration Certificate",
                    CreatedAt = new DateTime(2024, 1, 20),
                    IsActive = true
                },
                new Secretary
                {
                    Id = 21,
                    Email = "tamara.ilic@klinika.rs",
                    FirstName = "Tamara",
                    LastName = "Ilić",
                    PhoneNumber = "060-333-0002",
                    JMBG = "0807994700021",
                    Gender = "F",
                    DateOfBirth = new DateTime(1994, 7, 8),
                    AddressId = 26,
                    ClinicId = 1,
                    PasswordHash = "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe",
                    Qualification = "Healthcare Management Diploma",
                    CreatedAt = new DateTime(2024, 1, 20),
                    IsActive = true
                },

                // Novi Sad Secretaries
                new Secretary
                {
                    Id = 22,
                    Email = "sanja.kovacevic@klinika.rs",
                    FirstName = "Sanja",
                    LastName = "Kovačević",
                    PhoneNumber = "060-333-0003",
                    JMBG = "1510991700022",
                    Gender = "F",
                    DateOfBirth = new DateTime(1991, 10, 15),
                    AddressId = 27,
                    ClinicId = 2,
                    PasswordHash = "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe",
                    Qualification = "Medical Secretary Certificate",
                    CreatedAt = new DateTime(2024, 2, 5),
                    IsActive = true
                },
                new Secretary
                {
                    Id = 23,
                    Email = "ana.radic@klinika.rs",
                    FirstName = "Ana",
                    LastName = "Radić",
                    PhoneNumber = "060-333-0004",
                    JMBG = "2206993700023",
                    Gender = "F",
                    DateOfBirth = new DateTime(1993, 6, 22),
                    AddressId = 28,
                    ClinicId = 2,
                    PasswordHash = "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe",
                    Qualification = "Administrative Assistant Certificate",
                    CreatedAt = new DateTime(2024, 2, 5),
                    IsActive = true
                },

                // Niš Secretary
                new Secretary
                {
                    Id = 24,
                    Email = "milena.radovic@klinika.rs",
                    FirstName = "Milena",
                    LastName = "Radović",
                    PhoneNumber = "060-333-0005",
                    JMBG = "2001993700024",
                    Gender = "F",
                    DateOfBirth = new DateTime(1993, 1, 20),
                    AddressId = 29,
                    ClinicId = 3,
                    PasswordHash = "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe",
                    Qualification = "Medical Office Management",
                    CreatedAt = new DateTime(2024, 2, 15),
                    IsActive = true
                },

                // Kragujevac Secretary
                new Secretary
                {
                    Id = 25,
                    Email = "jelena.antic@klinika.rs",
                    FirstName = "Jelena",
                    LastName = "Antić",
                    PhoneNumber = "060-333-0006",
                    JMBG = "1203995700025",
                    Gender = "F",
                    DateOfBirth = new DateTime(1995, 3, 12),
                    AddressId = 30,
                    ClinicId = 4,
                    PasswordHash = "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe",
                    Qualification = "Healthcare Administration",
                    CreatedAt = new DateTime(2024, 3, 5),
                    IsActive = true
                },

                // Subotica Secretary
                new Secretary
                {
                    Id = 26,
                    Email = "maja.vukovic@klinika.rs",
                    FirstName = "Maja",
                    LastName = "Vuković",
                    PhoneNumber = "060-333-0007",
                    JMBG = "0508994700026",
                    Gender = "F",
                    DateOfBirth = new DateTime(1994, 8, 5),
                    AddressId = 31,
                    ClinicId = 5,
                    PasswordHash = "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe",
                    Qualification = "Medical Secretary Diploma",
                    CreatedAt = new DateTime(2024, 3, 20),
                    IsActive = true
                }
            );
        }
    }
}
