using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Content.Server.Database.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class AddHairGradientShapeToProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "facial_hair_gradient_blur",
                table: "profile",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "facial_hair_gradient_position",
                table: "profile",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "hair_gradient_blur",
                table: "profile",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "hair_gradient_position",
                table: "profile",
                type: "REAL",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "facial_hair_gradient_blur",
                table: "profile");

            migrationBuilder.DropColumn(
                name: "facial_hair_gradient_position",
                table: "profile");

            migrationBuilder.DropColumn(
                name: "hair_gradient_blur",
                table: "profile");

            migrationBuilder.DropColumn(
                name: "hair_gradient_position",
                table: "profile");
        }
    }
}
