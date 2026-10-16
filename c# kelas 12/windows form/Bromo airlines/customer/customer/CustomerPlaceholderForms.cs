namespace customer;

public sealed class ListPenerbanganForm : Form
{
    public ListPenerbanganForm(int dariId, int keId, DateTime tanggal, int jumlah)
    {
        Text = "Bromo Airlines - List Penerbangan";
        ClientSize = new Size(800, 500);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.White;
        Font = new Font("Segoe UI", 9F);

        var lbl = new Label
        {
            Text = $"List Penerbangan (DariID={dariId}, KeID={keId}, Tgl={tanggal:dd-MM-yyyy}, Penumpang={jumlah})",
            AutoSize = true,
            Location = new Point(20, 20)
        };
        Controls.Add(lbl);

        FormClosed += (_, _) => Application.Exit();
    }
}

public sealed class TiketSayaForm : Form
{
    public TiketSayaForm(int akunId, string nama)
    {
        Text = "Bromo Airlines - Tiket Saya";
        ClientSize = new Size(800, 500);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.White;
        Font = new Font("Segoe UI", 9F);

        var lbl = new Label
        {
            Text = $"Tiket Saya - {nama} (ID={akunId})",
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(20, 20)
        };
        Controls.Add(lbl);

        FormClosed += (_, _) => Application.Exit();
    }
}

