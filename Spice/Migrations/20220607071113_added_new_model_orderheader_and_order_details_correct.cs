using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Spice.Migrations
{
    public partial class added_new_model_orderheader_and_order_details_correct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderHeaderTb",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    OrderDate = table.Column<DateTime>(nullable: false),
                    OrderTotalOrginal = table.Column<double>(nullable: false),
                    OrderTotal = table.Column<double>(nullable: false),
                    PickupTime = table.Column<DateTime>(nullable: false),
                    CouponCode = table.Column<string>(nullable: true),
                    Discount = table.Column<double>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    PaymentStatus = table.Column<string>(nullable: true),
                    Comments = table.Column<string>(nullable: true),
                    PickUpName = table.Column<string>(nullable: true),
                    PickupNumber = table.Column<string>(nullable: true),
                    TransactionId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderHeaderTb", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderHeaderTb_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetailsTb",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(nullable: false),
                    MenuItemId = table.Column<int>(nullable: false),
                    Count = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Price = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetailsTb", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDetailsTb_MenuItemTb_MenuItemId",
                        column: x => x.MenuItemId,
                        principalTable: "MenuItemTb",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetailsTb_OrderHeaderTb_OrderId",
                        column: x => x.OrderId,
                        principalTable: "OrderHeaderTb",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetailsTb_MenuItemId",
                table: "OrderDetailsTb",
                column: "MenuItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetailsTb_OrderId",
                table: "OrderDetailsTb",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderHeaderTb_UserId",
                table: "OrderHeaderTb",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderDetailsTb");

            migrationBuilder.DropTable(
                name: "OrderHeaderTb");
        }
    }
}
