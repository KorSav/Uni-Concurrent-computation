package app.Utils;

public class MatrixComparator {

    public static double calcMaxAbsDiff(double[][] A, double[][] B) {
        if (A.length != B.length || A[0].length != B[0].length) {
            throw new IllegalArgumentException(
                    "Can't compare matrices:\n" +
                            "A " + A.length + " X " + A[0].length + " and " +
                            "B " + B.length + " X " + B[0].length);
        }

        double res = 0;
        for (int ir = 0; ir < A.length; ir++) {
            for (int ic = 0; ic < A[0].length; ic++) {
                double absDiff = Math.abs(A[ir][ic] - B[ir][ic]);
                if (absDiff > res) {
                    res = absDiff;
                }
            }
        }
        return res;
    }
}
