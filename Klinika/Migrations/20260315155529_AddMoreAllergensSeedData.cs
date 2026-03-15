using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Klinika.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreAllergensSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Allergens",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 10, "Alergija na gluten i pšenicu (celijakija i netolerancija pšenice)", "Gluten" },
                    { 11, "Alergija na jaja i proizvode koji sadrže jaja", "Jaja" },
                    { 12, "Alergija na proteine kravljeg mleka (IgE-posredovana)", "Kravlje mleko" },
                    { 13, "Alergija na soju i proizvode sa sojom", "Soja" },
                    { 14, "Alergija na orašaste plodove (orah, lešnik, badem, indijski orah)", "Orašasti plodovi" },
                    { 15, "Alergija na ribu i riblje proizvode", "Riba" },
                    { 16, "Alergija na školjke i plodove mora", "Školjke" },
                    { 17, "Alergija na sezam i sezamovo ulje", "Sezam" },
                    { 18, "Alergija na ubod pčele ili ose (himenoptera venom alergija)", "Pčelinji otrov" },
                    { 19, "Alergija na amoksicilin i aminopenicilinske antibiotike", "Amoksicilin" },
                    { 20, "Alergija na lateks i prirodne gumene proizvode", "Lateks" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Allergens",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Allergens",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Allergens",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Allergens",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Allergens",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Allergens",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Allergens",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Allergens",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Allergens",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Allergens",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Allergens",
                keyColumn: "Id",
                keyValue: 20);
        }
    }
}
