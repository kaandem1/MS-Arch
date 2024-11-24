using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeviceMS.Migrations
{
    public partial class MakeUserIdNullableInDevice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_PersonReferences_UserId",
                table: "Devices");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Devices",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_PersonReferences_UserId",
                table: "Devices",
                column: "UserId",
                principalTable: "PersonReferences",
                principalColumn: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_PersonReferences_UserId",
                table: "Devices");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Devices",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_PersonReferences_UserId",
                table: "Devices",
                column: "UserId",
                principalTable: "PersonReferences",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
