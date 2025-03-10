namespace MatrixMul;

public static class FoxMul
{
    /// <summary>
    /// Multiplies matrices using Fox algoritm.
    /// Actual amount of threads used (atu):
    /// atu = n^2 for natural n, while atu ≤ threadsCount
    /// </summary>
    public static double[,] Multiply(double[,] A, double[,] B, int threadsCount)
    {
        if (A.GetLength(1) != B.GetLength(0)) {
            throw new InvalidOperationException(
                $"[{A.GetLength(0)}, {A.GetLength(1)}] X " +
                $"[{B.GetLength(0)}, {B.GetLength(1)}]"
            );
        }
        var C = new double[A.GetLength(0), B.GetLength(1)];
        int subMsCountPerSide = (int)Math.Floor(Math.Sqrt(threadsCount));
        threadsCount = subMsCountPerSide * subMsCountPerSide;

        double[][][,] subMAs = GetSubMs(A, subMsCountPerSide);
        double[][][,] subMBs = GetSubMs(B, subMsCountPerSide);
        var multiplicators = new Multiplicator[threadsCount];
        var threads = InitializeMultiplicators(subMAs, subMBs, multiplicators);

        foreach (var th in threads)
            th.Start();
        foreach (var th in threads)
            th.Join();

        JoinMultiplicatorsResultsInto(multiplicators, C);
        return C;
    }

    private static void JoinMultiplicatorsResultsInto(Multiplicator[] multiplicators, double[,] M)
    {
        int subMSize = multiplicators[0].SubMC.GetLength(0);
        foreach (var m in multiplicators) {
            for (int ir = 0; ir < m.SubMC.GetLength(0); ir++)
                for (int ic = 0; ic < m.SubMC.GetLength(1); ic++) {
                    int irM = m.IRC * subMSize + ir;
                    int icM = m.ICC * subMSize + ic;
                    M[irM, icM] = m.SubMC[ir, ic];
                }
        }
    }

    private static Thread[] InitializeMultiplicators(double[][][,] subMAs, double[][][,] subMBs, Multiplicator[] multiplicators)
    {
        var threads = new Thread[multiplicators.Length];
        Barrier barrier = new(multiplicators.Length);
        for (int i = 0; i < threads.Length; i++) {
            int irSubM = i / subMAs.Length;
            int icSubM = i % subMAs.Length;
            multiplicators[i] = new(irSubM,
                subMAs[irSubM], subMBs[irSubM][icSubM],
                irSubM, icSubM, subMAs.Length, barrier
            );
            threads[i] = new Thread(multiplicators[i].Multiply) { Name = $"Th-{i + 1}" };
        }
        for (int i = 0; i < multiplicators.Length; i++) {
            int iDownNeig = (i + subMAs.Length) % multiplicators.Length;
            multiplicators[i].DownNeigh = multiplicators[iDownNeig];
        }
        return threads;
    }

    private static double[][][,] GetSubMs(double[,] M, int subMsCountPerSide)
    {
        int subMRowSize = (int)Math.Ceiling(M.GetLength(0) / (float)subMsCountPerSide);
        double[][][,] res = new double[subMsCountPerSide][][,];
        for (int irSM = 0; irSM < subMsCountPerSide; irSM++) {
            res[irSM] = new double[subMsCountPerSide][,];
            for (int icSM = 0; icSM < subMsCountPerSide; icSM++) {
                var irM = irSM * subMRowSize;
                var irMLim = Math.Min(irM + subMRowSize, M.GetLength(0));
                var icM = icSM * subMRowSize;
                var icMLim = Math.Min(icM + subMRowSize, M.GetLength(1));
                res[irSM][icSM] = GetSubM(M, irM, irMLim, icM, icMLim);
            }
        }
        return res;
    }

    /// <summary>
    /// Strip some part of matrix into new one
    /// </summary>
    /// <returns>Rows: [irFrom; irTo), Cols: [icFrom; icTo)</returns>
    private static double[,] GetSubM(double[,] M,
        int irFrom, int irTo,
        int icFrom, int icTo)
    {
        double[,] res = new double[irTo - irFrom, icTo - icFrom];
        for (int ir = 0; ir < res.GetLength(0); ir++)
            for (int ic = 0; ic < res.GetLength(1); ic++) {
                res[ir, ic] = M[irFrom + ir, icFrom + ic];
            }
        return res;
    }

    private class Multiplicator(
        int iA, double[][,] SubMARow, double[,] SubMB,
        int irC, int icC, int iterCount, Barrier barrier)
    {
        private int _iA = iA;
        private readonly int _iterCount = iterCount;
        private readonly double[][,] _subMARow = SubMARow;
        private double[,] _subMB = SubMB;
        private double[,] _nextSubMB = null!;

        public readonly double[,] SubMC = new double[
            SubMARow[iA].GetLength(0),
            SubMB.GetLength(1)
        ];
        public readonly int IRC = irC;
        public readonly int ICC = icC;
        public Multiplicator? DownNeigh { get; set; } = null;

        public void Multiply()
        {
            if (DownNeigh == null) {
                throw new InvalidOperationException(
                    "Multiplicator's bottom neighbour is not initialized"
                );
            }
            for (int iter = 0; iter < _iterCount; iter++) {
                SequentialMul.MultiplyAndIncrement(_subMARow[_iA], _subMB, SubMC);
                _iA = (_iA + 1) % _subMARow.Length;
                _nextSubMB = DownNeigh._subMB;
                barrier.SignalAndWait();
                _subMB = _nextSubMB;
                barrier.SignalAndWait();
            }
        }

    }
}