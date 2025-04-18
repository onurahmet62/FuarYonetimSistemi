using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuarYonetimSistemi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryToFair : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxStandCount",
                table: "Fairs");

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "Fairs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Fairs_CategoryId",
                table: "Fairs",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Fairs_Categories_CategoryId",
                table: "Fairs",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fairs_Categories_CategoryId",
                table: "Fairs");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Fairs_CategoryId",
                table: "Fairs");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Fairs");

            migrationBuilder.AddColumn<int>(
                name: "MaxStandCount",
                table: "Fairs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
