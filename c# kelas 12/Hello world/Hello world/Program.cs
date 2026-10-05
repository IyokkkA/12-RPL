namespace Hello_world;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}

internal sealed class MainForm : Form
{
    public MainForm()
    {
        Text = "C# Kelas 12 - Variabel dan Operator";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(560, 420);
        ClientSize = new Size(720, 520);

        var title = new Label
        {
            AutoSize = true,
            Dock = DockStyle.Top,
            Font = new Font(Font, FontStyle.Bold),
            Padding = new Padding(0, 0, 0, 12),
            Text = "Variabel, Tipe Data, dan Operator Sederhana"
        };

        var lesson = new TextBox
        {
            Dock = DockStyle.Fill,
            Font = new Font("Consolas", 11),
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            Text = CreateLessonText()
        };

        var content = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(20)
        };
        content.Controls.Add(lesson);
        content.Controls.Add(title);
        Controls.Add(content);
    }

    private static string CreateLessonText()
    {
        int umur = 17;
        string nama = "Budi";
        int tahun = 2008;
        double tinggi = 170.5;
        char nilai = 'A';
        string alamat = "Sidoarjo, Gedangan, Jawa Timur";
        bool gemarMembaca = true;
        bool sudahMakan = false;

        int a = 12;
        int b = 5;
        double c = 17;
        double d = 7;

        return $"""
            Contoh variabel dan tipe data
            -----------------------------
            int umur = {umur}
            string nama = {nama}
            int tahun = {tahun}
            double tinggi = {tinggi}
            char nilai = {nilai}
            string alamat = {alamat}
            bool gemarMembaca = {gemarMembaca}
            bool sudahMakan = {sudahMakan}

            Contoh operator dengan a = {a} dan b = {b}
            ---------------------------------------
            Penjumlahan : {a + b}
            Pengurangan : {a - b}
            Perkalian   : {a * b}
            Pembagian   : {a / b} (pembagian bilangan bulat)

            Pembagian desimal, c = {c} dan d = {d}
            ---------------------------------------
            c / d = {c / d:F2}
            """;
    }
}
