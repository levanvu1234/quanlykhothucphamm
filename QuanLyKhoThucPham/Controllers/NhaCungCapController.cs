using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoThucPham.Models;
using QuanLyKhoThucPham.Data;

namespace QuanLyKhoThucPham.Controllers
{
    public class NhaCungCapController : Controller
    {
        private readonly QuanLyKhoThucPhamContext _context;

        public NhaCungCapController(QuanLyKhoThucPhamContext context)
        {
            _context = context;
        }

        // GET: NhaCungCap
        public async Task<IActionResult> Index(string searchString, int? pageNumber)
        {
            if (_context.NhaCungCap == null)
            {
                return Problem("Danh sách kho hàng không có dữ liệu ");
            }

            var dsNhaCC = _context.NhaCungCap.AsNoTracking();

            if (!String.IsNullOrEmpty(searchString))
            {
                dsNhaCC = dsNhaCC.Where(s => s.TenNhaCungCap.ToUpper().Contains(searchString.ToUpper()));
            }

            ViewData["searchString"] = searchString;
            int pageSize = 5;
            int pageIndex = pageNumber ?? 1;

            var nhaCCPage = await PaginatedList<NhaCungCapModel>.CreateAsync(dsNhaCC, pageIndex, pageSize);

            // Lấy giá trị stt từ TempData nếu có, nếu không thì khởi tạo từ 1
            int stt = TempData["stt"] != null ? (int)TempData["stt"] : (pageIndex - 1) * pageSize + 1;

            // Lưu giá trị stt vào TempData để sử dụng ở trang tiếp theo
            TempData["stt"] = stt + nhaCCPage.Count;


            return View(nhaCCPage);
        }

        // GET: NhaCungCap/Details/5
        public async Task<IActionResult> Details(int? maNhaCungCap)
        {
            if (maNhaCungCap == null || _context.NhaCungCap == null)
            {
                return NotFound();
            }

            var nhaCungCap = await _context.NhaCungCap
                .FirstOrDefaultAsync(m => m.MaNhaCungCap == maNhaCungCap);
            if (nhaCungCap == null)
            {
                return NotFound();
            }

            return View(nhaCungCap);
        }

        // GET: NhaCungCap/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: NhaCungCap/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaNhaCungCap,TenNhaCungCap,DiaChi,SoDienThoai")] NhaCungCapModel nhaCungCap)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nhaCungCap);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(nhaCungCap);
        }

        // GET: NhaCungCap/Edit/5
        public async Task<IActionResult> Edit(int? maNhaCungCap)
        {
            if (maNhaCungCap == null || _context.NhaCungCap == null)
            {
                return NotFound();
            }

            var nhaCungCap = await _context.NhaCungCap.FindAsync(maNhaCungCap);
            if (nhaCungCap == null)
            {
                return NotFound();
            }
            return View(nhaCungCap);
        }

        // POST: NhaCungCap/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int maNhaCungCap, [Bind("MaNhaCungCap,TenNhaCungCap,DiaChi,SoDienThoai")] NhaCungCapModel nhaCungCap)
        {
            if (maNhaCungCap != nhaCungCap.MaNhaCungCap)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nhaCungCap);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NhaCungCapExists(nhaCungCap.MaNhaCungCap))
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
            return View(nhaCungCap);
        }

        // GET: NhaCungCap/Delete/5
        public async Task<IActionResult> Delete(int? maNhaCungCap)
        {
            if (maNhaCungCap == null || _context.NhaCungCap == null)
            {
                return NotFound();
            }

            var nhaCungCap = await _context.NhaCungCap
                .FirstOrDefaultAsync(m => m.MaNhaCungCap == maNhaCungCap);
            if (nhaCungCap == null)
            {
                return NotFound();
            }

            return View(nhaCungCap);
        }

        // POST: NhaCungCap/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int maNhaCungCap)
        {
            if (_context.NhaCungCap == null)
            {
                return Problem("Entity set 'QuanLyKhoThucPhamContext.NhaCungCap'  is null.");
            }
            var nhaCungCap = await _context.NhaCungCap.FindAsync(maNhaCungCap);
            if (nhaCungCap != null)
            {
                _context.NhaCungCap.Remove(nhaCungCap);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NhaCungCapExists(int maNhaCungCap)
        {
          return (_context.NhaCungCap?.Any(e => e.MaNhaCungCap == maNhaCungCap)).GetValueOrDefault();
        }
       
    }
}
