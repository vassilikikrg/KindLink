using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VolunteeringApp.Migrations
{
    /// <inheritdoc />
    public partial class RoleSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0baefd1f-3140-4da8-816a-f4bedab3ef71", "1", "Organization", "ORGANIZATION" },
                    { "33a14a6c-9c12-48cc-9d94-e6afec10ff49", "2", "Citizen", "CITIZEN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0baefd1f-3140-4da8-816a-f4bedab3ef71");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "33a14a6c-9c12-48cc-9d94-e6afec10ff49");
        }
    }
}
