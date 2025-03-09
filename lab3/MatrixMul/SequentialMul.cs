namespace MatrixMul;

public static class SequentialMul
{
    public static double[,] Multiply(double[,] matrixA, double[,] matrixB)
    {
        if (matrixA.GetLength(1) != matrixB.GetLength(0)) {
            throw new InvalidOperationException(
                $"[{matrixA.GetLength(0)}, {matrixA.GetLength(1)}] X " +
                $"[{matrixB.GetLength(0)}, {matrixB.GetLength(1)}]"
            );
        }
        double[,] res = new double[matrixA.GetLength(0), matrixB.GetLength(1)];
        for (int irA = 0; irA < matrixA.GetLength(0); irA++) {
            for (int icB = 0; icB < matrixB.GetLength(1); icB++) {
                res[irA, icB] = MultiplyRowByColumn(matrixA, matrixB, irA, icB);
            }
        }
        return res;
    }

    private static double MultiplyRowByColumn(double[,] A, double[,] B, int irA, int icB)
    {
        double res = 0;
        for (int i = 0; i < A.GetLength(1); i++)
                res += A[irA, i] * B[i, icB];
        return res;
    }
}