
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace QuanLyKhoThucPham.Models;

public class PhieuNhapChiTietModel
{
    [Key]
    [Required]
    public int MaPhieuNhapChiTiet { get; set; }
    public int MaSP { get; set; }

    public int MaPhieuNhap { get; set; }
    public int SoLuong { get; set; }
    public decimal DonGia { get; set; }
    public decimal? TongTIen { get; set; }
    [ForeignKey("MaSP")]
    public virtual SanPhamModel? SanPham { get; set; }
    [ForeignKey("MaPhieuNhap")]
    public virtual PhieuNhapModel? PhieuNhap { get; set; }
    
    
}
