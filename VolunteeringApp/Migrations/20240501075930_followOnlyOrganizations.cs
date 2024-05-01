using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VolunteeringApp.Migrations
{
    /// <inheritdoc />
    public partial class followOnlyOrganizations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FollowRelationships_AspNetUsers_FollowedId",
                table: "FollowRelationships");

            migrationBuilder.AddForeignKey(
                name: "FK_FollowRelationships_Organizations_FollowedId",
                table: "FollowRelationships",
                column: "FollowedId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FollowRelationships_Organizations_FollowedId",
                table: "FollowRelationships");

            migrationBuilder.AddForeignKey(
                name: "FK_FollowRelationships_AspNetUsers_FollowedId",
                table: "FollowRelationships",
                column: "FollowedId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
