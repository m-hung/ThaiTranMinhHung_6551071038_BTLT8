namespace TiemTapHoa;

partial class FormQuanLyHangHoa
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();

        tmrTim = new System.Windows.Forms.Timer(components) { Interval = 350 };
        tmrTim.Tick += tmrTim_Tick;

        // ===== GroupBox thông tin hàng hóa =====
        lblTen = new Label { Text = "Tên hàng:", AutoSize = true, Location = new Point(15, 28) };
        txtTenHang = new TextBox { Location = new Point(90, 25), Size = new Size(450, 23), MaxLength = 100 };

        lblLoai = new Label { Text = "Loại hàng:", AutoSize = true, Location = new Point(15, 62) };
        cboLoaiHang = new ComboBox { Location = new Point(90, 59), Size = new Size(150, 23), DropDownStyle = ComboBoxStyle.DropDownList };

        lblTon = new Label { Text = "Số lượng tồn:", AutoSize = true, Location = new Point(270, 62) };
        nudTon = new NumericUpDown { Location = new Point(365, 59), Size = new Size(175, 23), Minimum = 0, Maximum = 1000000, Increment = 500, ThousandsSeparator = true };

        lblDonVi = new Label { Text = "Đơn vị:", AutoSize = true, Location = new Point(15, 96) };
        cboDonVi = new ComboBox { Location = new Point(90, 93), Size = new Size(150, 23), DropDownStyle = ComboBoxStyle.DropDownList };

        lblNgay = new Label { Text = "Ngày nhập:", AutoSize = true, Location = new Point(270, 96) };
        dtpNgayNhap = new DateTimePicker { Location = new Point(365, 93), Size = new Size(175, 23), Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy" };

        lblGia = new Label { Text = "Giá bán:", AutoSize = true, Location = new Point(15, 130) };
        nudGiaBan = new NumericUpDown { Location = new Point(90, 127), Size = new Size(150, 23), Minimum = 0, Maximum = 1000000000, Increment = 500, DecimalPlaces = 0, ThousandsSeparator = true };

        chkKinhDoanh = new CheckBox { Text = "Còn kinh doanh", AutoSize = true, Checked = true, Location = new Point(365, 129) };

        grpThongTin = new GroupBox { Text = "Thông tin hàng hóa", Location = new Point(12, 8), Size = new Size(560, 170) };
        grpThongTin.Controls.AddRange(new Control[]
        {
            lblTen, txtTenHang, lblLoai, cboLoaiHang, lblTon, nudTon,
            lblDonVi, cboDonVi, lblNgay, dtpNgayNhap, lblGia, nudGiaBan, chkKinhDoanh
        });

        // ===== GroupBox tìm kiếm =====
        txtTim = new TextBox { Location = new Point(15, 25), Size = new Size(270, 23) };
        txtTim.TextChanged += txtTim_TextChanged;

        cboLocLoai = new ComboBox { Location = new Point(15, 59), Size = new Size(270, 23), DropDownStyle = ComboBoxStyle.DropDownList };
        cboLocLoai.SelectedIndexChanged += BoLoc_Changed;

        chkChiKD = new CheckBox { Text = "Chỉ hiện hàng còn kinh doanh", AutoSize = true, Location = new Point(15, 96) };
        chkChiKD.CheckedChanged += BoLoc_Changed;

        grpTim = new GroupBox { Text = "Tìm kiếm", Location = new Point(588, 8), Size = new Size(300, 170), Anchor = AnchorStyles.Top | AnchorStyles.Right };
        grpTim.Controls.AddRange(new Control[] { txtTim, cboLocLoai, chkChiKD });

        pnlTop = new Panel { Dock = DockStyle.Top, Height = 186 };
        pnlTop.Controls.Add(grpThongTin);
        pnlTop.Controls.Add(grpTim);

        // ===== DataGridView =====
        dgv = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoGenerateColumns = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            MultiSelect = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            BackgroundColor = SystemColors.Window
        };
        dgv.Columns.AddRange(new DataGridViewColumn[]
        {
            new DataGridViewTextBoxColumn { Name = "colMa", HeaderText = "Mã", DataPropertyName = "MaHang", Width = 50 },
            new DataGridViewTextBoxColumn { Name = "colTen", HeaderText = "Tên hàng", DataPropertyName = "TenHang", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill },
            new DataGridViewTextBoxColumn { Name = "colLoai", HeaderText = "Loại hàng", DataPropertyName = "LoaiHang", Width = 110 },
            new DataGridViewTextBoxColumn { Name = "colDonVi", HeaderText = "Đơn vị", DataPropertyName = "DonVi", Width = 70 },
            new DataGridViewTextBoxColumn { Name = "colGia", HeaderText = "Giá bán", DataPropertyName = "GiaBan", Width = 90,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight } },
            new DataGridViewTextBoxColumn { Name = "colTon", HeaderText = "Tồn", DataPropertyName = "SoLuongTon", Width = 70,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight } },
            new DataGridViewTextBoxColumn { Name = "colNgay", HeaderText = "Ngày nhập", DataPropertyName = "NgayNhap", Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" } },
            new DataGridViewCheckBoxColumn { Name = "colKD", HeaderText = "Kinh doanh", DataPropertyName = "ConKinhDoanh", Width = 80, ThreeState = false }
        });
        dgv.SelectionChanged += dgv_SelectionChanged;

        // ===== Các nút =====
        btnThem = TaoNut("Thêm", 80, btnThem_Click);
        btnSua = TaoNut("Sửa", 80, btnSua_Click);
        btnXoa = TaoNut("Xóa", 80, btnXoa_Click);
        btnLamMoi = TaoNut("Làm mới", 80, btnLamMoi_Click);
        btnXemBangGia = TaoNut("Xem bảng giá", 105, btnXemBangGia_Click);
        btnXuatPdf = TaoNut("Xuất PDF", 90, btnXuatPdf_Click);
        btnXuatExcelLuoi = TaoNut("Xuất Excel (lưới)", 125, btnXuatExcelLuoi_Click);
        btnXuatExcelTon = TaoNut("Xuất Excel tồn kho", 140, btnXuatExcelTon_Click);

        flpNut = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 46, Padding = new Padding(8, 8, 0, 0) };
        flpNut.Controls.AddRange(new Control[]
        {
            btnThem, btnSua, btnXoa, btnLamMoi, btnXemBangGia, btnXuatPdf, btnXuatExcelLuoi, btnXuatExcelTon
        });

        // ===== Status =====
        lblTrangThai = new ToolStripStatusLabel { Text = "Sẵn sàng" };
        ss = new StatusStrip();
        ss.Items.Add(lblTrangThai);

        // ===== Form =====
        // Thứ tự Add quan trọng: control Fill phải add đầu tiên
        Controls.Add(dgv);
        Controls.Add(flpNut);
        Controls.Add(ss);
        Controls.Add(pnlTop);

        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(900, 600);
        MinimumSize = new Size(920, 520);
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Quản lý hàng hóa - Tạp hóa UTC2";
        Name = "FormQuanLyHangHoa";
        Load += FormQuanLyHangHoa_Load;
    }

    private static Button TaoNut(string text, int width, EventHandler onClick)
    {
        var b = new Button { Text = text, Size = new Size(width, 30) };
        b.Click += onClick;
        return b;
    }

    #endregion

    private GroupBox grpThongTin, grpTim;
    private Panel pnlTop;
    private Label lblTen, lblLoai, lblDonVi, lblGia, lblTon, lblNgay;
    private TextBox txtTenHang, txtTim;
    private ComboBox cboLoaiHang, cboDonVi, cboLocLoai;
    private NumericUpDown nudGiaBan, nudTon;
    private DateTimePicker dtpNgayNhap;
    private CheckBox chkKinhDoanh, chkChiKD;
    private DataGridView dgv;
    private Button btnThem, btnSua, btnXoa, btnLamMoi, btnXemBangGia, btnXuatPdf, btnXuatExcelLuoi, btnXuatExcelTon;
    private FlowLayoutPanel flpNut;
    private StatusStrip ss;
    private ToolStripStatusLabel lblTrangThai;
    private System.Windows.Forms.Timer tmrTim;
}