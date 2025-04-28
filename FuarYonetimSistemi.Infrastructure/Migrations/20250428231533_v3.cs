using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuarYonetimSistemi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class v3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Participants_ParticipantId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_ParticipantId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "ParticipantId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Sponsors",
                table: "Fairs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ParticipantId",
                table: "Payments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Sponsors",
                table: "Fairs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ParticipantId",
                table: "Payments",
                column: "ParticipantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Participants_ParticipantId",
                table: "Payments",
                column: "ParticipantId",
                principalTable: "Participants",
                principalColumn: "Id");
        }
    }
}
