using System.ComponentModel.DataAnnotations;

namespace QuanLyKhoThucPham.Models
{
    public class NhaCungCapModel
    {

        public NhaCungCapModel()
        {
            this.PhieuNhap = new HashSet<PhieuNhapModel>();
        }

        [Key]
        [Required]
        [Display(Name = "Mã Nhà Cung Cấp")]
        public int MaNhaCungCap { get; set; }

        [Required]
        [Display(Name = "Tên Nhà Cung Cấp")]
        public string TenNhaCungCap { get; set; }

        [Display(Name = "Địa Chỉ")]
        public string DiaChi { get; set; }

        [Display(Name = "Số Điện Thoại")]
        public string SoDienThoai { get; set; }

        public ICollection<PhieuNhapModel> PhieuNhap {  get; set; } 

    }
}
