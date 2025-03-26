package app.ForkJoinImpl;

import java.util.concurrent.RecursiveTask;

public class MatrixMulTask extends RecursiveTask<double[][]> {
    private final double[][] A;
    private final double[][] B;

    public MatrixMulTask(double[][] ASubRows, double[][] BSubRows) {
        this.A = ASubRows;
        this.B = BSubRows;
    }

    @Override
    protected double[][] compute() {
        double[][] C = new double[A.length][B[0].length];
        for (int irA = 0; irA < A.length; irA++) {
            for (int icB = 0; icB < B[0].length; icB++) {
                for (int i = 0; i < A[0].length; i++) {
                    C[irA][icB] += A[irA][i] * B[i][icB];
                }
            }
        }
        return C;
    }
}
