﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuarYonetimSistemi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class paymentIsdeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Stands",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Payments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Payments");
        }
    }
}
