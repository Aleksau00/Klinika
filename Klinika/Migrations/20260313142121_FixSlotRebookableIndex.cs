using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Klinika.Migrations
{
    /// <inheritdoc />
    public partial class FixSlotRebookableIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the hard unique index that prevents re-booking a slot
            // after its previous appointment was Cancelled or NoShow.
            migrationBuilder.DropIndex(
                name: "IX_Appointments_AppointmentSlotId",
                table: "Appointments");

            // Create a filtered unique index so the same slot can only be
            // booked once among active (non-cancelled, non-noshow) appointments.
            // Status values: Scheduled=0, InProgress=1, Completed=2, Cancelled=3, NoShow=4
            // WHERE Status < 3 excludes Cancelled and NoShow, allowing those slots to be re-booked.
            migrationBuilder.Sql(
                "SET QUOTED_IDENTIFIER ON; SET ANSI_NULLS ON; " +
                "CREATE UNIQUE INDEX [IX_Appointments_AppointmentSlotId_Active] " +
                "ON [Appointments] ([AppointmentSlotId]) " +
                "WHERE [Status] < 3;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DROP INDEX IF EXISTS [IX_Appointments_AppointmentSlotId_Active] ON [Appointments];");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_AppointmentSlotId",
                table: "Appointments",
                column: "AppointmentSlotId",
                unique: true);
        }
    }
}
