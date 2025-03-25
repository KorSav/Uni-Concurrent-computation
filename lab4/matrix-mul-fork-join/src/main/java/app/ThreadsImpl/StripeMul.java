package app.ThreadsImpl;

import java.util.concurrent.CyclicBarrier;

import app.Utils.Result;

public class StripeMul {

    public static Result multiply(double[][] A, double[][] B, int threadsCount) {
        long startTime = System.nanoTime();
        double[][] C = new double[A.length][B[0].length];
        Thread[] threads = new Thread[threadsCount];
        int rowGroupSize = (int) Math.ceil((double) A.length / threadsCount);
        int iterGroupSize = (int) Math.ceil((double) A[0].length / threadsCount);
        Multiplicator[] multiplicators = new Multiplicator[threadsCount];
        CyclicBarrier barrier = new CyclicBarrier(threadsCount);

        for (int i = 0; i < threadsCount; i++) {
            double[][] ASubRows = getSubRows(A, rowGroupSize, i);
            double[][] BSubRows = getSubRows(B, rowGroupSize, i);
            multiplicators[i] = new Multiplicator(iterGroupSize, ASubRows, BSubRows, C, threadsCount, i, barrier);
            multiplicators[i].setMultiplicators(multiplicators);
            threads[i] = new Thread(multiplicators[i]::multiply, "Th-" + (i + 1));
        }

        for (Thread t : threads) {
            t.start();
        }
        for (Thread t : threads) {
            try {
                t.join();
            } catch (InterruptedException e) {
                Thread.currentThread().interrupt();
                throw new RuntimeException("Thread interrupted", e);
            }
        }

        long elapsedTime = System.nanoTime() - startTime;
        return new Result(C, elapsedTime);
    }

    private static double[][] getSubRows(double[][] arr, int rowGroupSize, int iGroup) {
        int irFrom = iGroup * rowGroupSize;
        int irTo = Math.min(irFrom + rowGroupSize, arr.length);

        double[][] res = new double[irTo - irFrom][arr[0].length];
        for (int ir = 0; ir < res.length; ir++) {
            System.arraycopy(arr[irFrom + ir], 0, res[ir], 0, arr[0].length);
        }
        return res;
    }
}