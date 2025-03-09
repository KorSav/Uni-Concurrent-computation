namespace MatrixMul;

public static class StripeMul
{
    public static double[,] Multiply(double[,] matrixA, double[,] matrixB, int threadsCount)
    {
        if (matrixA.GetLength(1) != matrixB.GetLength(0)) {
            throw new InvalidOperationException(
                $"[{matrixA.GetLength(0)}, {matrixA.GetLength(1)}] X " +
                $"[{matrixB.GetLength(0)}, {matrixB.GetLength(1)}]"
            );
        }
        var matrixC = new double[matrixA.GetLength(0), matrixB.GetLength(1)];
        var threads = new Thread[threadsCount];
        int rowGroupSize = (int)Math.Ceiling(matrixA.GetLength(0) / (float)threadsCount);
        int iterGroupSize = (int)Math.Ceiling(matrixA.GetLength(1) / (float)threadsCount);
        var multiplicators = new Multiplicator[threads.Length];

        for (int i = 0; i < threads.Length; i++) {
            multiplicators[i] = new(
                iterGroupSize, GetSubRows(matrixA, rowGroupSize, i), GetSubRows(matrixB, rowGroupSize, i), matrixC,
                threads.Length, i) {
                Multiplicators = multiplicators
            };
            threads[i] = new Thread(multiplicators[i].Multiply){Name=$"Th-{i+1}"};
        }
        foreach (var th in threads)
            th.Start();
        foreach (var th in threads)
            th.Join();
        return matrixC;
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
        double[,] C, int iterCount, int iThis)
    {
        private int iA = iThis * iterGroupSize;
        private double[,] NextBSubRows;
        private double[,] BSubRows = BSubRows;
        private bool _isReadyToSwap;
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
                _isReadyToSwap = false;
                IncrementCCells();
                iA += iterGroupSize;
                if (iA > ASubRows.GetLength(1))
                    iA = 0;
                NextBSubRows = Multiplicators[INext].BSubRows;
                _isReadyToSwap = true;
                lock (Multiplicators) {
                    while (!IsAllReadyToSwap()) {
                        Monitor.Wait(Multiplicators);
                    }
                    Monitor.PulseAll(Multiplicators);
                }
                BSubRows = NextBSubRows;
            }
        }

        private int TranslateArToCr(int irA){
            return iThis * iterGroupSize + irA;
        }

        private bool IsAllReadyToSwap()
        {
            foreach (var m in Multiplicators!) {
                if (!m._isReadyToSwap)
                    return false;
            }
            return true;
        }

        private void IncrementCCells()
        {
            int limIC = Math.Min(iA + iterGroupSize, ASubRows.GetLength(1));
            for (int icA = iA; icA < limIC; icA++)
                for (int irA = 0; irA < ASubRows.GetLength(0); irA++)
                    for (int icB = 0; icB < BSubRows.GetLength(1); icB++) {
                        var cr = TranslateArToCr(irA);
                        C[cr, icB] += ASubRows[irA, icA] * BSubRows[icA-iA, icB];
                    }
        }
    }
}