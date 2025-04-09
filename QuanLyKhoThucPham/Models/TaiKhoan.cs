// Models/TaiKhoan.cs  
using System.ComponentModel.DataAnnotations;

namespace QuanLyKhoThucPham.Models
{
    public class TaiKhoan
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tài khoản là bắt buộc.")]
        [Display(Name = "Tài khoản")]
        public string TaiKhoanNguoiDung { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string MatKhau { get; set; }

        [Display(Name = "Họ tên")]
        public string HoTen { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        public string Email { get; set; }

        // Các thuộc tính khác (ví dụ: Role, quyền hạn,...)  
    }
}
