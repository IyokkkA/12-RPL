using System.Data;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

namespace admin.Views;

public sealed class UbahStatusView : UserControl
{
    private readonly DataGridView _grid;

    public UbahStatusView()
    {
        BackColor = Color.White;
        Font = new Font("Segoe UI", 9F);

        var title = new Label
        {
            Text = "Ubah Status Penerbangan",
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(20, 15)
        };
        var subtitle = new Label
        {
            Text = "Anda bisa mengubah status jadwal penerbangan di sini",
            AutoSize = true,
            Location = new Point(22, 48)
        };

        _grid = new DataGridView
        {
            Location = new Point(20, 75),
            Size = new Size(840, 420),
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            RowTemplate = { Height = 28 }
        };
        _grid.CellContentClick += Grid_CellContentClick;

        Controls.AddRange([title, subtitle, _grid]);

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
            const string sql = """
                SELECT j.ID, j.KodePenerbangan, m.Nama AS Maskapai,
                       b1.KodeIATA AS Berangkat, b2.KodeIATA AS Tujuan,
                       j.TanggalWaktuKeberangkatan, j.DurasiPenerbangan,
                       p.StatusNama, p.WaktuPerubahanTerjadi, p.PerkiraanDurasiDelay
                FROM JadwalPenerbangan j
                JOIN Bandara b1 ON j.BandaraKeberangkatanID = b1.ID
                JOIN Bandara b2 ON j.BandaraTujuanID = b2.ID
                JOIN Maskapai m ON j.MaskapaiID = m.ID
                OUTER APPLY (
                    SELECT TOP 1 sp.Nama AS StatusNama, ps.WaktuPerubahanTerjadi, ps.PerkiraanDurasiDelay
                    FROM PerubahanStatusJadwalPenerbangan ps
                    JOIN StatusPenerbangan sp ON ps.StatusPenerbanganID = sp.ID
                    WHERE ps.JadwalPenerbanganID = j.ID
                    ORDER BY ps.WaktuPerubahanTerjadi DESC
                ) p
                ORDER BY j.TanggalWaktuKeberangkatan DESC
                """;
            using var da = new SqlDataAdapter(sql, conn);
            var raw = new DataTable();
            da.Fill(raw);

            var dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("Kode", typeof(string));
            dt.Columns.Add("Maskapai", typeof(string));
            dt.Columns.Add("Dari", typeof(string));
            dt.Columns.Add("Ke", typeof(string));
            dt.Columns.Add("Tanggal", typeof(string));
            dt.Columns.Add("Waktu", typeof(string));
            dt.Columns.Add("Durasi", typeof(string));
            dt.Columns.Add("Status", typeof(string));
            dt.Columns.Add("Terakhir", typeof(string));

            foreach (DataRow r in raw.Rows)
            {
                var dtw = Convert.ToDateTime(r["TanggalWaktuKeberangkatan"]);
                var menit = Convert.ToInt32(r["DurasiPenerbangan"]);

                string status;
                if (r["StatusNama"] == DBNull.Value)
                {
                    status = "Sesuai Jadwal";
                }
                else
                {
                    var s = r["StatusNama"].ToString()!;
                    if (s.Equals("Delay", StringComparison.OrdinalIgnoreCase))
                    {
                        var dm = r["PerkiraanDurasiDelay"] == DBNull.Value ? 0 : Convert.ToInt32(r["PerkiraanDurasiDelay"]);
                        status = $"Delay (selama ±{dm / 60} jam {dm % 60} menit)";
                    }
                    else
                    {
                        status = s;
                    }
                }

                var terakhir = r["WaktuPerubahanTerjadi"] == DBNull.Value
                    ? "-"
                    : Convert.ToDateTime(r["WaktuPerubahanTerjadi"]).ToString("dd-MM-yyyy HH:mm:ss");

                dt.Rows.Add(r["ID"], r["KodePenerbangan"], r["Maskapai"], r["Berangkat"], r["Tujuan"],
                    dtw.ToString("dd-MM-yyyy"), dtw.ToString("HH:mm"),
                    $"{menit / 60} jam {menit % 60} menit", status, terakhir);
            }

            _grid.Columns.Clear();
            _grid.DataSource = dt;
            _grid.Columns["ID"]!.Visible = false;

            // Bobot kolom agar muat tanpa scroll horizontal, tombol Ubah selalu terlihat
            _grid.Columns["Kode"]!.FillWeight = 62;
            _grid.Columns["Kode"]!.MinimumWidth = 55;
            _grid.Columns["Maskapai"]!.FillWeight = 78;
            _grid.Columns["Maskapai"]!.MinimumWidth = 60;
            _grid.Columns["Dari"]!.FillWeight = 48;
            _grid.Columns["Dari"]!.MinimumWidth = 40;
            _grid.Columns["Ke"]!.FillWeight = 48;
            _grid.Columns["Ke"]!.MinimumWidth = 40;
            _grid.Columns["Tanggal"]!.FillWeight = 68;
            _grid.Columns["Tanggal"]!.MinimumWidth = 60;
            _grid.Columns["Waktu"]!.FillWeight = 45;
            _grid.Columns["Waktu"]!.MinimumWidth = 40;
            _grid.Columns["Durasi"]!.FillWeight = 72;
            _grid.Columns["Durasi"]!.MinimumWidth = 60;
            _grid.Columns["Status"]!.FillWeight = 115;
            _grid.Columns["Status"]!.MinimumWidth = 70;
            _grid.Columns["Terakhir"]!.FillWeight = 105;
            _grid.Columns["Terakhir"]!.MinimumWidth = 65;
            foreach (DataGridViewColumn c in _grid.Columns)
            {
                c.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                c.Resizable = DataGridViewTriState.True;
            }

            _grid.Columns.Add(new DataGridViewButtonColumn
            {
                HeaderText = "",
                Text = "Ubah",
                UseColumnTextForButtonValue = true,
                Width = 60,
                MinimumWidth = 60,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show("Gagal memuat data: " + ex.Message);
        }
    }

    private void Grid_CellContentClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0)
            return;
        if (_grid.Columns[e.ColumnIndex] is not DataGridViewButtonColumn)
            return;
        var row = _grid.Rows[e.RowIndex];
        var id = Convert.ToInt32(row.Cells["ID"].Value);
        var kode = row.Cells["Kode"].Value?.ToString() ?? id.ToString();

