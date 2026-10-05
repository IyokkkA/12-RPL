// Simple examples of for, while, and foreach loops
using System;

var numbers = new int[] { 1, 2, 3, 4, 5 };

Console.WriteLine("For loop:");
for (int i = 0; i < numbers.Length; i++)
{
    Console.WriteLine($"Index {i} = {numbers[i]}");
}

Console.WriteLine();
Console.WriteLine("While loop:");
int j = 0;
while (j < numbers.Length)
{
    Console.WriteLine($"Value at position {j} = {numbers[j]}");
    j++;
}

Console.WriteLine();
Console.WriteLine("Foreach loop:");
foreach (var n in numbers)
{
    Console.WriteLine(n);
}

// Keep console open when running from Visual Studio
Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();
