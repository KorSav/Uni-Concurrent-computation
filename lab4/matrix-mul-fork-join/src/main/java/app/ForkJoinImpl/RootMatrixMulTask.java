package app.ForkJoinImpl;

import java.util.concurrent.RecursiveTask;

public class RootMatrixMulTask extends RecursiveTask<double[][]> {

    private double[][] A;
    private double[][] B;
    private static int[] groupIndexForTask;
    private static int iterGroupSize;
    private static int rowGroupSize;

    RootMatrixMulTask(double[][] A, double[][] B) {
        this.A = A;
        this.B = B;
    }

    @Override
    protected double[][] compute() {
        int procCount = Runtime.getRuntime().availableProcessors();
        double[][] C = new double[A.length][B[0].length];
        rowGroupSize = (int) Math.ceil((double) A.length / procCount);
        iterGroupSize = (int) Math.ceil((double) A[0].length / procCount);
        groupIndexForTask = new int[procCount];
        for (int i = 0; i < groupIndexForTask.length; i++) {
            groupIndexForTask[i] = i;
        }
        MatrixMulTask[] partialMulTasks = new MatrixMulTask[procCount];

        for (int i = 0; i < procCount; i++) {
            for (int it = 0; it < procCount; it++) {
                partialMulTasks[it] = new MatrixMulTask(
                        getSubCols(getSubRows(A, it), groupIndexForTask[it]),
                        getSubRows(B, groupIndexForTask[it]));
                partialMulTasks[it].fork();
            }
            for (int it = 0; it < procCount; it++) {
                addPartial(C, partialMulTasks[it].join(),
                        it * rowGroupSize, 0);
            }
            shiftIndices();
        }
        return C;
    }

    private static void addPartial(double[][] C, double[][] M, int irC, int icC) {
        for (int irM = 0; irM < M.length; irM++) {
            for (int icM = 0; icM < M[0].length; icM++) {
                C[irC + irM][icC + icM] += M[irM][icM];
            }
        }
    }

    private static void shiftIndices() {
        for (int i = 0; i < groupIndexForTask.length; i++) {
            groupIndexForTask[i] = (groupIndexForTask[i] + 1) % groupIndexForTask.length;
        }
    }

    private static double[][] getSubRows(double[][] arr, int iGroup) {
        int irFrom = iGroup * rowGroupSize;
        int remainingRows = arr.length - irFrom;

        double[][] res = new double[Math.min(remainingRows, rowGroupSize)][arr[0].length];
        for (int ir = 0; ir < res.length; ir++) {
            System.arraycopy(arr[irFrom + ir], 0, res[ir], 0, arr[0].length);
        }
        return res;
    }

    private static double[][] getSubCols(double[][] arr, int iGroup) {
        int icFrom = iGroup * iterGroupSize;
        int remainingCols = arr[0].length - icFrom;

        double[][] res = new double[arr.length][Math.min(remainingCols, iterGroupSize)];
        for (int ir = 0; ir < res.length; ir++) {
            for (int ic = 0; ic < res[0].length; ic++) {
                res[ir][ic] = arr[ir][icFrom + ic];
            }
        }
        return res;
    }
}
