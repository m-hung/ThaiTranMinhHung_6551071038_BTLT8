using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using FastReport.Export.Pdf;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;
using TiemTapHoa.Models;


namespace TiemTapHoa;

public partial class FormQuanLyHangHoa : Form
{
    private static readonly string[] DsLoai = { "Đồ uống", "Thực phẩm khô", "Gia vị", "Đồ gia dụng" };
    private static readonly string[] DsDonVi = { "Chai", "Gói", "Hộp", "Túi" };

    private bool _dangNap; // true khi đang nạp dữ liệu -> bỏ qua sự kiện

    public FormQuanLyHangHoa()
    {
        InitializeComponent();
    }

    // ====================== LOAD ======================
    private void FormQuanLyHangHoa_Load(object? sender, EventArgs e)
    {
        _dangNap = true;
        cboLoaiHang.Items.AddRange(DsLoai);
        cboDonVi.Items.AddRange(DsDonVi);

        cboLocLoai.Items.Add("Tất cả loại");
        cboLocLoai.Items.AddRange(DsLoai);
        cboLocLoai.SelectedIndex = 0;
        _dangNap = false;

        try
        {
            TaiDuLieu();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Không kết nối được CSDL. Kiểm tra lại chuỗi kết nối trong TiemTapHoaContext.cs.\n\n" + ex.Message,
                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // ====================== TRUY VẤN + HIỂN THỊ ======================
    private IQueryable<HangHoa> LayTruyVanLoc(TiemTapHoaContext db)
    {
        var q = db.Set<HangHoa>().AsNoTracking().AsQueryable();

        var tu = txtTim.Text.Trim();
        if (tu.Length > 0)
            q = q.Where(h => h.TenHang.Contains(tu));   // LIKE '%tu%'

        if (cboLocLoai.SelectedIndex > 0)
        {
            var loai = cboLocLoai.SelectedItem!.ToString();
            q = q.Where(h => h.LoaiHang == loai);
        }

        if (chkChiKD.Checked)
            q = q.Where(h => h.ConKinhDoanh == true);

        return q.OrderBy(h => h.MaHang);
    }

    /// <summary>Nạp lại lưới theo bộ lọc. Nếu chonMa != null thì chọn lại dòng đó.</summary>
    private void TaiDuLieu(int? chonMa = null)
    {
        List<HangHoa> ds;
        using (var db = new TiemTapHoaContext())
            ds = LayTruyVanLoc(db).ToList();

        _dangNap = true;
        dgv.DataSource = ds;
        dgv.ClearSelection();
        dgv.CurrentCell = null;
        _dangNap = false;

        lblTrangThai.Text = $"Tìm thấy {ds.Count} mặt hàng";

        if (chonMa != null)
        {
            foreach (DataGridViewRow r in dgv.Rows)
            {
                if (r.DataBoundItem is HangHoa h && h.MaHang == chonMa)
                {
                    dgv.CurrentCell = r.Cells[1];
                    r.Selected = true;
                    break;
                }
            }
        }
    }

    // ====================== LƯỚI <-> FORM ======================
    private void dgv_SelectionChanged(object? sender, EventArgs e)
    {
        if (_dangNap) return;
        if (dgv.CurrentRow?.DataBoundItem is HangHoa h)
            HienLenForm(h);
    }

    private void HienLenForm(HangHoa h)
    {
        txtTenHang.Text = h.TenHang;
        cboLoaiHang.SelectedIndex = cboLoaiHang.FindStringExact(h.LoaiHang ?? "");
        cboDonVi.SelectedIndex = cboDonVi.FindStringExact(h.DonVi ?? "");
        nudGiaBan.Value = Math.Min(h.GiaBan ?? 0, nudGiaBan.Maximum);
        nudTon.Value = Math.Min(h.SoLuongTon ?? 0, (int)nudTon.Maximum);
        var ngay = h.NgayNhap ?? DateTime.Today;
        dtpNgayNhap.Value = ngay < dtpNgayNhap.MinDate ? dtpNgayNhap.MinDate : ngay;
        chkKinhDoanh.Checked = h.ConKinhDoanh ?? false;
    }

    private void DocTuForm(HangHoa h)
    {
        h.TenHang = txtTenHang.Text.Trim();
        h.LoaiHang = cboLoaiHang.SelectedItem?.ToString();
        h.DonVi = cboDonVi.SelectedItem?.ToString();
        h.GiaBan = nudGiaBan.Value;
        h.SoLuongTon = (int)nudTon.Value;
        h.NgayNhap = dtpNgayNhap.Value;
        h.ConKinhDoanh = chkKinhDoanh.Checked;
    }

    private void XoaTrangForm()
    {
        txtTenHang.Clear();
        cboLoaiHang.SelectedIndex = -1;
        cboDonVi.SelectedIndex = -1;
        nudGiaBan.Value = 0;
        nudTon.Value = 0;
        dtpNgayNhap.Value = DateTime.Today;
        chkKinhDoanh.Checked = true;
    }

    // ====================== KIỂM TRA DỮ LIỆU ======================
    private bool KiemTraDuLieu()
    {
        if (string.IsNullOrWhiteSpace(txtTenHang.Text))
        {
            MessageBox.Show("Tên hàng không được để trống.", "Dữ liệu không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtTenHang.Focus();
            return false;
        }
        if (nudGiaBan.Value <= 0)
        {
            MessageBox.Show("Giá bán phải lớn hơn 0.", "Dữ liệu không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            nudGiaBan.Focus();
            return false;
        }
        return true;
    }

    // ====================== THÊM / SỬA / XÓA / LÀM MỚI ======================
    private void btnThem_Click(object? sender, EventArgs e)
    {
        if (!KiemTraDuLieu()) return;
        try
        {
            var h = new HangHoa();
            DocTuForm(h);
            using (var db = new TiemTapHoaContext())
            {
                db.Set<HangHoa>().Add(h);
                db.SaveChanges();
            }
            TaiDuLieu(h.MaHang);
            MessageBox.Show("Đã thêm mặt hàng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Lỗi khi thêm: " + (ex.InnerException?.Message ?? ex.Message), "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnSua_Click(object? sender, EventArgs e)
    {
        if (dgv.CurrentRow?.DataBoundItem is not HangHoa chon)
        {
            MessageBox.Show("Hãy chọn một dòng trên lưới để sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (!KiemTraDuLieu()) return;
        try
        {
            using (var db = new TiemTapHoaContext())
            {
                var h = db.Set<HangHoa>().Find(chon.MaHang);
                if (h == null)
                {
                    MessageBox.Show("Mặt hàng không còn tồn tại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    TaiDuLieu();
                    return;
                }
                DocTuForm(h);
                db.SaveChanges();
            }
            TaiDuLieu(chon.MaHang);
            MessageBox.Show("Đã cập nhật mặt hàng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Lỗi khi sửa: " + (ex.InnerException?.Message ?? ex.Message), "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnXoa_Click(object? sender, EventArgs e)
    {
        if (dgv.CurrentRow?.DataBoundItem is not HangHoa chon)
        {
            MessageBox.Show("Hãy chọn một dòng trên lưới để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        var kq = MessageBox.Show($"Bạn có chắc muốn xóa \"{chon.TenHang}\"?", "Xác nhận xóa",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (kq != DialogResult.Yes) return;

        try
        {
            using (var db = new TiemTapHoaContext())
            {
                var h = db.Set<HangHoa>().Find(chon.MaHang);
                if (h != null)
                {
                    db.Set<HangHoa>().Remove(h);
                    db.SaveChanges();
                }
            }
            XoaTrangForm();
            TaiDuLieu();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Lỗi khi xóa: " + (ex.InnerException?.Message ?? ex.Message), "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnLamMoi_Click(object? sender, EventArgs e)
    {
        tmrTim.Stop();
        _dangNap = true;
        txtTim.Clear();
        cboLocLoai.SelectedIndex = 0;
        chkChiKD.Checked = false;
        _dangNap = false;

        XoaTrangForm();
        TaiDuLieu();
    }

    // ====================== TÌM KIẾM ======================
    private void txtTim_TextChanged(object? sender, EventArgs e)
    {
        if (_dangNap) return;
        tmrTim.Stop();   // debounce: chờ 350ms sau lần gõ cuối
        tmrTim.Start();
    }

    private void tmrTim_Tick(object? sender, EventArgs e)
    {
        tmrTim.Stop();
        TaiDuLieu();
    }

    private void BoLoc_Changed(object? sender, EventArgs e)
    {
        if (_dangNap) return;
        tmrTim.Stop();
        TaiDuLieu();
    }

    // ====================== FASTREPORT ======================
    private DataTable TaoDataTableBangGia()
    {
        List<HangHoa> ds;
        using (var db = new TiemTapHoaContext())
        {
            ds = db.Set<HangHoa>().AsNoTracking()
                   .Where(h => h.ConKinhDoanh == true)
                   .OrderBy(h => h.MaHang)
                   .ToList();
        }

        var dt = new DataTable("HangHoa");
        dt.Columns.Add("TenHang", typeof(string));
        dt.Columns.Add("DonVi", typeof(string));
        dt.Columns.Add("GiaBan", typeof(decimal));
        foreach (var h in ds)
            dt.Rows.Add(h.TenHang, h.DonVi ?? "", h.GiaBan ?? 0m);
        return dt;
    }

    private FastReport.Report TaoReportBangGia()
    {
        var dt = TaoDataTableBangGia();

        var rpt = new FastReport.Report();
        rpt.Load(Path.Combine(AppContext.BaseDirectory, "BangGia.frx"));
        rpt.RegisterData(dt, "HangHoa");
        rpt.GetDataSource("HangHoa").Enabled = true;
        rpt.SetParameterValue("NgayIn", DateTime.Now.ToString("dd/MM/yyyy"));
        return rpt;
    }

    private void btnXemBangGia_Click(object? sender, EventArgs e)
    {
        try
        {
            using var rpt = TaoReportBangGia();
            rpt.Show();   // cửa sổ FastReport Preview
        }
        catch (Exception ex)
        {
            MessageBox.Show("Không xem được bảng giá: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnXuatPdf_Click(object? sender, EventArgs e)
    {
        using var sfd = new SaveFileDialog
        {
            Filter = "PDF (*.pdf)|*.pdf",
            FileName = "BangGia.pdf"
        };
        if (sfd.ShowDialog() != DialogResult.OK) return;

        try
        {
            using var rpt = TaoReportBangGia();
            rpt.Prepare();
            rpt.Export(new PDFExport(), sfd.FileName);
            HoiMoFile(sfd.FileName);
        }
        catch (IOException)
        {
            MessageBox.Show("Không ghi được file vì file đang được mở ở chương trình khác.\nHãy đóng file rồi thử lại.",
                "File đang mở", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Lỗi khi xuất PDF: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // ====================== EXCEL (ClosedXML) ======================
    private void btnXuatExcelLuoi_Click(object? sender, EventArgs e)
    {
        if (dgv.Rows.Count == 0)
        {
            MessageBox.Show("Không có dữ liệu để xuất.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var wb = new XLWorkbook();
        var ws = wb.AddWorksheet("Danh sách hàng hóa");

        string[] tieuDe = { "Mã hàng", "Tên hàng", "Loại hàng", "Đơn vị", "Giá bán", "Số lượng tồn", "Ngày nhập", "Kinh doanh" };
        for (int c = 0; c < tieuDe.Length; c++)
            ws.Cell(1, c + 1).Value = tieuDe[c];
        ws.Range(1, 1, 1, tieuDe.Length).Style.Font.Bold = true;

        int r = 2;
        // Xuất đúng các dòng đang hiển thị trên lưới
        foreach (DataGridViewRow row in dgv.Rows)
        {
            if (row.DataBoundItem is not HangHoa h) continue;

            ws.Cell(r, 1).Value = h.MaHang;
            ws.Cell(r, 2).Value = h.TenHang;
            ws.Cell(r, 3).Value = h.LoaiHang ?? "";
            ws.Cell(r, 4).Value = h.DonVi ?? "";
            ws.Cell(r, 5).Value = h.GiaBan ?? 0m;          // số, không phải chuỗi
            ws.Cell(r, 6).Value = h.SoLuongTon ?? 0;       // số
            if (h.NgayNhap.HasValue) ws.Cell(r, 7).Value = h.NgayNhap.Value;
            ws.Cell(r, 8).Value = (h.ConKinhDoanh ?? false) ? "Còn bán" : "Ngừng bán";
            r++;
        }

        ws.Column(5).Style.NumberFormat.Format = "#,##0";
        ws.Column(6).Style.NumberFormat.Format = "#,##0";
        ws.Column(7).Style.NumberFormat.Format = "dd/MM/yyyy";
        ws.Columns().AdjustToContents();

        LuuExcel(wb, "DanhSachHangHoa.xlsx");
    }

    private void btnXuatExcelTon_Click(object? sender, EventArgs e)
    {
        List<HangHoa> ds;
        try
        {
            using var db = new TiemTapHoaContext();
            ds = db.Set<HangHoa>().AsNoTracking()
                   .Where(h => h.ConKinhDoanh == true)
                   .OrderBy(h => h.MaHang)
                   .ToList();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Lỗi truy vấn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        if (ds.Count == 0)
        {
            MessageBox.Show("Không có hàng còn kinh doanh để xuất.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var wb = new XLWorkbook();
        var ws = wb.AddWorksheet("Tồn kho");

        string[] tieuDe = { "Mã hàng", "Tên hàng", "Loại hàng", "Đơn vị", "Giá bán", "Số lượng tồn", "Giá trị tồn" };
        for (int c = 0; c < tieuDe.Length; c++)
            ws.Cell(1, c + 1).Value = tieuDe[c];
        ws.Range(1, 1, 1, tieuDe.Length).Style.Font.Bold = true;

        int r = 2;
        foreach (var h in ds)
        {
            ws.Cell(r, 1).Value = h.MaHang;
            ws.Cell(r, 2).Value = h.TenHang;
            ws.Cell(r, 3).Value = h.LoaiHang ?? "";
            ws.Cell(r, 4).Value = h.DonVi ?? "";
            ws.Cell(r, 5).Value = h.GiaBan ?? 0m;
            ws.Cell(r, 6).Value = h.SoLuongTon ?? 0;
            ws.Cell(r, 7).FormulaA1 = $"E{r}*F{r}";        // Giá trị tồn = Giá bán * SL tồn (công thức)
            r++;
        }
        int dongCuoi = r - 1;

        // Dòng Tổng dùng SUM
        ws.Cell(r, 1).Value = "Tổng";
        ws.Cell(r, 7).FormulaA1 = $"SUM(G2:G{dongCuoi})";
        ws.Range(r, 1, r, tieuDe.Length).Style.Font.Bold = true;

        ws.Range(2, 5, r, 7).Style.NumberFormat.Format = "#,##0";

        ws.Range(1, 1, dongCuoi, tieuDe.Length).SetAutoFilter();   // AutoFilter
        ws.SheetView.FreezeRows(1);                                 // Freeze dòng tiêu đề
        ws.Columns().AdjustToContents();

        LuuExcel(wb, "TonKho.xlsx");
    }

    private void LuuExcel(XLWorkbook wb, string tenGoiY)
    {
        using var sfd = new SaveFileDialog
        {
            Filter = "Excel (*.xlsx)|*.xlsx",
            FileName = tenGoiY
        };
        if (sfd.ShowDialog() != DialogResult.OK) return;

        try
        {
            wb.SaveAs(sfd.FileName);
            HoiMoFile(sfd.FileName);
        }
        catch (IOException)
        {
            MessageBox.Show("Không ghi được file vì file đang được mở trong Excel (hoặc chương trình khác).\nHãy đóng file rồi thử lại.",
                "File đang mở", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (UnauthorizedAccessException)
        {
            MessageBox.Show("Không có quyền ghi vào vị trí này hoặc file đang bị khóa.\nHãy đóng file / chọn nơi khác rồi thử lại.",
                "Không ghi được file", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Lỗi khi xuất Excel: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static void HoiMoFile(string path)
    {
        var kq = MessageBox.Show("Xuất file thành công. Bạn có muốn mở file không?", "Hoàn tất",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (kq == DialogResult.Yes)
            Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
    }
}
