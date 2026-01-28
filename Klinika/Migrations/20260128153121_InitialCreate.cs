using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Klinika.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
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
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Workers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Workers_Persons_Id",
                        column: x => x.Id,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    BloodType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GuardianId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Patients_Guardians_GuardianId",
                        column: x => x.GuardianId,
                        principalTable: "Guardians",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Patients_Persons_Id",
                        column: x => x.Id,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                table: "Addresses",
                columns: new[] { "Id", "AdditionalInfo", "ApartmentNumber", "CityId", "StreetName", "StreetNumber" },
                values: new object[] { 1, null, null, 1, "Admin Street", "1" });

            migrationBuilder.InsertData(
                table: "Persons",
                columns: new[] { "Id", "AddressId", "CreatedAt", "DateOfBirth", "Email", "FirstName", "Gender", "JMBG", "LastName", "PhoneNumber" },
                values: new object[] { 1, 1, new DateTime(2025, 1, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@clinic.com", "Admin", "M", "0000000000000", "Adminovic", "000-000-0000" });

            migrationBuilder.InsertData(
                table: "Workers",
                columns: new[] { "Id", "IsActive", "PasswordHash" },
                values: new object[] { 1, true, "$2a$11$9P.it7ACUqHO8E4jPTtWPeg2smro6UFF7CFueUL5TLokpHqEmFBRe" });

            migrationBuilder.InsertData(
                table: "Administrators",
                columns: new[] { "Id", "SeniorityLevel" },
                values: new object[] { 1, "Senior" });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CityId",
                table: "Addresses",
                column: "CityId");

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
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Administrators");

            migrationBuilder.DropTable(
                name: "AppointmentSlots");

            migrationBuilder.DropTable(
                name: "Patients");

            migrationBuilder.DropTable(
                name: "Secretaries");

            migrationBuilder.DropTable(
                name: "Doctors");

            migrationBuilder.DropTable(
                name: "Guardians");

            migrationBuilder.DropTable(
                name: "Workers");

            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Cities");
        }
    }
}
