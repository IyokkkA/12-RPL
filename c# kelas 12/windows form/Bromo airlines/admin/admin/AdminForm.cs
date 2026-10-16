namespace admin;

public sealed class AdminForm : Form
{
    private readonly Panel _sidePanel;
    private readonly Panel _contentPanel;
    private readonly Label _topTitle;
    private readonly List<Button> _menuButtons = new();
    private bool _expanded = true;

    private const int ExpandedWidth = 220;
    private const int CollapsedWidth = 60;

    public AdminForm()
    {
        Text = "Bromo Airlines - Admin";
        ClientSize = new Size(1100, 640);
        MinimumSize = new Size(1000, 600);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.White;
        Font = new Font("Segoe UI", 9F);

        // ===== Top bar =====
        var topBar = new Panel
        {
            Dock = DockStyle.Top,
            Height = 48,
            BackColor = Color.FromArgb(29, 87, 171)
        };

        var menuButton = new Button
        {
            Text = "☰",
            Font = new Font("Segoe UI", 14F),
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            FlatStyle = FlatStyle.Flat,
            Dock = DockStyle.Left,
            Width = 48,
            Cursor = Cursors.Hand
        };
        menuButton.FlatAppearance.BorderSize = 0;
        menuButton.Click += (_, _) => ToggleMenu();

        _topTitle = new Label
        {
            Text = "Dashboard",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft,
            Dock = DockStyle.Fill,
            Padding = new Padding(8, 0, 0, 0)
        };

        topBar.Controls.Add(_topTitle);
        topBar.Controls.Add(menuButton);

        // ===== Side panel =====
        _sidePanel = new Panel
        {
            Dock = DockStyle.Left,
            Width = ExpandedWidth,
            BackColor = Color.White
        };

        var separator = new Panel
        {
            Dock = DockStyle.Right,
            Width = 1,
            BackColor = Color.FromArgb(225, 225, 225)
        };
        _sidePanel.Controls.Add(separator);

        var logoutButton = new Button
        {
            Text = "    Logout",
            Image = LoadIcon("log-out-regular-72.png", 20),
            ImageAlign = ContentAlignment.MiddleLeft,
            TextAlign = ContentAlignment.MiddleLeft,
            TextImageRelation = TextImageRelation.ImageBeforeText,
            Dock = DockStyle.Bottom,
            Height = 44,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.White,
            ForeColor = Color.FromArgb(200, 40, 40),
            Cursor = Cursors.Hand,
            Padding = new Padding(10, 0, 0, 0)
        };
        logoutButton.FlatAppearance.BorderSize = 0;
        logoutButton.Tag = "Logout";
        logoutButton.Click += (_, _) => Logout();

        var btnUbahStatus = CreateMenuButton("Ubah Status Penerbangan", "notepad-unselected-72.png");
        var btnKodePromo = CreateMenuButton("Master Kode Promo", "purchase-tag-alt-unselected-72.png");
        var btnJadwal = CreateMenuButton("Master Jadwal Penerbangan", "calendar-unselected-72.png");
        var btnMaskapai = CreateMenuButton("Master Maskapai", "plane-take-off-unselected-72.png");
        var btnBandara = CreateMenuButton("Master Bandara", "map-unselected-72.png");

        // Urutan dari atas: Bandara, Maskapai, Jadwal, Kode Promo, Ubah Status
        // Dock Top = tambah dengan urutan terbalik
        _sidePanel.Controls.Add(btnUbahStatus);
        _sidePanel.Controls.Add(btnKodePromo);
        _sidePanel.Controls.Add(btnJadwal);
        _sidePanel.Controls.Add(btnMaskapai);
        _sidePanel.Controls.Add(btnBandara);
        _sidePanel.Controls.Add(logoutButton);

        btnBandara.Click += (_, _) => ShowContent("Master Bandara", new Views.MasterBandaraView(), btnBandara);
        btnMaskapai.Click += (_, _) => ShowContent("Master Maskapai", new Views.MasterMaskapaiView(), btnMaskapai);
        btnJadwal.Click += (_, _) => ShowContent("Master Jadwal Penerbangan", new Views.MasterJadwalView(), btnJadwal);
        btnKodePromo.Click += (_, _) => ShowContent("Master Kode Promo", new Views.MasterKodePromoView(), btnKodePromo);
        btnUbahStatus.Click += (_, _) => ShowContent("Ubah Status Penerbangan", new Views.UbahStatusView(), btnUbahStatus);

        // ===== Content panel =====
        _contentPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White
        };

        Controls.Add(_contentPanel);
        Controls.Add(_sidePanel);
        Controls.Add(topBar);

        // Default langsung ke Master Bandara setelah login
        ShowContent("Master Bandara", new Views.MasterBandaraView(), btnBandara);

        FormClosed += (_, _) => Application.Exit();
    }

    private Button CreateMenuButton(string text, string iconFile)
    {
        var btn = new Button
        {
            Text = "  " + text,
            Tag = text,
            Image = LoadIcon(iconFile, 20),
            ImageAlign = ContentAlignment.MiddleLeft,
            TextAlign = ContentAlignment.MiddleLeft,
            TextImageRelation = TextImageRelation.ImageBeforeText,
            Dock = DockStyle.Top,
            Height = 44,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.White,
            ForeColor = Color.FromArgb(60, 60, 60),
            Cursor = Cursors.Hand,
            Padding = new Padding(10, 0, 0, 0)
        };
        btn.FlatAppearance.BorderSize = 0;
        _menuButtons.Add(btn);
        return btn;
    }

    private void ToggleMenu()
    {
        _expanded = !_expanded;
        _sidePanel.Width = _expanded ? ExpandedWidth : CollapsedWidth;
        foreach (var btn in _menuButtons)
        {
            var full = btn.Tag as string ?? "";
            btn.Text = _expanded ? "  " + full : "";
        }
    }

    private void ShowDashboard()
    {
        _topTitle.Text = "Dashboard";
        ClearSelection();
        _contentPanel.Controls.Clear();
    }

    private void ShowContent(string title, UserControl view, Button activeButton)
    {
        _topTitle.Text = title;
        ClearSelection();
        activeButton.BackColor = Color.FromArgb(235, 242, 252);
        _contentPanel.Controls.Clear();
        view.Dock = DockStyle.Fill;
        _contentPanel.Controls.Add(view);
    }

    private void ClearSelection()
    {
        foreach (var btn in _menuButtons)
            btn.BackColor = Color.White;
    }

    private void Logout()
    {
        Hide();
        using var login = new LoginForm();
        login.ShowDialog();
        Close();
    }

    private static Image? LoadIcon(string fileName, int size = 20)
    {
        try
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Resources", "Icons", fileName);
            if (File.Exists(path))
            {
                using var original = Image.FromFile(path);
                var thumb = new Bitmap(size, size);
                using var g = Graphics.FromImage(thumb);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(original, 0, 0, size, size);
                return thumb;
            }
        }
        catch
        {
            // abaikan, tampil tanpa ikon
        }
        return null;
    }
}
