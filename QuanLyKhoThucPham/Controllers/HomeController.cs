using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using QuanLyKhoThucPham.Data;
using QuanLyKhoThucPham.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace QuanLyKhoThucPham.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private QuanLyKhoThucPhamContext _context;

        public HomeController(ILogger<HomeController> logger, QuanLyKhoThucPhamContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            
            var nhapHang = _context.PhieuNhap;
            var xuatHang = _context.PhieuXuat;
            decimal tongGiaTrinhhapHang = nhapHang.Sum(t => t.TongTien);
            decimal tongGiaTriXuatHang = xuatHang.Sum(t => t.TongTien);

            ViewData["SoLuongSanPham"] = _context.SanPham.Count();
            ViewData["SoLuongKho"] = _context.KhoHang.Count();
            ViewData["SoLuongNhaCungCap"] = _context.NhaCungCap.Count();
            ViewData["SoLuongNhanVien"] = _context.NhanVien.Count();
            ViewData["TongGiaTriMua"] = tongGiaTrinhhapHang;
            ViewData["SoLuongPhieuNhap"] = nhapHang.Count();
            ViewData["SoLuongPhieuXuat"] = xuatHang.Count();
            ViewData["TongDoanhThu"] = tongGiaTriXuatHang;
            ViewData["TongLoiNhuan"] = tongGiaTriXuatHang - tongGiaTrinhhapHang;
            ViewData["SoLuongKhachHang"] = _context.KhachHang.Count();

            var hoTen = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            // Truyền vào ViewData để sử dụng trong layout
            ViewData["HoTenNhanVien"] = hoTen;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
