using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ncp.Mom.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMomCoreDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "product",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ProductCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProductName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "production_plan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PlanNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProductId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    PlannedStartDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    PlannedEndDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RowVersion = table.Column<int>(type: "int", nullable: false),
                    UpdateTime = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_production_plan", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "routing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RoutingNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProductId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RowVersion = table.Column<int>(type: "int", nullable: false),
                    UpdateTime = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_routing", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "work_center",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    WorkCenterCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    WorkCenterName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_center", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "work_order",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    WorkOrderNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProductionPlanId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ProductId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    CompletedQuantity = table.Column<int>(type: "int", nullable: false),
                    RoutingId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StartTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    EndTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    RowVersion = table.Column<int>(type: "int", nullable: false),
                    UpdateTime = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_order", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "routing_operation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    OperationName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    WorkCenterId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    StandardTime = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RoutingId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_routing_operation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_routing_operation_routing_RoutingId",
                        column: x => x.RoutingId,
                        principalTable: "routing",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_product_ProductCode",
                table: "product",
                column: "ProductCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_production_plan_PlanNumber",
                table: "production_plan",
                column: "PlanNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_routing_ProductId",
                table: "routing",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_routing_RoutingNumber",
                table: "routing",
                column: "RoutingNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_routing_operation_RoutingId",
                table: "routing_operation",
                column: "RoutingId");

            migrationBuilder.CreateIndex(
                name: "IX_work_center_WorkCenterCode",
                table: "work_center",
                column: "WorkCenterCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_work_order_ProductionPlanId",
                table: "work_order",
                column: "ProductionPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_work_order_WorkOrderNumber",
                table: "work_order",
                column: "WorkOrderNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "product");

            migrationBuilder.DropTable(
                name: "production_plan");

            migrationBuilder.DropTable(
                name: "routing_operation");

            migrationBuilder.DropTable(
                name: "work_center");

            migrationBuilder.DropTable(
                name: "work_order");

            migrationBuilder.DropTable(
                name: "routing");
        }
    }
}
