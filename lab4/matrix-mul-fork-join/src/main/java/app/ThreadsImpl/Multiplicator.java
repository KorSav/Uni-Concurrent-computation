package app.ThreadsImpl;

import java.util.concurrent.BrokenBarrierException;
import java.util.concurrent.CyclicBarrier;

public class Multiplicator {
    private final int iterGroupSize;
    private final double[][] ASubRows;
    private double[][] BSubRows;
    private final double[][] C;
    private final int iterCount;
    private final int iThis;
    private final CyclicBarrier barrier;

    private Multiplicator[] multiplicators;

    private int iA;

    private double[][] nextBSubRows;

    public Multiplicator(int iterGroupSize, double[][] ASubRows, double[][] BSubRows,
            double[][] C, int iterCount, int iThis, CyclicBarrier barrier) {
        this.iterGroupSize = iterGroupSize;
        this.ASubRows = ASubRows;
        this.BSubRows = BSubRows;
        this.C = C;
        this.iterCount = iterCount;
        this.iThis = iThis;
        this.barrier = barrier;
        this.iA = iThis * iterGroupSize;
    }

    public void setMultiplicators(Multiplicator[] multiplicators) {
        this.multiplicators = multiplicators;
    }

    private int nextIndex() {
        return (iThis + 1) % multiplicators.length;
    }

    public void multiply() {
        if (multiplicators == null) {
            throw new IllegalStateException("Multiplicators array is not initialized");
        }

        for (int iter = 0; iter < iterCount; iter++) {
            incrementCCells();

            iA += iterGroupSize;
            if (iA >= ASubRows[0].length) {
                iA = 0;
            }

            nextBSubRows = multiplicators[nextIndex()].BSubRows;

            try {
                barrier.await();
            } catch (InterruptedException | BrokenBarrierException e) {
                Thread.currentThread().interrupt();
                throw new RuntimeException(e);
            }

            BSubRows = nextBSubRows;

            try {
                barrier.await();
            } catch (InterruptedException | BrokenBarrierException e) {
                Thread.currentThread().interrupt();
                throw new RuntimeException(e);
            }
        }
    }

    private void incrementCCells() {
        int limIC = Math.min(iA + iterGroupSize, ASubRows[0].length);
        for (int icA = iA; icA < limIC; icA++) {
            for (int irA = 0; irA < ASubRows.length; irA++) {
                for (int icB = 0; icB < BSubRows[0].length; icB++) {
                    int irC = iThis * iterGroupSize + irA;
                    C[irC][icB] += ASubRows[irA][icA] * BSubRows[icA - iA][icB];
                }
            }
        }
    }
}
