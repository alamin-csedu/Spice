using Microsoft.EntityFrameworkCore.Migrations;

namespace Spice.Migrations
{
    public partial class AddMenuItemTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MenuItemDb",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Spicyness = table.Column<string>(nullable: true),
                    Image = table.Column<string>(nullable: true),
                    CategoryId = table.Column<int>(nullable: false),
                    SubCategoryId = table.Column<int>(nullable: false),
                    Price = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItemDb", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuItemDb_CategoryTb_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "CategoryTb",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_MenuItemDb_SubCategoryTb_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalTable: "SubCategoryTb",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MenuItemDb_CategoryId",
                table: "MenuItemDb",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItemDb_SubCategoryId",
                table: "MenuItemDb",
                column: "SubCategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuItemDb");
        }
    }
}
