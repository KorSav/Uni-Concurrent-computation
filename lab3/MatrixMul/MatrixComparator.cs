namespace MatrixMul;

static class MatrixComparator
{
    public static double CalcMaxAbsDiff(double[,] A, double[,] B)
    {
        if (A.GetLength(0) != B.GetLength(0) ||
            A.GetLength(1) != B.GetLength(1))
            throw new ArgumentException(
                "Can't compare matrices:\n" +
                $"A {A.GetLength(0)} X {A.GetLength(1)} and " +
                $"B {B.GetLength(0)} X {B.GetLength(1)}"
                );
        double res = 0;
        for (int ir = 0; ir < A.GetLength(0); ir++)
            for (int ic = 0; ic < A.GetLength(1); ic++) {
                var absDiff = Math.Abs(A[ir, ic] - B[ir, ic]);
                if (absDiff > res) {
                    res = absDiff;
                }
            }
        return res;
    }
}