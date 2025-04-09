using System.ComponentModel.DataAnnotations;

namespace QuanLyKhoThucPham.Models
{
    public class KhachHangModel
    {

        [Key]
        public int MaKH { get; set; }
        [Display(Name = "Tên Khách Hàng")]

        public string TenKH { get; set; }
            
        [Display(Name = "SĐT")]
        public string SDT { get; set; }

        [Display(Name = "Địa chỉ")]

        public string? DiaChi { get; set; }

    }
}
