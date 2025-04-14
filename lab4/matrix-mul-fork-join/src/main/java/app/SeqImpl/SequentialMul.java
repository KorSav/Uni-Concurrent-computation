package app.SeqImpl;

import app.Utils.Result;

public class SequentialMul {

    public static Result multiply(double[][] matrixA, double[][] matrixB) {
        long startTime = System.nanoTime();
        validateMultiplication(matrixA, matrixB);
        double[][] res = new double[matrixA.length][matrixB[0].length];

        for (int irA = 0; irA < matrixA.length; irA++) {
            for (int icB = 0; icB < matrixB[0].length; icB++) {
                res[irA][icB] = multiplyRowByColumn(matrixA, matrixB, irA, icB);
            }
        }

        long elapsedTime = System.nanoTime() - startTime;
        return new Result(res, elapsedTime);
    }

    public static void multiplyAndIncrement(double[][] matrixA, double[][] matrixB, double[][] matrixResult) {
        validateMultiplication(matrixA, matrixB);
        if (matrixA.length != matrixResult.length || matrixB[0].length != matrixResult[0].length) {
            throw new IllegalArgumentException(
                    "Unable to increment matrix [" + matrixResult.length + "x" + matrixResult[0].length + "] " +
                            "with values from [" + matrixA.length + "x" + matrixB[0].length + "]");
        }

        for (int irA = 0; irA < matrixA.length; irA++) {
            for (int icB = 0; icB < matrixB[0].length; icB++) {
                matrixResult[irA][icB] += multiplyRowByColumn(matrixA, matrixB, irA, icB);
            }
        }
    }

    public static void validateMultiplication(double[][] matrixA, double[][] matrixB) {
        if (matrixA[0].length != matrixB.length) {
            throw new IllegalArgumentException(
                    "[" + matrixA.length + ", " + matrixA[0].length + "] X " +
                            "[" + matrixB.length + ", " + matrixB[0].length + "]");
        }
    }

    private static double multiplyRowByColumn(double[][] A, double[][] B, int irA, int icB) {
        double res = 0;
        for (int i = 0; i < A[0].length; i++) {
            res += A[irA][i] * B[i][icB];
        }
        return res;
    }
}
