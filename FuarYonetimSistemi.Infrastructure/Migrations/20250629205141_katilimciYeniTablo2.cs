using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuarYonetimSistemi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class katilimciYeniTablo2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Branch_Participants_ParticipantId",
                table: "Branch");

            migrationBuilder.DropForeignKey(
                name: "FK_Brand_Participants_ParticipantId",
                table: "Brand");

            migrationBuilder.DropForeignKey(
                name: "FK_ExhibitedProduct_Participants_ParticipantId",
                table: "ExhibitedProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductCategory_Participants_ParticipantId",
                table: "ProductCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_RepresentativeCompany_Participants_ParticipantId",
                table: "RepresentativeCompany");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RepresentativeCompany",
                table: "RepresentativeCompany");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductCategory",
                table: "ProductCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExhibitedProduct",
                table: "ExhibitedProduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Brand",
                table: "Brand");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Branch",
                table: "Branch");

            migrationBuilder.RenameTable(
                name: "RepresentativeCompany",
                newName: "RepresentativeCompanies");

            migrationBuilder.RenameTable(
                name: "ProductCategory",
                newName: "ProductCategories");

            migrationBuilder.RenameTable(
                name: "ExhibitedProduct",
                newName: "ExhibitedProducts");

            migrationBuilder.RenameTable(
                name: "Brand",
                newName: "Brands");

            migrationBuilder.RenameTable(
                name: "Branch",
                newName: "Branches");

            migrationBuilder.RenameIndex(
                name: "IX_RepresentativeCompany_ParticipantId",
                table: "RepresentativeCompanies",
                newName: "IX_RepresentativeCompanies_ParticipantId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductCategory_ParticipantId",
                table: "ProductCategories",
                newName: "IX_ProductCategories_ParticipantId");

            migrationBuilder.RenameIndex(
                name: "IX_ExhibitedProduct_ParticipantId",
                table: "ExhibitedProducts",
                newName: "IX_ExhibitedProducts_ParticipantId");

            migrationBuilder.RenameIndex(
                name: "IX_Brand_ParticipantId",
                table: "Brands",
                newName: "IX_Brands_ParticipantId");

            migrationBuilder.RenameIndex(
                name: "IX_Branch_ParticipantId",
                table: "Branches",
                newName: "IX_Branches_ParticipantId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RepresentativeCompanies",
                table: "RepresentativeCompanies",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductCategories",
                table: "ProductCategories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExhibitedProducts",
                table: "ExhibitedProducts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Brands",
                table: "Brands",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Branches",
                table: "Branches",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_Participants_ParticipantId",
                table: "Branches",
                column: "ParticipantId",
                principalTable: "Participants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Brands_Participants_ParticipantId",
                table: "Brands",
                column: "ParticipantId",
                principalTable: "Participants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExhibitedProducts_Participants_ParticipantId",
                table: "ExhibitedProducts",
                column: "ParticipantId",
                principalTable: "Participants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCategories_Participants_ParticipantId",
                table: "ProductCategories",
                column: "ParticipantId",
                principalTable: "Participants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RepresentativeCompanies_Participants_ParticipantId",
                table: "RepresentativeCompanies",
                column: "ParticipantId",
                principalTable: "Participants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Branches_Participants_ParticipantId",
                table: "Branches");

            migrationBuilder.DropForeignKey(
                name: "FK_Brands_Participants_ParticipantId",
                table: "Brands");

            migrationBuilder.DropForeignKey(
                name: "FK_ExhibitedProducts_Participants_ParticipantId",
                table: "ExhibitedProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductCategories_Participants_ParticipantId",
                table: "ProductCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_RepresentativeCompanies_Participants_ParticipantId",
                table: "RepresentativeCompanies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RepresentativeCompanies",
                table: "RepresentativeCompanies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductCategories",
                table: "ProductCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExhibitedProducts",
                table: "ExhibitedProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Brands",
                table: "Brands");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Branches",
                table: "Branches");

            migrationBuilder.RenameTable(
                name: "RepresentativeCompanies",
                newName: "RepresentativeCompany");

            migrationBuilder.RenameTable(
                name: "ProductCategories",
                newName: "ProductCategory");

            migrationBuilder.RenameTable(
                name: "ExhibitedProducts",
                newName: "ExhibitedProduct");

            migrationBuilder.RenameTable(
                name: "Brands",
                newName: "Brand");

            migrationBuilder.RenameTable(
                name: "Branches",
                newName: "Branch");

            migrationBuilder.RenameIndex(
                name: "IX_RepresentativeCompanies_ParticipantId",
                table: "RepresentativeCompany",
                newName: "IX_RepresentativeCompany_ParticipantId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductCategories_ParticipantId",
                table: "ProductCategory",
                newName: "IX_ProductCategory_ParticipantId");

            migrationBuilder.RenameIndex(
                name: "IX_ExhibitedProducts_ParticipantId",
                table: "ExhibitedProduct",
                newName: "IX_ExhibitedProduct_ParticipantId");

            migrationBuilder.RenameIndex(
                name: "IX_Brands_ParticipantId",
                table: "Brand",
                newName: "IX_Brand_ParticipantId");

            migrationBuilder.RenameIndex(
                name: "IX_Branches_ParticipantId",
                table: "Branch",
                newName: "IX_Branch_ParticipantId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RepresentativeCompany",
                table: "RepresentativeCompany",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductCategory",
                table: "ProductCategory",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExhibitedProduct",
                table: "ExhibitedProduct",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Brand",
                table: "Brand",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Branch",
                table: "Branch",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Branch_Participants_ParticipantId",
                table: "Branch",
                column: "ParticipantId",
                principalTable: "Participants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Brand_Participants_ParticipantId",
                table: "Brand",
                column: "ParticipantId",
                principalTable: "Participants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExhibitedProduct_Participants_ParticipantId",
                table: "ExhibitedProduct",
                column: "ParticipantId",
                principalTable: "Participants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCategory_Participants_ParticipantId",
                table: "ProductCategory",
                column: "ParticipantId",
                principalTable: "Participants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RepresentativeCompany_Participants_ParticipantId",
                table: "RepresentativeCompany",
                column: "ParticipantId",
                principalTable: "Participants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
