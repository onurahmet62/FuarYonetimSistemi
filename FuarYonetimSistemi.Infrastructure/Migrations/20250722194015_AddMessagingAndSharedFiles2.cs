using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuarYonetimSistemi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMessagingAndSharedFiles2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SharedFiles_Users_UploadedByUserId",
                table: "SharedFiles");

            migrationBuilder.DropIndex(
                name: "IX_SharedFiles_UploadedByUserId",
                table: "SharedFiles");

            migrationBuilder.DropColumn(
                name: "UploadedByUserId",
                table: "SharedFiles");

            migrationBuilder.RenameColumn(
                name: "UploadedAt",
                table: "SharedFiles",
                newName: "CreatedAt");

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "SharedFiles",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "SharedFiles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Messages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "SharedFiles");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "SharedFiles",
                newName: "UploadedAt");

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "SharedFiles",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "UploadedByUserId",
                table: "SharedFiles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_SharedFiles_UploadedByUserId",
                table: "SharedFiles",
                column: "UploadedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SharedFiles_Users_UploadedByUserId",
                table: "SharedFiles",
                column: "UploadedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
