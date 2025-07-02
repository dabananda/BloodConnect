using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BloodConnect.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedCurrentAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurrentAddress",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentAddress",
                table: "AspNetUsers");
        }
    }
}
