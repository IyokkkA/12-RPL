namespace customer;

public sealed class LoginForm : Form
{
    public LoginForm()
    {
        Text = "Bromo Airlines - Login";
        ClientSize = new Size(744, 460);
        MinimumSize = new Size(760, 500);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.White;
        Font = new Font("Segoe UI", 9F);

        var logo = new PictureBox
        {
            Location = new Point(170, 55),
            Size = new Size(404, 174),
            SizeMode = PictureBoxSizeMode.Zoom,
            BackColor = Color.Transparent,
            Image = Image.FromFile(Path.Combine(AppContext.BaseDirectory, "Resources", "Logo Rect.png"))
        };

        var usernameLabel = new Label
        {
            AutoSize = false,
            Text = "Username",
            TextAlign = ContentAlignment.MiddleRight,
            Location = new Point(205, 281),
            Size = new Size(95, 24)
        };

        var usernameTextBox = new TextBox
        {
            Text = "user",
            Location = new Point(312, 282),
            Size = new Size(212, 23)
        };

        var passwordLabel = new Label
        {
            AutoSize = false,
            Text = "Password",
            TextAlign = ContentAlignment.MiddleRight,
            Location = new Point(205, 315),
            Size = new Size(95, 24)
        };

        var passwordTextBox = new TextBox
        {
            Text = "password",
            UseSystemPasswordChar = true,
            Location = new Point(312, 316),
            Size = new Size(212, 23)
        };

        var loginButton = new Button
        {
            Text = "Login",
            Location = new Point(415, 351),
            Size = new Size(108, 32)
        };

        var registerPrompt = new Label
        {
            AutoSize = true,
            Text = "Belum punya akun?",
            Location = new Point(267, 414)
        };

        var registerLink = new LinkLabel
        {
            AutoSize = true,
            Text = "Daftar Sekarang!",
            Location = new Point(392, 414)
        };

        loginButton.Click += (_, _) =>
        {
            var username = usernameTextBox.Text.Trim();
            var password = passwordTextBox.Text;
            if (username == "" || password == "")
            {
                MessageBox.Show("Isi username dan password.");
                return;
            }

            try
            {
                using var conn = Db.GetConnection();
                conn.Open();
                using var cmd = new Microsoft.Data.SqlClient.SqlCommand(
                    "SELECT ID, Nama, MerupakanAdmin FROM Akun WHERE Username = @u AND Password = @p", conn);
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", password);
                using var r = cmd.ExecuteReader();
                if (!r.Read())
                {
                    MessageBox.Show("Username atau password salah.");
                    return;
                }

                if (Convert.ToBoolean(r["MerupakanAdmin"]))
                {
                    MessageBox.Show("Akun ini adalah admin. Gunakan aplikasi admin.");
                    return;
                }

                var id = Convert.ToInt32(r["ID"]);
                var nama = r["Nama"].ToString()!;

                Hide();
                using var main = new CustomerForm(id, nama);
                main.ShowDialog();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal login: " + ex.Message);
            }
        };

        registerLink.LinkClicked += (_, _) =>
        {
            Hide();
            using var register = new RegisterForm();
            register.ShowDialog();
            Close();
        };

        Controls.AddRange(
        [
            logo,
            usernameLabel,
            usernameTextBox,
            passwordLabel,
            passwordTextBox,
            loginButton,
            registerPrompt,
            registerLink
        ]);
    }
}
