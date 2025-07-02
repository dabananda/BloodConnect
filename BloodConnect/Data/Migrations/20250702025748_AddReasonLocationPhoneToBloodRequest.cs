using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BloodConnect.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddReasonLocationPhoneToBloodRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "BloodRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "BloodRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RequestorPhone",
                table: "BloodRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "BloodRequests");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "BloodRequests");

            migrationBuilder.DropColumn(
                name: "RequestorPhone",
                table: "BloodRequests");
        }
    }
}
