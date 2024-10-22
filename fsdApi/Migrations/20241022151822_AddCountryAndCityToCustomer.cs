using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fsdApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCountryAndCityToCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Customer",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Customer",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Customer");
        }
    }
}
