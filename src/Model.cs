using System;

namespace SeatMC
{
    // Model letadla: počet volných míst v řadách a penalizační funkce
    public static class Model
    {
        public const int Rows = 10;
        public const int NoSeatPenalty = 20;      // místo neobsazeno
        public const double FailProb = 0.10;      // porucha sedadla v řadě

        // Pravděpodobnost p pro Binom(3, p); index 0 = řada 1 ... index 9 = řada 10
        public static readonly double[] Pr =
            { 0.3, 0.3, 0.3, 0.6, 0.6, 0.6, 0.6, 0.5, 0.5, 0.5 };

        // Penalizační funkce P(r); index 0 = řada 1 ... index 9 = řada 10
        public static readonly int[] Pen =
            { 14, 9, 5, 2, 0, 0, 2, 5, 9, 14 };

        // Binom(3, p) = součet 3 Bernoulliho pokusů
        public static int Binom3(Random rnd, double p)
        {
            int k = 0;
            for (int i = 0; i < 3; i++)
                if (rnd.NextDouble() < p) k++;
            return k;
        }

        // Volná místa v každé řadě (včetně poruchy sedadla)
        public static int[] GenerateRows(Random rnd)
        {
            var free = new int[Rows];
            for (int r = 0; r < Rows; r++)
            {
                free[r] = Binom3(rnd, Pr[r]);
                if (rnd.NextDouble() < FailProb)           // porucha: -1 místo, min. 0
                    free[r] = Math.Max(0, free[r] - 1);
            }
            return free;
        }
    }
}