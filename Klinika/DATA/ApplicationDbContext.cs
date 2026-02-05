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
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<TreatmentAppointment> TreatmentAppointments { get; set; }
        public DbSet<PreventiveAppointment> PreventiveAppointments { get; set; }
        public DbSet<Allergen> Allergens { get; set; }
        public DbSet<PatientAllergen> PatientAllergens { get; set; }
        public DbSet<Vaccination> Vaccinations { get; set; }
        public DbSet<VaccinationRecord> VaccinationRecords { get; set; }

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
            modelBuilder.Entity<Appointment>().ToTable("Appointments");

            // Configure Person
            modelBuilder.Entity<Person>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.JMBG).HasMaxLength(20);

                entity.HasOne(e => e.Address)
                      .WithMany()
                      .HasForeignKey(e => e.AddressId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // ✅ ADD THIS: Configure TPT inheritance delete behavior
            modelBuilder.Entity<Worker>()
                .HasOne<Person>()
                .WithOne()
                .HasForeignKey<Worker>(w => w.Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Patient>()
                .HasOne<Person>()
                .WithOne()
                .HasForeignKey<Patient>(p => p.Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Guardian>()
                .HasOne<Person>()
                .WithOne()
                .HasForeignKey<Guardian>(g => g.Id)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure PreventiveAppointment (Derived class)


            modelBuilder.Entity<Allergen>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.HasIndex(e => e.Name).IsUnique(); // Ensure allergen names are unique
            });

            modelBuilder.Entity<PatientAllergen>(entity =>
            {
                entity.HasKey(pa => new { pa.PatientId, pa.AllergenId }); // Composite key

                entity.HasOne(pa => pa.Patient)
                      .WithMany(p => p.PatientAllergens)
                      .HasForeignKey(pa => pa.PatientId)
                      .OnDelete(DeleteBehavior.Restrict); // ✅ Changed from Cascade

                entity.HasOne(pa => pa.Allergen)
                      .WithMany(a => a.PatientAllergens)
                      .HasForeignKey(pa => pa.AllergenId)
                      .OnDelete(DeleteBehavior.Restrict); // ✅ Changed from Cascade

                entity.Property(pa => pa.Notes).HasMaxLength(500);
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

            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasKey(e => e.Id);

                // TPH Discriminator - EF Core automatically uses AppointmentType
                entity.HasDiscriminator<AppointmentType>("AppointmentType")
                      .HasValue<TreatmentAppointment>(AppointmentType.Treatment)
                      .HasValue<PreventiveAppointment>(AppointmentType.Preventive);

                // Base properties
                entity.Property(e => e.CancellationReason).HasMaxLength(500);

                // Relationship to AppointmentSlot (One-to-One)
                entity.HasOne(e => e.AppointmentSlot)
                      .WithOne(s => s.Appointment)
                      .HasForeignKey<Appointment>(e => e.AppointmentSlotId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Relationship to Patient (One-to-Many)
                entity.HasOne(e => e.Patient)
                      .WithMany(p => p.Appointments)
                      .HasForeignKey(e => e.PatientId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Relationship to Doctor (One-to-Many)
                entity.HasOne(e => e.Doctor)
                      .WithMany()
                      .HasForeignKey(e => e.DoctorId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Relationship to Clinic (One-to-Many)
                entity.HasOne(e => e.Clinic)
                      .WithMany()
                      .HasForeignKey(e => e.ClinicId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Relationship to BookedByWorker (Secretary)
                entity.HasOne(e => e.BookedByWorker)
                      .WithMany()
                      .HasForeignKey(e => e.BookedByWorkerId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Indexes for performance
                entity.HasIndex(e => e.PatientId);
                entity.HasIndex(e => e.DoctorId);
                entity.HasIndex(e => e.ClinicId);
                entity.HasIndex(e => e.ScheduledDate);
                entity.HasIndex(e => new { e.DoctorId, e.ScheduledDate });
            });

            // Configure TreatmentAppointment (Derived class)
            modelBuilder.Entity<TreatmentAppointment>(entity =>
            {
                // Remove .IsRequired() - these are filled by doctor on completion
                entity.Property(e => e.Anamnesis).HasMaxLength(2000);
                entity.Property(e => e.StatusObservation).HasMaxLength(2000);
                entity.Property(e => e.Therapy).HasMaxLength(2000);
                entity.Property(e => e.DiagnosedCondition).HasMaxLength(500);
            });

            modelBuilder.Entity<PreventiveAppointment>(entity =>
            {
                entity.Property(e => e.PreventiveNotes).HasMaxLength(2000);
                entity.Property(e => e.ChildDevelopmentNotes).HasMaxLength(2000);

                entity.HasOne(e => e.Vaccination)
                      .WithMany()
                      .HasForeignKey(e => e.VaccinationId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Vaccination>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.HasIndex(e => e.Name).IsUnique();
            });

            modelBuilder.Entity<VaccinationRecord>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Notes).HasMaxLength(1000);

                entity.HasOne(e => e.Patient)
                      .WithMany(p => p.VaccinationRecords)
                      .HasForeignKey(e => e.PatientId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Vaccination)
                      .WithMany(v => v.VaccinationRecords)
                      .HasForeignKey(e => e.VaccinationId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.AdministeredByDoctor)
                      .WithMany()
                      .HasForeignKey(e => e.AdministeredByDoctorId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.PreventiveAppointment)
                      .WithOne(pa => pa.VaccinationRecord)
                      .HasForeignKey<VaccinationRecord>(e => e.PreventiveAppointmentId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => e.PatientId);
            });


            modelBuilder.Entity<Allergen>().HasData(
                new Allergen { Id = 1, Name = "Penicilin", Description = "Alergija na penicilin i srodne antibiotike" },
                new Allergen { Id = 2, Name = "Polen breze", Description = "Sezonska alergija na polen breze" },
                new Allergen { Id = 3, Name = "Laktoza", Description = "Intolerancija na laktozu" },
                new Allergen { Id = 4, Name = "Kikiriki", Description = "Alergija na kikiriki i proizvode sa kirikikijem" },
                new Allergen { Id = 5, Name = "Jod", Description = "Alergija na jod i kontrastna sredstva" },
                new Allergen { Id = 6, Name = "Aspirin", Description = "Alergija na aspirin i NSAIL lekove" },
                new Allergen { Id = 7, Name = "Prašina", Description = "Alergija na kućnu prašinu i grinje" },
                new Allergen { Id = 8, Name = "Mačja dlaka", Description = "Alergija na mačju dlaku" },
                new Allergen { Id = 9, Name = "Sulfonamidi", Description = "Alergija na sulfonamidne antibiotike" }
            );

            modelBuilder.Entity<Vaccination>().HasData(
                new Vaccination { Id = 1, Name = "COVID-19", Description = "Vakcina protiv COVID-19" },
                new Vaccination { Id = 2, Name = "Grip", Description = "Sezonska vakcina protiv gripa" },
                new Vaccination { Id = 3, Name = "Hepatitis B", Description = "Vakcina protiv hepatitisa B" },
                new Vaccination { Id = 4, Name = "MMR", Description = "Vakcina protiv malih boginja, mumpsa i rubele" },
                new Vaccination { Id = 5, Name = "Tetanus", Description = "Vakcina protiv tetanusa" },
                new Vaccination { Id = 6, Name = "HPV", Description = "Vakcina protiv humanog papiloma virusa" },
                new Vaccination { Id = 7, Name = "Pneumokokna", Description = "Vakcina protiv pneumokoka" },
                new Vaccination { Id = 8, Name = "BCG", Description = "Vakcina protiv tuberkuloze" }
            );

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
                new Address { Id = 31, StreetName = "Đure Cvejića", StreetNumber = "8", CityId = 5 },

                // Guardian Addresses - ADD THESE!
                new Address { Id = 32, StreetName = "Cara Lazara", StreetNumber = "55", CityId = 1 },
                new Address { Id = 33, StreetName = "Branislava Nušića", StreetNumber = "12", CityId = 1 },
                new Address { Id = 34, StreetName = "Svetog Save", StreetNumber = "8", CityId = 2 },
                new Address { Id = 35, StreetName = "Vojvode Stepe", StreetNumber = "23", CityId = 3 }
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
            // Seed Guardians
            // Seed Guardians
            modelBuilder.Entity<Guardian>().HasData(
                new Guardian
                {
                    Id = 33,
                    Email = "milena.testic@example.com",
                    FirstName = "Milena",
                    LastName = "Testić",
                    PhoneNumber = "060-888-0001",
                    JMBG = "1508985700033",
                    Gender = "F",
                    DateOfBirth = new DateTime(1985, 8, 15),
                    AddressId = 32,
                    CreatedAt = new DateTime(2025, 1, 10)
                },
                new Guardian
                {
                    Id = 34,
                    Email = "igor.petrovic@example.com",
                    FirstName = "Igor",
                    LastName = "Petrović",
                    PhoneNumber = "060-888-0002",
                    JMBG = "2203983700034",
                    Gender = "M",
                    DateOfBirth = new DateTime(1983, 3, 22),
                    AddressId = 33,
                    CreatedAt = new DateTime(2025, 1, 11)
                },
                new Guardian
                {
                    Id = 35,
                    Email = "sandra.jovanovic@example.com",
                    FirstName = "Sandra",
                    LastName = "Jovanović",
                    PhoneNumber = "060-888-0003",
                    JMBG = "0812990700035",
                    Gender = "F",
                    DateOfBirth = new DateTime(1990, 12, 8),
                    AddressId = 34,
                    CreatedAt = new DateTime(2025, 1, 12)
                },
                new Guardian
                {
                    Id = 36,
                    Email = "darko.nikolic@example.com",
                    FirstName = "Darko",
                    LastName = "Nikolić",
                    PhoneNumber = "060-888-0004",
                    JMBG = "1505988700036",
                    Gender = "M",
                    DateOfBirth = new DateTime(1988, 5, 15),
                    AddressId = 35,
                    CreatedAt = new DateTime(2025, 1, 13)
                }
            );
            // Configure Patient-Guardian relationship
            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasOne(p => p.Guardian)
                      .WithMany(g => g.Children)
                      .HasForeignKey(p => p.GuardianId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Patient>().HasData(
                new Patient
                {
                    Id = 27,
                    Email = "marko.testic@example.com",
                    FirstName = "Marko",
                    LastName = "Testić",
                    PhoneNumber = "060-555-0001",
                    JMBG = "0101995800027",
                    Gender = "M",
                    BloodType = "A+",
                    DateOfBirth = new DateTime(1995, 1, 1),
                    AddressId = 25, // Beograd
                    NoShowCount = 0,
                    CreatedAt = new DateTime(2025, 1, 20)
                },
                new Patient
                {
                    Id = 28,
                    Email = "ana.jovic@example.com",
                    FirstName = "Ana",
                    LastName = "Jović",
                    PhoneNumber = "060-555-0002",
                    JMBG = "1502992700028",
                    BloodType = "A+",
                    Gender = "F",
                    DateOfBirth = new DateTime(1992, 2, 15),
                    AddressId = 26, // Beograd
                    NoShowCount = 1, // Has 1 no-show
                    CreatedAt = new DateTime(2025, 1, 20)
                },
                new Patient
                {
                    Id = 29,
                    Email = "petar.petrovic@example.com",
                    FirstName = "Petar",
                    LastName = "Petrović",
                    PhoneNumber = "060-555-0003",
                    BloodType = "A+",
                    JMBG = "2003998700029",
                    Gender = "M",
                    DateOfBirth = new DateTime(1998, 3, 20),
                    AddressId = 27, // Novi Sad
                    NoShowCount = 0,
                    CreatedAt = new DateTime(2025, 1, 20)
                },
                new Patient
                {
                    Id = 30,
                    Email = "jovana.milic@example.com",
                    FirstName = "Jovana",
                    LastName = "Milić",
                    PhoneNumber = "060-555-0004",
                    BloodType = "A+",
                    JMBG = "1206993700030",
                    Gender = "F",
                    DateOfBirth = new DateTime(1993, 6, 12),
                    AddressId = 28, // Novi Sad
                    NoShowCount = 0,
                    CreatedAt = new DateTime(2025, 1, 20)
                },
                new Patient
                {
                    Id = 31,
                    Email = "stefan.nikolic@example.com",
                    FirstName = "Stefan",
                    LastName = "Nikolić",
                    BloodType = "A+",
                    PhoneNumber = "060-555-0005",
                    JMBG = "0509996700031",
                    Gender = "M",
                    DateOfBirth = new DateTime(1996, 9, 5),
                    AddressId = 29, // Niš
                    NoShowCount = 2, // Has 2 no-shows
                    CreatedAt = new DateTime(2025, 1, 20)
                },
                new Patient
                {
                    Id = 32,
                    Email = "milica.djordjevic@example.com",
                    FirstName = "Milica",
                    LastName = "Đorđević",
                    PhoneNumber = "060-555-0006",
                    JMBG = "2801994700032",
                    BloodType = "A+",
                    Gender = "F",
                    DateOfBirth = new DateTime(1994, 1, 28),
                    AddressId = 29, // Niš
                    NoShowCount = 0,
                    CreatedAt = new DateTime(2025, 1, 20)
                },
                    new Patient
                    {
                        Id = 37,
                        Email = "luka.testic@example.com",
                        FirstName = "Luka",
                        LastName = "Testić",
                        PhoneNumber = "060-555-0007",
                        JMBG = "1505201100037",
                        Gender = "M",
                        BloodType = "A+",
                        DateOfBirth = new DateTime(2011, 5, 15),
                        AddressId = 32,
                        GuardianId = 33, // Mother Milena
                        NoShowCount = 0,
                        CreatedAt = new DateTime(2025, 1, 15)
                    },

                    // Toddler (2 years old)
                    new Patient
                    {
                        Id = 38,
                        FirstName = "Nikola",
                        LastName = "Petrović",
                        PhoneNumber = "060-555-0008",
                        JMBG = "1008202300038",
                        Gender = "M",
                        BloodType = "O+",
                        DateOfBirth = new DateTime(2023, 8, 10),
                        AddressId = 33,
                        GuardianId = 34, // Father Igor
                        NoShowCount = 0,
                        CreatedAt = new DateTime(2023, 8, 15)
                    },

                    // Baby 1 (6 months old) - Sara
                    new Patient
                    {
                        Id = 39,
                        FirstName = "Sara",
                        LastName = "Jovanović",
                        PhoneNumber = "060-555-0009",
                        JMBG = "1507202500039",
                        Gender = "F",
                        BloodType = "A+",
                        DateOfBirth = new DateTime(2025, 7, 15), // 6 months old
                        AddressId = 34,
                        GuardianId = 35, // Mother Sandra
                        NoShowCount = 0,
                        CreatedAt = new DateTime(2025, 7, 16)
                    },

                    // Baby 2 (9 months old) - David
                    new Patient
                    {
                        Id = 40,
                        FirstName = "David",
                        LastName = "Nikolić",
                        PhoneNumber = "060-555-0010",
                        JMBG = "1004202500040",
                        Gender = "M",
                        BloodType = "B+",
                        DateOfBirth = new DateTime(2025, 4, 10), // 9 months old
                        AddressId = 35,
                        GuardianId = 36, // Father Darko
                        NoShowCount = 0,
                        CreatedAt = new DateTime(2025, 4, 11)
                    }

            );

            modelBuilder.Entity<AppointmentSlot>().HasData(
                // February 10, 2026
                new AppointmentSlot { Id = 1, DoctorId = 4, Date = new DateOnly(2026, 2, 10), StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(9, 15), IsAvailable = false },
                new AppointmentSlot { Id = 2, DoctorId = 4, Date = new DateOnly(2026, 2, 10), StartTime = new TimeOnly(9, 15), EndTime = new TimeOnly(9, 30), IsAvailable = true },
                new AppointmentSlot { Id = 3, DoctorId = 4, Date = new DateOnly(2026, 2, 10), StartTime = new TimeOnly(9, 30), EndTime = new TimeOnly(9, 45), IsAvailable = true },
                new AppointmentSlot { Id = 4, DoctorId = 4, Date = new DateOnly(2026, 2, 10), StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(10, 15), IsAvailable = true },
    
                // February 11, 2026
                new AppointmentSlot { Id = 5, DoctorId = 4, Date = new DateOnly(2026, 2, 11), StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(9, 15), IsAvailable = true },
                new AppointmentSlot { Id = 6, DoctorId = 4, Date = new DateOnly(2026, 2, 11), StartTime = new TimeOnly(9, 15), EndTime = new TimeOnly(9, 30), IsAvailable = true },
                new AppointmentSlot { Id = 7, DoctorId = 4, Date = new DateOnly(2026, 2, 11), StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(10, 15), IsAvailable = true },
    
                // February 12, 2026
                new AppointmentSlot { Id = 8, DoctorId = 4, Date = new DateOnly(2026, 2, 12), StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(9, 15), IsAvailable = true },
                new AppointmentSlot { Id = 9, DoctorId = 4, Date = new DateOnly(2026, 2, 12), StartTime = new TimeOnly(11, 0), EndTime = new TimeOnly(11, 15), IsAvailable = true }
            );

            // Slots for Dr. Ana Jovanović (Pediatrician, Beograd) - ID 5
            modelBuilder.Entity<AppointmentSlot>().HasData(
                new AppointmentSlot { Id = 10, DoctorId = 5, Date = new DateOnly(2026, 2, 10), StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(10, 15), IsAvailable = false },
                new AppointmentSlot { Id = 11, DoctorId = 5, Date = new DateOnly(2026, 2, 10), StartTime = new TimeOnly(10, 15), EndTime = new TimeOnly(10, 30), IsAvailable = true },
                new AppointmentSlot { Id = 12, DoctorId = 5, Date = new DateOnly(2026, 2, 10), StartTime = new TimeOnly(10, 30), EndTime = new TimeOnly(10, 45), IsAvailable = true },
                new AppointmentSlot { Id = 13, DoctorId = 5, Date = new DateOnly(2026, 2, 11), StartTime = new TimeOnly(14, 0), EndTime = new TimeOnly(14, 15), IsAvailable = true },
                new AppointmentSlot { Id = 14, DoctorId = 5, Date = new DateOnly(2026, 2, 11), StartTime = new TimeOnly(14, 15), EndTime = new TimeOnly(14, 30), IsAvailable = true }
            );

            // Slots for Dr. Jelena Popović (Neurologist, Novi Sad) - ID 9
            modelBuilder.Entity<AppointmentSlot>().HasData(
                new AppointmentSlot { Id = 15, DoctorId = 9, Date = new DateOnly(2026, 2, 10), StartTime = new TimeOnly(13, 0), EndTime = new TimeOnly(13, 15), IsAvailable = false },
                new AppointmentSlot { Id = 16, DoctorId = 9, Date = new DateOnly(2026, 2, 10), StartTime = new TimeOnly(13, 15), EndTime = new TimeOnly(13, 30), IsAvailable = true },
                new AppointmentSlot { Id = 17, DoctorId = 9, Date = new DateOnly(2026, 2, 11), StartTime = new TimeOnly(15, 0), EndTime = new TimeOnly(15, 15), IsAvailable = true },
                new AppointmentSlot { Id = 18, DoctorId = 9, Date = new DateOnly(2026, 2, 11), StartTime = new TimeOnly(15, 15), EndTime = new TimeOnly(15, 30), IsAvailable = true }
            );

            // Slots for Dr. Aleksandra Ilić (Gynecologist, Niš) - ID 13
            modelBuilder.Entity<AppointmentSlot>().HasData(
                new AppointmentSlot { Id = 19, DoctorId = 13, Date = new DateOnly(2026, 2, 10), StartTime = new TimeOnly(11, 0), EndTime = new TimeOnly(11, 15), IsAvailable = true },
                new AppointmentSlot { Id = 20, DoctorId = 13, Date = new DateOnly(2026, 2, 10), StartTime = new TimeOnly(11, 15), EndTime = new TimeOnly(11, 30), IsAvailable = true },
                new AppointmentSlot { Id = 21, DoctorId = 13, Date = new DateOnly(2026, 2, 11), StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(9, 15), IsAvailable = true }
            );

            modelBuilder.Entity<PatientAllergen>().HasData(
    // Patient 27 (Marko Testić) - Penicilin and Polen breze
                new PatientAllergen
                {
                    PatientId = 27,
                    AllergenId = 1,
                    DiagnosedDate = new DateTime(2020, 3, 15),
                    Notes = "Reakcija manifestovana osipom i crvenilom kože"
                },
                new PatientAllergen
                {
                    PatientId = 27,
                    AllergenId = 2,
                    DiagnosedDate = new DateTime(2018, 4, 10),
                    Notes = "Simptomi kijanja i curenja nosa tokom proleća"
                },

                // Patient 28 (Ana Jović) - Laktoza
                new PatientAllergen
                {
                    PatientId = 28,
                    AllergenId = 3,
                    DiagnosedDate = new DateTime(2019, 6, 20),
                    Notes = "Izbegavati mlečne proizvode"
                },

                // Patient 29 (Petar Petrović) - Kikiriki, Jod, Aspirin
                new PatientAllergen
                {
                    PatientId = 29,
                    AllergenId = 4,
                    DiagnosedDate = new DateTime(2015, 8, 5),
                    Notes = "Ozbiljna reakcija, može izazvati anafilaksiju"
                },
                new PatientAllergen
                {
                    PatientId = 29,
                    AllergenId = 5,
                    DiagnosedDate = new DateTime(2021, 11, 12),
                    Notes = "Izbegavati kontrastna sredstva sa jodom"
                },
                new PatientAllergen
                {
                    PatientId = 29,
                    AllergenId = 6,
                    DiagnosedDate = new DateTime(2022, 2, 18)
                },

                // Patient 31 (Stefan Nikolić) - Prašina and Mačja dlaka
                new PatientAllergen
                {
                    PatientId = 31,
                    AllergenId = 7,
                    DiagnosedDate = new DateTime(2017, 9, 25),
                    Notes = "Otežano disanje u prašnjavim prostorima"
                },
                new PatientAllergen
                {
                    PatientId = 31,
                    AllergenId = 8,
                    DiagnosedDate = new DateTime(2019, 5, 30),
                    Notes = "Kijanje i suzenje očiju"
                },

                // Patient 32 (Milica Đorđević) - Sulfonamidi
                new PatientAllergen
                {
                    PatientId = 32,
                    AllergenId = 9,
                    DiagnosedDate = new DateTime(2020, 12, 8)
                }
            );

            // ===============================================================
            // SEED SAMPLE APPOINTMENTS (for testing different scenarios)
            // ===============================================================

            // Appointment 1: Treatment - COMPLETED
            modelBuilder.Entity<TreatmentAppointment>().HasData(
                new
                {
                    Id = 1,
                    AppointmentSlotId = 1,
                    PatientId = 27,
                    ClinicId = 1,
                    DoctorId = 4,
                    AppointmentType = AppointmentType.Treatment,
                    Status = AppointmentStatus.Completed,
                    ScheduledDate = new DateOnly(2026, 2, 10),
                    ScheduledStartTime = new TimeOnly(9, 0),
                    ScheduledEndTime = new TimeOnly(9, 15),
                    BookedAt = new DateTime(2026, 2, 1, 10, 30, 0),
                    BookedByWorkerId = 20, // Secretary Jovana
                    CheckedInAt = new DateTime(2026, 2, 10, 8, 55, 0),
                    CompletedAt = new DateTime(2026, 2, 10, 9, 12, 0),
                    Anamnesis = "Pacijent se žali na bol u grudima koji traje 2 dana, kratkoća daha tokom fizičke aktivnosti, povremeno vrtoglavica. Nema porodičnu istoriju srčanih oboljenja. Puši 10 cigareta dnevno poslednjih 5 godina.",
                    StatusObservation = "Krvni pritisak: 145/95 mmHg (povišen), Puls: 88 otkucaja/min (blago ubrzan), Pacijent izgleda blago zabrinuto, osluškivanje srca pokazuje pravilne tonove bez šumova. EKG: blage promene u ST segmentu.",
                    Therapy = "Propisano: Aspirin 100mg jednom dnevno, ACE inhibitor (Enalapril 5mg). Preporučeno: EKG test pod opterećenjem, smanjiti unos soli, umerena fizička aktivnost (šetnja 30min dnevno), prestanak pušenja. Kontrolni pregled za 2 nedelje.",
                    DiagnosedCondition = "Blaga hipertenzija sa sumnjom na anginu pektoris"
                }
            );

            // Appointment 2: Preventive - SCHEDULED (upcoming)
            modelBuilder.Entity<PreventiveAppointment>().HasData(
                new
                {
                    Id = 2,
                    AppointmentSlotId = 10,
                    PatientId = 28,
                    ClinicId = 1,
                    DoctorId = 5,
                    AppointmentType = AppointmentType.Preventive,
                    Status = AppointmentStatus.Scheduled,
                    ScheduledDate = new DateOnly(2026, 2, 10),
                    ScheduledStartTime = new TimeOnly(10, 0),
                    ScheduledEndTime = new TimeOnly(10, 15),
                    BookedAt = new DateTime(2026, 2, 3, 14, 20, 0),
                    BookedByWorkerId = 20, // Secretary Jovana
                    PreventiveNotes = "Prehlada",
                    IsVaccination = false
                }
            );

            // Appointment 3: Treatment - IN PROGRESS (being examined now)
            modelBuilder.Entity<TreatmentAppointment>().HasData(
                new
                {
                    Id = 3,
                    AppointmentSlotId = 15,
                    PatientId = 29,
                    ClinicId = 2,
                    DoctorId = 9,
                    AppointmentType = AppointmentType.Treatment,
                    Status = AppointmentStatus.InProgress,
                    ScheduledDate = new DateOnly(2026, 2, 10),
                    ScheduledStartTime = new TimeOnly(13, 0),
                    ScheduledEndTime = new TimeOnly(13, 15),
                    BookedAt = new DateTime(2026, 2, 2, 11, 0, 0),
                    BookedByWorkerId = 22, // Secretary Sanja (Novi Sad)
                    CheckedInAt = new DateTime(2026, 2, 10, 12, 58, 0),
                    Anamnesis = "Pacijent ima jake glavobolje koje traju već nedelju dana, lokalizovane na levoj strani glave. Bol se pogoršava ujutru, praćen je mučninom. Svetlost i buka pogoršavaju simptome. Nema poremećaja vida.",
                    StatusObservation = "Neurološki pregled: uredan. Pupile jednake, reaguju na svetlo. Nema rigidnosti vrata. Krvni pritisak: 125/80 mmHg (normalan). Pacijent osećljiv na dodir leve temporalne regije.",
                    Therapy = "Bromazepam, Brufen", // Doctor hasn't filled this yet
                    DiagnosedCondition = "Migrena" // Doctor hasn't filled this yet
                }
            );

            // Appointment 4: Treatment - CANCELLED
            modelBuilder.Entity<TreatmentAppointment>().HasData(
                new
                {
                    Id = 4,
                    AppointmentSlotId = 5,
                    PatientId = 30,
                    ClinicId = 1,
                    DoctorId = 4,
                    AppointmentType = AppointmentType.Treatment,
                    Status = AppointmentStatus.Cancelled,
                    ScheduledDate = new DateOnly(2026, 2, 11),
                    ScheduledStartTime = new TimeOnly(9, 0),
                    ScheduledEndTime = new TimeOnly(9, 15),
                    BookedAt = new DateTime(2026, 1, 28, 9, 15, 0),
                    BookedByWorkerId = 20,
                    CancelledAt = new DateTime(2026, 2, 8, 16, 30, 0),
                    CancellationReason = "Pacijent zatražio otkazivanje - zakazao posao u inostranstvu, neće biti u gradu",
                    Anamnesis = "Pacijent ima jake glavobolje koje traju već nedelju dana, lokalizovane na levoj strani glave. Bol se pogoršava ujutru, praćen je mučninom. Svetlost i buka pogoršavaju simptome. Nema poremećaja vida.",
                    StatusObservation = "Neurološki pregled: uredan. Pupile jednake, reaguju na svetlo. Nema rigidnosti vrata. Krvni pritisak: 125/80 mmHg (normalan). Pacijent osećljiv na dodir leve temporalne regije.",
                    Therapy = "Bromazepam, Brufen", // Doctor hasn't filled this yet
                    DiagnosedCondition = "Migrena" // Doctor hasn't filled this yet
                }
            );

            // Appointment 5: Preventive - NO SHOW
            modelBuilder.Entity<PreventiveAppointment>().HasData(
                new
                {
                    Id = 5,
                    AppointmentSlotId = 19,
                    PatientId = 31,
                    ClinicId = 3,
                    DoctorId = 13,
                    AppointmentType = AppointmentType.Preventive,
                    Status = AppointmentStatus.NoShow,
                    ScheduledDate = new DateOnly(2026, 2, 10),
                    ScheduledStartTime = new TimeOnly(11, 0),
                    ScheduledEndTime = new TimeOnly(11, 15),
                    BookedAt = new DateTime(2026, 2, 1, 15, 45, 0),
                    BookedByWorkerId = 24, // Secretary Milena (Niš)
                    PreventiveNotes = "Ostati u krevetu ako se pojave simptomi prehlade ili gripa.",
                    IsVaccination = false
                }
            );

            modelBuilder.Entity<PreventiveAppointment>().HasData(
    new
    {
        Id = 6,
        AppointmentSlotId = 11,
        PatientId = 39, // Baby Sara
        ClinicId = 1,
        DoctorId = 5, // Pediatrician Dr. Ana
        AppointmentType = AppointmentType.Preventive,
        Status = AppointmentStatus.Completed,
        ScheduledDate = new DateOnly(2025, 10, 15),
        ScheduledStartTime = new TimeOnly(10, 15),
        ScheduledEndTime = new TimeOnly(10, 30),
        BookedAt = new DateTime(2025, 10, 1, 9, 0, 0),
        BookedByWorkerId = 20,
        CheckedInAt = new DateTime(2025, 10, 15, 10, 10, 0),
        CompletedAt = new DateTime(2025, 10, 15, 10, 28, 0),
        PreventiveNotes = "Redovna kontrola razvoja deteta u 3. mesecu života.",
        ChildDevelopmentNotes = "Beba Sara - 3 meseca: Odličan napredak. Težina: 5.8 kg, dužina: 60 cm. Drži glavicu samostalno, prati predmete očima, osmehuje se na glas roditelja. Reaguje na zvukove. Počinje da grabi igračke. Preporučeno: nastaviti dojenje, uvesti vitamin D3. Sledeći pregled za 3 meseca.",
        IsVaccination = false
    }
);

            // Sara's 6-month checkup (SCHEDULED - upcoming)
            modelBuilder.Entity<PreventiveAppointment>().HasData(
                new
                {
                    Id = 7,
                    AppointmentSlotId = 13,
                    PatientId = 39, // Baby Sara
                    ClinicId = 1,
                    DoctorId = 5,
                    AppointmentType = AppointmentType.Preventive,
                    Status = AppointmentStatus.Scheduled,
                    ScheduledDate = new DateOnly(2026, 1, 15),
                    ScheduledStartTime = new TimeOnly(14, 0),
                    ScheduledEndTime = new TimeOnly(14, 15),
                    BookedAt = new DateTime(2025, 12, 20, 11, 30, 0),
                    BookedByWorkerId = 20,
                    PreventiveNotes = "Šestomesečna kontrola razvoja i eventualna vakcinacija.",
                    IsVaccination = false
                }
            );

            // ===== BABY DEVELOPMENT TRACKING - DAVID (9 months) =====

            // David's 3-month checkup (COMPLETED)
            modelBuilder.Entity<PreventiveAppointment>().HasData(
                new
                {
                    Id = 8,
                    AppointmentSlotId = 12,
                    PatientId = 40, // Baby David
                    ClinicId = 1,
                    DoctorId = 5,
                    AppointmentType = AppointmentType.Preventive,
                    Status = AppointmentStatus.Completed,
                    ScheduledDate = new DateOnly(2025, 7, 10),
                    ScheduledStartTime = new TimeOnly(10, 30),
                    ScheduledEndTime = new TimeOnly(10, 45),
                    BookedAt = new DateTime(2025, 6, 25, 14, 0, 0),
                    BookedByWorkerId = 20,
                    CheckedInAt = new DateTime(2025, 7, 10, 10, 25, 0),
                    CompletedAt = new DateTime(2025, 7, 10, 10, 43, 0),
                    PreventiveNotes = "Kontrola razvoja u 3. mesecu.",
                    ChildDevelopmentNotes = "Beba David - 3 meseca: Normalan razvoj. Težina: 6.2 kg, dužina: 62 cm. Dobro drži glavu, aktivno pomera ruke i noge. Pravi glasove (gugutanje). Prepoznaje roditelje. Spava 4-5 sati noću. Preporučeno: nastaviti dojenje ili adaptirano mleko, vitamin D3.",
                    IsVaccination = false
                }
            );

            // David's 6-month checkup (COMPLETED)
            modelBuilder.Entity<PreventiveAppointment>().HasData(
                new
                {
                    Id = 9,
                    AppointmentSlotId = 14,
                    PatientId = 40, // Baby David
                    ClinicId = 1,
                    DoctorId = 5,
                    AppointmentType = AppointmentType.Preventive,
                    Status = AppointmentStatus.Completed,
                    ScheduledDate = new DateOnly(2025, 10, 10),
                    ScheduledStartTime = new TimeOnly(14, 15),
                    ScheduledEndTime = new TimeOnly(14, 30),
                    BookedAt = new DateTime(2025, 9, 25, 10, 15, 0),
                    BookedByWorkerId = 20,
                    CheckedInAt = new DateTime(2025, 10, 10, 14, 10, 0),
                    CompletedAt = new DateTime(2025, 10, 10, 14, 27, 0),
                    PreventiveNotes = "Šestomesečna kontrola.",
                    ChildDevelopmentNotes = "Beba David - 6 meseci: Odličan napredak. Težina: 8.1 kg, dužina: 68 cm. Sedi uz potporu, okreće se sa stomaka na leđa i obrnuto. Hvata igračke objema rukama, prebacuje iz ruke u ruku. Brblja (ma-ma, ba-ba). Počinje zanimanje za čvrstu hranu. Preporučeno: uvesti kašice (povrće, voće), nastaviti dojenje.",
                    IsVaccination = false
                }
            );

            // David's 9-month checkup (SCHEDULED - upcoming)
            modelBuilder.Entity<PreventiveAppointment>().HasData(
                new
                {
                    Id = 10,
                    AppointmentSlotId = 2,
                    PatientId = 40, // Baby David
                    ClinicId = 1,
                    DoctorId = 5,
                    AppointmentType = AppointmentType.Preventive,
                    Status = AppointmentStatus.Scheduled,
                    ScheduledDate = new DateOnly(2026, 1, 10),
                    ScheduledStartTime = new TimeOnly(9, 15),
                    ScheduledEndTime = new TimeOnly(9, 30),
                    BookedAt = new DateTime(2025, 12, 15, 13, 45, 0),
                    BookedByWorkerId = 20,
                    PreventiveNotes = "Devetomesečna kontrola razvoja.",
                    IsVaccination = false
                }
            );

            // Seed Vaccination Records
            modelBuilder.Entity<VaccinationRecord>().HasData(
                // Patient 27 (Marko Testić) - COVID-19 and Tetanus
                new VaccinationRecord
                {
                    Id = 1,
                    PatientId = 27,
                    VaccinationId = 1, // COVID-19
                    AdministeredDate = new DateTime(2024, 10, 15, 14, 30, 0),
                    AdministeredByDoctorId = 4,
                    Notes = "Prva doza COVID-19 vakcine. Pacijent dobro podnosi, bez neželjenih reakcija."
                },
                new VaccinationRecord
                {
                    Id = 2,
                    PatientId = 27,
                    VaccinationId = 5, // Tetanus
                    AdministeredDate = new DateTime(2023, 3, 20, 10, 15, 0),
                    AdministeredByDoctorId = 4,
                    Notes = "Revakcinacija protiv tetanusa nakon male povrede."
                },

                // Patient 28 (Ana Jović) - Grip (Flu)
                new VaccinationRecord
                {
                    Id = 3,
                    PatientId = 28,
                    VaccinationId = 2, // Grip
                    AdministeredDate = new DateTime(2024, 11, 5, 9, 0, 0),
                    AdministeredByDoctorId = 5,
                    Notes = "Sezonska vakcina protiv gripa 2024/2025."
                },

                // Patient 29 (Petar Petrović) - COVID-19, Hepatitis B
                new VaccinationRecord
                {
                    Id = 4,
                    PatientId = 29,
                    VaccinationId = 1, // COVID-19
                    AdministeredDate = new DateTime(2024, 9, 12, 11, 30, 0),
                    AdministeredByDoctorId = 9,
                    Notes = "Booster doza COVID-19 vakcine."
                },
                new VaccinationRecord
                {
                    Id = 5,
                    PatientId = 29,
                    VaccinationId = 3, // Hepatitis B
                    AdministeredDate = new DateTime(2022, 5, 18, 14, 0, 0),
                    AdministeredByDoctorId = 9,
                    Notes = "Hepatitis B vakcina - treća doza."
                },

                // Patient 30 (Jovana Milić) - HPV
                new VaccinationRecord
                {
                    Id = 6,
                    PatientId = 30,
                    VaccinationId = 6, // HPV
                    AdministeredDate = new DateTime(2023, 7, 22, 15, 45, 0),
                    AdministeredByDoctorId = 13,
                    Notes = "Prva doza HPV vakcine."
                },

                // Patient 31 (Stefan Nikolić) - Grip, Tetanus
                new VaccinationRecord
                {
                    Id = 7,
                    PatientId = 31,
                    VaccinationId = 2, // Grip
                    AdministeredDate = new DateTime(2024, 10, 28, 10, 0, 0),
                    AdministeredByDoctorId = 13,
                    Notes = "Sezonska vakcina protiv gripa."
                },
                new VaccinationRecord
                {
                    Id = 8,
                    PatientId = 31,
                    VaccinationId = 5, // Tetanus
                    AdministeredDate = new DateTime(2021, 8, 10, 13, 20, 0),
                    AdministeredByDoctorId = 13,
                    Notes = "Standardna revakcinacija protiv tetanusa."
                },

                // Patient 32 (Milica Đorđević) - MMR, COVID-19
                new VaccinationRecord
                {
                    Id = 9,
                    PatientId = 32,
                    VaccinationId = 4, // MMR
                    AdministeredDate = new DateTime(2020, 1, 15, 9, 30, 0),
                    AdministeredByDoctorId = 13,
                    Notes = "MMR vakcina - revakcinacija."
                },
                new VaccinationRecord
                {
                    Id = 10,
                    PatientId = 32,
                    VaccinationId = 1, // COVID-19
                    AdministeredDate = new DateTime(2024, 12, 3, 16, 0, 0),
                    AdministeredByDoctorId = 13,
                    Notes = "COVID-19 vakcina - booster doza za zimu 2024/2025."
                },
                // Baby Sara vaccinations
                new VaccinationRecord
                {
                    Id = 11,
                    PatientId = 39, // Baby Sara
                    VaccinationId = 8, // BCG
                    AdministeredDate = new DateTime(2025, 7, 16, 10, 0, 0),
                    AdministeredByDoctorId = 5,
                    Notes = "BCG vakcina pri rođenju. Beba dobro podnela."
                },
                new VaccinationRecord
                {
                    Id = 12,
                    PatientId = 39,
                    VaccinationId = 3, // Hepatitis B
                    AdministeredDate = new DateTime(2025, 7, 16, 10, 5, 0),
                    AdministeredByDoctorId = 5,
                    Notes = "Hepatitis B - prva doza odmah nakon rođenja."
                },

                // Baby David vaccinations
                new VaccinationRecord
                {
                    Id = 13,
                    PatientId = 40, // Baby David
                    VaccinationId = 8, // BCG
                    AdministeredDate = new DateTime(2025, 4, 11, 9, 30, 0),
                    AdministeredByDoctorId = 5,
                    Notes = "BCG vakcina pri rođenju."
                },
                new VaccinationRecord
                {
                    Id = 14,
                    PatientId = 40,
                    VaccinationId = 3, // Hepatitis B
                    AdministeredDate = new DateTime(2025, 4, 11, 9, 35, 0),
                    AdministeredByDoctorId = 5,
                    Notes = "Hepatitis B - prva doza."
                }

            );
        }
    }
}
