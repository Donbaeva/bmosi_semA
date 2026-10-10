namespace SeatMC
{
    // Rozhodovací strategie pasažéra
    public static class Strategies
    {
        // Strategie 1 (práhová): první řada s volnými místy >= T
        public static int Threshold(int[] free, int T)
        {
            for (int r = 0; r < Model.Rows; r++)
                if (free[r] >= T) return Model.Pen[r];
            return Model.NoSeatPenalty;
        }

        // Strategie 2 (cílová): ignoruj řady před C, pak první řada s >= 1 místem
        public static int Target(int[] free, int C)
        {
            for (int r = C - 1; r < Model.Rows; r++)
                if (free[r] >= 1) return Model.Pen[r];
            return Model.NoSeatPenalty;
        }
    }
}