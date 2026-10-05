using System;

// Contoh sederhana percabangan if, else, else if, dan switch
Console.WriteLine("Contoh percabangan: if, else, else if, switch\n");

// 1) if - jalankan kode hanya bila kondisi terpenuhi
int number = 7;
Console.WriteLine($"Number = {number}");
if (number > 0)
{
    Console.WriteLine("if: Number positif");
}

// 2) if - else - pilih satu dari dua jalur
int age = 17;
Console.WriteLine($"\nAge = {age}");
if (age >= 17)
{
    Console.WriteLine("if-else: Boleh memilih (>= 17)");
}
else
{
    Console.WriteLine("if-else: Belum boleh memilih (< 17)");
}

// 3) else if - beberapa kondisi berurutan
int score = 85;
Console.WriteLine($"\nScore = {score}");
if (score >= 90)
{
    Console.WriteLine("Grade: A");
}
else if (score >= 80)
{
    Console.WriteLine("Grade: B");
}
else if (score >= 70)
{
    Console.WriteLine("Grade: C");
}
else
{
    Console.WriteLine("Grade: D atau E");
}

// 4) switch - cocok untuk memilih berdasarkan nilai diskret
int day = 3; // 1=Senin, 2=Selasa, 3=Rabu...
Console.WriteLine($"\nDay number = {day}");
switch (day)
{
    case 1:
        Console.WriteLine("Monday (Senin)");
        break;
    case 2:
        Console.WriteLine("Tuesday (Selasa)");
        break;
    case 3:
        Console.WriteLine("Wednesday (Rabu)");
        break;
    default:
        Console.WriteLine("Other day");
        break;
}

// Penjelasan singkat (dicetak ke konsol)
Console.WriteLine("\nKeterangan:");
Console.WriteLine("if: menjalankan blok kode bila kondisi bernilai true.");
Console.WriteLine("if-else: memilih antara dua jalur berdasarkan kondisi.");
Console.WriteLine("else if: memeriksa beberapa kondisi berurutan hingga salah satu terpenuhi.");
Console.WriteLine("switch: memilih jalur berdasarkan nilai variabel; lebih rapi untuk banyak kasus diskret.");

Console.WriteLine("\nSelesai. Tekan Enter untuk keluar.");
Console.ReadLine();
