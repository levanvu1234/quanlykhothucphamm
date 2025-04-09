// Controllers/TaiKhoanController.cs  
using Microsoft.AspNetCore.Mvc;
using QuanLyKhoThucPham.Models;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyKhoThucPham.Controllers
{
    public class TaiKhoanController : Controller
    {
        private static List<TaiKhoan> _taiKhoans = new List<TaiKhoan>()
        {
            new TaiKhoan { Id = 1, TaiKhoanNguoiDung = "admin", MatKhau = "1", HoTen = "Admin", Email = "admin@example.com" }
        }; // Dữ liệu tạm thời, thay bằng DB  

        public IActionResult Index()
        {
            return View(_taiKhoans);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TaiKhoan taiKhoan)
        {
            if (ModelState.IsValid)
            {
                // Thêm tài khoản vào database (thay vì list tạm)  
                taiKhoan.Id = _taiKhoans.Count + 1; // Gán Id tạm thời  
                _taiKhoans.Add(taiKhoan);

                return RedirectToAction(nameof(Index));
            }
            return View(taiKhoan);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taiKhoan = _taiKhoans.FirstOrDefault(t => t.Id == id);
            if (taiKhoan == null)
            {
                return NotFound();
            }
            return View(taiKhoan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, TaiKhoan taiKhoan)
        {
            if (id != taiKhoan.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Cập nhật tài khoản trong database (thay vì list tạm)  
                var existingTaiKhoan = _taiKhoans.FirstOrDefault(t => t.Id == id);
                if (existingTaiKhoan != null)
                {
                    existingTaiKhoan.TaiKhoanNguoiDung = taiKhoan.TaiKhoanNguoiDung;
                    existingTaiKhoan.MatKhau = taiKhoan.MatKhau;
                    existingTaiKhoan.HoTen = taiKhoan.HoTen;
                    existingTaiKhoan.Email = taiKhoan.Email;
                }

                return RedirectToAction(nameof(Index));
            }
            return View(taiKhoan);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taiKhoan = _taiKhoans.FirstOrDefault(t => t.Id == id);
            if (taiKhoan == null)
            {
                return NotFound();
            }

            return View(taiKhoan);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var taiKhoan = _taiKhoans.FirstOrDefault(t => t.Id == id);
            if (taiKhoan == null)
            {
                return NotFound();
            }

            _taiKhoans.Remove(taiKhoan);
            return RedirectToAction(nameof(Index));
        }
    }
}
