using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_CuaHangCafe.Migrations
{
    /// <inheritdoc />
    public partial class AddGhiChuToChiTietHoaDonBan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GiaTang",
                table: "tbSize");

            migrationBuilder.AlterColumn<string>(
                name: "TenSize",
                table: "tbSize",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<decimal>(
                name: "HeSoGia",
                table: "tbSize",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "GiaTang",
                table: "tbSanPhamSize",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GhiChu",
                table: "tbChiTietHoaDonBan",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Size",
                table: "tbChiTietHoaDonBan",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeSoGia",
                table: "tbSize");

            migrationBuilder.DropColumn(
                name: "GiaTang",
                table: "tbSanPhamSize");

            migrationBuilder.DropColumn(
                name: "GhiChu",
                table: "tbChiTietHoaDonBan");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "tbChiTietHoaDonBan");

            migrationBuilder.AlterColumn<string>(
                name: "TenSize",
                table: "tbSize",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<decimal>(
                name: "GiaTang",
                table: "tbSize",
                type: "money",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
