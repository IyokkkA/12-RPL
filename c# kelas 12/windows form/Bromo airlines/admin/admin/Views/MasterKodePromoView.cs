using System.Data;
using Microsoft.Data.SqlClient;

namespace admin.Views;

public sealed class MasterKodePromoView : UserControl
{
    private readonly DataGridView _grid;
    private readonly TextBox _txtKode;
    private readonly DateTimePicker _dtpBerlaku;
    private readonly NumericUpDown _numPersen;
    private readonly NumericUpDown _numMaks;
    private readonly TextBox _txtDeskripsi;
    private readonly Button _btnSimpan;
    private readonly Button _btnBatal;

    private int? _editId;

    public MasterKodePromoView()
    {
        BackColor = Color.White;
        Font = new Font("Segoe UI", 9F);

        var title = new Label
        {
            Text = "Master Kode Promo",
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(20, 15)
        };
        var subtitle = new Label
        {
            Text = "Semua kode promo yang terdaftar akan muncul di sini",
            AutoSize = true,
            Location = new Point(22, 48)
        };

        _grid = new DataGridView
        {
            Location = new Point(20, 75),
            Size = new Size(740, 220),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };
        _grid.CellContentClick += Grid_CellContentClick;

        var lblKode = new Label { Text = "Kode Promo", AutoSize = true, Location = new Point(20, 312) };
        _txtKode = new TextBox { Location = new Point(150, 309), Size = new Size(200, 23), CharacterCasing = CharacterCasing.Upper };

        var lblBerlaku = new Label { Text = "Berlaku Sampai", AutoSize = true, Location = new Point(20, 342) };
        _dtpBerlaku = new DateTimePicker
        {
            Location = new Point(150, 339),
            Size = new Size(200, 23),
            Format = DateTimePickerFormat.Custom,
            CustomFormat = "dd-MM-yyyy"
        };

        var lblPersen = new Label { Text = "Persentase Diskon", AutoSize = true, Location = new Point(20, 372) };
        _numPersen = new NumericUpDown
        {
            Location = new Point(150, 369),
            Size = new Size(200, 23),
            Minimum = 1,
            Maximum = 100,
            Value = 1
        };

        var lblMaks = new Label { Text = "Maksimum Diskon", AutoSize = true, Location = new Point(20, 402) };
        _numMaks = new NumericUpDown
        {
            Location = new Point(150, 399),
            Size = new Size(200, 23),
            Minimum = 1,
            Maximum = 1000000000,
            Value = 1
        };

        var lblDeskripsi = new Label { Text = "Deskripsi", AutoSize = true, Location = new Point(380, 312) };
        _txtDeskripsi = new TextBox
        {
            Location = new Point(490, 309),
            Size = new Size(250, 113),
            Multiline = true,
            ScrollBars = ScrollBars.Vertical
        };

        _btnBatal = new Button { Text = "Batal", Location = new Point(490, 435), Size = new Size(115, 30) };
        _btnSimpan = new Button { Text = "Simpan", Location = new Point(625, 435), Size = new Size(115, 30) };
        _btnBatal.Click += (_, _) => ResetForm();
        _btnSimpan.Click += (_, _) => Simpan();

        Controls.AddRange(
        [
            title, subtitle, _grid,
            lblKode, _txtKode,
            lblBerlaku, _dtpBerlaku,
            lblPersen, _numPersen,
            lblMaks, _numMaks,
            lblDeskripsi, _txtDeskripsi,
            _btnBatal, _btnSimpan
        ]);

        if (!DesignMode)
            LoadData();
        Load += (_, _) => LoadData();
    }

    private void LoadData()
    {
        try
        {
            using var conn = Db.GetConnection();
            conn.Open();
            const string sql = "SELECT ID, Kode, PersentaseDiskon, MaksimumDiskon, BerlakuSampai, Deskripsi FROM KodePromo ORDER BY Kode ASC";
            using var da = new SqlDataAdapter(sql, conn);
            var raw = new DataTable();
            da.Fill(raw);

            var dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("Kode", typeof(string));
            dt.Columns.Add("Persen", typeof(double));
            dt.Columns.Add("Maksimum", typeof(double));
            dt.Columns.Add("Berlaku", typeof(string));
            dt.Columns.Add("Deskripsi", typeof(string));

            foreach (DataRow r in raw.Rows)
                dt.Rows.Add(r["ID"], r["Kode"], Convert.ToDouble(r["PersentaseDiskon"]),
                    Convert.ToDouble(r["MaksimumDiskon"]),
                    Convert.ToDateTime(r["BerlakuSampai"]).ToString("dd-MM-yyyy"),
                    r["Deskripsi"]);

            _grid.Columns.Clear();
            _grid.DataSource = dt;
            _grid.Columns["ID"]!.Visible = false;

            _grid.Columns.Add(new DataGridViewButtonColumn
            {
                HeaderText = "",
                Text = "Ubah",
                UseColumnTextForButtonValue = true,
                Width = 70,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            });
            _grid.Columns.Add(new DataGridViewButtonColumn
            {
                HeaderText = "",
                Text = "Hapus",
                UseColumnTextForButtonValue = true,
                Width = 70,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show("Gagal memuat kode promo: " + ex.Message);
        }
    }

    private void Grid_CellContentClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0)
            return;
        if (_grid.Columns[e.ColumnIndex] is not DataGridViewButtonColumn)
            return;
        var id = Convert.ToInt32(_grid.Rows[e.RowIndex].Cells["ID"].Value);
        var isUbah = e.ColumnIndex == _grid.Columns.Count - 2;
        if (isUbah)
            MasukModeUbah(id);
        else
            HapusData(id);
    }