        using var dlg = new UbahStatusDialog(id, kode);
        if (dlg.ShowDialog(this) == DialogResult.OK)
            LoadData();
    }

    private sealed class UbahStatusDialog : Form
    {
        private readonly int _jadwalId;
        private readonly ComboBox _cmbStatus;
        private readonly Label _lblDurasi;
        private readonly TextBox _txtDurasi;

        public UbahStatusDialog(int jadwalId, string kode)
        {
            _jadwalId = jadwalId;
            Text = $"Ubah Status - {kode}";
            ClientSize = new Size(360, 210);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.White;
            Font = new Font("Segoe UI", 9F);

            var lblInfo = new Label { Text = $"Kode penerbangan: {kode}", AutoSize = true, Location = new Point(20, 15) };
            var lblStatus = new Label { Text = "Status", AutoSize = true, Location = new Point(20, 45) };
            _cmbStatus = new ComboBox
            {
                Location = new Point(120, 42),
                Size = new Size(210, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cmbStatus.SelectedIndexChanged += (_, _) => UpdateDurasiVisibility();

            _lblDurasi = new Label { Text = "Perkiraan Delay", AutoSize = true, Location = new Point(20, 78), Visible = false };
            _txtDurasi = new TextBox
            {
                Location = new Point(120, 75),
                Size = new Size(210, 23),
                PlaceholderText = "cth: 2 jam 30 menit",
                Visible = false
            };

            var btnBatal = new Button { Text = "Batal", Location = new Point(120, 130), Size = new Size(100, 30), DialogResult = DialogResult.Cancel };
            var btnSimpan = new Button { Text = "Simpan", Location = new Point(230, 130), Size = new Size(100, 30) };
            btnSimpan.Click += (_, _) => Simpan();

            Controls.AddRange([lblInfo, lblStatus, _cmbStatus, _lblDurasi, _txtDurasi, btnBatal, btnSimpan]);

            LoadStatus();
        }

        private void LoadStatus()
        {
            try
            {
                using var conn = Db.GetConnection();
                conn.Open();
                using var da = new SqlDataAdapter("SELECT ID, Nama FROM StatusPenerbangan ORDER BY Nama", conn);
                var dt = new DataTable();
                da.Fill(dt);
                _cmbStatus.DisplayMember = "Nama";
                _cmbStatus.ValueMember = "ID";
                _cmbStatus.DataSource = dt;
                _cmbStatus.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat status: " + ex.Message);
            }
        }

        private void UpdateDurasiVisibility()
        {
            var isDelay = _cmbStatus.Text.Equals("Delay", StringComparison.OrdinalIgnoreCase);
            _lblDurasi.Visible = isDelay;
            _txtDurasi.Visible = isDelay;
        }

        private void Simpan()
        {
            if (_cmbStatus.SelectedValue is not int statusId)
            {
                MessageBox.Show("Pilih status terlebih dahulu.");
                return;
            }

            int? delayMenit = null;
            if (_cmbStatus.Text.Equals("Delay", StringComparison.OrdinalIgnoreCase))
            {
                var m = Regex.Match(_txtDurasi.Text.Trim(), @"^(\d+)\s*jam\s*(\d+)\s*menit$", RegexOptions.IgnoreCase);
                if (!m.Success || !int.TryParse(m.Groups[1].Value, out var jam) ||
                    !int.TryParse(m.Groups[2].Value, out var menit) ||
                    jam < 0 || menit < 0 || menit >= 60 || jam * 60 + menit <= 0)
                {
                    MessageBox.Show("Isi perkiraan delay format 'XX jam YY menit' (cth: 1 jam 30 menit).");
                    return;
                }
                delayMenit = jam * 60 + menit;
            }

            try
            {
                using var conn = Db.GetConnection();
                conn.Open();
                using var cmd = new SqlCommand(
                    "INSERT INTO PerubahanStatusJadwalPenerbangan (JadwalPenerbanganID, StatusPenerbanganID, WaktuPerubahanTerjadi, PerkiraanDurasiDelay) VALUES (@j,@s,@w,@d)", conn);
                cmd.Parameters.AddWithValue("@j", _jadwalId);
                cmd.Parameters.AddWithValue("@s", statusId);
                cmd.Parameters.AddWithValue("@w", DateTime.Now);
                cmd.Parameters.AddWithValue("@d", (object?)delayMenit ?? DBNull.Value);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Status berhasil disimpan.");
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menyimpan status: " + ex.Message);
            }
        }
    }
}
