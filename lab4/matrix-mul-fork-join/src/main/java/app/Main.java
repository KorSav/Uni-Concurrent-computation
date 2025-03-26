package app;

import app.SeqImpl.SequentialMul;
import app.ThreadsImpl.StripeMul;
import app.Utils.MatrixComparator;
import app.Utils.MatrixPair;
import app.Utils.MatrixUtils;
import app.Utils.Result;

public class Main {
    public static void main(String[] args) {
        int[] matrixSizes = new int[] { 500, 1000, 1500, 2000, 2500, 3000 };
        System.out.println("Warm up...");
        doExperiment(matrixSizes[1], 3, false);
        System.out.println("Matrix size | T_seq, s | T_ts, s | T_fj, s | S_ts | S_fj");
        for (int ms : matrixSizes) {
            ExperimentResult er = doExperiment(ms, 3, true);
            System.out.println("\r%12d|%10.3f|%9.3f|%9.3f|%6.3f|%6.3f - %.3e | %.3e"
                    .formatted(ms, er.GetTimeSeqSecs(),
                            er.GetTimeThreadsSecs(), er.GetTimeForkJoinSecs(),
                            er.GetSpeedupThreads(), er.GetSpeedupForkJoin(),
                            er.diffThreads, er.diffForkJoin));
        }
    }

    private static ExperimentResult doExperiment(int matrixSize, int passesCount, boolean verbose) {
        ExperimentResult result = new ExperimentResult();
        int procCount = Runtime.getRuntime().availableProcessors();
        Result resForkJoin, resSeq, resStripe;
        for (int i = 0; i < passesCount; i++) {
            if (verbose)
                System.out.print("\r%12d| pass %2d/%-2d"
                        .formatted(matrixSize, i + 1, passesCount));
            MatrixPair mp = MatrixUtils.generateRandomMatrices(matrixSize);
            resStripe = StripeMul.multiply(mp.getMatrixA(), mp.getMatrixB(), procCount);
            result.timeThreadsNanos += resStripe.getElapsedTimeNano();
            resForkJoin = app.ForkJoinImpl.StripeMul.multiply(mp.getMatrixA(), mp.getMatrixB());
            result.timeForkJoinNanos += resForkJoin.getElapsedTimeNano();
            resSeq = SequentialMul.multiply(mp.getMatrixA(), mp.getMatrixB());
            result.timeSeqNanos += resSeq.getElapsedTimeNano();
            result.diffThreads = MatrixComparator.calcMaxAbsDiff(
                    resStripe.getResultMatrix(),
                    resSeq.getResultMatrix());
            result.diffForkJoin = MatrixComparator.calcMaxAbsDiff(
                    resForkJoin.getResultMatrix(),
                    resSeq.getResultMatrix());
        }
        result.timeForkJoinNanos /= passesCount;
        result.timeThreadsNanos /= passesCount;
        result.timeSeqNanos /= passesCount;
        return result;
    }
}