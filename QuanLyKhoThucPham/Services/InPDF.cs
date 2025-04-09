using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Pdf;
using QuanLyKhoThucPham.Models;
using System;
using System.Globalization;
using System.Linq;

namespace QuanLyKhoThucPham.Services
{
    public class InPDF
    {
        public PdfDocument GeneratePhieuNhapPdf(PhieuNhapModel phieuNhap, List<PhieuNhapChiTietModel> chiTietPhieuNhap)
        {
            var document = new Document();
            var section = document.AddSection();

            // Định dạng chung
            var style = document.Styles["Normal"];
            style.Font.Name = "Arial";
            style.Font.Size = 10;

            // Tiêu đề phiếu nhập
            var paragraphTitle = section.AddParagraph("PHIẾU NHẬP KHO");
            paragraphTitle.Format.Alignment = ParagraphAlignment.Center;
            paragraphTitle.Format.Font.Bold = true;
            paragraphTitle.Format.Font.Size = 16;
            section.AddParagraph(); // Thêm khoảng trắng

            // Thông tin công ty
            var headerTable = section.AddTable();
            headerTable.AddColumn("8cm");
            headerTable.AddColumn("8cm");
            headerTable.Rows.LeftIndent = 0;

            var row = headerTable.AddRow();
            row.Cells[0].AddParagraph("CÔNG TY CỔ PHẦN ...");
            row.Cells[0].Format.Font.Bold = true;
            row.Cells[0].Format.Font.Size = 14;

            row = headerTable.AddRow();
            row.Cells[0].AddParagraph("Địa chỉ: Hà Nội");

            row = headerTable.AddRow();
            row.Cells[0].AddParagraph("Điện thoại: 00091997045");

            row = headerTable.AddRow();
            row.Cells[0].AddParagraph("Email: yarshidia@gmail.com");

            row = headerTable.AddRow();
            row.Cells[1].AddParagraph($"Mã phiếu: {phieuNhap.MaPhieuNhap}");
            row.Cells[1].Format.Alignment = ParagraphAlignment.Right;

            row = headerTable.AddRow();
            row.Cells[1].AddParagraph($"Ngày nhập: {phieuNhap.NgayNhap.ToString("dd/MM/yyyy")}");
            row.Cells[1].Format.Alignment = ParagraphAlignment.Right;

            section.AddParagraph(); // Thêm khoảng trắng

            // Thông tin người nhận/nhà cung cấp (có thể thêm thông tin chi tiết hơn)
            section.AddParagraph("Thông tin nhập hàng:");
            section.AddParagraph($"Nhà cung cấp: {phieuNhap.NhaCungCap?.TenNhaCungCap ?? "Chưa xác định"}");
            section.AddParagraph($"Địa chỉ NCC: {phieuNhap.NhaCungCap?.DiaChi ?? ""}");
            section.AddParagraph($"Số hóa đơn (nếu có): {phieuNhap.MaPhieuNhap}");
            section.AddParagraph(); // Thêm khoảng trắng

            // Bảng chi tiết sản phẩm nhập
            var table = section.AddTable();
            table.Borders.Width = 0.5;
            table.AddColumn("1cm"); // STT
            table.AddColumn("6cm"); // Tên sản phẩm
            table.AddColumn("2cm"); // Số lượng
            table.AddColumn("3cm"); // Đơn giá
            table.AddColumn("3cm"); // Thành tiền

            // Tiêu đề bảng
            row = table.AddRow();
            row.Cells[0].AddParagraph("STT");
            row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[1].AddParagraph("Tên sản phẩm");
            row.Cells[2].AddParagraph("Số lượng");
            row.Cells[2].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[3].AddParagraph("Đơn giá");
            row.Cells[3].Format.Alignment = ParagraphAlignment.Right;
            row.Cells[4].AddParagraph("Thành tiền");
            row.Cells[4].Format.Alignment = ParagraphAlignment.Right;
            row.Format.Font.Bold = true;
            row.Format.Alignment = ParagraphAlignment.Center;

            // Dữ liệu chi tiết
            int stt = 1;
            foreach (var item in chiTietPhieuNhap)
            {
                row = table.AddRow();
                row.Cells[0].AddParagraph(stt.ToString());
                row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[1].AddParagraph(item.SanPham?.TenSP ?? "");
                row.Cells[2].AddParagraph(item.SoLuong.ToString("N0"));
                row.Cells[2].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[3].AddParagraph(item.DonGia.ToString("N2"));
                row.Cells[3].Format.Alignment = ParagraphAlignment.Right;
                row.Cells[4].AddParagraph(item.TongTIen.ToString());
                row.Cells[4].Format.Alignment = ParagraphAlignment.Right;
                stt++;
            }

            // Dòng tổng cộng
            row = table.AddRow();
            row.Cells[3].AddParagraph("Tổng cộng:");
            row.Cells[3].Format.Alignment = ParagraphAlignment.Right;
            row.Cells[4].AddParagraph(chiTietPhieuNhap.Sum(ct => ct.TongTIen).ToString());
            row.Cells[4].Format.Alignment = ParagraphAlignment.Right;
            row.Format.Font.Bold = true;

            section.AddParagraph(); // Thêm khoảng trắng

            // Thông tin thanh toán (có thể điều chỉnh)
            var paragraphTongTien = section.AddParagraph($"Tổng tiền (bằng chữ): {chiTietPhieuNhap.Sum(ct => ct.TongTIen)} đồng");
            paragraphTongTien.Format.Font.Italic = true;
            section.AddParagraph(); // Thêm khoảng trắng

            // Phần chữ ký
            var footerTable = section.AddTable();
            footerTable.AddColumn("8cm");
            footerTable.AddColumn("8cm");

            row = footerTable.AddRow();
            row.Cells[0].AddParagraph("Người lập phiếu");
            row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[1].AddParagraph("Người nhận hàng");
            row.Cells[1].Format.Alignment = ParagraphAlignment.Center;

            row = footerTable.AddRow();
            row.Cells[0].AddParagraph("(Ký và ghi rõ họ tên)");
            row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[1].AddParagraph("(Ký và ghi rõ họ tên)");
            row.Cells[1].Format.Alignment = ParagraphAlignment.Center;

            // Ghi chú (nếu có)
            if (!string.IsNullOrWhiteSpace(phieuNhap.Ghichu))
            {
                section.AddParagraph();
                section.AddParagraph("Ghi chú:");
                section.AddParagraph(phieuNhap.Ghichu);
            }

            var pdfRenderer = new PdfDocumentRenderer();
            pdfRenderer.Document = document;
            pdfRenderer.RenderDocument();

            return pdfRenderer.PdfDocument;
        }      
    }
}