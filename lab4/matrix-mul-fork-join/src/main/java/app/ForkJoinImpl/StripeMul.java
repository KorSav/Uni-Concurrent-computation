package app.ForkJoinImpl;

import java.util.concurrent.ForkJoinPool;

import app.Utils.Result;

public class StripeMul {

    public static Result multiply(double[][] A, double[][] B) {
        long startTime = System.nanoTime();
        double[][] C = ForkJoinPool.commonPool().invoke(new RootMatrixMulTask(A, B));
        long elapsedTime = System.nanoTime() - startTime;
        return new Result(C, elapsedTime);
    }

}