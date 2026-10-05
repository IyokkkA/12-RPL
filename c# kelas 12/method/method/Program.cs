Console.WriteLine("=== Contoh Method dan Function ===");
Console.WriteLine("Method digunakan untuk menjalankan tugas tertentu.\n");

// 1. Method tanpa parameter dan tanpa return value
Console.WriteLine("1. Method tanpa parameter dan tanpa return value:");
Salam();

// 2. Method dengan parameter
// Parameter digunakan untuk mengirim data ke dalam method.
Console.WriteLine("\n2. Method dengan parameter:");
Sapa("Budi");

// 3. Method dengan return value
// Return value digunakan untuk mengembalikan hasil dari method.
Console.WriteLine("\n3. Method dengan return value:");
int hasil = Tambah(5, 7);
Console.WriteLine($"Hasil dari Tambah(5, 7) adalah: {hasil}");

// Method void tidak mengembalikan nilai.
static void Salam()
{
    Console.WriteLine("Halo! Ini contoh method tanpa parameter.");
}

// nama adalah parameter bertipe string yang menerima data dari pemanggil method.
static void Sapa(string nama)
{
    Console.WriteLine("Halo, " + nama + "!");
}

// Method bertipe int mengembalikan nilai bertipe int menggunakan return.
static int Tambah(int a, int b)
{
    return a + b;
}