    private void MasukModeUbah(int id)
    {
        try
        {
            using var conn = Db.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("SELECT Kode, PersentaseDiskon, MaksimumDiskon, BerlakuSampai, Deskripsi FROM KodePromo WHERE ID=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var r = cmd.ExecuteReader();
            if (!r.Read())
                return;
            _editId = id;
            _txtKode.Text = r["Kode"].ToString();
            _numPersen.Value = Math.Max(1, Convert.ToDecimal(r["PersentaseDiskon"]));
            _numMaks.Value = Math.Max(1, Convert.ToDecimal(r["MaksimumDiskon"]));
            _dtpBerlaku.Value = Convert.ToDateTime(r["BerlakuSampai"]);
            _txtDeskripsi.Text = r["Deskripsi"].ToString();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Gagal memuat data: " + ex.Message);
        }
    }

    private void Simpan()
    {
        var kode = _txtKode.Text.Trim().ToUpper();
        if (kode == "")
        {
            MessageBox.Show("Kode promo harus diisi.");
            return;
        }
        if (kode != _txtKode.Text.Trim())
            _txtKode.Text = kode;
        if (_txtDeskripsi.Text.Trim() == "")
        {
            MessageBox.Show("Deskripsi harus diisi.");
            return;
        }

        try
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using (var cek = new SqlCommand(
                "SELECT COUNT(*) FROM KodePromo WHERE Kode = @kode AND (@id IS NULL OR ID <> @id)", conn))
            {
                cek.Parameters.AddWithValue("@kode", kode);
                cek.Parameters.AddWithValue("@id", (object?)_editId ?? DBNull.Value);
                if (Convert.ToInt32(cek.ExecuteScalar()) > 0)
                {
                    MessageBox.Show("Kode promo sudah ada (tidak boleh duplikat).");
                    return;
                }
            }

            if (_editId is int id)
            {
                using var cmd = new SqlCommand(
                    "UPDATE KodePromo SET Kode=@k, PersentaseDiskon=@p, MaksimumDiskon=@m, BerlakuSampai=@b, Deskripsi=@d WHERE ID=@id", conn);
                cmd.Parameters.AddWithValue("@k", kode);
                cmd.Parameters.AddWithValue("@p", (double)_numPersen.Value);
                cmd.Parameters.AddWithValue("@m", (double)_numMaks.Value);
                cmd.Parameters.AddWithValue("@b", _dtpBerlaku.Value.Date);
                cmd.Parameters.AddWithValue("@d", _txtDeskripsi.Text.Trim());
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Perubahan berhasil disimpan.");
            }
            else
            {
                using var cmd = new SqlCommand(
                    "INSERT INTO KodePromo (Kode, PersentaseDiskon, MaksimumDiskon, BerlakuSampai, Deskripsi) VALUES (@k,@p,@m,@b,@d)", conn);
                cmd.Parameters.AddWithValue("@k", kode);
                cmd.Parameters.AddWithValue("@p", (double)_numPersen.Value);
                cmd.Parameters.AddWithValue("@m", (double)_numMaks.Value);
                cmd.Parameters.AddWithValue("@b", _dtpBerlaku.Value.Date);
                cmd.Parameters.AddWithValue("@d", _txtDeskripsi.Text.Trim());
                cmd.ExecuteNonQuery();
                MessageBox.Show("Kode promo berhasil disimpan.");
            }

            ResetForm();
            LoadData();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Gagal menyimpan: " + ex.Message);
        }
    }

    private void HapusData(int id)
    {
        var kode = _grid.CurrentRow?.Cells["Kode"].Value?.ToString() ?? id.ToString();
        if (MessageBox.Show($"Hapus kode promo '{kode}'?", "Konfirmasi Hapus",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            return;
        try
        {
            using var conn = Db.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("DELETE FROM KodePromo WHERE ID=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
            if (_editId == id)
                ResetForm();
            LoadData();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Gagal menghapus (data mungkin dipakai transaksi): " + ex.Message);
        }
    }

    private void ResetForm()
    {
        _editId = null;
        _txtKode.Clear();
        _txtDeskripsi.Clear();
        _numPersen.Value = 1;
        _numMaks.Value = 1;
        _dtpBerlaku.Value = DateTime.Today;
    }
}
