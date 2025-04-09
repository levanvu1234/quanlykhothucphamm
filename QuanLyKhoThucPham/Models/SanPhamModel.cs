using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKhoThucPham.Models
{
    public class SanPhamModel
    {
        [Key]
        public int MaSP { get; set; }
        public string TenSP { get; set; }
        public int MaKho { get; set; }

        public int SoLuong { get; set; }
        public decimal DonGiaNhap { get; set; }
        public decimal DonGiaXuat { get; set; }

        public string NhaSanXuat { get; set; }
        public string? MoTa { get; set; }

        [ForeignKey("MaKho")]
        public KhoHangModel? KhoHang { get; set; }

        public ICollection<PhieuXuatChiTietModel>? PhieuXuatChiTiets { get; set; }
        public ICollection<PhieuNhapChiTietModel>? PhieuNhapChiTiets { get; set; }

    }
}
