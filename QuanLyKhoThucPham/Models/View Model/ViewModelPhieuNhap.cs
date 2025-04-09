namespace QuanLyKhoThucPham.Models.View_Model
{
    public class ViewModelPhieuNhap
    {
        public List<NhanVienModel> DSNhanVien { get; set; } = new List<NhanVienModel>();
        public NhanVienModel NhanVien { get; set; }
        public List<KhoHangModel> DSKhoHang { get; set; } = new List<KhoHangModel>();
        public KhoHangModel KhoHang { get; set; }
        public List<NhaCungCapModel> DSNhaCungCap { get; set; } = new List<NhaCungCapModel>();
        public NhaCungCapModel NhaCungCap { get; set; }
        public PhieuNhapModel PhieuNhap { get; set; } = new PhieuNhapModel();
        public List<PhieuNhapChiTietModel> DSChiTietPhieuNhap { get; set; } = new List<PhieuNhapChiTietModel>();
        public List<SanPhamModel> DSSanPham { get; set; } = new List<SanPhamModel>();
        public SanPhamModel SanPham { get; set; }
    }
}
