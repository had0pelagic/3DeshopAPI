using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class imageuploadupdate1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Data",
                table: "Images",
                newName: "Format");

            migrationBuilder.AddColumn<double>(
                name: "Size",
                table: "Images",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Size",
                table: "Images");

            migrationBuilder.RenameColumn(
                name: "Format",
                table: "Images",
                newName: "Data");
        }
    }
}
