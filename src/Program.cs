using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SeatMC;

const int N = 100_000;     // počet replikací
const int Seed = 12345;    // opakovatelnost pokusů
const int Step = 500;      // jak často ukládat průběžný průměr

var names = new List<string>();
var strategies = new List<Func<int[], int>>();
for (int T = 1; T <= 3; T++) { int t = T; names.Add($"Prah_T={t}"); strategies.Add(f => Strategies.Threshold(f, t)); }
for (int C = 4; C <= 6; C++) { int c = C; names.Add($"Cil_C={c}");  strategies.Add(f => Strategies.Target(f, c)); }

int S = strategies.Count;
var sum   = new double[S];
var sumSq = new double[S];
var hist  = new int[S, Model.NoSeatPenalty + 1];

var rnd = new Random(Seed);
var running = new StringBuilder();
running.AppendLine("n;" + string.Join(";", names));

for (int n = 1; n <= N; n++)
{
    // stejné náhodné letadlo pro všechny strategie (férové srovnání)
    int[] free = Model.GenerateRows(rnd);

    for (int s = 0; s < S; s++)
    {
        int p = strategies[s](free);
        sum[s]   += p;
        sumSq[s] += (double)p * p;
        hist[s, p]++;
    }

    if (n % Step == 0)
    {
        var row = new List<string> { n.ToString() };
        for (int s = 0; s < S; s++) row.Add((sum[s] / n).ToString("F4"));
        running.AppendLine(string.Join(";", row));
    }
}

// ---- výsledky ----
var results = new StringBuilder();
results.AppendLine("strategie;E[P];smer_odchylka;CI95_dolni;CI95_horni");
Console.WriteLine($"N = {N}, seed = {Seed}\n");
Console.WriteLine($"{"Strategie",-12} {"E[P]",8} {"SD",8} {"95% CI",20}");

int best = 0;
for (int s = 0; s < S; s++)
{
    double mean = sum[s] / N;
    double variance = (sumSq[s] - N * mean * mean) / (N - 1);
    double sd = Math.Sqrt(variance);
    double half = 1.96 * sd / Math.Sqrt(N);

    Console.WriteLine($"{names[s],-12} {mean,8:F4} {sd,8:F3}   [{mean - half:F4}; {mean + half:F4}]");
    results.AppendLine($"{names[s]};{mean:F4};{sd:F4};{mean - half:F4};{mean + half:F4}");

    if (sum[s] < sum[best]) best = s;
}
Console.WriteLine($"\nNejlepší: {names[best]} (E[P] = {sum[best] / N:F4})");

// histogramy (relativní četnosti penalizace)
var h = new StringBuilder();
h.AppendLine("penalizace;" + string.Join(";", names));
foreach (int pen in new[] { 0, 2, 5, 9, 14, 20 })
{
    var row = new List<string> { pen.ToString() };
    for (int s = 0; s < S; s++) row.Add(((double)hist[s, pen] / N).ToString("F5"));
    h.AppendLine(string.Join(";", row));
}

Directory.CreateDirectory("../results");
File.WriteAllText("../results/results.csv", results.ToString());
File.WriteAllText("../results/running_mean.csv", running.ToString());
File.WriteAllText("../results/histogram.csv", h.ToString());
Console.WriteLine("\nUloženo do složky results/");