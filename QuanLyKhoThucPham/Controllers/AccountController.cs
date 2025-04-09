using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using QuanLyKhoThucPham.Models; // Đảm bảo namespace này chính xác  
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoThucPham.Data;

namespace QuanLyKhoThucPham.Controllers
{
    public class AccountController : Controller
    {
        private readonly QuanLyKhoThucPhamContext _context;

        public AccountController(QuanLyKhoThucPhamContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string TaiKhoan, string MatKhau, bool GhiNho)
        {
            // Check tài khoản ẩn admin
            if (TaiKhoan == "admin" && MatKhau == "admin")
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "Admin"),
            new Claim("TaiKhoan", "admin"),
            new Claim(ClaimTypes.Role, "Admin")
        };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = GhiNho,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Index", "Home");
            }

            // Kiểm tra trong bảng NhanVien
            var nhanVien = _context.NhanVien
                .FirstOrDefault(nv => nv.TaiKhoan == TaiKhoan && nv.MatKhau == MatKhau);

            if (nhanVien != null)
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, nhanVien.HoTen),
            new Claim("MaNhanVien", nhanVien.MaNhanVien.ToString()),
            new Claim("TaiKhoan", nhanVien.TaiKhoan),
            new Claim(ClaimTypes.Role, nhanVien.ChucVu ?? "NhanVien")
        };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = GhiNho,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Thông tin đăng nhập không hợp lệ.");
            return View();
        }



        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View(); // Tạo một view AccessDenied nếu cần  
        }
    }
}