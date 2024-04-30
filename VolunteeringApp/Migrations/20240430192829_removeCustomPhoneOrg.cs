﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VolunteeringApp.Migrations
{
    /// <inheritdoc />
    public partial class removeCustomPhoneOrg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Organizations");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Organizations",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
