using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Content.Server.Database.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class AddGradientToProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "facial_hair_color2",
                table: "profile",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "facial_hair_use_gradient",
                table: "profile",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "hair_color2",
                table: "profile",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "hair_use_gradient",
                table: "profile",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "facial_hair_color2",
                table: "profile");

            migrationBuilder.DropColumn(
                name: "facial_hair_use_gradient",
                table: "profile");

            migrationBuilder.DropColumn(
                name: "hair_color2",
                table: "profile");

            migrationBuilder.DropColumn(
                name: "hair_use_gradient",
                table: "profile");
        }
    }
}
