using Microsoft.EntityFrameworkCore.Migrations;

namespace Spice.Migrations
{
    public partial class changenameoftable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItemDb_CategoryTb_CategoryId",
                table: "MenuItemDb");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuItemDb_SubCategoryTb_SubCategoryId",
                table: "MenuItemDb");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MenuItemDb",
                table: "MenuItemDb");

            migrationBuilder.RenameTable(
                name: "MenuItemDb",
                newName: "MenuItemTb");

            migrationBuilder.RenameIndex(
                name: "IX_MenuItemDb_SubCategoryId",
                table: "MenuItemTb",
                newName: "IX_MenuItemTb_SubCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_MenuItemDb_CategoryId",
                table: "MenuItemTb",
                newName: "IX_MenuItemTb_CategoryId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MenuItemTb",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MenuItemTb",
                table: "MenuItemTb",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItemTb_CategoryTb_CategoryId",
                table: "MenuItemTb",
                column: "CategoryId",
                principalTable: "CategoryTb",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItemTb_SubCategoryTb_SubCategoryId",
                table: "MenuItemTb",
                column: "SubCategoryId",
                principalTable: "SubCategoryTb",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItemTb_CategoryTb_CategoryId",
                table: "MenuItemTb");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuItemTb_SubCategoryTb_SubCategoryId",
                table: "MenuItemTb");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MenuItemTb",
                table: "MenuItemTb");

            migrationBuilder.RenameTable(
                name: "MenuItemTb",
                newName: "MenuItemDb");

            migrationBuilder.RenameIndex(
                name: "IX_MenuItemTb_SubCategoryId",
                table: "MenuItemDb",
                newName: "IX_MenuItemDb_SubCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_MenuItemTb_CategoryId",
                table: "MenuItemDb",
                newName: "IX_MenuItemDb_CategoryId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MenuItemDb",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddPrimaryKey(
                name: "PK_MenuItemDb",
                table: "MenuItemDb",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItemDb_CategoryTb_CategoryId",
                table: "MenuItemDb",
                column: "CategoryId",
                principalTable: "CategoryTb",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItemDb_SubCategoryTb_SubCategoryId",
                table: "MenuItemDb",
                column: "SubCategoryId",
                principalTable: "SubCategoryTb",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
