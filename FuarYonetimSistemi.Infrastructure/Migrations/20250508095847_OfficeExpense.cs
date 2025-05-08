using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuarYonetimSistemi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OfficeExpense : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OfficeExpenseTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficeExpenseTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OfficeExpenses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OfficeExpenseTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficeExpenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfficeExpenses_OfficeExpenseTypes_OfficeExpenseTypeId",
                        column: x => x.OfficeExpenseTypeId,
                        principalTable: "OfficeExpenseTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OfficeExpenses_OfficeExpenseTypeId",
                table: "OfficeExpenses",
                column: "OfficeExpenseTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OfficeExpenses");

            migrationBuilder.DropTable(
                name: "OfficeExpenseTypes");
        }
    }
}
