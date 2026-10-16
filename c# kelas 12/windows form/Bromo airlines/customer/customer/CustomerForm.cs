using System.Data;
using Microsoft.Data.SqlClient;

namespace customer;

public sealed class CustomerForm : Form
{
    private readonly int _akunId;
    private readonly string _nama;

    private readonly TextBox _txtDari;
    private readonly TextBox _txtKe;
    private readonly DateTimePicker _dtpTanggal;
    private readonly NumericUpDown _numPenumpang;

    private DataTable _bandaraTable = new();

    public CustomerForm(int akunId, string nama)
    {
        _akunId = akunId;
        _nama = nama;

        Text = "Bromo Airlines - Customer";
        ClientSize = new Size(860, 520);
        MinimumSize = new Size(860, 520);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.White;
        Font = new Font("Segoe UI", 9F);

        // ===== Top bar =====
        var topBar = new Panel
        {
            Dock = DockStyle.Top,
            Height = 90,
            BackColor = Color.FromArgb(29, 87, 171)
        };

        var logo = new PictureBox
        {
            Location = new Point(20, 12),
            Size = new Size(220, 66),
            SizeMode = PictureBoxSizeMode.Zoom,
            BackColor = Color.Transparent
        };
        try
        {
            var p = Path.Combine(AppContext.BaseDirectory, "Resources", "Logo Rect Alt Without Tagline.png");
            if (File.Exists(p))
                logo.Image = Image.FromFile(p);
        }
        catch { }

        var btnLogout = new Button
        {
            Text = "Logout",
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            FlatStyle = FlatStyle.Flat,
            Size = new Size(80, 40),
            Location = new Point(760, 25),
            Cursor = Cursors.Hand,
            Image = LoadIcon("log-out-regular-72.png", 22),
            ImageAlign = ContentAlignment.MiddleLeft,
            TextAlign = ContentAlignment.MiddleRight
        };
        btnLogout.FlatAppearance.BorderSize = 0;
        btnLogout.Click += (_, _) => Logout();

        var btnTiket = new Button
        {
            Text = "Tiket",
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            FlatStyle = FlatStyle.Flat,
            Size = new Size(80, 40),
            Location = new Point(670, 25),
            Cursor = Cursors.Hand,
            Image = LoadIcon("airplane-ticket.png", 22),
            ImageAlign = ContentAlignment.MiddleLeft,
            TextAlign = ContentAlignment.MiddleRight
        };
        btnTiket.FlatAppearance.BorderSize = 0;
        btnTiket.Click += (_, _) => BukaTiketSaya();

        topBar.Controls.AddRange([logo, btnTiket, btnLogout]);

        // ===== Content =====
        var lblHalo = new Label
        {
            Text = "Halo!",
            Font = new Font("Segoe UI", 20F, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(25, 110)
        };
        var lblSub = new Label
        {
            Text = $"Mau terbang ke mana hari ini, {_nama}?",
            AutoSize = true,
            Location = new Point(27, 150)
        };

        var lblDari = new Label { Text = "Berangkat Dari", AutoSize = true, Location = new Point(60, 195) };
        _txtDari = new TextBox { Location = new Point(60, 218), Size = new Size(300, 23) };
        SetupAutocomplete(_txtDari);

        var lblKe = new Label { Text = "Tujuan", AutoSize = true, Location = new Point(430, 195) };
        _txtKe = new TextBox { Location = new Point(430, 218), Size = new Size(300, 23) };
        SetupAutocomplete(_txtKe);

        var lblTanggal = new Label { Text = "Tanggal Berangkat", AutoSize = true, Location = new Point(60, 260) };
        _dtpTanggal = new DateTimePicker
        {
            Location = new Point(60, 283),
            Size = new Size(300, 23),
            Format = DateTimePickerFormat.Custom,
            CustomFormat = "dddd, dd MMMM yyyy",
            MinDate = DateTime.Today
        };

        var lblPenumpang = new Label { Text = "Jumlah Penumpang", AutoSize = true, Location = new Point(430, 260) };
        _numPenumpang = new NumericUpDown
        {
            Location = new Point(430, 283),
            Size = new Size(300, 23),
            Minimum = 1,
            Maximum = 20,
            Value = 1
        };

        var btnCari = new Button { Text = "Cari Penerbangan", Location = new Point(60, 330), Size = new Size(150, 34) };
        btnCari.Click += (_, _) => CariPenerbangan();

        var mountain = new PictureBox
        {
            Size = new Size(280, 160),
            Location = new Point(580, 360),
            SizeMode = PictureBoxSizeMode.Zoom,
            BackColor = Color.Transparent,
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right
        };
        try
        {
            var p = Path.Combine(AppContext.BaseDirectory, "Resources", "Icons", "mountain.png");
            if (File.Exists(p))
                mountain.Image = Image.FromFile(p);
        }
        catch { }

        Controls.Add(topBar);
        Controls.AddRange([lblHalo, lblSub, lblDari, _txtDari, lblKe, _txtKe,
            lblTanggal, _dtpTanggal, lblPenumpang, _numPenumpang, btnCari, mountain]);

        if (!DesignMode)
            LoadBandara();
        Load += (_, _) => LoadBandara();

        FormClosed += (_, _) => Application.Exit();
    }

    private void SetupAutocomplete(TextBox txt)
    {
        txt.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        txt.AutoCompleteSource = AutoCompleteSource.CustomSource;
    }

    private void LoadBandara()
    {
        try
        {
            using var conn = Db.GetConnection();
            conn.Open();
            using var da = new SqlDataAdapter("SELECT ID, Nama, KodeIATA, Kota FROM Bandara ORDER BY Nama", conn);
            _bandaraTable = new DataTable();
            da.Fill(_bandaraTable);

            var src = new AutoCompleteStringCollection();
            foreach (DataRow r in _bandaraTable.Rows)
                src.Add($"{r["Nama"]} ({r["KodeIATA"]}), {r["Kota"]}");
            _txtDari.AutoCompleteCustomSource = src;
            _txtKe.AutoCompleteCustomSource = src;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Gagal memuat bandara: " + ex.Message);
        }
    }

    private int? ResolveBandaraId(string text)
    {
        if (_bandaraTable.Rows.Count == 0 || string.IsNullOrWhiteSpace(text))
            return null;
        // coba cocok KodeIATA dalam kurung
        var start = text.IndexOf('(');
        var end = text.IndexOf(')');
        if (start >= 0 && end > start)
        {
            var kode = text.Substring(start + 1, end - start - 1).Trim();
            foreach (DataRow r in _bandaraTable.Rows)
                if (r["KodeIATA"].ToString()!.Equals(kode, StringComparison.OrdinalIgnoreCase))
                    return Convert.ToInt32(r["ID"]);
        }
        // fallback: nama persis / mengandung
        foreach (DataRow r in _bandaraTable.Rows)
            if (r["Nama"].ToString()!.Equals(text.Trim(), StringComparison.OrdinalIgnoreCase))
                return Convert.ToInt32(r["ID"]);
        foreach (DataRow r in _bandaraTable.Rows)
            if (text.Contains(r["Nama"].ToString()!, StringComparison.OrdinalIgnoreCase))
                return Convert.ToInt32(r["ID"]);
        return null;
    }

    private void CariPenerbangan()
    {
        var dari = ResolveBandaraId(_txtDari.Text);
        var ke = ResolveBandaraId(_txtKe.Text);

        if (dari is null || ke is null)
        {
            MessageBox.Show("Pilih bandara keberangkatan dan tujuan dari daftar auto-complete.");
            return;
        }
        if (dari == ke)
        {
            MessageBox.Show("Bandara keberangkatan dan tujuan tidak boleh sama.");
            return;
        }

        Hide();
        using var list = new ListPenerbanganForm(dari.Value, ke.Value, _dtpTanggal.Value.Date, (int)_numPenumpang.Value);
        list.ShowDialog();
        Show();
    }

    private void BukaTiketSaya()
    {
        Hide();
        using var tiket = new TiketSayaForm(_akunId, _nama);
        tiket.ShowDialog();
        Show();
    }

    private void Logout()
    {
        Hide();
        using var login = new LoginForm();
        login.ShowDialog();
        Close();
    }

    private static Image? LoadIcon(string fileName, int size)
    {
        try
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Resources", "Icons", fileName);
            if (!File.Exists(path))
                return null;
            using var original = Image.FromFile(path);
            var thumb = new Bitmap(size, size);
            using var g = Graphics.FromImage(thumb);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(original, 0, 0, size, size);
            return thumb;
        }
        catch
        {
            return null;
        }
    }
}

