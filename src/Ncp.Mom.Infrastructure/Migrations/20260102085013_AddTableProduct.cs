using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ncp.Mom.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTableProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_product",
                table: "product");

            migrationBuilder.DropPrimaryKey(
                name: "PK_work_center",
                table: "work_center");

            migrationBuilder.RenameTable(
                name: "product",
                newName: "Product");

            migrationBuilder.RenameTable(
                name: "work_center",
                newName: "WorkCenter");

            migrationBuilder.RenameIndex(
                name: "IX_product_ProductCode",
                table: "Product",
                newName: "IX_Product_ProductCode");

            migrationBuilder.RenameIndex(
                name: "IX_work_center_WorkCenterCode",
                table: "WorkCenter",
                newName: "IX_WorkCenter_WorkCenterCode");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Product",
                table: "Product",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkCenter",
                table: "WorkCenter",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Product_ProductName",
                table: "Product",
                column: "ProductName");

            migrationBuilder.CreateIndex(
                name: "IX_WorkCenter_WorkCenterName",
                table: "WorkCenter",
                column: "WorkCenterName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Product",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_ProductName",
                table: "Product");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkCenter",
                table: "WorkCenter");

            migrationBuilder.DropIndex(
                name: "IX_WorkCenter_WorkCenterName",
                table: "WorkCenter");

            migrationBuilder.RenameTable(
                name: "Product",
                newName: "product");

            migrationBuilder.RenameTable(
                name: "WorkCenter",
                newName: "work_center");

            migrationBuilder.RenameIndex(
                name: "IX_Product_ProductCode",
                table: "product",
                newName: "IX_product_ProductCode");

            migrationBuilder.RenameIndex(
                name: "IX_WorkCenter_WorkCenterCode",
                table: "work_center",
                newName: "IX_work_center_WorkCenterCode");

            migrationBuilder.AddPrimaryKey(
                name: "PK_product",
                table: "product",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_work_center",
                table: "work_center",
                column: "Id");
        }
    }
}
