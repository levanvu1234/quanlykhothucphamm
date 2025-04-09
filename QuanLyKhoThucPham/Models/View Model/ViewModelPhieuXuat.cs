namespace QuanLyKhoThucPham.Models.View_Model
{
    public class ViewModelPhieuXuat
    {
        public List<NhanVienModel> DSNhanVien { get; set; } = new List<NhanVienModel>();
        public NhanVienModel NhanVien { get; set; }
        public List<KhoHangModel> DSKhoHang { get; set; } = new List<KhoHangModel>();
        public KhoHangModel KhoHang { get; set; }
        public List<KhachHangModel> DSKhachHang { get; set; } = new List<KhachHangModel>();
        public KhachHangModel KhachHangModel { get; set; }
        public PhieuXuatModel PhieuXuat { get; set; } = new PhieuXuatModel();
        public List<PhieuXuatChiTietModel> DSChiTietPhieuXuat { get; set; } = new List<PhieuXuatChiTietModel>();
        public List<SanPhamModel> DSSanPham { get; set; } = new List<SanPhamModel>();
        public SanPhamModel SanPham { get; set; }
    }
}
