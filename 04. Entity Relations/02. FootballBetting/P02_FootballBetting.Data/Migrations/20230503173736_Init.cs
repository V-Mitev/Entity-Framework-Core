using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace P02_FootballBetting.Data.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Positions_Positions_PositionId1",
                table: "Positions");

            migrationBuilder.DropIndex(
                name: "IX_Positions_PositionId1",
                table: "Positions");

            migrationBuilder.DropColumn(
                name: "PositionId1",
                table: "Positions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PositionId1",
                table: "Positions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Positions_PositionId1",
                table: "Positions",
                column: "PositionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Positions_Positions_PositionId1",
                table: "Positions",
                column: "PositionId1",
                principalTable: "Positions",
                principalColumn: "PositionId");
        }
    }
}
