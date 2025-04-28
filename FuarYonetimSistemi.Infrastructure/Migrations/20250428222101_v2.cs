using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuarYonetimSistemi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fairs_Categories_CategoryId",
                table: "Fairs");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Stands",
                newName: "VAT20Amount");

            migrationBuilder.RenameColumn(
                name: "PaymentStatus",
                table: "Stands",
                newName: "SalesRepresentative");

            migrationBuilder.RenameColumn(
                name: "DueDate",
                table: "Stands",
                newName: "ActualDueDate");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Stands",
                newName: "Note");

            migrationBuilder.RenameColumn(
                name: "Area",
                table: "Stands",
                newName: "ContractArea");

            migrationBuilder.RenameColumn(
                name: "AmountRemaining",
                table: "Stands",
                newName: "VAT10Amount");

            migrationBuilder.RenameColumn(
                name: "AmountPaid",
                table: "Stands",
                newName: "UnitPrice");

            migrationBuilder.AddColumn<decimal>(
                name: "AdvertisingIncome",
                table: "Stands",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<double>(
                name: "AreaExchange",
                table: "Stands",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "AreaSold",
                table: "Stands",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "Stands",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BarterAmount",
                table: "Stands",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BarterBalance",
                table: "Stands",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BarterInvoiceAmount",
                table: "Stands",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CashCollection",
                table: "Stands",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CollectibleBalance",
                table: "Stands",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ContractAmountWithoutVAT",
                table: "Stands",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "ContractDate",
                table: "Stands",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "DocumentCollection",
                table: "Stands",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ElectricityConnectionFee",
                table: "Stands",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ReceivablesInLaw",
                table: "Stands",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SaleAmountWithoutVAT",
                table: "Stands",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SolidWasteFee",
                table: "Stands",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "StampTaxAmount",
                table: "Stands",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "StandSetupIncome",
                table: "Stands",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ThirdPartyInsuranceShare",
                table: "Stands",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmountWithVAT",
                table: "Stands",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalReturnInvoice",
                table: "Stands",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PaymentDescription",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Participants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Branches",
                table: "Participants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "Participants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumbers",
                table: "Participants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Participants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "ActualExpense",
                table: "Fairs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ActualNetProfit",
                table: "Fairs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ActualRevenue",
                table: "Fairs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Budget",
                table: "Fairs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId1",
                table: "Fairs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Fairs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "ExpenseTarget",
                table: "Fairs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "FairType",
                table: "Fairs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ForeignParticipantCount",
                table: "Fairs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ForeignVisitorCount",
                table: "Fairs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "NetProfitTarget",
                table: "Fairs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ParticipatingCountries",
                table: "Fairs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<decimal>(
                name: "RevenueTarget",
                table: "Fairs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Sponsors",
                table: "Fairs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<int>(
                name: "TotalParticipantCount",
                table: "Fairs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "TotalStandArea",
                table: "Fairs",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "TotalVisitorCount",
                table: "Fairs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Fairs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Fairs_CategoryId1",
                table: "Fairs",
                column: "CategoryId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Fairs_Categories_CategoryId",
                table: "Fairs",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Fairs_Categories_CategoryId1",
                table: "Fairs",
                column: "CategoryId1",
                principalTable: "Categories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fairs_Categories_CategoryId",
                table: "Fairs");

            migrationBuilder.DropForeignKey(
                name: "FK_Fairs_Categories_CategoryId1",
                table: "Fairs");

            migrationBuilder.DropIndex(
                name: "IX_Fairs_CategoryId1",
                table: "Fairs");

            migrationBuilder.DropColumn(
                name: "AdvertisingIncome",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "AreaExchange",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "AreaSold",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "Balance",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "BarterAmount",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "BarterBalance",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "BarterInvoiceAmount",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "CashCollection",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "CollectibleBalance",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "ContractAmountWithoutVAT",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "ContractDate",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "DocumentCollection",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "ElectricityConnectionFee",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "ReceivablesInLaw",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "SaleAmountWithoutVAT",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "SolidWasteFee",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "StampTaxAmount",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "StandSetupIncome",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "ThirdPartyInsuranceShare",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "TotalAmountWithVAT",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "TotalReturnInvoice",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "PaymentDescription",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "Branches",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "PhoneNumbers",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "ActualExpense",
                table: "Fairs");

            migrationBuilder.DropColumn(
                name: "ActualNetProfit",
                table: "Fairs");

            migrationBuilder.DropColumn(
                name: "ActualRevenue",
                table: "Fairs");

            migrationBuilder.DropColumn(
                name: "Budget",
                table: "Fairs");

            migrationBuilder.DropColumn(
                name: "CategoryId1",
                table: "Fairs");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Fairs");

            migrationBuilder.DropColumn(
                name: "ExpenseTarget",
                table: "Fairs");

            migrationBuilder.DropColumn(
                name: "FairType",
                table: "Fairs");

            migrationBuilder.DropColumn(
                name: "ForeignParticipantCount",
                table: "Fairs");

            migrationBuilder.DropColumn(
                name: "ForeignVisitorCount",
                table: "Fairs");

            migrationBuilder.DropColumn(
                name: "NetProfitTarget",
                table: "Fairs");

            migrationBuilder.DropColumn(
                name: "ParticipatingCountries",
                table: "Fairs");

            migrationBuilder.DropColumn(
                name: "RevenueTarget",
                table: "Fairs");

            migrationBuilder.DropColumn(
                name: "Sponsors",
                table: "Fairs");

            migrationBuilder.DropColumn(
                name: "TotalParticipantCount",
                table: "Fairs");

            migrationBuilder.DropColumn(
                name: "TotalStandArea",
                table: "Fairs");

            migrationBuilder.DropColumn(
                name: "TotalVisitorCount",
                table: "Fairs");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "Fairs");

            migrationBuilder.RenameColumn(
                name: "VAT20Amount",
                table: "Stands",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "VAT10Amount",
                table: "Stands",
                newName: "AmountRemaining");

            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "Stands",
                newName: "AmountPaid");

            migrationBuilder.RenameColumn(
                name: "SalesRepresentative",
                table: "Stands",
                newName: "PaymentStatus");

            migrationBuilder.RenameColumn(
                name: "Note",
                table: "Stands",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "ContractArea",
                table: "Stands",
                newName: "Area");

            migrationBuilder.RenameColumn(
                name: "ActualDueDate",
                table: "Stands",
                newName: "DueDate");

            migrationBuilder.AddForeignKey(
                name: "FK_Fairs_Categories_CategoryId",
                table: "Fairs",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
