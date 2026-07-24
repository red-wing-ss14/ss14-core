using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Content.Server.Database.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class RemoveNSFW : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "nsfwflavor_text",
                table: "profile");

            migrationBuilder.DropColumn(
                name: "nsfwlinks_flavor_text",
                table: "profile");

            migrationBuilder.DropColumn(
                name: "nsfwoocflavor_text",
                table: "profile");

            migrationBuilder.DropColumn(
                name: "nsfwtags_flavor_text",
                table: "profile");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "nsfwflavor_text",
                table: "profile",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "nsfwlinks_flavor_text",
                table: "profile",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "nsfwoocflavor_text",
                table: "profile",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "nsfwtags_flavor_text",
                table: "profile",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
