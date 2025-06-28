using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BloodConnect.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsSuspendedToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSuspended",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSuspended",
                table: "AspNetUsers");
        }
    }
}
