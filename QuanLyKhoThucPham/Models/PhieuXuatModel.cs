using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKhoThucPham.Models
{
    public class PhieuXuatModel
    {
        [Key]
        public int MaPhieuXuat { get; set; }

        public DateTime NgayXuat { get; set; }

        public int MaNhanVien { get; set; }

        public int MaKho { get; set; }

        public int MaKH { get; set; }

        public decimal TongTien { get; set; }
        public string? Ghichu { get; set; }
        [ForeignKey("MaKH")]

        public virtual KhachHangModel? KhachHang { get; set; }

        [ForeignKey("MaNhanVien")]
        public virtual NhanVienModel? NhanVien { get; set; }
        [ForeignKey("MaKho")]
        public virtual KhoHangModel? KhoHang { get; set; }
        public virtual ICollection<PhieuXuatChiTietModel>? DSChiTietPhieuXuat { get; set; }
    }
}
