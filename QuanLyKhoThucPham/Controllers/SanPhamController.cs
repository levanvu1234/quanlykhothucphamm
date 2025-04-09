using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoThucPham.Data;
using QuanLyKhoThucPham.Models;
using QuanLyKhoThucPham.Models.View_Model;

namespace QuanLyKhoThucPham.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly QuanLyKhoThucPhamContext _context;

        public SanPhamController(QuanLyKhoThucPhamContext context)
        {
            _context = context;
        }

        private async Task<ViewModelKhoSanPham> GetKhoSPViewModel()
        {
            return new ViewModelKhoSanPham
            {
                DSKhoHang = await _context.KhoHang.ToListAsync(),
                DSSanPham = await _context.SanPham.ToListAsync(),
            };
        }


        // Get: SanPham/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = await GetKhoSPViewModel();
            return View(viewModel);
        }

        // POST: SanPham/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SanPhamModel sanPham)
        {
            if (ModelState.IsValid)
            {

                // Kiểm tra xem MaKho có hợp lệ không
                if (sanPham.MaKho == 0)
                {
                    ModelState.AddModelError("SanPham.MaKho", "Kho Hàng là bắt buộc.");
                }

                if (ModelState.IsValid)
                {
                    _context.Add(sanPham);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            var viewModel = await GetKhoSPViewModel();  
            

            return View(viewModel);
        }

        // GET: SanPham/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            

            if (id == null || _context.SanPham == null)
            {
                return NotFound();
            }
            //var sanPhamModel = await _context.SanPham
            //    .FirstOrDefaultAsync(m => m.MaSP == id);

            var sanPham = await _context.SanPham
                .Include(sp=>sp.KhoHang)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.MaSP == id);


            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        // GET: SanPham/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            var viewModel = await GetKhoSPViewModel();
            if (id == null || _context.SanPham == null)
            {
                return NotFound();
            }
            viewModel.SanPham = await _context.SanPham.FindAsync(id);
            //var sanPhamModel = await _context.SanPham.FindAsync(id);
            if (viewModel.SanPham == null)
            {
                return NotFound();
            }
            return View(viewModel);
        }

        // POST: SanPham/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ViewModelKhoSanPham viewModel)
        {
            if (id != viewModel.SanPham.MaSP)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var sanPham = viewModel.SanPham;
                try
                {
                    _context.Update(sanPham);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SanPhamModelExists(sanPham.MaSP))
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
            return View(viewModel);
        }

        // GET: SanPham/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.SanPham == null)
            {
                return NotFound();
            }

            var sanPhamModel = await _context.SanPham
                .Include(sp=>sp.KhoHang)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.MaSP == id);
            if (sanPhamModel == null)
            {
                return NotFound();
            }

            return View(sanPhamModel);
        }

        // POST: SanPham/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.SanPham == null)
            {
                return Problem("Entity set 'QuanLyKhoThucPhamContext.SanPham' is null.");
            }

            // Tìm sản phẩm, bao gồm các phiếu nhập và xuất chi tiết có liên quan
            var sanPham = await _context.SanPham
                .Include(sp => sp.PhieuNhapChiTiets)
                .Include(sp => sp.PhieuXuatChiTiets)
                .FirstOrDefaultAsync(sp => sp.MaSP == id);

            if (sanPham != null)
            {
                // Xóa các chi tiết phiếu nhập liên quan
                if (sanPham.PhieuNhapChiTiets != null && sanPham.PhieuNhapChiTiets.Any())
                {
                    _context.PhieuNhapChiTiet.RemoveRange(sanPham.PhieuNhapChiTiets);
                }

                // Xóa các chi tiết phiếu xuất liên quan
                if (sanPham.PhieuXuatChiTiets != null && sanPham.PhieuXuatChiTiets.Any())
                {
                    _context.PhieuXuatChiTiet.RemoveRange(sanPham.PhieuXuatChiTiets);
                }

                // Sau khi xóa chi tiết, xóa sản phẩm
                _context.SanPham.Remove(sanPham);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool SanPhamModelExists(int id)
        {
            return (_context.SanPham?.Any(e => e.MaSP == id)).GetValueOrDefault();
        }


        //tìm kiếm
      
        public async Task<IActionResult> Index(
            string searchString,
            string sortOrder,
            string currentFilter,
            int? pageNumber)
        {
            if (_context.SanPham == null)
            {
                return Problem("Danh sách kho hàng không có dữ liệu ");
            }

            var dsthucphams = _context.SanPham
                .Include(sp=>sp.KhoHang)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(searchString))
            {
                dsthucphams = dsthucphams.Where(s => s.TenSP.ToUpper().Contains(searchString.ToUpper()));
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
                ViewData["searchString"] = searchString;
                int pageSize = 5;
                int pageIndex = pageNumber ?? 1;

                // Lấy giá trị stt từ TempData nếu có, nếu không thì khởi tạo từ 1
                int stt = TempData["stt"] != null ? (int)TempData["stt"] : (pageIndex - 1) * pageSize + 1;

                // Lưu giá trị stt vào TempData để sử dụng ở trang tiếp theo
                TempData["stt"] = stt + (await PaginatedList<SanPhamModel>.CreateAsync(dsthucphams.AsNoTracking(), pageIndex, pageSize)).Count;
                return View(await PaginatedList<SanPhamModel>.CreateAsync(dsthucphams.AsNoTracking(), pageIndex, pageSize));
            }
        }

    }
}
