using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuarYonetimSistemi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFairExpensesRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FairExpenseTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FairExpenseTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FairExpenses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FairId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FairExpenseTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AnnualTarget = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AnnualActual = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurrentTarget = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurrentActual = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RealizedExpense = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    FairExpenseTypeId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FairExpenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FairExpenses_FairExpenseTypes_FairExpenseTypeId",
                        column: x => x.FairExpenseTypeId,
                        principalTable: "FairExpenseTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FairExpenses_FairExpenseTypes_FairExpenseTypeId1",
                        column: x => x.FairExpenseTypeId1,
                        principalTable: "FairExpenseTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FairExpenses_Fairs_FairId",
                        column: x => x.FairId,
                        principalTable: "Fairs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FairExpenses_FairExpenseTypeId",
                table: "FairExpenses",
                column: "FairExpenseTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FairExpenses_FairExpenseTypeId1",
                table: "FairExpenses",
                column: "FairExpenseTypeId1");

            migrationBuilder.CreateIndex(
                name: "IX_FairExpenses_FairId",
                table: "FairExpenses",
                column: "FairId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FairExpenses");

            migrationBuilder.DropTable(
                name: "FairExpenseTypes");
        }
    }
}
