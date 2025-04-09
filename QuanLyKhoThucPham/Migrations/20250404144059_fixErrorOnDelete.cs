using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyKhoThucPham.Migrations
{
    public partial class fixErrorOnDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhieuNhapChiTiet_SanPham_MaSP",
                table: "PhieuNhapChiTiet");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuXuatChiTiet_SanPham_MaSP",
                table: "PhieuXuatChiTiet");

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuNhapChiTiet_SanPham_MaSP",
                table: "PhieuNhapChiTiet",
                column: "MaSP",
                principalTable: "SanPham",
                principalColumn: "MaSP",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuXuatChiTiet_SanPham_MaSP",
                table: "PhieuXuatChiTiet",
                column: "MaSP",
                principalTable: "SanPham",
                principalColumn: "MaSP",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhieuNhapChiTiet_SanPham_MaSP",
                table: "PhieuNhapChiTiet");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuXuatChiTiet_SanPham_MaSP",
                table: "PhieuXuatChiTiet");

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuNhapChiTiet_SanPham_MaSP",
                table: "PhieuNhapChiTiet",
                column: "MaSP",
                principalTable: "SanPham",
                principalColumn: "MaSP",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuXuatChiTiet_SanPham_MaSP",
                table: "PhieuXuatChiTiet",
                column: "MaSP",
                principalTable: "SanPham",
                principalColumn: "MaSP",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
