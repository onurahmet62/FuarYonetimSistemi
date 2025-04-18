using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuarYonetimSistemi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class stand : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stands_Participants_ParticipantId",
                table: "Stands");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "StandReservations");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Stands");

            migrationBuilder.RenameColumn(
                name: "MinimumPrice",
                table: "Stands",
                newName: "AmountRemaining");

            migrationBuilder.RenameColumn(
                name: "Discount",
                table: "Stands",
                newName: "AmountPaid");

            migrationBuilder.RenameColumn(
                name: "TotalStandCount",
                table: "Fairs",
                newName: "StandCount");

            migrationBuilder.AlterColumn<Guid>(
                name: "ParticipantId",
                table: "Stands",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Area",
                table: "Stands",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Stands",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "Stands",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "Stands",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Stands_Participants_ParticipantId",
                table: "Stands",
                column: "ParticipantId",
                principalTable: "Participants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stands_Participants_ParticipantId",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "Area",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Stands");

            migrationBuilder.RenameColumn(
                name: "AmountRemaining",
                table: "Stands",
                newName: "MinimumPrice");

            migrationBuilder.RenameColumn(
                name: "AmountPaid",
                table: "Stands",
                newName: "Discount");

            migrationBuilder.RenameColumn(
                name: "StandCount",
                table: "Fairs",
                newName: "TotalStandCount");

            migrationBuilder.AlterColumn<Guid>(
                name: "ParticipantId",
                table: "Stands",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Stands",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Stands",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "StandReservations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParticipantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StandId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AmountRemaining = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FairId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PaymentStatus = table.Column<int>(type: "int", nullable: false),
                    ReservationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StandReservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StandReservations_Fairs_FairId",
                        column: x => x.FairId,
                        principalTable: "Fairs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StandReservations_Participants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Participants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StandReservations_Stands_StandId",
                        column: x => x.StandId,
                        principalTable: "Stands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StandReservationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_StandReservations_StandReservationId",
                        column: x => x.StandReservationId,
                        principalTable: "StandReservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_StandReservationId",
                table: "Payments",
                column: "StandReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_StandReservations_FairId",
                table: "StandReservations",
                column: "FairId");

            migrationBuilder.CreateIndex(
                name: "IX_StandReservations_ParticipantId",
                table: "StandReservations",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_StandReservations_StandId",
                table: "StandReservations",
                column: "StandId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stands_Participants_ParticipantId",
                table: "Stands",
                column: "ParticipantId",
                principalTable: "Participants",
                principalColumn: "Id");
        }
    }
}
