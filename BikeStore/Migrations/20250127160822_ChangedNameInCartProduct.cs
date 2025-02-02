using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BikeStore.Migrations
{
    /// <inheritdoc />
    public partial class ChangedNameInCartProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Product",
                table: "Cart",
                newName: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "Cart",
                newName: "Product");
        }
    }
}
