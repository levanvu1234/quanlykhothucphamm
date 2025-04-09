using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKhoThucPham.Models;
public class PhieuNhapModel
{

    [Key]
    public int MaPhieuNhap { get; set; }

    public DateTime NgayNhap { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn nhân viên")]
    
    public int MaNhanVien { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn kho nhập")]
    
    public int MaKho { get; set; }

    public int MaNhaCungCap { get; set; }

    public decimal TongTien { get; set; }

    public string? Ghichu { get; set; }

    [ForeignKey("MaNhaCungCap")]
    public NhaCungCapModel? NhaCungCap { get; set; }
    [ForeignKey("MaNhanVien")]
    public NhanVienModel? NhanVien { get; set; }
    [ForeignKey("MaKho")]
    public KhoHangModel? KhoHang { get; set; }

    public ICollection<PhieuNhapChiTietModel>? DSChiTietPhieuNhap { get; set; }
}
