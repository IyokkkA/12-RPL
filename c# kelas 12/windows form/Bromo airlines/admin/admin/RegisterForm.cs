namespace admin;

public sealed class RegisterForm : Form
{
    public RegisterForm()
    {
        Text = "Bromo Airlines - Daftar Akun";
        ClientSize = new Size(764, 441);
        MinimumSize = new Size(780, 480);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.White;
        Font = new Font("Segoe UI", 9F);

        var titleLabel = new Label
        {
            AutoSize = true,
            Text = "Daftar Akun",
            Font = new Font("Segoe UI", 16F, FontStyle.Bold),
            Location = new Point(20, 20)
        };

        var subtitleLabel = new Label
        {
            AutoSize = true,
            Text = "Daftarkan diri Anda untuk menggunakan Bromo Airlines",
            Location = new Point(22, 57)
        };

        var usernameLabel = new Label
        {
            AutoSize = false,
            Text = "Username",
            TextAlign = ContentAlignment.MiddleLeft,
            Location = new Point(22, 100),
            Size = new Size(100, 24)
        };

        var usernameTextBox = new TextBox
        {
            Text = "andi",
            Location = new Point(135, 101),
            Size = new Size(220, 23)
        };

        var namaLabel = new Label
        {
            AutoSize = false,
            Text = "Nama",
            TextAlign = ContentAlignment.MiddleLeft,
            Location = new Point(22, 132),
            Size = new Size(100, 24)
        };

        var namaTextBox = new TextBox
        {
            Text = "Andy Taulany",
            Location = new Point(135, 133),
            Size = new Size(220, 23)
        };

        var tanggalLahirLabel = new Label
        {
            AutoSize = false,
            Text = "Tanggal Lahir",
            TextAlign = ContentAlignment.MiddleLeft,
            Location = new Point(22, 164),
            Size = new Size(100, 24)
        };

        var tanggalLahirPicker = new DateTimePicker
        {
            Format = DateTimePickerFormat.Custom,
            CustomFormat = "dd-MM-yyyy",
            Value = new DateTime(2003, 2, 1),
            Location = new Point(135, 165),
            Size = new Size(220, 23)
        };

        var nomorTeleponLabel = new Label
        {
            AutoSize = false,
            Text = "Nomor Telepon",
            TextAlign = ContentAlignment.MiddleLeft,
            Location = new Point(22, 196),
            Size = new Size(100, 24)
        };

        var nomorTeleponTextBox = new TextBox
        {
            Text = "08123456789",
            Location = new Point(135, 197),
            Size = new Size(220, 23)
        };

        var passwordLabel = new Label
        {
            AutoSize = false,
            Text = "Password",
            TextAlign = ContentAlignment.MiddleLeft,
            Location = new Point(22, 228),
            Size = new Size(100, 24)
        };

        var passwordTextBox = new TextBox
        {
            Text = "password",
            UseSystemPasswordChar = true,
            Location = new Point(135, 229),
            Size = new Size(220, 23)
        };

        var daftarButton = new Button
        {
            Text = "Daftar",
            Location = new Point(220, 272),
            Size = new Size(135, 32),
            Enabled = false
        };

        var logo = new PictureBox
        {
            Location = new Point(410, 100),
            Size = new Size(325, 210),
            SizeMode = PictureBoxSizeMode.Zoom,
            BackColor = Color.Transparent,
            Image = Image.FromFile(Path.Combine(AppContext.BaseDirectory, "Resources", "Logo Rect Without Tagline.png"))
        };

        var haveAccountLabel = new Label
        {
            AutoSize = true,
            Text = "Sudah punya akun?",
            Location = new Point(22, 400)
        };

        var loginLink = new LinkLabel
        {
            AutoSize = true,
            Text = "Login",
            Location = new Point(155, 400)
        };
        loginLink.LinkClicked += (_, _) =>
        {
            Hide();
            using var login = new LoginForm();
            login.ShowDialog();
            Close();
        };

        Controls.AddRange(
        [
            titleLabel,
            subtitleLabel,
            usernameLabel,
            usernameTextBox,
            namaLabel,
            namaTextBox,
            tanggalLahirLabel,
            tanggalLahirPicker,
            nomorTeleponLabel,
            nomorTeleponTextBox,
            passwordLabel,
            passwordTextBox,
            daftarButton,
            logo,
            haveAccountLabel,
            loginLink
        ]);
    }
}
