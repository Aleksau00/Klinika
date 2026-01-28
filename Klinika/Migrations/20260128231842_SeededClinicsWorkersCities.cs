using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Klinika.Migrations
{
    /// <inheritdoc />
    public partial class SeededClinicsWorkersCities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClinicId",
                table: "Workers",
                type: "int",
                nullable: true);

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

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "StreetName", "StreetNumber" },
                values: new object[] { "Kneza Miloša", "15" });

            migrationBuilder.InsertData(
                table: "Addresses",
                columns: new[] { "Id", "AdditionalInfo", "ApartmentNumber", "CityId", "StreetName", "StreetNumber" },
                values: new object[,]
                {
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
                    { 31, null, null, 5, "Đure Cvejića", "8" }
                });

            migrationBuilder.InsertData(
                table: "Clinics",
                columns: new[] { "Id", "AddressId", "CreatedAt", "Email", "IsActive", "Name", "PhoneNumber" },
                values: new object[] { 1, 1, new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "info@klinika-beograd.rs", true, "Klinika Beograd Centar", "011-123-4567" });

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AddressId", "CreatedAt", "Email", "FirstName", "JMBG", "LastName", "PhoneNumber" },
                values: new object[] { 6, new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@klinika.rs", "Marko", "0101990800001", "Administratorović", "060-111-0001" });

            migrationBuilder.UpdateData(
                table: "Workers",
                keyColumn: "Id",
                keyValue: 1,
                column: "ClinicId",
                value: 1);

            migrationBuilder.InsertData(
                table: "Clinics",
                columns: new[] { "Id", "AddressId", "CreatedAt", "Email", "IsActive", "Name", "PhoneNumber" },
                values: new object[,]
                {
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
                    { 26, 31, new DateTime(2024, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1994, 8, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "maja.vukovic@klinika.rs", "Maja", "F", "0508994700026", "Vuković", "060-333-0007" }
                });

            migrationBuilder.InsertData(
                table: "Workers",
                columns: new[] { "Id", "ClinicId", "IsActive", "PasswordHash" },
                values: new object[,]
                {
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

            migrationBuilder.CreateIndex(
                name: "IX_Workers_ClinicId",
                table: "Workers",
                column: "ClinicId");

            migrationBuilder.CreateIndex(
                name: "IX_Clinics_AddressId",
                table: "Clinics",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Clinics_Email",
                table: "Clinics",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Workers_Clinics_ClinicId",
                table: "Workers",
                column: "ClinicId",
                principalTable: "Clinics",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Workers_Clinics_ClinicId",
                table: "Workers");

            migrationBuilder.DropTable(
                name: "Clinics");

            migrationBuilder.DropIndex(
                name: "IX_Workers_ClinicId",
                table: "Workers");

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Administrators",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Administrators",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Secretaries",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Secretaries",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Secretaries",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Secretaries",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Secretaries",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Secretaries",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Secretaries",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Workers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Workers",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Workers",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Workers",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Workers",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Workers",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Workers",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Workers",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Workers",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Workers",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Workers",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Workers",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Workers",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Workers",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Workers",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Workers",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Workers",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Workers",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Workers",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Workers",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Workers",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Workers",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Workers",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Workers",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Workers",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DropColumn(
                name: "ClinicId",
                table: "Workers");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "StreetName", "StreetNumber" },
                values: new object[] { "Admin Street", "1" });

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AddressId", "CreatedAt", "Email", "FirstName", "JMBG", "LastName", "PhoneNumber" },
                values: new object[] { 1, new DateTime(2025, 1, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@clinic.com", "Admin", "0000000000000", "Adminovic", "000-000-0000" });
        }
    }
}
