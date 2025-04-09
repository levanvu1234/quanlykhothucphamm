using System.ComponentModel.DataAnnotations;

namespace QuanLyKhoThucPham.Models
{
    public class NhanVienModel
    {
        [Key]

        public int MaNhanVien  { get; set; }

        [Required]
        [Display(Name = "Họ Tên Nhân Viên")]
        public string HoTen { get; set; }

        [Required]
        [Display(Name = "Giới tính")]
        public string GioiTinh { get; set; }

        [Display(Name = "Chức Vụ")]
        [Required]
        public string ChucVu { get; set; }

        [Required]
        [Display(Name = "Tài Khoản")]
        public string TaiKhoan { get; set; }

        [Required]
        [Display(Name = "Mật Khẩu")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; }




    }
}
