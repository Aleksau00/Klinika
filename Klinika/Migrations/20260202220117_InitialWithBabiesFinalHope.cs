using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Klinika.Migrations
{
    /// <inheritdoc />
    public partial class InitialWithBabiesFinalHope : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Allergens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Allergens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vaccinations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vaccinations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StreetName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StreetNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    ApartmentNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    AdditionalInfo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Addresses_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Clinics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AddressId = table.Column<int>(type: "int", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clinics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clinics_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JMBG = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AddressId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Persons_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Guardians",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guardians", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Guardians_Persons_Id",
                        column: x => x.Id,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Workers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ClinicId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Workers_Clinics_ClinicId",
                        column: x => x.ClinicId,
                        principalTable: "Clinics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Workers_Persons_Id",
                        column: x => x.Id,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    BloodType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GuardianId = table.Column<int>(type: "int", nullable: true),
                    NoShowCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Patients_Guardians_GuardianId",
                        column: x => x.GuardianId,
                        principalTable: "Guardians",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Patients_Persons_Id",
                        column: x => x.Id,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Administrators",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    SeniorityLevel = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administrators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Administrators_Workers_Id",
                        column: x => x.Id,
                        principalTable: "Workers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Doctors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Specialty = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LicenseNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doctors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Doctors_Workers_Id",
                        column: x => x.Id,
                        principalTable: "Workers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Secretaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Qualification = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Secretaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Secretaries_Workers_Id",
                        column: x => x.Id,
                        principalTable: "Workers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatientAllergens",
                columns: table => new
                {
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    AllergenId = table.Column<int>(type: "int", nullable: false),
                    DiagnosedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientAllergens", x => new { x.PatientId, x.AllergenId });
                    table.ForeignKey(
                        name: "FK_PatientAllergens_Allergens_AllergenId",
                        column: x => x.AllergenId,
                        principalTable: "Allergens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PatientAllergens_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppointmentSlots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentSlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppointmentSlots_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentSlotId = table.Column<int>(type: "int", nullable: false),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    ClinicId = table.Column<int>(type: "int", nullable: false),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    AppointmentType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ScheduledDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ScheduledStartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    ScheduledEndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    BookedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BookedByWorkerId = table.Column<int>(type: "int", nullable: false),
                    CheckedInAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PreventiveNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ChildDevelopmentNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsVaccination = table.Column<bool>(type: "bit", nullable: true),
                    VaccinationId = table.Column<int>(type: "int", nullable: true),
                    Anamnesis = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    StatusObservation = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Therapy = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DiagnosedCondition = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointments_AppointmentSlots_AppointmentSlotId",
                        column: x => x.AppointmentSlotId,
                        principalTable: "AppointmentSlots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointments_Clinics_ClinicId",
                        column: x => x.ClinicId,
                        principalTable: "Clinics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointments_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointments_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointments_Vaccinations_VaccinationId",
                        column: x => x.VaccinationId,
                        principalTable: "Vaccinations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointments_Workers_BookedByWorkerId",
                        column: x => x.BookedByWorkerId,
                        principalTable: "Workers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VaccinationRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    VaccinationId = table.Column<int>(type: "int", nullable: false),
                    AdministeredDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AdministeredByDoctorId = table.Column<int>(type: "int", nullable: false),
                    PreventiveAppointmentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaccinationRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VaccinationRecords_Appointments_PreventiveAppointmentId",
                        column: x => x.PreventiveAppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VaccinationRecords_Doctors_AdministeredByDoctorId",
                        column: x => x.AdministeredByDoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VaccinationRecords_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VaccinationRecords_Vaccinations_VaccinationId",
                        column: x => x.VaccinationId,
                        principalTable: "Vaccinations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Allergens",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Alergija na penicilin i srodne antibiotike", "Penicilin" },
                    { 2, "Sezonska alergija na polen breze", "Polen breze" },
                    { 3, "Intolerancija na laktozu", "Laktoza" },
                    { 4, "Alergija na kikiriki i proizvode sa kirikikijem", "Kikiriki" },
                    { 5, "Alergija na jod i kontrastna sredstva", "Jod" },
                    { 6, "Alergija na aspirin i NSAIL lekove", "Aspirin" },
                    { 7, "Alergija na kućnu prašinu i grinje", "Prašina" },
                    { 8, "Alergija na mačju dlaku", "Mačja dlaka" },
                    { 9, "Alergija na sulfonamidne antibiotike", "Sulfonamidi" }
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "Country", "Name", "PostalCode" },
                values: new object[,]
                {
                    { 1, "Serbia", "Beograd", "11000" },
                    { 2, "Serbia", "Novi Sad", "21000" },
                    { 3, "Serbia", "Niš", "18000" },
                    { 4, "Serbia", "Kragujevac", "34000" },
                    { 5, "Serbia", "Subotica", "24000" },
                    { 6, "Serbia", "Zrenjanin", "23000" },
                    { 7, "Serbia", "Pančevo", "26000" },
                    { 8, "Serbia", "Čačak", "32000" },
                    { 9, "Serbia", "Kruševac", "37000" },
                    { 10, "Serbia", "Kraljevo", "36000" },
                    { 11, "Serbia", "Smederevo", "11300" },
                    { 12, "Serbia", "Leskovac", "16000" },
                    { 13, "Serbia", "Užice", "31000" },
                    { 14, "Serbia", "Vranje", "17500" },
                    { 15, "Serbia", "Šabac", "15000" },
                    { 16, "Serbia", "Valjevo", "14000" },
                    { 17, "Serbia", "Sombor", "25000" },
                    { 18, "Serbia", "Požarevac", "12000" },
                    { 19, "Serbia", "Pirot", "18300" },
                    { 20, "Serbia", "Zaječar", "19000" },
                    { 21, "Serbia", "Kikinda", "23300" },
                    { 22, "Serbia", "Sremska Mitrovica", "22000" },
                    { 23, "Serbia", "Jagodina", "35000" },
                    { 24, "Serbia", "Vršac", "26300" },
                    { 25, "Serbia", "Bor", "19210" },
                    { 26, "Serbia", "Prokuplje", "18400" },
                    { 27, "Serbia", "Loznica", "15300" },
                    { 28, "Serbia", "Bečej", "21220" },
                    { 29, "Serbia", "Aranđelovac", "34300" },
                    { 30, "Serbia", "Novi Pazar", "36300" },
                    { 31, "Serbia", "Negotin", "19300" },
                    { 32, "Serbia", "Paraćin", "35250" },
                    { 33, "Serbia", "Inđija", "22320" },
                    { 34, "Serbia", "Lazarevac", "11550" },
                    { 35, "Serbia", "Ćuprija", "35230" },
                    { 36, "Serbia", "Mladenovac", "11400" },
                    { 37, "Serbia", "Ruma", "22400" },
                    { 38, "Serbia", "Stara Pazova", "22300" },
                    { 39, "Serbia", "Kovačica", "26210" },
                    { 40, "Serbia", "Kula", "25230" },
                    { 41, "Serbia", "Gornji Milanovac", "32300" },
                    { 42, "Serbia", "Temerin", "21235" },
                    { 43, "Serbia", "Čoka", "23320" },
                    { 44, "Serbia", "Aleksinac", "18220" },
                    { 45, "Serbia", "Novi Bečej", "23272" },
                    { 46, "Serbia", "Bačka Palanka", "21400" },
                    { 47, "Serbia", "Svilajnac", "35210" },
                    { 48, "Serbia", "Petrovac na Mlavi", "12300" },
                    { 49, "Serbia", "Ada", "24430" },
                    { 50, "Serbia", "Šid", "22240" },
                    { 51, "Serbia", "Priboj", "31330" },
                    { 52, "Serbia", "Srbobran", "21480" },
                    { 53, "Serbia", "Vlasotince", "16210" },
                    { 54, "Serbia", "Kučevo", "12240" },
                    { 55, "Serbia", "Velika Plana", "11320" },
                    { 56, "Serbia", "Knjaževac", "19350" },
                    { 57, "Serbia", "Apatin", "25260" },
                    { 58, "Serbia", "Arilje", "31230" },
                    { 59, "Serbia", "Žabalj", "21230" },
                    { 60, "Serbia", "Nova Varoš", "31320" },
                    { 61, "Serbia", "Senta", "24400" },
                    { 62, "Serbia", "Kovin", "26220" },
                    { 63, "Serbia", "Surdulica", "17530" },
                    { 64, "Serbia", "Žagubica", "12330" },
                    { 65, "Serbia", "Kladovo", "19320" },
                    { 66, "Serbia", "Despotovac", "35213" },
                    { 67, "Serbia", "Beočin", "21300" },
                    { 68, "Serbia", "Bački Petrovac", "21470" },
                    { 69, "Serbia", "Rekovac", "35243" },
                    { 70, "Serbia", "Titel", "21240" },
                    { 71, "Serbia", "Golubac", "12223" },
                    { 72, "Serbia", "Barajevo", "11460" },
                    { 73, "Serbia", "Babušnica", "18330" },
                    { 74, "Serbia", "Dimitrovgrad", "18320" },
                    { 75, "Serbia", "Bela Palanka", "18310" },
                    { 76, "Serbia", "Boljevac", "19370" },
                    { 77, "Serbia", "Majdanpek", "19250" },
                    { 78, "Serbia", "Novi Kneževac", "23330" },
                    { 79, "Serbia", "Sopot", "11450" },
                    { 80, "Serbia", "Veliko Gradište", "12220" },
                    { 81, "Serbia", "Žitište", "23210" },
                    { 82, "Serbia", "Ub", "14210" },
                    { 83, "Serbia", "Bajina Bašta", "31250" },
                    { 84, "Serbia", "Kosjerić", "31260" },
                    { 85, "Serbia", "Ivanjica", "32310" },
                    { 86, "Serbia", "Tutin", "36320" },
                    { 87, "Serbia", "Ljubovija", "15320" },
                    { 88, "Serbia", "Mali Zvornik", "15318" },
                    { 89, "Serbia", "Crna Trava", "17527" },
                    { 90, "Serbia", "Gadžin Han", "18205" },
                    { 91, "Serbia", "Bosilegrad", "17540" },
                    { 92, "Serbia", "Merošina", "18233" },
                    { 93, "Serbia", "Žabari", "12260" },
                    { 94, "Serbia", "Ražanj", "18230" },
                    { 95, "Serbia", "Malo Crniće", "12306" },
                    { 96, "Serbia", "Ljig", "14240" },
                    { 97, "Serbia", "Medveđa", "17516" },
                    { 98, "Serbia", "Osečina", "14253" },
                    { 99, "Serbia", "Preševo", "17523" },
                    { 100, "Serbia", "Bujanovac", "17520" }
                });

            migrationBuilder.InsertData(
                table: "Vaccinations",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Vakcina protiv COVID-19", "COVID-19" },
                    { 2, "Sezonska vakcina protiv gripa", "Grip" },
                    { 3, "Vakcina protiv hepatitisa B", "Hepatitis B" },
                    { 4, "Vakcina protiv malih boginja, mumpsa i rubele", "MMR" },
                    { 5, "Vakcina protiv tetanusa", "Tetanus" },
                    { 6, "Vakcina protiv humanog papiloma virusa", "HPV" },
                    { 7, "Vakcina protiv pneumokoka", "Pneumokokna" },
                    { 8, "Vakcina protiv tuberkuloze", "BCG" }
                });

            migrationBuilder.InsertData(
                table: "Addresses",
                columns: new[] { "Id", "AdditionalInfo", "ApartmentNumber", "CityId", "StreetName", "StreetNumber" },
                values: new object[,]
                {
                    { 1, null, null, 1, "Kneza Miloša", "15" },
                    { 2, null, null, 2, "Bulevar Oslobođenja", "5" },
                    { 3, null, null, 3, "Njegoševa", "10" },
                    { 4, null, null, 4, "Kralja Aleksandra", "88" },
                    { 5, null, null, 5, "Korzo", "3" },
                    { 6, null, null, 1, "Terazije", "25" },
                    { 7, null, null, 2, "Dunavska", "12" },
                    { 8, null, null, 3, "Vojvode Tankosića", "8" },
                    { 9, null, null, 1, "Makedonska", "30" },
                    { 10, null, null, 1, "Svetozara Markovića", "12" },
                    { 11, null, null, 1, "Kralja Milana", "45" },
                    { 12, null, null, 1, "Resavska", "22" },
                    { 13, null, null, 1, "Nemanjina", "18" },
                    { 14, null, null, 2, "Narodnog Fronta", "8" },
                    { 15, null, null, 2, "Bulevar Evrope", "20" },
                    { 16, null, null, 2, "Jevrejska", "22" },
                    { 17, null, null, 2, "Modene", "5" },
                    { 18, null, null, 3, "Obrenovićeva", "18" },
                    { 19, null, null, 3, "Cara Dušana", "15" },
                    { 20, null, null, 3, "Generala Milojka Lešjanina", "7" },
                    { 21, null, null, 4, "Svetozara Markovića", "33" },
                    { 22, null, null, 4, "Đure Daničića", "11" },
                    { 23, null, null, 5, "Matije Korvina", "6" },
                    { 24, null, null, 5, "Segedinski put", "44" },
                    { 25, null, null, 1, "Bulevar Kralja Aleksandra", "77" },
                    { 26, null, null, 1, "Ruzveltova", "14" },
                    { 27, null, null, 2, "Janka Čmelika", "9" },
                    { 28, null, null, 2, "Laze Nančića", "16" },
                    { 29, null, null, 3, "Vožda Karađorđa", "23" },
                    { 30, null, null, 4, "Radničke brigade", "31" },
                    { 31, null, null, 5, "Đure Cvejića", "8" },
                    { 32, null, null, 1, "Cara Lazara", "55" },
                    { 33, null, null, 1, "Branislava Nušića", "12" },
                    { 34, null, null, 2, "Svetog Save", "8" },
                    { 35, null, null, 3, "Vojvode Stepe", "23" }
                });

            migrationBuilder.InsertData(
                table: "Clinics",
                columns: new[] { "Id", "AddressId", "CreatedAt", "Email", "IsActive", "Name", "PhoneNumber" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "info@klinika-beograd.rs", true, "Klinika Beograd Centar", "011-123-4567" },
                    { 2, 2, new DateTime(2024, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "kontakt@mc-novisad.rs", true, "Medicinski Centar Novi Sad", "021-987-6543" },
                    { 3, 3, new DateTime(2024, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "info@poliklinika-nis.rs", true, "Poliklinika Niš", "018-456-7890" },
                    { 4, 4, new DateTime(2024, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "info@dz-kragujevac.rs", true, "Dom Zdravlja Kragujevac", "034-555-1234" },
                    { 5, 5, new DateTime(2024, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "info@klinika-subotica.rs", true, "Klinika Subotica", "024-333-2222" }
                });

            migrationBuilder.InsertData(
                table: "Persons",
                columns: new[] { "Id", "AddressId", "CreatedAt", "DateOfBirth", "Email", "FirstName", "Gender", "JMBG", "LastName", "PhoneNumber" },
                values: new object[,]
                {
                    { 1, 6, new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@klinika.rs", "Marko", "M", "0101990800001", "Administratorović", "060-111-0001" },
                    { 2, 7, new DateTime(2024, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1987, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "marija.petrovic@klinika.rs", "Marija", "F", "1503988700002", "Petrović", "060-111-0002" },
                    { 3, 8, new DateTime(2024, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1992, 6, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "jovana.stojanovic@klinika.rs", "Jovana", "F", "2206992800003", "Stojanović", "060-111-0003" },
                    { 4, 9, new DateTime(2024, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1985, 5, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "dr.nikolic@klinika.rs", "Nikola", "M", "1205985800004", "Nikolić", "060-222-0001" },
                    { 5, 10, new DateTime(2024, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1987, 12, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "dr.jovanovic@klinika.rs", "Ana", "F", "0812987700005", "Jovanović", "060-222-0002" },
                    { 6, 11, new DateTime(2024, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1983, 8, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "dr.stojanovic@klinika.rs", "Marko", "M", "1108983800006", "Stojanović", "060-222-0003" },
                    { 7, 12, new DateTime(2024, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1990, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "dr.djordjevic@klinika.rs", "Milica", "F", "2502990700007", "Đorđević", "060-222-0004" },
                    { 8, 13, new DateTime(2024, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1986, 7, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "dr.milosevic@klinika.rs", "Stefan", "M", "1807986700008", "Milošević", "060-222-0005" },
                    { 9, 14, new DateTime(2024, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1989, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "dr.popovic@klinika.rs", "Jelena", "F", "0602989700009", "Popović", "060-222-0006" },
                    { 10, 15, new DateTime(2024, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1984, 11, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "dr.milic@klinika.rs", "Dejan", "M", "2211984700010", "Milić", "060-222-0007" },
                    { 11, 16, new DateTime(2024, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1991, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "dr.pavlovic@klinika.rs", "Tamara", "F", "1404991700011", "Pavlović", "060-222-0008" },
                    { 12, 17, new DateTime(2024, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1988, 9, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "dr.lazic@klinika.rs", "Vladimir", "M", "0709988700012", "Lazić", "060-222-0009" },
                    { 13, 18, new DateTime(2024, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1992, 10, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "dr.ilic@klinika.rs", "Aleksandra", "F", "1910992700013", "Ilić", "060-222-0010" },
                    { 14, 19, new DateTime(2024, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1985, 1, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "dr.djokic@klinika.rs", "Nenad", "M", "2601985700014", "Đokić", "060-222-0011" },
                    { 15, 20, new DateTime(2024, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1989, 5, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "dr.ristic@klinika.rs", "Dragana", "F", "0305989700015", "Ristić", "060-222-0012" },
                    { 16, 21, new DateTime(2024, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1983, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "dr.markovic@klinika.rs", "Milan", "M", "1207983700016", "Marković", "060-222-0013" },
                    { 17, 22, new DateTime(2024, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1991, 8, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "dr.simic@klinika.rs", "Ivana", "F", "2808991700017", "Simić", "060-222-0014" },
                    { 18, 23, new DateTime(2024, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1987, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "dr.kostic@klinika.rs", "Aleksandar", "M", "0411987700018", "Kostić", "060-222-0015" },
                    { 19, 24, new DateTime(2024, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1993, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "dr.tomic@klinika.rs", "Bojana", "F", "1502993700019", "Tomić", "060-222-0016" },
                    { 20, 25, new DateTime(2024, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1992, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "jovana.simic@klinika.rs", "Jovana", "F", "1004992800020", "Simić", "060-333-0001" },
                    { 21, 26, new DateTime(2024, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1994, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "tamara.ilic@klinika.rs", "Tamara", "F", "0807994700021", "Ilić", "060-333-0002" },
                    { 22, 27, new DateTime(2024, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1991, 10, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "sanja.kovacevic@klinika.rs", "Sanja", "F", "1510991700022", "Kovačević", "060-333-0003" },
                    { 23, 28, new DateTime(2024, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1993, 6, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "ana.radic@klinika.rs", "Ana", "F", "2206993700023", "Radić", "060-333-0004" },
                    { 24, 29, new DateTime(2024, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1993, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "milena.radovic@klinika.rs", "Milena", "F", "2001993700024", "Radović", "060-333-0005" },
                    { 25, 30, new DateTime(2024, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1995, 3, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "jelena.antic@klinika.rs", "Jelena", "F", "1203995700025", "Antić", "060-333-0006" },
                    { 26, 31, new DateTime(2024, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1994, 8, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "maja.vukovic@klinika.rs", "Maja", "F", "0508994700026", "Vuković", "060-333-0007" },
                    { 27, 25, new DateTime(2025, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1995, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "marko.testic@example.com", "Marko", "M", "0101995800027", "Testić", "060-555-0001" },
                    { 28, 26, new DateTime(2025, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1992, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "ana.jovic@example.com", "Ana", "F", "1502992700028", "Jović", "060-555-0002" },
                    { 29, 27, new DateTime(2025, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1998, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "petar.petrovic@example.com", "Petar", "M", "2003998700029", "Petrović", "060-555-0003" },
                    { 30, 28, new DateTime(2025, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1993, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "jovana.milic@example.com", "Jovana", "F", "1206993700030", "Milić", "060-555-0004" },
                    { 31, 29, new DateTime(2025, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1996, 9, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "stefan.nikolic@example.com", "Stefan", "M", "0509996700031", "Nikolić", "060-555-0005" },
                    { 32, 29, new DateTime(2025, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1994, 1, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "milica.djordjevic@example.com", "Milica", "F", "2801994700032", "Đorđević", "060-555-0006" },
                    { 33, 32, new DateTime(2025, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1985, 8, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "milena.testic@example.com", "Milena", "F", "1508985700033", "Testić", "060-888-0001" },
                    { 34, 33, new DateTime(2025, 1, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1983, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "igor.petrovic@example.com", "Igor", "M", "2203983700034", "Petrović", "060-888-0002" },
                    { 35, 34, new DateTime(2025, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1990, 12, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "sandra.jovanovic@example.com", "Sandra", "F", "0812990700035", "Jovanović", "060-888-0003" },
                    { 36, 35, new DateTime(2025, 1, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1988, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "darko.nikolic@example.com", "Darko", "M", "1505988700036", "Nikolić", "060-888-0004" },
                    { 37, 32, new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2011, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "luka.testic@example.com", "Luka", "M", "1505201100037", "Testić", "060-555-0007" },
                    { 38, 33, new DateTime(2023, 8, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 8, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Nikola", "M", "1008202300038", "Petrović", "060-555-0008" },
                    { 39, 34, new DateTime(2025, 7, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 7, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Sara", "F", "1507202500039", "Jovanović", "060-555-0009" },
                    { 40, 35, new DateTime(2025, 4, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "David", "M", "1004202500040", "Nikolić", "060-555-0010" }
                });

            migrationBuilder.InsertData(
                table: "Guardians",
                column: "Id",
                values: new object[]
                {
                    33,
                    34,
                    35,
                    36
                });

            migrationBuilder.InsertData(
                table: "Patients",
                columns: new[] { "Id", "BloodType", "GuardianId", "NoShowCount" },
                values: new object[,]
                {
                    { 27, "A+", null, 0 },
                    { 28, "A+", null, 1 },
                    { 29, "A+", null, 0 },
                    { 30, "A+", null, 0 },
                    { 31, "A+", null, 2 },
                    { 32, "A+", null, 0 }
                });

            migrationBuilder.InsertData(
                table: "Workers",
                columns: new[] { "Id", "ClinicId", "IsActive", "PasswordHash" },
                values: new object[,]
                {
                    { 1, 1, true, "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe" },
                    { 2, 2, true, "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe" },
                    { 3, 3, true, "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe" },
                    { 4, 1, true, "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe" },
                    { 5, 1, true, "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe" },
                    { 6, 1, true, "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe" },
                    { 7, 1, true, "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe" },
                    { 8, 1, true, "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe" },
                    { 9, 2, true, "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe" },
                    { 10, 2, true, "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe" },
                    { 11, 2, true, "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe" },
                    { 12, 2, true, "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe" },
                    { 13, 3, true, "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe" },
                    { 14, 3, true, "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe" },
                    { 15, 3, true, "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe" },
                    { 16, 4, true, "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe" },
                    { 17, 4, true, "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe" },
                    { 18, 5, true, "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe" },
                    { 19, 5, true, "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe" },
                    { 20, 1, true, "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe" },
                    { 21, 1, true, "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe" },
                    { 22, 2, true, "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe" },
                    { 23, 2, true, "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe" },
                    { 24, 3, true, "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe" },
                    { 25, 4, true, "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe" },
                    { 26, 5, true, "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe" }
                });

            migrationBuilder.InsertData(
                table: "Administrators",
                columns: new[] { "Id", "SeniorityLevel" },
                values: new object[,]
                {
                    { 1, "Senior" },
                    { 2, "Mid" },
                    { 3, "Junior" }
                });

            migrationBuilder.InsertData(
                table: "Doctors",
                columns: new[] { "Id", "LicenseNumber", "Specialty" },
                values: new object[,]
                {
                    { 4, "LIC-2010-001", "Cardiology" },
                    { 5, "LIC-2012-045", "Pediatrics" },
                    { 6, "LIC-2008-123", "General Practice" },
                    { 7, "LIC-2015-078", "Dermatology" },
                    { 8, "LIC-2011-092", "Orthopedics" },
                    { 9, "LIC-2014-156", "Neurology" },
                    { 10, "LIC-2009-201", "Psychiatry" },
                    { 11, "LIC-2016-234", "Ophthalmology" },
                    { 12, "LIC-2013-189", "ENT" },
                    { 13, "LIC-2017-267", "Gynecology" },
                    { 14, "LIC-2010-301", "Surgery" },
                    { 15, "LIC-2014-345", "Endocrinology" },
                    { 16, "LIC-2008-412", "Urology" },
                    { 17, "LIC-2016-478", "Pulmonology" },
                    { 18, "LIC-2012-523", "Rheumatology" },
                    { 19, "LIC-2018-589", "Gastroenterology" }
                });

            migrationBuilder.InsertData(
                table: "PatientAllergens",
                columns: new[] { "AllergenId", "PatientId", "DiagnosedDate", "Notes" },
                values: new object[,]
                {
                    { 1, 27, new DateTime(2020, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Reakcija manifestovana osipom i crvenilom kože" },
                    { 2, 27, new DateTime(2018, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Simptomi kijanja i curenja nosa tokom proleća" },
                    { 3, 28, new DateTime(2019, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Izbegavati mlečne proizvode" },
                    { 4, 29, new DateTime(2015, 8, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ozbiljna reakcija, može izazvati anafilaksiju" },
                    { 5, 29, new DateTime(2021, 11, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Izbegavati kontrastna sredstva sa jodom" },
                    { 6, 29, new DateTime(2022, 2, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { 7, 31, new DateTime(2017, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Otežano disanje u prašnjavim prostorima" },
                    { 8, 31, new DateTime(2019, 5, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kijanje i suzenje očiju" },
                    { 9, 32, new DateTime(2020, 12, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), null }
                });

            migrationBuilder.InsertData(
                table: "Patients",
                columns: new[] { "Id", "BloodType", "GuardianId", "NoShowCount" },
                values: new object[,]
                {
                    { 37, "A+", 33, 0 },
                    { 38, "O+", 34, 0 },
                    { 39, "A+", 35, 0 },
                    { 40, "B+", 36, 0 }
                });

            migrationBuilder.InsertData(
                table: "Secretaries",
                columns: new[] { "Id", "Qualification" },
                values: new object[,]
                {
                    { 20, "Medical Administration Certificate" },
                    { 21, "Healthcare Management Diploma" },
                    { 22, "Medical Secretary Certificate" },
                    { 23, "Administrative Assistant Certificate" },
                    { 24, "Medical Office Management" },
                    { 25, "Healthcare Administration" },
                    { 26, "Medical Secretary Diploma" }
                });

            migrationBuilder.InsertData(
                table: "AppointmentSlots",
                columns: new[] { "Id", "Date", "DoctorId", "EndTime", "IsAvailable", "StartTime" },
                values: new object[,]
                {
                    { 1, new DateOnly(2026, 2, 10), 4, new TimeOnly(9, 15, 0), false, new TimeOnly(9, 0, 0) },
                    { 2, new DateOnly(2026, 2, 10), 4, new TimeOnly(9, 30, 0), true, new TimeOnly(9, 15, 0) },
                    { 3, new DateOnly(2026, 2, 10), 4, new TimeOnly(9, 45, 0), true, new TimeOnly(9, 30, 0) },
                    { 4, new DateOnly(2026, 2, 10), 4, new TimeOnly(10, 15, 0), true, new TimeOnly(10, 0, 0) },
                    { 5, new DateOnly(2026, 2, 11), 4, new TimeOnly(9, 15, 0), true, new TimeOnly(9, 0, 0) },
                    { 6, new DateOnly(2026, 2, 11), 4, new TimeOnly(9, 30, 0), true, new TimeOnly(9, 15, 0) },
                    { 7, new DateOnly(2026, 2, 11), 4, new TimeOnly(10, 15, 0), true, new TimeOnly(10, 0, 0) },
                    { 8, new DateOnly(2026, 2, 12), 4, new TimeOnly(9, 15, 0), true, new TimeOnly(9, 0, 0) },
                    { 9, new DateOnly(2026, 2, 12), 4, new TimeOnly(11, 15, 0), true, new TimeOnly(11, 0, 0) },
                    { 10, new DateOnly(2026, 2, 10), 5, new TimeOnly(10, 15, 0), false, new TimeOnly(10, 0, 0) },
                    { 11, new DateOnly(2026, 2, 10), 5, new TimeOnly(10, 30, 0), true, new TimeOnly(10, 15, 0) },
                    { 12, new DateOnly(2026, 2, 10), 5, new TimeOnly(10, 45, 0), true, new TimeOnly(10, 30, 0) },
                    { 13, new DateOnly(2026, 2, 11), 5, new TimeOnly(14, 15, 0), true, new TimeOnly(14, 0, 0) },
                    { 14, new DateOnly(2026, 2, 11), 5, new TimeOnly(14, 30, 0), true, new TimeOnly(14, 15, 0) },
                    { 15, new DateOnly(2026, 2, 10), 9, new TimeOnly(13, 15, 0), false, new TimeOnly(13, 0, 0) },
                    { 16, new DateOnly(2026, 2, 10), 9, new TimeOnly(13, 30, 0), true, new TimeOnly(13, 15, 0) },
                    { 17, new DateOnly(2026, 2, 11), 9, new TimeOnly(15, 15, 0), true, new TimeOnly(15, 0, 0) },
                    { 18, new DateOnly(2026, 2, 11), 9, new TimeOnly(15, 30, 0), true, new TimeOnly(15, 15, 0) },
                    { 19, new DateOnly(2026, 2, 10), 13, new TimeOnly(11, 15, 0), true, new TimeOnly(11, 0, 0) },
                    { 20, new DateOnly(2026, 2, 10), 13, new TimeOnly(11, 30, 0), true, new TimeOnly(11, 15, 0) },
                    { 21, new DateOnly(2026, 2, 11), 13, new TimeOnly(9, 15, 0), true, new TimeOnly(9, 0, 0) }
                });

            migrationBuilder.InsertData(
                table: "VaccinationRecords",
                columns: new[] { "Id", "AdministeredByDoctorId", "AdministeredDate", "Notes", "PatientId", "PreventiveAppointmentId", "VaccinationId" },
                values: new object[,]
                {
                    { 1, 4, new DateTime(2024, 10, 15, 14, 30, 0, 0, DateTimeKind.Unspecified), "Prva doza COVID-19 vakcine. Pacijent dobro podnosi, bez neželjenih reakcija.", 27, null, 1 },
                    { 2, 4, new DateTime(2023, 3, 20, 10, 15, 0, 0, DateTimeKind.Unspecified), "Revakcinacija protiv tetanusa nakon male povrede.", 27, null, 5 },
                    { 3, 5, new DateTime(2024, 11, 5, 9, 0, 0, 0, DateTimeKind.Unspecified), "Sezonska vakcina protiv gripa 2024/2025.", 28, null, 2 },
                    { 4, 9, new DateTime(2024, 9, 12, 11, 30, 0, 0, DateTimeKind.Unspecified), "Booster doza COVID-19 vakcine.", 29, null, 1 },
                    { 5, 9, new DateTime(2022, 5, 18, 14, 0, 0, 0, DateTimeKind.Unspecified), "Hepatitis B vakcina - treća doza.", 29, null, 3 },
                    { 6, 13, new DateTime(2023, 7, 22, 15, 45, 0, 0, DateTimeKind.Unspecified), "Prva doza HPV vakcine.", 30, null, 6 },
                    { 7, 13, new DateTime(2024, 10, 28, 10, 0, 0, 0, DateTimeKind.Unspecified), "Sezonska vakcina protiv gripa.", 31, null, 2 },
                    { 8, 13, new DateTime(2021, 8, 10, 13, 20, 0, 0, DateTimeKind.Unspecified), "Standardna revakcinacija protiv tetanusa.", 31, null, 5 },
                    { 9, 13, new DateTime(2020, 1, 15, 9, 30, 0, 0, DateTimeKind.Unspecified), "MMR vakcina - revakcinacija.", 32, null, 4 },
                    { 10, 13, new DateTime(2024, 12, 3, 16, 0, 0, 0, DateTimeKind.Unspecified), "COVID-19 vakcina - booster doza za zimu 2024/2025.", 32, null, 1 },
                    { 11, 5, new DateTime(2025, 7, 16, 10, 0, 0, 0, DateTimeKind.Unspecified), "BCG vakcina pri rođenju. Beba dobro podnela.", 39, null, 8 },
                    { 12, 5, new DateTime(2025, 7, 16, 10, 5, 0, 0, DateTimeKind.Unspecified), "Hepatitis B - prva doza odmah nakon rođenja.", 39, null, 3 },
                    { 13, 5, new DateTime(2025, 4, 11, 9, 30, 0, 0, DateTimeKind.Unspecified), "BCG vakcina pri rođenju.", 40, null, 8 },
                    { 14, 5, new DateTime(2025, 4, 11, 9, 35, 0, 0, DateTimeKind.Unspecified), "Hepatitis B - prva doza.", 40, null, 3 }
                });

            migrationBuilder.InsertData(
                table: "Appointments",
                columns: new[] { "Id", "Anamnesis", "AppointmentSlotId", "AppointmentType", "BookedAt", "BookedByWorkerId", "CancellationReason", "CancelledAt", "CheckedInAt", "ClinicId", "CompletedAt", "DiagnosedCondition", "DoctorId", "PatientId", "ScheduledDate", "ScheduledEndTime", "ScheduledStartTime", "Status", "StatusObservation", "Therapy" },
                values: new object[] { 1, "Pacijent se žali na bol u grudima koji traje 2 dana, kratkoća daha tokom fizičke aktivnosti, povremeno vrtoglavica. Nema porodičnu istoriju srčanih oboljenja. Puši 10 cigareta dnevno poslednjih 5 godina.", 1, 1, new DateTime(2026, 2, 1, 10, 30, 0, 0, DateTimeKind.Unspecified), 20, null, null, new DateTime(2026, 2, 10, 8, 55, 0, 0, DateTimeKind.Unspecified), 1, new DateTime(2026, 2, 10, 9, 12, 0, 0, DateTimeKind.Unspecified), "Blaga hipertenzija sa sumnjom na anginu pektoris", 4, 27, new DateOnly(2026, 2, 10), new TimeOnly(9, 15, 0), new TimeOnly(9, 0, 0), 2, "Krvni pritisak: 145/95 mmHg (povišen), Puls: 88 otkucaja/min (blago ubrzan), Pacijent izgleda blago zabrinuto, osluškivanje srca pokazuje pravilne tonove bez šumova. EKG: blage promene u ST segmentu.", "Propisano: Aspirin 100mg jednom dnevno, ACE inhibitor (Enalapril 5mg). Preporučeno: EKG test pod opterećenjem, smanjiti unos soli, umerena fizička aktivnost (šetnja 30min dnevno), prestanak pušenja. Kontrolni pregled za 2 nedelje." });

            migrationBuilder.InsertData(
                table: "Appointments",
                columns: new[] { "Id", "AppointmentSlotId", "AppointmentType", "BookedAt", "BookedByWorkerId", "CancellationReason", "CancelledAt", "CheckedInAt", "ChildDevelopmentNotes", "ClinicId", "CompletedAt", "DoctorId", "IsVaccination", "PatientId", "PreventiveNotes", "ScheduledDate", "ScheduledEndTime", "ScheduledStartTime", "Status", "VaccinationId" },
                values: new object[] { 2, 10, 0, new DateTime(2026, 2, 3, 14, 20, 0, 0, DateTimeKind.Unspecified), 20, null, null, null, null, 1, null, 5, false, 28, "Prehlada", new DateOnly(2026, 2, 10), new TimeOnly(10, 15, 0), new TimeOnly(10, 0, 0), 0, null });

            migrationBuilder.InsertData(
                table: "Appointments",
                columns: new[] { "Id", "Anamnesis", "AppointmentSlotId", "AppointmentType", "BookedAt", "BookedByWorkerId", "CancellationReason", "CancelledAt", "CheckedInAt", "ClinicId", "CompletedAt", "DiagnosedCondition", "DoctorId", "PatientId", "ScheduledDate", "ScheduledEndTime", "ScheduledStartTime", "Status", "StatusObservation", "Therapy" },
                values: new object[,]
                {
                    { 3, "Pacijent ima jake glavobolje koje traju već nedelju dana, lokalizovane na levoj strani glave. Bol se pogoršava ujutru, praćen je mučninom. Svetlost i buka pogoršavaju simptome. Nema poremećaja vida.", 15, 1, new DateTime(2026, 2, 2, 11, 0, 0, 0, DateTimeKind.Unspecified), 22, null, null, new DateTime(2026, 2, 10, 12, 58, 0, 0, DateTimeKind.Unspecified), 2, null, "Migrena", 9, 29, new DateOnly(2026, 2, 10), new TimeOnly(13, 15, 0), new TimeOnly(13, 0, 0), 1, "Neurološki pregled: uredan. Pupile jednake, reaguju na svetlo. Nema rigidnosti vrata. Krvni pritisak: 125/80 mmHg (normalan). Pacijent osećljiv na dodir leve temporalne regije.", "Bromazepam, Brufen" },
                    { 4, "Pacijent ima jake glavobolje koje traju već nedelju dana, lokalizovane na levoj strani glave. Bol se pogoršava ujutru, praćen je mučninom. Svetlost i buka pogoršavaju simptome. Nema poremećaja vida.", 5, 1, new DateTime(2026, 1, 28, 9, 15, 0, 0, DateTimeKind.Unspecified), 20, "Pacijent zatražio otkazivanje - zakazao posao u inostranstvu, neće biti u gradu", new DateTime(2026, 2, 8, 16, 30, 0, 0, DateTimeKind.Unspecified), null, 1, null, "Migrena", 4, 30, new DateOnly(2026, 2, 11), new TimeOnly(9, 15, 0), new TimeOnly(9, 0, 0), 3, "Neurološki pregled: uredan. Pupile jednake, reaguju na svetlo. Nema rigidnosti vrata. Krvni pritisak: 125/80 mmHg (normalan). Pacijent osećljiv na dodir leve temporalne regije.", "Bromazepam, Brufen" }
                });

            migrationBuilder.InsertData(
                table: "Appointments",
                columns: new[] { "Id", "AppointmentSlotId", "AppointmentType", "BookedAt", "BookedByWorkerId", "CancellationReason", "CancelledAt", "CheckedInAt", "ChildDevelopmentNotes", "ClinicId", "CompletedAt", "DoctorId", "IsVaccination", "PatientId", "PreventiveNotes", "ScheduledDate", "ScheduledEndTime", "ScheduledStartTime", "Status", "VaccinationId" },
                values: new object[,]
                {
                    { 5, 19, 0, new DateTime(2026, 2, 1, 15, 45, 0, 0, DateTimeKind.Unspecified), 24, null, null, null, null, 3, null, 13, false, 31, "Ostati u krevetu ako se pojave simptomi prehlade ili gripa.", new DateOnly(2026, 2, 10), new TimeOnly(11, 15, 0), new TimeOnly(11, 0, 0), 4, null },
                    { 6, 11, 0, new DateTime(2025, 10, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), 20, null, null, new DateTime(2025, 10, 15, 10, 10, 0, 0, DateTimeKind.Unspecified), "Beba Sara - 3 meseca: Odličan napredak. Težina: 5.8 kg, dužina: 60 cm. Drži glavicu samostalno, prati predmete očima, osmehuje se na glas roditelja. Reaguje na zvukove. Počinje da grabi igračke. Preporučeno: nastaviti dojenje, uvesti vitamin D3. Sledeći pregled za 3 meseca.", 1, new DateTime(2025, 10, 15, 10, 28, 0, 0, DateTimeKind.Unspecified), 5, false, 39, "Redovna kontrola razvoja deteta u 3. mesecu života.", new DateOnly(2025, 10, 15), new TimeOnly(10, 30, 0), new TimeOnly(10, 15, 0), 2, null },
                    { 7, 13, 0, new DateTime(2025, 12, 20, 11, 30, 0, 0, DateTimeKind.Unspecified), 20, null, null, null, null, 1, null, 5, false, 39, "Šestomesečna kontrola razvoja i eventualna vakcinacija.", new DateOnly(2026, 1, 15), new TimeOnly(14, 15, 0), new TimeOnly(14, 0, 0), 0, null },
                    { 8, 12, 0, new DateTime(2025, 6, 25, 14, 0, 0, 0, DateTimeKind.Unspecified), 20, null, null, new DateTime(2025, 7, 10, 10, 25, 0, 0, DateTimeKind.Unspecified), "Beba David - 3 meseca: Normalan razvoj. Težina: 6.2 kg, dužina: 62 cm. Dobro drži glavu, aktivno pomera ruke i noge. Pravi glasove (gugutanje). Prepoznaje roditelje. Spava 4-5 sati noću. Preporučeno: nastaviti dojenje ili adaptirano mleko, vitamin D3.", 1, new DateTime(2025, 7, 10, 10, 43, 0, 0, DateTimeKind.Unspecified), 5, false, 40, "Kontrola razvoja u 3. mesecu.", new DateOnly(2025, 7, 10), new TimeOnly(10, 45, 0), new TimeOnly(10, 30, 0), 2, null },
                    { 9, 14, 0, new DateTime(2025, 9, 25, 10, 15, 0, 0, DateTimeKind.Unspecified), 20, null, null, new DateTime(2025, 10, 10, 14, 10, 0, 0, DateTimeKind.Unspecified), "Beba David - 6 meseci: Odličan napredak. Težina: 8.1 kg, dužina: 68 cm. Sedi uz potporu, okreće se sa stomaka na leđa i obrnuto. Hvata igračke objema rukama, prebacuje iz ruke u ruku. Brblja (ma-ma, ba-ba). Počinje zanimanje za čvrstu hranu. Preporučeno: uvesti kašice (povrće, voće), nastaviti dojenje.", 1, new DateTime(2025, 10, 10, 14, 27, 0, 0, DateTimeKind.Unspecified), 5, false, 40, "Šestomesečna kontrola.", new DateOnly(2025, 10, 10), new TimeOnly(14, 30, 0), new TimeOnly(14, 15, 0), 2, null },
                    { 10, 2, 0, new DateTime(2025, 12, 15, 13, 45, 0, 0, DateTimeKind.Unspecified), 20, null, null, null, null, 1, null, 5, false, 40, "Devetomesečna kontrola razvoja.", new DateOnly(2026, 1, 10), new TimeOnly(9, 30, 0), new TimeOnly(9, 15, 0), 0, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CityId",
                table: "Addresses",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Allergens_Name",
                table: "Allergens",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_AppointmentSlotId",
                table: "Appointments",
                column: "AppointmentSlotId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_BookedByWorkerId",
                table: "Appointments",
                column: "BookedByWorkerId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ClinicId",
                table: "Appointments",
                column: "ClinicId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_DoctorId",
                table: "Appointments",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_DoctorId_ScheduledDate",
                table: "Appointments",
                columns: new[] { "DoctorId", "ScheduledDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PatientId",
                table: "Appointments",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ScheduledDate",
                table: "Appointments",
                column: "ScheduledDate");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_VaccinationId",
                table: "Appointments",
                column: "VaccinationId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentSlots_DoctorId_Date_StartTime",
                table: "AppointmentSlots",
                columns: new[] { "DoctorId", "Date", "StartTime" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cities_Name_Country",
                table: "Cities",
                columns: new[] { "Name", "Country" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clinics_AddressId",
                table: "Clinics",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Clinics_Email",
                table: "Clinics",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatientAllergens_AllergenId",
                table: "PatientAllergens",
                column: "AllergenId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_GuardianId",
                table: "Patients",
                column: "GuardianId");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_AddressId",
                table: "Persons",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_Email",
                table: "Persons",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_VaccinationRecords_AdministeredByDoctorId",
                table: "VaccinationRecords",
                column: "AdministeredByDoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_VaccinationRecords_PatientId",
                table: "VaccinationRecords",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_VaccinationRecords_PreventiveAppointmentId",
                table: "VaccinationRecords",
                column: "PreventiveAppointmentId",
                unique: true,
                filter: "[PreventiveAppointmentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_VaccinationRecords_VaccinationId",
                table: "VaccinationRecords",
                column: "VaccinationId");

            migrationBuilder.CreateIndex(
                name: "IX_Vaccinations_Name",
                table: "Vaccinations",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Workers_ClinicId",
                table: "Workers",
                column: "ClinicId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Administrators");

            migrationBuilder.DropTable(
                name: "PatientAllergens");

            migrationBuilder.DropTable(
                name: "Secretaries");

            migrationBuilder.DropTable(
                name: "VaccinationRecords");

            migrationBuilder.DropTable(
                name: "Allergens");

            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "AppointmentSlots");

            migrationBuilder.DropTable(
                name: "Patients");

            migrationBuilder.DropTable(
                name: "Vaccinations");

            migrationBuilder.DropTable(
                name: "Doctors");

            migrationBuilder.DropTable(
                name: "Guardians");

            migrationBuilder.DropTable(
                name: "Workers");

            migrationBuilder.DropTable(
                name: "Clinics");

            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Cities");
        }
    }
}
