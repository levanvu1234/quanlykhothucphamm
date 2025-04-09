using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoThucPham.Models.View_Model;
using QuanLyKhoThucPham.Models;
using QuanLyKhoThucPham.Data;

namespace QuanLyKhoThucPham.Controllers
{
    public class PhieuXuatController : Controller
    {
        private readonly QuanLyKhoThucPhamContext _context;
        public PhieuXuatController(QuanLyKhoThucPhamContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int? pageNumber, string searchString, DateTime? searchDateFrom, DateTime? searchDateTo)
        {

            var phieuXuat = _context.PhieuXuat
                .Include(p => p.KhachHang)
                .Include(p => p.NhanVien)
                .Include(p => p.KhoHang)
                .AsNoTracking();
            

            ViewData["CurrentFilter"] = searchString;
            ViewData["TimKiemTheoNgayTu"] = searchDateFrom?.ToString("yyyy-MM-dd");
            ViewData["TimKiemTheoNgayDen"] = searchDateTo?.ToString("yyyy-MM-dd");



            if (!string.IsNullOrEmpty(searchString))
            {
                phieuXuat = phieuXuat.Where(s => s.KhachHang != null && s.KhachHang.TenKH.ToLower().Contains(searchString));
            }

            if (searchDateFrom.HasValue)
            {
                phieuXuat = phieuXuat.Where(s => s.NgayXuat.Date >= searchDateFrom.Value.Date);
            }

            if (searchDateTo.HasValue)
            {
                phieuXuat = phieuXuat.Where(s => s.NgayXuat.Date <= searchDateTo.Value.Date);
            }

            int pageSize = 5;
            int pageIndex = pageNumber ?? 1;

            var phieuXuatPage = PaginatedList<PhieuXuatModel>.CreateAsync(phieuXuat.AsNoTracking(), pageIndex, pageSize);

            // Lấy giá trị stt từ TempData nếu có, nếu không thì khởi tạo từ 1
            int stt = TempData["stt"] != null ? (int)TempData["stt"] : (pageIndex - 1) * pageSize + 1;

            // Lưu giá trị stt vào TempData để sử dụng ở trang tiếp theo
            TempData["stt"] = stt + (await phieuXuatPage).Count;

            return  View(await phieuXuatPage);
    
        }

        private async Task<ViewModelPhieuXuat> GetPhieuXuatViewModel()
        {
            return new ViewModelPhieuXuat
            {
                DSKhachHang = await _context.KhachHang.ToListAsync(),
                DSSanPham = await _context.SanPham.ToListAsync(),
                DSKhoHang = await _context.KhoHang.ToListAsync(),
                DSNhanVien = await _context.NhanVien.ToListAsync(),
            };
        }

        public async Task<IActionResult> Create()
        {

            var phieuXuatViewModel = await GetPhieuXuatViewModel();
            return View(phieuXuatViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PhieuXuatModel phieuXuat, List<PhieuXuatChiTietModel> phieuXuatChiTiet)
        {
            if (!ModelState.IsValid)
            {
                var phieuXuatViewModel = await GetPhieuXuatViewModel();

                return View(phieuXuatViewModel);
            }


            phieuXuat.TongTien = phieuXuatChiTiet.Sum(t => t.SoLuong * t.DonGia);

            _context.PhieuXuat.Add(phieuXuat);

            await _context.SaveChangesAsync();

            if (phieuXuatChiTiet != null && phieuXuatChiTiet.Any())
            {
                foreach (var phieuXuatCT in phieuXuatChiTiet)
                {
                    phieuXuatCT.MaPhieuXuat = phieuXuat.MaPhieuXuat;
                    phieuXuatCT.TongTIen = phieuXuatCT.SoLuong * phieuXuatCT.DonGia;

                    var sanPham = await _context.SanPham.FindAsync(phieuXuatCT.MaSP);
                    var khoHang = await _context.KhoHang.FindAsync(phieuXuat.MaKho);

                    if(khoHang != null)
                    {
                        if (khoHang.soluongtrong + phieuXuatCT.SoLuong > khoHang.soluongtong)
                        {
                            ModelState.AddModelError("", $"{khoHang.TenKho} đã hết sản phẩm.");
                            var phieuXuatViewModel = await GetPhieuXuatViewModel();
                            return View(phieuXuatViewModel);
                        }
                        khoHang.soluongtrong += phieuXuatCT.SoLuong;
                    }

                    if (sanPham != null)
                    {
                        if(sanPham.SoLuong - phieuXuatCT.SoLuong >= 0)
                        {
                            sanPham.SoLuong -= phieuXuatCT.SoLuong;
                        }
                        else
                        {
                            ModelState.AddModelError("", $"Sản phẩm '{sanPham.TenSP}' không đủ số lượng tồn.");
                            var phieuXuatViewModel = await GetPhieuXuatViewModel();
                            return View(phieuXuatViewModel);
                        }
                    }

                    _context.PhieuXuatChiTiet.Add(phieuXuatCT);
                    _context.KhoHang.Update(khoHang);
                    _context.SanPham.Update(sanPham);
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(int maPX)
        {
            var phieuXuat = await _context.PhieuXuat
            .Include(p => p.KhachHang)
            .Include(p => p.NhanVien)
            .Include(p => p.KhoHang)
            .FirstOrDefaultAsync(p => p.MaPhieuXuat == maPX);

            if (phieuXuat == null)
            {
                return NotFound();
            }

            var chiTietPhieuXuat = await _context.PhieuXuatChiTiet
                .Where(ct => ct.MaPhieuXuat == maPX)
                .Include(ct => ct.SanPham)
                .ToListAsync();

            phieuXuat.DSChiTietPhieuXuat = chiTietPhieuXuat;

            return View(phieuXuat);
        }

        public async Task<IActionResult> Delete(int maPX)
        {
            var phieuXuat = await _context.PhieuXuat
                .Include(p => p.KhachHang)
                .Include(p => p.NhanVien)
                .Include(p => p.KhoHang)
                .Include(p => p.DSChiTietPhieuXuat)
                    .ThenInclude(p => p.SanPham)
                .FirstOrDefaultAsync(p => p.MaPhieuXuat == maPX);

            if (phieuXuat == null)
            {
                return NotFound();
            }

            return View(phieuXuat);
        }

        // POST: PhieuNhap/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int maPX)
        {
            var phieuXuat = await _context.PhieuXuat
             .FirstOrDefaultAsync(p => p.MaPhieuXuat == maPX);

            if (phieuXuat == null)
            {
                return NotFound();
            }

            var chiTietPhieuXuat = await _context.PhieuXuatChiTiet
               .Where(ct => ct.MaPhieuXuat == maPX)
               .Include(ct => ct.SanPham)
               .ToListAsync();


            phieuXuat.DSChiTietPhieuXuat = chiTietPhieuXuat;
            if (chiTietPhieuXuat != null)
            {
                foreach (var ct in chiTietPhieuXuat)
                {

                    _context.PhieuXuatChiTiet.Remove(ct);
                }
            }



            _context.PhieuXuat.Remove(phieuXuat);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<JsonResult> GetSanPhamByKho(int maKho)
        {
            var sanPhamList = await _context.SanPham
                .Where(sp => sp.MaKho == maKho)
                .Select(sp => new {
                    sp.MaSP,
                    sp.TenSP,
                    sp.DonGiaXuat,
                    sp.SoLuong
                })
                .ToListAsync();

            return Json(sanPhamList);
        }


    }

}
