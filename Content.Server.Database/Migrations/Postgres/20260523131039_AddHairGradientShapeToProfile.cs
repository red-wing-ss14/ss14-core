using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Content.Server.Database.Migrations.Postgres
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
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "facial_hair_gradient_position",
                table: "profile",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "hair_gradient_blur",
                table: "profile",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "hair_gradient_position",
                table: "profile",
                type: "real",
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
