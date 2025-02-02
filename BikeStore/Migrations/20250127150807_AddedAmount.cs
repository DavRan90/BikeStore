using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BikeStore.Migrations
{
    /// <inheritdoc />
    public partial class AddedAmount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Products",
                table: "Cart",
                newName: "Product");

            migrationBuilder.AddColumn<int>(
                name: "Amount",
                table: "Cart",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Cart");

            migrationBuilder.RenameColumn(
                name: "Product",
                table: "Cart",
                newName: "Products");
        }
    }
}
