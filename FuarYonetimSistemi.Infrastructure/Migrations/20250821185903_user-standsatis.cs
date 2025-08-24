using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuarYonetimSistemi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class userstandsatis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Stands_FairId",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "SalesRepresentative",
                table: "Stands");

            migrationBuilder.AddColumn<Guid>(
                name: "SalesRepresentativeId",
                table: "Stands",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Role",
                table: "Users",
                column: "Role");

            migrationBuilder.CreateIndex(
                name: "IX_Stands_ActualDueDate",
                table: "Stands",
                column: "ActualDueDate");

            migrationBuilder.CreateIndex(
                name: "IX_Stands_ContractDate",
                table: "Stands",
                column: "ContractDate");

            migrationBuilder.CreateIndex(
                name: "IX_Stands_Fair_SalesRep",
                table: "Stands",
                columns: new[] { "FairId", "SalesRepresentativeId" });

            migrationBuilder.CreateIndex(
                name: "IX_Stands_SalesRep_ContractDate",
                table: "Stands",
                columns: new[] { "SalesRepresentativeId", "ContractDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Stands_SalesRepresentativeId",
                table: "Stands",
                column: "SalesRepresentativeId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentDate",
                table: "Payments",
                column: "PaymentDate");

            migrationBuilder.AddForeignKey(
                name: "FK_Stands_Users_SalesRepresentativeId",
                table: "Stands",
                column: "SalesRepresentativeId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stands_Users_SalesRepresentativeId",
                table: "Stands");

            migrationBuilder.DropIndex(
                name: "IX_Users_Role",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Stands_ActualDueDate",
                table: "Stands");

            migrationBuilder.DropIndex(
                name: "IX_Stands_ContractDate",
                table: "Stands");

            migrationBuilder.DropIndex(
                name: "IX_Stands_Fair_SalesRep",
                table: "Stands");

            migrationBuilder.DropIndex(
                name: "IX_Stands_SalesRep_ContractDate",
                table: "Stands");

            migrationBuilder.DropIndex(
                name: "IX_Stands_SalesRepresentativeId",
                table: "Stands");

            migrationBuilder.DropIndex(
                name: "IX_Payments_PaymentDate",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "SalesRepresentativeId",
                table: "Stands");

            migrationBuilder.AddColumn<string>(
                name: "SalesRepresentative",
                table: "Stands",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Stands_FairId",
                table: "Stands",
                column: "FairId");
        }
    }
}
