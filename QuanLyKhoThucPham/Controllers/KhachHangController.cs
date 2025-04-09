using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoThucPham.Data;
using QuanLyKhoThucPham.Models;

namespace QuanLyKhoThucPham.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly QuanLyKhoThucPhamContext _context;

        public KhachHangController(QuanLyKhoThucPhamContext context)
        {
            _context = context;
        }

        // GET: Quanlykhachhang
        public async Task<IActionResult> Index(string searchString, int? pageNumber)
        {
           
            if (_context.KhachHang == null)
            {
                return Problem("Entity set 'QuanLyKhoThucPhamContext.KhachHang' is null.");
            }


            var quanlykhachhangs = _context.KhachHang.AsNoTracking();

            if (!String.IsNullOrEmpty(searchString))
            {
                quanlykhachhangs = quanlykhachhangs.Where(s => s.TenKH.ToUpper().Contains(searchString.ToUpper()));

            }
            ViewData["searchString"] = searchString;
            int pageSize = 5;
            int pageIndex = pageNumber ?? 1;

            var khachHangPage = await PaginatedList<KhachHangModel>.CreateAsync(quanlykhachhangs.AsNoTracking(), pageIndex, pageSize);

            // Lấy giá trị stt từ TempData nếu có, nếu không thì khởi tạo từ 1
            int stt = TempData["stt"] != null ? (int)TempData["stt"] : (pageIndex - 1) * pageSize + 1;

            // Lưu giá trị stt vào TempData để sử dụng ở trang tiếp theo
            TempData["stt"] = stt + khachHangPage.Count;

            return View(khachHangPage);

        }

        // GET: Quanlykhachhang/Details/5
        public async Task<IActionResult> Details(int? maKH)
        {
            if (maKH == null || _context.KhachHang == null)
            {
                return NotFound();
            }

            var quanlykhachhang = await _context.KhachHang
                .FirstOrDefaultAsync(m => m.MaKH == maKH);
            if (quanlykhachhang == null)
            {
                return NotFound();
            }

            return View(quanlykhachhang);
        }

        // GET: Quanlykhachhang/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Quanlykhachhang/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaKH,TenKH,DiaChi,Email,SDT")] Models.KhachHangModel quanlykhachhang)
        {
            if (ModelState.IsValid)
            {
                _context.Add(quanlykhachhang);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(quanlykhachhang);
        }

        // GET: Quanlykhachhang/Edit/5
        public async Task<IActionResult> Edit(int? maKH)
        {
            if (maKH == null || _context.KhachHang == null)
            {
                return NotFound();
            }

            var quanlykhachhang = await _context.KhachHang.FindAsync(maKH);
            if (quanlykhachhang == null)
            {
                return NotFound();
            }
            return View(quanlykhachhang);
        }

        // POST: Quanlykhachhang/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int maKH, [Bind("MaKH,TenKH,DiaChi,Email,SDT")] Models.KhachHangModel dsKhachHang)
        {
            if (maKH != dsKhachHang.MaKH)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dsKhachHang);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuanlykhachhangExists(dsKhachHang.MaKH))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(dsKhachHang);
        }

        // GET: Quanlykhachhang/Delete/5
        public async Task<IActionResult> Delete(int? maKH)
        {
            if (maKH == null || _context.KhachHang == null)
            {
                return NotFound();
            }

            var quanlykhachhang = await _context.KhachHang
                .FirstOrDefaultAsync(m => m.MaKH == maKH);
            if (quanlykhachhang == null)
            {
                return NotFound();
            }

            return View(quanlykhachhang);
        }

        // POST: Quanlykhachhang/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int maKH)
        {
            if (_context.KhachHang == null)
            {
                return Problem("Entity set 'QuanLyKhoThucPhamContext.Quanlykhachhang'  is null.");
            }
            var quanlykhachhang = await _context.KhachHang.FindAsync(maKH);
            if (quanlykhachhang != null)
            {
                _context.KhachHang.Remove(quanlykhachhang);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuanlykhachhangExists(int maKH)
        {
            return (_context.KhachHang?.Any(e => e.MaKH == maKH)).GetValueOrDefault();
        }
    }
 }

