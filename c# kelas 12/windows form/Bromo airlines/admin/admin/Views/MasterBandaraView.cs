using System.Data;
using Microsoft.Data.SqlClient;

namespace admin.Views;

public sealed class MasterBandaraView : UserControl
{
    private readonly DataGridView _grid;
    private readonly TextBox _txtNama;
    private readonly TextBox _txtKode;
    private readonly TextBox _txtKota;
    private readonly ComboBox _cmbNegara;
    private readonly NumericUpDown _numTerminal;
    private readonly TextBox _txtAlamat;
    private readonly Button _btnSimpan;
    private readonly Button _btnBatal;

    private int? _editId;
    private DataTable _negaraTable = new();

    public MasterBandaraView()
    {
        BackColor = Color.White;
        Font = new Font("Segoe UI", 9F);

        var title = new Label
        {
            Text = "Master Bandara",
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(20, 15)
        };
        var subtitle = new Label
        {
            Text = "Semua bandara yang terdaftar akan muncul di sini",
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

        // ===== Input area =====
        var lblNama = new Label { Text = "Nama", AutoSize = true, Location = new Point(20, 312) };
        _txtNama = new TextBox { Location = new Point(110, 309), Size = new Size(220, 23) };

        var lblKode = new Label { Text = "Kode IATA", AutoSize = true, Location = new Point(20, 342) };
        _txtKode = new TextBox { Location = new Point(110, 339), Size = new Size(220, 23), MaxLength = 3 };

        var lblKota = new Label { Text = "Kota", AutoSize = true, Location = new Point(20, 372) };
        _txtKota = new TextBox { Location = new Point(110, 369), Size = new Size(220, 23) };

        var lblNegara = new Label { Text = "Negara", AutoSize = true, Location = new Point(20, 402) };
        _cmbNegara = new ComboBox
        {
            Location = new Point(110, 399),
            Size = new Size(220, 23),
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        var lblTerminal = new Label { Text = "Jumlah Terminal", AutoSize = true, Location = new Point(380, 312) };
        _numTerminal = new NumericUpDown
        {
            Location = new Point(490, 309),
            Size = new Size(250, 23),
            Minimum = 1,
            Maximum = 100,
            Value = 1
        };

        var lblAlamat = new Label { Text = "Alamat", AutoSize = true, Location = new Point(380, 342) };
        _txtAlamat = new TextBox
        {
            Location = new Point(490, 339),
            Size = new Size(250, 83),
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
            lblKode, _txtKode,
            lblKota, _txtKota,
            lblNegara, _cmbNegara,
            lblTerminal, _numTerminal,
            lblAlamat, _txtAlamat,
            _btnBatal, _btnSimpan
        ]);

        Load += (_, _) =>
        {
            LoadNegara();
            LoadData();
        };

        // kalau Load tidak kepanggil (embed), panggil langsung
        if (!DesignMode)
        {
            LoadNegara();
            LoadData();
        }
    }

    private void LoadNegara()
    {
        try
        {
            using var conn = Db.GetConnection();
            conn.Open();
            using var da = new SqlDataAdapter("SELECT ID, Nama FROM Negara ORDER BY Nama", conn);
            _negaraTable = new DataTable();
            da.Fill(_negaraTable);
            _cmbNegara.DisplayMember = "Nama";
            _cmbNegara.ValueMember = "ID";
            _cmbNegara.DataSource = _negaraTable;
            _cmbNegara.SelectedIndex = -1;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Gagal memuat data negara: " + ex.Message);
        }
    }

    private void LoadData()
    {
        try
        {
            using var conn = Db.GetConnection();
            conn.Open();
            const string sql = """
                SELECT b.ID, b.Nama, b.KodeIATA, b.Kota,
                       n.Nama AS Negara, b.JumlahTerminal, b.Alamat, b.NegaraID
                FROM Bandara b
                JOIN Negara n ON b.NegaraID = n.ID
                ORDER BY b.Nama ASC
                """;
            using var da = new SqlDataAdapter(sql, conn);
            var dt = new DataTable();
            da.Fill(dt);

            _grid.Columns.Clear();
            _grid.DataSource = dt;

            _grid.Columns["ID"].Visible = false;
            _grid.Columns["NegaraID"].Visible = false;
            _grid.Columns["Nama"].HeaderText = "Nama";
            _grid.Columns["KodeIATA"].HeaderText = "KodeIATA";
            _grid.Columns["Kota"].HeaderText = "Kota";
            _grid.Columns["Negara"].HeaderText = "Negara";
            _grid.Columns["JumlahTerminal"].HeaderText = "JumlahTermi";
            _grid.Columns["Alamat"].HeaderText = "Alamat";

            var colUbah = new DataGridViewButtonColumn
            {
                HeaderText = "",
                Text = "Ubah",
                UseColumnTextForButtonValue = true,
                Width = 70,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            };
            var colHapus = new DataGridViewButtonColumn
            {
                HeaderText = "",
                Text = "Hapus",
                UseColumnTextForButtonValue = true,
                Width = 70,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            };
            _grid.Columns.Add(colUbah);
            _grid.Columns.Add(colHapus);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Gagal memuat data bandara: " + ex.Message);
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
        var action = _grid.Columns[e.ColumnIndex].HeaderText == "" ? row.Cells[e.ColumnIndex].Value?.ToString() : "";

        // kolom tombol tidak punya header, bedakan dari Text tombol
        var btnText = (_grid.Columns[e.ColumnIndex] as DataGridViewButtonColumn)?.Text;
        // karena UseColumnTextForButtonValue, ambil dari index: kolom terakhir = Hapus
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
            using var cmd = new SqlCommand("SELECT Nama, KodeIATA, Kota, NegaraID, JumlahTerminal, Alamat FROM Bandara WHERE ID = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var r = cmd.ExecuteReader();
            if (!r.Read())
                return;
            _editId = id;
            _txtNama.Text = r["Nama"].ToString();
            _txtKode.Text = r["KodeIATA"].ToString();
            _txtKota.Text = r["Kota"].ToString();
            _cmbNegara.SelectedValue = Convert.ToInt32(r["NegaraID"]);
            _numTerminal.Value = Math.Max(1, Convert.ToDecimal(r["JumlahTerminal"]));
            _txtAlamat.Text = r["Alamat"].ToString();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Gagal memuat data: " + ex.Message);
        }
    }

    private void Simpan()
    {
        var nama = _txtNama.Text.Trim();
        var kode = _txtKode.Text.Trim().ToUpper();
        var kota = _txtKota.Text.Trim();
        var alamat = _txtAlamat.Text.Trim();
        var terminal = (int)_numTerminal.Value;

        if (nama == "" || kode == "" || kota == "" || alamat == "")
        {
            MessageBox.Show("Semua input harus diisi.");
            return;
        }
        if (_cmbNegara.SelectedValue is not int negaraId)
        {
            MessageBox.Show("Pilih negara terlebih dahulu.");
            return;
        }
        if (kode.Length != 3 || !kode.All(char.IsLetter))
        {
            MessageBox.Show("Kode IATA harus berupa 3 huruf.");
            return;
        }
        if (terminal < 1)
        {
            MessageBox.Show("Jumlah terminal minimal 1.");
            return;
        }

        try
        {
            using var conn = Db.GetConnection();
            conn.Open();

            // Nama unik (case insensitive)
            using (var cek = new SqlCommand(
                "SELECT COUNT(*) FROM Bandara WHERE LOWER(Nama) = LOWER(@nama) AND (@id IS NULL OR ID <> @id)", conn))
            {
                cek.Parameters.AddWithValue("@nama", nama);
                cek.Parameters.AddWithValue("@id", (object?)_editId ?? DBNull.Value);
                if (Convert.ToInt32(cek.ExecuteScalar()) > 0)
                {
                    MessageBox.Show("Nama bandara sudah ada (tidak boleh duplikat).");
                    return;
                }
            }

            // Kode IATA unik
            using (var cek = new SqlCommand(
                "SELECT COUNT(*) FROM Bandara WHERE KodeIATA = @kode AND (@id IS NULL OR ID <> @id)", conn))
            {
                cek.Parameters.AddWithValue("@kode", kode);
                cek.Parameters.AddWithValue("@id", (object?)_editId ?? DBNull.Value);
                if (Convert.ToInt32(cek.ExecuteScalar()) > 0)
                {
                    MessageBox.Show("Kode IATA sudah ada (tidak boleh duplikat).");
                    return;
                }
            }

            if (_editId is int id)
            {
                using var cmd = new SqlCommand(
                    "UPDATE Bandara SET Nama=@nama, KodeIATA=@kode, Kota=@kota, NegaraID=@negara, JumlahTerminal=@t, Alamat=@alamat WHERE ID=@id", conn);
                cmd.Parameters.AddWithValue("@nama", nama);
                cmd.Parameters.AddWithValue("@kode", kode);
                cmd.Parameters.AddWithValue("@kota", kota);
                cmd.Parameters.AddWithValue("@negara", negaraId);
                cmd.Parameters.AddWithValue("@t", terminal);
                cmd.Parameters.AddWithValue("@alamat", alamat);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Perubahan berhasil disimpan.");
            }
            else
            {
                using var cmd = new SqlCommand(
                    "INSERT INTO Bandara (Nama, KodeIATA, Kota, NegaraID, JumlahTerminal, Alamat) VALUES (@nama,@kode,@kota,@negara,@t,@alamat)", conn);
                cmd.Parameters.AddWithValue("@nama", nama);
                cmd.Parameters.AddWithValue("@kode", kode);
                cmd.Parameters.AddWithValue("@kota", kota);
                cmd.Parameters.AddWithValue("@negara", negaraId);
                cmd.Parameters.AddWithValue("@t", terminal);
                cmd.Parameters.AddWithValue("@alamat", alamat);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Data bandara berhasil disimpan.");
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
            $"Hapus bandara '{nama}'?",
            "Konfirmasi Hapus",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);
        if (confirm != DialogResult.Yes)
            return;

        try
        {
            using var conn = Db.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("DELETE FROM Bandara WHERE ID = @id", conn);
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
        _txtKode.Clear();
        _txtKota.Clear();
        _txtAlamat.Clear();
        _numTerminal.Value = 1;
        _cmbNegara.SelectedIndex = -1;
    }
}
