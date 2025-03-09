namespace MatrixMul;

public static class StripeMul
{
    public static double[,] Multiply(double[,] A, double[,] B, int threadsCount)
    {
        if (A.GetLength(1) != B.GetLength(0)) {
            throw new InvalidOperationException(
                $"[{A.GetLength(0)}, {A.GetLength(1)}] X " +
                $"[{B.GetLength(0)}, {B.GetLength(1)}]"
            );
        }
        var C = new double[A.GetLength(0), B.GetLength(1)];
        var threads = new Thread[threadsCount];
        int rowGroupSize = (int)Math.Ceiling(A.GetLength(0) / (float)threadsCount);
        int iterGroupSize = (int)Math.Ceiling(A.GetLength(1) / (float)threadsCount);
        var multiplicators = new Multiplicator[threads.Length];

        Barrier barrier = new(threadsCount);
        for (int i = 0; i < threads.Length; i++) {
            multiplicators[i] = new(
                iterGroupSize,
                GetSubRows(A, rowGroupSize, i),
                GetSubRows(B, rowGroupSize, i),
                C, threads.Length, i, barrier) {
                Multiplicators = multiplicators
            };
            threads[i] = new Thread(multiplicators[i].Multiply) { Name = $"Th-{i + 1}" };
        }
        foreach (var th in threads)
            th.Start();
        foreach (var th in threads)
            th.Join();
        return C;
    }

    private static double[,] GetSubRows(double[,] Arr, int rowGroupSize, int iGroup)
    {
        int irFrom = iGroup * rowGroupSize;
        int irTo = Math.Min(irFrom + rowGroupSize, Arr.GetLength(0));
        if (irFrom > irTo || irTo - irFrom == 0 ||
            irFrom < 0 || irTo > Arr.GetLength(0))
            throw new ArgumentException(
                "Incorrect sub rows bounds for " +
                $"A={Arr.GetLength(0)}X{Arr.GetLength(1)}:\n" +
                $"Rows: [{irFrom}; {irTo})"
            );
        var res = new double[irTo - irFrom, Arr.GetLength(1)];
        for (int ir = 0; ir < res.GetLength(0); ir++)
            for (int ic = 0; ic < res.GetLength(1); ic++)
                res[ir, ic] = Arr[irFrom + ir, ic];
        return res;
    }

    private class Multiplicator(
        int iterGroupSize, double[,] ASubRows, double[,] BSubRows,
        double[,] C, int iterCount, int iThis, Barrier barrier)
    {
        private int iA = iThis * iterGroupSize;
        private double[,] NextBSubRows = null!;
        private double[,] BSubRows = BSubRows;
        public Multiplicator[]? Multiplicators { get; set; } = null;
        private int INext => (iThis + 1) % Multiplicators!.Length;

        public void Multiply()
        {
            if (Multiplicators == null) {
                throw new InvalidOperationException(
                    "Multiplicators array is not initialized"
                );
            }
            for (int iter = 0; iter < iterCount; iter++) {
                IncrementCCells();
                iA += iterGroupSize;
                if (iA >= ASubRows.GetLength(1))
                    iA = 0;
                NextBSubRows = Multiplicators[INext].BSubRows;
                barrier.SignalAndWait();
                BSubRows = NextBSubRows;
                barrier.SignalAndWait();
            }
        }

        private void IncrementCCells()
        {
            int limIC = Math.Min(iA + iterGroupSize, ASubRows.GetLength(1));
            for (int icA = iA; icA < limIC; icA++)
                for (int irA = 0; irA < ASubRows.GetLength(0); irA++)
                    for (int icB = 0; icB < BSubRows.GetLength(1); icB++) {
                        var irC = iThis * iterGroupSize + irA;
                        C[irC, icB] += ASubRows[irA, icA] * BSubRows[icA - iA, icB];
                    }
        }
    }
}