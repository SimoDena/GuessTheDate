using Microsoft.EntityFrameworkCore.Migrations;

namespace GuessTheDate.Migrations
{
    public partial class AppUserUpdateMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PointsNextLevel",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PointsNextLevel",
                table: "AspNetUsers");
        }
    }
}
