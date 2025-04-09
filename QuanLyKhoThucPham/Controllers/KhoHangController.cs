using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoThucPham.Data;
using QuanLyKhoThucPham.Models;

namespace QuanLyKhoThucPham.Controllers
{
    public class KhoHangController : Controller
    {
        private readonly QuanLyKhoThucPhamContext _context;

        public KhoHangController(QuanLyKhoThucPhamContext context)
        {
            _context = context;
        }

        // GET: KhoHang/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.KhoHang == null)
            {
                return NotFound();
            }

            var khoHangModel = await _context.KhoHang
                .FirstOrDefaultAsync(m => m.MaKho == id);
            if (khoHangModel == null)
            {
                return NotFound();
            }

            return View(khoHangModel);
        }

        // GET: KhoHang/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: KhoHang/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaKho,TenKho,KhoLoaiSP,mota,soluongtong,soluongtrong")] KhoHangModel khoHangModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(khoHangModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                foreach (var modelStateKey in ModelState.Keys)
                {
                    var modelStateVal = ModelState[modelStateKey];
                    foreach (var error in modelStateVal.Errors)
                    {
                        Console.WriteLine($"Key: {modelStateKey}, Error: {error.ErrorMessage}");
                    }
                }
                return View(khoHangModel);
            }
        }

        // GET: KhoHang/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.KhoHang == null)
            {
                return NotFound();
            }

            var khoHangModel = await _context.KhoHang.FindAsync(id);
            if (khoHangModel == null)
            {
                return NotFound();
            }
            return View(khoHangModel);
        }

        // POST: KhoHang/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaKho,TenKho,KhoLoaiSP,mota,soluongtong,soluongtrong")] KhoHangModel khoHangModel)
        {
            if (id != khoHangModel.MaKho)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(khoHangModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KhoHangModelExists(khoHangModel.MaKho))
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
            return View(khoHangModel);
        }

        // GET: KhoHang/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.KhoHang == null)
            {
                return NotFound();
            }

            var khoHangModel = await _context.KhoHang
                .FirstOrDefaultAsync(m => m.MaKho == id);
            if (khoHangModel == null)
            {
                return NotFound();
            }

            return View(khoHangModel);
        }

        // POST: KhoHang/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.KhoHang == null)
            {
                return Problem("Entity set 'QuanLyKhoThucPhamContext.KhoHang'  is null.");
            }
            var khoHangModel = await _context.KhoHang.FindAsync(id);
            if (khoHangModel != null)
            {
                _context.KhoHang.Remove(khoHangModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KhoHangModelExists(int id)
        {
            return (_context.KhoHang?.Any(e => e.MaKho == id)).GetValueOrDefault();
        }


        //tìm kiếm
       
        public async Task<IActionResult> Index(
            string searchString,
            string sortOrder,
            string currentFilter,
            int? pageNumber
        )
        {
            if (_context.KhoHang == null)
            {
                return Problem("Danh sách kho hàng không có dữ liệu ");
            }

            var dskhohangs = from m in _context.KhoHang select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                dskhohangs = dskhohangs.Where(s => s.TenKho.ToUpper().Contains(searchString.ToUpper()));
            }

            
            //phân trang
            {
                ViewData["CurrentSort"] = sortOrder;


                if (searchString != null)
                {
                    pageNumber = 1;
                }
                else
                {
                    searchString = currentFilter;
                }
                int pageSize = 5;

                int pageIndex = pageNumber ?? 1;

                var khoHangPage = await PaginatedList<KhoHangModel>.CreateAsync(dskhohangs.AsNoTracking(), pageNumber ?? 1, pageSize);

                // Lấy giá trị stt từ TempData nếu có, nếu không thì khởi tạo từ 1
                int stt = TempData["stt"] != null ? (int)TempData["stt"] : (pageIndex - 1) * pageSize + 1;

                TempData["stt"] = stt + khoHangPage.Count;


                return View(khoHangPage);
            }
        }


       
       
    }
}
