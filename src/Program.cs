using System;
using SeatMC;

var rnd = new Random(12345);

Console.WriteLine("Test modelu: 5 náhodných letadel (volná místa v řadách 1-10)");
for (int i = 1; i <= 5; i++)
{
    int[] free = Model.GenerateRows(rnd);
    Console.WriteLine($"Letadlo {i}: " + string.Join(" ", free));
}