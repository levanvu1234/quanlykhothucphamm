using QuanLyKhoThucPham.Data;
using QuanLyKhoThucPham.Models;
using QuanLyKhoThucPham.Models.View_Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Data.SqlClient;
using QuanLyKhoThucPham.Services;
using PdfSharp.Snippets.Font;
using Newtonsoft.Json;

namespace QuanLyKhoThucPham.Controllers
{
    public class PhieuNhapController : Controller
    {
        private readonly QuanLyKhoThucPhamContext _context;

        public PhieuNhapController(QuanLyKhoThucPhamContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? pageNumber, string searchString, DateTime? searchDateFrom, DateTime? searchDateTo, string sortOrder)
        {
            var phieuNhap = _context.PhieuNhap
                .Include(p => p.NhaCungCap)
                .Include(p => p.NhanVien)
                .Include(p => p.KhoHang)
                .AsNoTracking();

            ViewData["TimKiemTheoTenNhaCC"] = searchString;
            ViewData["SXTheoNgayNhap"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["TimKiemTheoNgayTu"] = searchDateFrom?.ToString("yyyy-MM-dd");
            ViewData["TimKiemTheoNgayDen"] = searchDateTo?.ToString("yyyy-MM-dd");

            switch (sortOrder)
            {
                case "Date":
                    phieuNhap = phieuNhap.OrderBy(s => s.NgayNhap);
                    break;
                case "date_desc":
                    phieuNhap = phieuNhap.OrderByDescending(s => s.NgayNhap);
                    break;

                default:
                    phieuNhap = phieuNhap.OrderBy(s => s.MaPhieuNhap);
                    break;
            } 

            if (!string.IsNullOrEmpty(searchString))
            {
                phieuNhap = phieuNhap.Where(s => s.NhaCungCap != null && s.NhaCungCap.TenNhaCungCap.ToLower().Contains(searchString));
            }

            if (searchDateFrom.HasValue)
            {
                phieuNhap = phieuNhap.Where(s => s.NgayNhap.Date >= searchDateFrom.Value.Date);
            }

            if (searchDateTo.HasValue)
            {
                phieuNhap = phieuNhap.Where(s => s.NgayNhap.Date <= searchDateTo.Value.Date);
            }

            int pageSize = 5;
            int pageIndex = pageNumber ?? 1;

            var phieuNhapPage = await PaginatedList<PhieuNhapModel>.CreateAsync(phieuNhap.AsNoTracking(), pageIndex, pageSize);

            // Lấy giá trị stt từ TempData nếu có, nếu không thì khởi tạo từ 1
            int stt = TempData["stt"] != null ? (int)TempData["stt"] : (pageIndex - 1) * pageSize + 1;

            // Lưu giá trị stt vào TempData để sử dụng ở trang tiếp theo
            TempData["stt"] = stt +  phieuNhapPage.Count;

            return View(phieuNhapPage);
        }


        private async Task<ViewModelPhieuNhap> GetPhieuNhapViewModel()
        {
            return new ViewModelPhieuNhap
            {
                DSNhaCungCap = await _context.NhaCungCap.ToListAsync(),
                DSSanPham = await _context.SanPham.ToListAsync(),
                DSKhoHang = await _context.KhoHang.ToListAsync(),
                DSNhanVien = await _context.NhanVien.ToListAsync()
            };
        }

        public async Task<IActionResult> Create()
        {
            var phieuNhapViewModel = await GetPhieuNhapViewModel();
            return View(phieuNhapViewModel);
        }



        // POST: PhieuNhap/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PhieuNhapModel phieuNhap, List<PhieuNhapChiTietModel> phieuNhapChiTiet, int? pageNumber)
        {
            if (!ModelState.IsValid)
            {

                var phieuNhapViewModel = await GetPhieuNhapViewModel();
                phieuNhapViewModel.PhieuNhap = phieuNhap;
                phieuNhapViewModel.DSChiTietPhieuNhap = phieuNhapChiTiet;                                      
                return View(phieuNhapViewModel);
            }


            phieuNhap.TongTien = phieuNhapChiTiet.Sum(t => t.SoLuong * t.DonGia);

            _context.PhieuNhap.Add(phieuNhap);
            await _context.SaveChangesAsync(); 

            if (phieuNhapChiTiet != null)
            {
                foreach (var phieuNhapCT in phieuNhapChiTiet)
                {
                    phieuNhapCT.MaPhieuNhap = phieuNhap.MaPhieuNhap; 
                    phieuNhapCT.TongTIen = phieuNhapCT.SoLuong * phieuNhapCT.DonGia;

                    var sanPham = await _context.SanPham.FindAsync(phieuNhapCT.MaSP);
                    var khoHang = await _context.KhoHang.FindAsync(phieuNhap.MaKho);


                    if (khoHang.soluongtrong - phieuNhapCT.SoLuong < 0)
                    {
                        ModelState.AddModelError("", $"{khoHang.TenKho} đã đầy.");
                        var phieuNhapViewModel = await GetPhieuNhapViewModel();
                        return View(phieuNhapViewModel);
                    }

                   

                    khoHang.soluongtrong -= phieuNhapCT.SoLuong;

                    if (sanPham != null)
                    {
                        sanPham.SoLuong += phieuNhapCT.SoLuong;
                    }
                    else
                    {
                        ModelState.AddModelError("", $"Sản phẩm '{sanPham.TenSP}' không tồn tại.");
                        var phieuNhapViewModel = await GetPhieuNhapViewModel();
                        return View(phieuNhapViewModel);
                    }

                    _context.PhieuNhapChiTiet.Add(phieuNhapCT);
                    _context.KhoHang.Update(khoHang);
                    _context.SanPham.Update(sanPham);
                }
                await _context.SaveChangesAsync();
            }

           
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Pdf(int maPN)
        {
            var phieuNhap = await _context.PhieuNhap
            .Include(p => p.NhaCungCap)
            .Include(p => p.NhanVien)
            .Include(p => p.KhoHang)
            .FirstOrDefaultAsync(p => p.MaPhieuNhap == maPN);

            if(phieuNhap == null)
            {
                return NotFound();
            }

            var chiTietPhieuNhap = await _context.PhieuNhapChiTiet
                .Where(ct => ct.MaPhieuNhap == maPN)
                .Include(ct => ct.SanPham)
                .ToListAsync();


            phieuNhap.DSChiTietPhieuNhap = chiTietPhieuNhap;

            var inPDF = new InPDF();
            var pdfDocument = inPDF.GeneratePhieuNhapPdf(phieuNhap, chiTietPhieuNhap);

            // Trả về file PDF
            using (var stream = new MemoryStream())
            {
                pdfDocument.Save(stream, false);
                return File(stream.ToArray(), "application/pdf", $"PhieuNhap_{phieuNhap.MaPhieuNhap}.pdf");
            }
        }



        public async Task<IActionResult> Detail(int maPN)
        {
            var phieuNhap = await _context.PhieuNhap
            .Include(p => p.NhaCungCap)
            .Include(p => p.NhanVien)
            .Include(p => p.KhoHang)
            .FirstOrDefaultAsync(p => p.MaPhieuNhap == maPN);

            if (phieuNhap == null)
            {
                return NotFound();
            }
        
            var chiTietPhieuNhap = await _context.PhieuNhapChiTiet
                .Where(ct => ct.MaPhieuNhap == maPN)
                .Include(ct => ct.SanPham)
                .ToListAsync();

            phieuNhap.DSChiTietPhieuNhap = chiTietPhieuNhap;

            return View(phieuNhap);
        }

        //Get
        public async Task<IActionResult> Delete(int maPN)
        {
            var phieuNhap = await _context.PhieuNhap
                .Include(p => p.NhaCungCap)
                .Include(p => p.NhanVien)
                .Include(p => p.KhoHang)
                .Include(p => p.DSChiTietPhieuNhap)
                    .ThenInclude(p => p.SanPham)
                .FirstOrDefaultAsync(p => p.MaPhieuNhap == maPN);

            if (phieuNhap == null)
            {
                return NotFound();
            }

            return View(phieuNhap);
        }

        // POST: PhieuNhap/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int maPN)
        {
            var phieuNhap = await _context.PhieuNhap
             .FirstOrDefaultAsync(p => p.MaPhieuNhap == maPN);

            if (phieuNhap == null)
            {
                return NotFound();
            }

            var chiTietPhieuNhap = await _context.PhieuNhapChiTiet
               .Where(ct => ct.MaPhieuNhap == maPN)
               .ToListAsync();
              
            _context.PhieuNhapChiTiet.RemoveRange(chiTietPhieuNhap);
            _context.PhieuNhap.Remove(phieuNhap);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //public IActionResult Print(int maPN)
        //{
        //    var pdfService = new InPDF();
        //    var pdfDocument = pdfService.GeneratePhieuNhapPdf();

        //    using (MemoryStream stream = new MemoryStream())
        //    {
        //        pdfDocument.Save(stream, false);
        //        return File(stream.ToArray(), "application/pdf", $"PhieuNhap_{maPN}.pdf");
        //    }
        //}

        [HttpGet]
        public async Task<IActionResult> GetSanPhamByKho(int maKho)
        {
            var sanPhamList = await _context.SanPham
                .Where(sp => sp.MaKho == maKho)
                .Select(sp => new
                {
                    sp.MaSP,
                    sp.TenSP,
                    sp.DonGiaNhap
                })
                .ToListAsync();

            return Json(sanPhamList);
        }


    }
}
