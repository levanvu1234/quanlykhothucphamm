using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKhoThucPham.Models
{
    public class KhoHangModel
    {
        [Key]
        public int MaKho { get; set; }
        public string KhoLoaiSP { get; set; }
        public string TenKho { get; set; }
        public string mota { get; set; }

        public int soluongtong { get; set; }
        public int? soluongtrong { get; set; }

        public ICollection<SanPhamModel>? SanPhams { get; set; }
    }
}
