using System.Data;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

namespace admin.Views;

public sealed class MasterJadwalView : UserControl
{
    private readonly DataGridView _grid;
    private readonly TextBox _txtKode;
    private readonly ComboBox _cmbDari;
    private readonly ComboBox _cmbKe;
    private readonly ComboBox _cmbMaskapai;
    private readonly DateTimePicker _dtpTanggal;
    private readonly DateTimePicker _dtpWaktu;
    private readonly TextBox _txtDurasi;
    private readonly NumericUpDown _numHarga;
    private readonly Button _btnSimpan;
    private readonly Button _btnBatal;

    private int? _editId;

    public MasterJadwalView()
    {
        BackColor = Color.White;
        Font = new Font("Segoe UI", 9F);

        var title = new Label
        {
            Text = "Master Jadwal Penerbangan",
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(20, 15)
        };
        var subtitle = new Label
        {
            Text = "Semua jadwal penerbangan akan muncul di sini",
            AutoSize = true,
            Location = new Point(22, 48)
        };

        _grid = new DataGridView
        {
            Location = new Point(20, 75),
            Size = new Size(740, 200),
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

        // ===== Kiri =====
        var lblKode = new Label { Text = "Kode Penerbangan", AutoSize = true, Location = new Point(20, 290) };
        _txtKode = new TextBox { Location = new Point(150, 287), Size = new Size(200, 23) };

        var lblDari = new Label { Text = "Dari", AutoSize = true, Location = new Point(20, 320) };
        _cmbDari = new ComboBox { Location = new Point(150, 317), Size = new Size(200, 23), DropDownStyle = ComboBoxStyle.DropDownList };

        var lblKe = new Label { Text = "Ke", AutoSize = true, Location = new Point(20, 350) };
        _cmbKe = new ComboBox { Location = new Point(150, 347), Size = new Size(200, 23), DropDownStyle = ComboBoxStyle.DropDownList };

        var lblMaskapai = new Label { Text = "Maskapai", AutoSize = true, Location = new Point(20, 380) };
        _cmbMaskapai = new ComboBox { Location = new Point(150, 377), Size = new Size(200, 23), DropDownStyle = ComboBoxStyle.DropDownList };

        // ===== Kanan =====
        var lblTanggal = new Label { Text = "Tanggal", AutoSize = true, Location = new Point(380, 290) };
        _dtpTanggal = new DateTimePicker
        {
            Location = new Point(510, 287),
            Size = new Size(230, 23),
            Format = DateTimePickerFormat.Short
        };

        var lblWaktu = new Label { Text = "Waktu Keberangkatan", AutoSize = true, Location = new Point(380, 320) };
        _dtpWaktu = new DateTimePicker
        {
            Location = new Point(510, 317),
            Size = new Size(230, 23),
            Format = DateTimePickerFormat.Time,
            ShowUpDown = true
        };

        var lblDurasi = new Label { Text = "Durasi Penerbangan", AutoSize = true, Location = new Point(380, 350) };
        _txtDurasi = new TextBox { Location = new Point(510, 347), Size = new Size(230, 23), PlaceholderText = "cth: 2 jam 30 menit" };

        var lblHarga = new Label { Text = "Harga per Tiket", AutoSize = true, Location = new Point(380, 380) };
        _numHarga = new NumericUpDown
        {
            Location = new Point(510, 377),
            Size = new Size(230, 23),
            Minimum = 1,
            Maximum = 100000000,
            Value = 1
        };

        _btnBatal = new Button { Text = "Batal", Location = new Point(510, 412), Size = new Size(105, 30) };
        _btnSimpan = new Button { Text = "Simpan", Location = new Point(635, 412), Size = new Size(105, 30) };
        _btnBatal.Click += (_, _) => ResetForm();
        _btnSimpan.Click += (_, _) => Simpan();

        Controls.AddRange(
        [
            title, subtitle, _grid,
            lblKode, _txtKode,
            lblDari, _cmbDari,
            lblKe, _cmbKe,
            lblMaskapai, _cmbMaskapai,
            lblTanggal, _dtpTanggal,
            lblWaktu, _dtpWaktu,
            lblDurasi, _txtDurasi,
            lblHarga, _numHarga,
            _btnBatal, _btnSimpan
        ]);

        if (!DesignMode)
            Init();
        Load += (_, _) => Init();
    }

    private void Init()
    {
        LoadCombo();
        LoadData();
    }

    private void LoadCombo()
    {
        try
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using (var da = new SqlDataAdapter("SELECT ID, Nama + ' (' + KodeIATA + ')' AS Tampil FROM Bandara ORDER BY Nama", conn))
            {
                var dt = new DataTable();
                da.Fill(dt);
                _cmbDari.DisplayMember = "Tampil";
                _cmbDari.ValueMember = "ID";
                _cmbDari.DataSource = dt.Copy();
                _cmbKe.DisplayMember = "Tampil";
                _cmbKe.ValueMember = "ID";
                _cmbKe.DataSource = dt;
                _cmbDari.SelectedIndex = -1;
                _cmbKe.SelectedIndex = -1;
            }

            using (var da = new SqlDataAdapter("SELECT ID, Nama FROM Maskapai ORDER BY Nama", conn))
            {
                var dt = new DataTable();
                da.Fill(dt);
                _cmbMaskapai.DisplayMember = "Nama";
                _cmbMaskapai.ValueMember = "ID";
                _cmbMaskapai.DataSource = dt;
                _cmbMaskapai.SelectedIndex = -1;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Gagal memuat combo: " + ex.Message);
        }
    }

    private void LoadData()
    {
        try
        {
            using var conn = Db.GetConnection();
            conn.Open();
            const string sql = """
                SELECT j.ID, j.KodePenerbangan,
                       b1.KodeIATA AS Berangkat, b2.KodeIATA AS Tujuan,
                       m.Nama AS Maskapai, j.TanggalWaktuKeberangkatan,
                       j.DurasiPenerbangan, j.HargaPerTiket
                FROM JadwalPenerbangan j
                JOIN Bandara b1 ON j.BandaraKeberangkatanID = b1.ID
                JOIN Bandara b2 ON j.BandaraTujuanID = b2.ID
                JOIN Maskapai m ON j.MaskapaiID = m.ID
                ORDER BY j.TanggalWaktuKeberangkatan DESC
                """;
            using var da = new SqlDataAdapter(sql, conn);
            var raw = new DataTable();
            da.Fill(raw);

            var dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("KodePenerbangan", typeof(string));
            dt.Columns.Add("Berangkat", typeof(string));
            dt.Columns.Add("Tujuan", typeof(string));
            dt.Columns.Add("Maskapai", typeof(string));
            dt.Columns.Add("Tanggal", typeof(string));
            dt.Columns.Add("Waktu", typeof(string));
            dt.Columns.Add("Durasi", typeof(string));
            dt.Columns.Add("Harga", typeof(double));

            foreach (DataRow r in raw.Rows)
            {
                var dtw = Convert.ToDateTime(r["TanggalWaktuKeberangkatan"]);
                var menit = Convert.ToInt32(r["DurasiPenerbangan"]);
                dt.Rows.Add(
                    r["ID"],
                    r["KodePenerbangan"],
                    r["Berangkat"],
                    r["Tujuan"],
                    r["Maskapai"],
                    dtw.ToString("dd-MM-yyyy"),
                    dtw.ToString("HH:mm"),
                    $"{menit / 60} jam {menit % 60} menit",
                    Convert.ToDouble(r["HargaPerTiket"]));
            }

            _grid.Columns.Clear();
            _grid.DataSource = dt;
            _grid.Columns["ID"]!.Visible = false;

            _grid.Columns["KodePenerbangan"]!.FillWeight = 65;
            _grid.Columns["KodePenerbangan"]!.MinimumWidth = 55;
            _grid.Columns["Berangkat"]!.FillWeight = 55;
            _grid.Columns["Berangkat"]!.MinimumWidth = 45;
            _grid.Columns["Tujuan"]!.FillWeight = 55;
            _grid.Columns["Tujuan"]!.MinimumWidth = 45;
            _grid.Columns["Maskapai"]!.FillWeight = 80;
            _grid.Columns["Maskapai"]!.MinimumWidth = 60;
            _grid.Columns["Tanggal"]!.FillWeight = 65;
            _grid.Columns["Tanggal"]!.MinimumWidth = 60;
            _grid.Columns["Waktu"]!.FillWeight = 45;
            _grid.Columns["Waktu"]!.MinimumWidth = 40;
            _grid.Columns["Durasi"]!.FillWeight = 75;
            _grid.Columns["Durasi"]!.MinimumWidth = 60;
            _grid.Columns["Harga"]!.FillWeight = 70;
            _grid.Columns["Harga"]!.MinimumWidth = 55;
            foreach (DataGridViewColumn c in _grid.Columns)
                c.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            _grid.Columns.Add(new DataGridViewButtonColumn
            {
                HeaderText = "",
                Text = "Ubah",
                UseColumnTextForButtonValue = true,
                Width = 60,
                MinimumWidth = 60,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            });
            _grid.Columns.Add(new DataGridViewButtonColumn
            {
                HeaderText = "",
                Text = "Hapus",
                UseColumnTextForButtonValue = true,
                Width = 60,
                MinimumWidth = 60,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show("Gagal memuat jadwal: " + ex.Message);
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
            using var cmd = new SqlCommand(
                "SELECT KodePenerbangan, BandaraKeberangkatanID, BandaraTujuanID, MaskapaiID, TanggalWaktuKeberangkatan, DurasiPenerbangan, HargaPerTiket FROM JadwalPenerbangan WHERE ID=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var r = cmd.ExecuteReader();
            if (!r.Read())
                return;
            _editId = id;
            _txtKode.Text = r["KodePenerbangan"].ToString();
            _cmbDari.SelectedValue = Convert.ToInt32(r["BandaraKeberangkatanID"]);
            _cmbKe.SelectedValue = Convert.ToInt32(r["BandaraTujuanID"]);
            _cmbMaskapai.SelectedValue = Convert.ToInt32(r["MaskapaiID"]);
            var dtw = Convert.ToDateTime(r["TanggalWaktuKeberangkatan"]);
            _dtpTanggal.Value = dtw.Date;
            _dtpWaktu.Value = DateTime.Today.Date + dtw.TimeOfDay;
            var menit = Convert.ToInt32(r["DurasiPenerbangan"]);
            _txtDurasi.Text = $"{menit / 60} jam {menit % 60} menit";
            _numHarga.Value = Math.Max(1, Convert.ToDecimal(r["HargaPerTiket"]));
        }
        catch (Exception ex)
        {
            MessageBox.Show("Gagal memuat data: " + ex.Message);
        }
    }

    private static bool CobaParseDurasi(string s, out int totalMenit)
    {
        totalMenit = 0;
        var m = Regex.Match(s.Trim(), @"^(\d+)\s*jam\s*(\d+)\s*menit$", RegexOptions.IgnoreCase);
        if (!m.Success)
            return false;
        if (!int.TryParse(m.Groups[1].Value, out var jam))
            return false;
        if (!int.TryParse(m.Groups[2].Value, out var menit))
            return false;
        if (jam < 0 || menit < 0 || menit >= 60)
            return false;
        totalMenit = jam * 60 + menit;
        return totalMenit > 0;
    }

    private void Simpan()
    {
        var kode = _txtKode.Text.Trim().ToUpper();

        if (!Regex.IsMatch(kode, @"^[A-Z]{2}-\d{4}$"))
        {
            MessageBox.Show("Kode penerbangan harus format: 2 huruf - 4 angka (cth: GA-0325).");
            return;
        }
        if (_cmbDari.SelectedValue is not int dariId || _cmbKe.SelectedValue is not int keId || _cmbMaskapai.SelectedValue is not int maskapaiId)
        {
            MessageBox.Show("Pilih bandara keberangkatan, tujuan, dan maskapai.");
            return;
        }
        if (dariId == keId)
        {
            MessageBox.Show("Bandara keberangkatan dan tujuan tidak boleh sama.");
            return;
        }
        var waktu = _dtpWaktu.Value.TimeOfDay;
        if (!CobaParseDurasi(_txtDurasi.Text, out var durasiMenit))
        {
            MessageBox.Show("Durasi harus format 'XX jam YY menit' (cth: 1 jam 30 menit, menit 0-59).");
            return;
        }
        if (_numHarga.Value < 1)
        {
            MessageBox.Show("Harga per tiket minimal 1.");
            return;
        }

        var berangkat = _dtpTanggal.Value.Date + waktu;

        try
        {
            using var conn = Db.GetConnection();
            conn.Open();

            if (_editId is int id)
            {
                using var cmd = new SqlCommand("""
                    UPDATE JadwalPenerbangan SET KodePenerbangan=@kode, BandaraKeberangkatanID=@dari,
                      BandaraTujuanID=@ke, MaskapaiID=@m, TanggalWaktuKeberangkatan=@t,
                      DurasiPenerbangan=@d, HargaPerTiket=@h WHERE ID=@id
                    """, conn);
                cmd.Parameters.AddWithValue("@kode", kode);
                cmd.Parameters.AddWithValue("@dari", dariId);
                cmd.Parameters.AddWithValue("@ke", keId);
                cmd.Parameters.AddWithValue("@m", maskapaiId);
                cmd.Parameters.AddWithValue("@t", berangkat);
                cmd.Parameters.AddWithValue("@d", durasiMenit);
                cmd.Parameters.AddWithValue("@h", (double)_numHarga.Value);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Perubahan berhasil disimpan.");
            }
            else
            {
                using var cmd = new SqlCommand("""
                    INSERT INTO JadwalPenerbangan (KodePenerbangan, BandaraKeberangkatanID, BandaraTujuanID,
                      MaskapaiID, TanggalWaktuKeberangkatan, DurasiPenerbangan, HargaPerTiket)
                    VALUES (@kode,@dari,@ke,@m,@t,@d,@h)
                    """, conn);
                cmd.Parameters.AddWithValue("@kode", kode);
                cmd.Parameters.AddWithValue("@dari", dariId);
                cmd.Parameters.AddWithValue("@ke", keId);
                cmd.Parameters.AddWithValue("@m", maskapaiId);
                cmd.Parameters.AddWithValue("@t", berangkat);
                cmd.Parameters.AddWithValue("@d", durasiMenit);
                cmd.Parameters.AddWithValue("@h", (double)_numHarga.Value);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Jadwal berhasil disimpan.");
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
        var kode = _grid.CurrentRow?.Cells["KodePenerbangan"].Value?.ToString() ?? id.ToString();
        if (MessageBox.Show($"Hapus jadwal '{kode}'?", "Konfirmasi Hapus",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            return;
        try
        {
            using var conn = Db.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("DELETE FROM JadwalPenerbangan WHERE ID=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
            if (_editId == id)
                ResetForm();
            LoadData();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Gagal menghapus (data mungkin dipakai transaksi/status): " + ex.Message);
        }
    }

    private void ResetForm()
    {
        _editId = null;
        _txtKode.Clear();
        _cmbDari.SelectedIndex = -1;
        _cmbKe.SelectedIndex = -1;
        _cmbMaskapai.SelectedIndex = -1;
        _dtpTanggal.Value = DateTime.Today;
        _dtpWaktu.Value = DateTime.Today;
        _txtDurasi.Clear();
        _numHarga.Value = 1;
    }
}
