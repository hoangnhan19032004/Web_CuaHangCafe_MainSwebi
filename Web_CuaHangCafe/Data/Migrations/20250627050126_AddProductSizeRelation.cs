using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_CuaHangCafe.Migrations
{
    /// <inheritdoc />
    public partial class AddProductSizeRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaPhanHoi",
                table: "tbPhanHoi");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NgayDang",
                table: "tbTinTuc",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SoDienThoai",
                table: "tbPhanHoi",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NoiDung",
                table: "tbPhanHoi",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "tbPhanHoi",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayPhanHoi",
                table: "tbPhanHoi",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_tbPhanHoi",
                table: "tbPhanHoi",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "tbSize",
                columns: table => new
                {
                    MaSize = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenSize = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GiaTang = table.Column<decimal>(type: "money", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbSize", x => x.MaSize);
                });

            migrationBuilder.CreateTable(
                name: "tbSanPhamSize",
                columns: table => new
                {
                    MaSanPham = table.Column<int>(type: "int", nullable: false),
                    MaSize = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbSanPhamSize", x => new { x.MaSanPham, x.MaSize });
                    table.ForeignKey(
                        name: "FK_tbSanPhamSize_tbSanPham_MaSanPham",
                        column: x => x.MaSanPham,
                        principalTable: "tbSanPham",
                        principalColumn: "MaSanPham",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbSanPhamSize_tbSize_MaSize",
                        column: x => x.MaSize,
                        principalTable: "tbSize",
                        principalColumn: "MaSize",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbSanPhamSize_MaSize",
                table: "tbSanPhamSize",
                column: "MaSize");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbSanPhamSize");

            migrationBuilder.DropTable(
                name: "tbSize");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tbPhanHoi",
                table: "tbPhanHoi");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "tbPhanHoi");

            migrationBuilder.DropColumn(
                name: "NgayPhanHoi",
                table: "tbPhanHoi");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "NgayDang",
                table: "tbTinTuc",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SoDienThoai",
                table: "tbPhanHoi",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "NoiDung",
                table: "tbPhanHoi",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "MaPhanHoi",
                table: "tbPhanHoi",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");
        }
    }
}
