using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKhoThucPham.Models
{
    public class PhieuXuatChiTietModel
    {
        [Key]
        public int MaPhieuXuatChiTiet { get; set; }
        public int MaSP { get; set; }
        public int MaPhieuXuat { get; set; }
        public int SoLuong { get; set; }

        public decimal DonGia { get; set; }

        public decimal TongTIen { get; set; }

        public string? GhiChu { get; set; }

        [ForeignKey("MaSP")]        
        public virtual SanPhamModel? SanPham { get; set; }
        

        [ForeignKey("MaPhieuXuat")]
        public virtual PhieuXuatModel? PhieuXuat { get; set; }
        
        

    }
}
