using System.Data;
using Microsoft.Data.SqlClient;

namespace admin.Views;

public sealed class MasterMaskapaiView : UserControl
{
    private readonly DataGridView _grid;
    private readonly TextBox _txtNama;
    private readonly TextBox _txtPerusahaan;
    private readonly NumericUpDown _numKru;
    private readonly TextBox _txtDeskripsi;
    private readonly Button _btnSimpan;
    private readonly Button _btnBatal;

    private int? _editId;

    public MasterMaskapaiView()
    {
        BackColor = Color.White;
        Font = new Font("Segoe UI", 9F);

        var title = new Label
        {
            Text = "Master Maskapai",
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(20, 15)
        };
        var subtitle = new Label
        {
            Text = "Semua maskapai yang terdaftar akan muncul di sini",
            AutoSize = true,
            Location = new Point(22, 48)
        };

        _grid = new DataGridView
        {
            Location = new Point(20, 75),
            Size = new Size(720, 220),
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

        var lblNama = new Label { Text = "Nama", AutoSize = true, Location = new Point(20, 312) };
        _txtNama = new TextBox { Location = new Point(110, 309), Size = new Size(220, 23) };

        var lblPerusahaan = new Label { Text = "Perusahaan", AutoSize = true, Location = new Point(20, 342) };
        _txtPerusahaan = new TextBox { Location = new Point(110, 339), Size = new Size(220, 23) };

        var lblKru = new Label { Text = "Jumlah Kru", AutoSize = true, Location = new Point(20, 372) };
        _numKru = new NumericUpDown
        {
            Location = new Point(110, 369),
            Size = new Size(220, 23),
            Minimum = 1,
            Maximum = 1000,
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
            lblNama, _txtNama,
            lblPerusahaan, _txtPerusahaan,
            lblKru, _numKru,
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
            const string sql = "SELECT ID, Nama, Perusahaan, JumlahKru, Deskripsi FROM Maskapai ORDER BY Nama ASC";
            using var da = new SqlDataAdapter(sql, conn);
            var dt = new DataTable();
            da.Fill(dt);

            _grid.Columns.Clear();
            _grid.DataSource = dt;
            _grid.Columns["ID"]!.Visible = false;
            _grid.Columns["Nama"]!.HeaderText = "Nama";
            _grid.Columns["Perusahaan"]!.HeaderText = "Perusahaan";
            _grid.Columns["JumlahKru"]!.HeaderText = "JumlahKru";
            _grid.Columns["Deskripsi"]!.HeaderText = "Deskripsi";

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
            MessageBox.Show("Gagal memuat data maskapai: " + ex.Message);
        }
    }

    private void Grid_CellContentClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex < 0)
            return;
        if (_grid.Columns[e.ColumnIndex] is not DataGridViewButtonColumn)
            return;

        var row = _grid.Rows[e.RowIndex];
        var id = Convert.ToInt32(row.Cells["ID"].Value);
        var isUbah = e.ColumnIndex == _grid.Columns.Count - 2;

        if (isUbah)
            MasukModeUbah(id);
        else
            HapusData(id, row.Cells["Nama"].Value?.ToString() ?? "");
    }

    private void MasukModeUbah(int id)
    {
        try
        {
            using var conn = Db.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("SELECT Nama, Perusahaan, JumlahKru, Deskripsi FROM Maskapai WHERE ID = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var r = cmd.ExecuteReader();
            if (!r.Read())
                return;
            _editId = id;
            _txtNama.Text = r["Nama"].ToString();
            _txtPerusahaan.Text = r["Perusahaan"].ToString();
            _numKru.Value = Math.Max(1, Convert.ToDecimal(r["JumlahKru"]));
            _txtDeskripsi.Text = r["Deskripsi"].ToString();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Gagal memuat data: " + ex.Message);
        }
    }

    private void Simpan()
    {
        var nama = _txtNama.Text.Trim();
        var perusahaan = _txtPerusahaan.Text.Trim();
        var deskripsi = _txtDeskripsi.Text.Trim();
        var kru = (int)_numKru.Value;

        if (nama == "" || perusahaan == "" || deskripsi == "")
        {
            MessageBox.Show("Semua input harus diisi.");
            return;
        }
        if (kru < 1)
        {
            MessageBox.Show("Jumlah kru minimal 1.");
            return;
        }

        try
        {
            using var conn = Db.GetConnection();
            conn.Open();

            if (_editId is int id)
            {
                using var cmd = new SqlCommand(
                    "UPDATE Maskapai SET Nama=@nama, Perusahaan=@p, JumlahKru=@k, Deskripsi=@d WHERE ID=@id", conn);
                cmd.Parameters.AddWithValue("@nama", nama);
                cmd.Parameters.AddWithValue("@p", perusahaan);
                cmd.Parameters.AddWithValue("@k", kru);
                cmd.Parameters.AddWithValue("@d", deskripsi);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Perubahan berhasil disimpan.");
            }
            else
            {
                using var cmd = new SqlCommand(
                    "INSERT INTO Maskapai (Nama, Perusahaan, JumlahKru, Deskripsi) VALUES (@nama,@p,@k,@d)", conn);
                cmd.Parameters.AddWithValue("@nama", nama);
                cmd.Parameters.AddWithValue("@p", perusahaan);
                cmd.Parameters.AddWithValue("@k", kru);
                cmd.Parameters.AddWithValue("@d", deskripsi);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Data maskapai berhasil disimpan.");
            }

            ResetForm();
            LoadData();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Gagal menyimpan: " + ex.Message);
        }
    }

    private void HapusData(int id, string nama)
    {
        var confirm = MessageBox.Show(
            $"Hapus maskapai '{nama}'?",
            "Konfirmasi Hapus",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);
        if (confirm != DialogResult.Yes)
            return;

        try
        {
            using var conn = Db.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("DELETE FROM Maskapai WHERE ID = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
            if (_editId == id)
                ResetForm();
            LoadData();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Gagal menghapus (data mungkin dipakai jadwal penerbangan): " + ex.Message);
        }
    }

    private void ResetForm()
    {
        _editId = null;
        _txtNama.Clear();
        _txtPerusahaan.Clear();
        _txtDeskripsi.Clear();
        _numKru.Value = 1;
    }
}
