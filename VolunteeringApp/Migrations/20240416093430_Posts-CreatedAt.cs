using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VolunteeringApp.Migrations
{
    /// <inheritdoc />
    public partial class PostsCreatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a3c04081-c36b-44e3-b507-aebc3e533e85");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "da89485d-9c8c-403d-b095-b4788d392200");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Posts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "7fae40a4-1a28-4536-85c1-88a390297583", "2", "Citizen", "CITIZEN" },
                    { "e9b2087c-0ad7-4b9a-9bff-4198c5e252d8", "1", "Organization", "ORGANIZATION" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7fae40a4-1a28-4536-85c1-88a390297583");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e9b2087c-0ad7-4b9a-9bff-4198c5e252d8");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Posts");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "a3c04081-c36b-44e3-b507-aebc3e533e85", "1", "Organization", "ORGANIZATION" },
                    { "da89485d-9c8c-403d-b095-b4788d392200", "2", "Citizen", "CITIZEN" }
                });
        }
    }
}
