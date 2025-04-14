package app;

import java.io.File;
import java.io.IOException;

import app.analyzer.ExperimentResult;
import app.analyzer.WordsLengthCounter;

public class Main {
    public static void main(String[] args) throws IOException {
        Folder folder = Folder.fromDirectory(new File(args[0]));
        System.out.println("Warm up...");
        doExperiment(folder, 3);
        System.out.println("Calculating results...");
        ExperimentResult res = doExperiment(folder, 10, true);
        printResult(res);
    }

    private static ExperimentResult doExperiment(Folder folder, int passesCount) {
        return doExperiment(folder, passesCount, false);
    }

    private static ExperimentResult doExperiment(Folder folder, int passesCount, boolean verbose) {
        WordsLengthCounter wlc = new WordsLengthCounter();
        ExperimentResult result = new ExperimentResult();
        long tpSum = 0, tsSum = 0;
        for (int i = 0; i < passesCount; i++) {
            if (verbose)
                System.out.print("\rDoing experiment %2d/%-2d".formatted(i + 1, passesCount));
            long t1 = System.currentTimeMillis();
            result.histSeq = wlc.getWordsLengthHistogramSequential(folder);
            long t2 = System.currentTimeMillis();
            result.histPar = wlc.getWordsLengthHistogramParallel(folder);
            tpSum += System.currentTimeMillis() - t2;
            tsSum += t2 - t1;
        }
        if (verbose)
            System.out.println();
        result.timePar = tpSum / passesCount;
        result.timeSeq = tsSum / passesCount;
        result.procCount = Runtime.getRuntime().availableProcessors();
        return result;
    }

    private static void printResult(ExperimentResult result) {
        System.out.println("Histogram:");
        System.out.println("Word lenght | Sequential count | Parallel count");
        for (int wl = 1; wl < result.histSeq.getCount() + 1; wl++) {
            System.out.println("%-2d - %7d  %-7d"
                    .formatted(wl, result.histSeq.Get(wl), result.histPar.Get(wl)));
        }
        System.out.println("Mean: %.3f".formatted(result.histSeq.CalculateMean()));
        System.out.println("Variance: %.3f".formatted(result.histSeq.CalculateVariance()));
        System.out.println("\nMethods comparison:");
        System.out.println("Time sequential: %d ms".formatted(result.timeSeq));
        System.out.println("Time parallel  : %d ms".formatted(result.timePar));
        System.out.println("Speedup: %f".formatted(result.GetSpeedup()));
        System.out.println("Efficiency: %f".formatted(result.GetEfficiency()));
        System.out.println("Cost: %d".formatted(result.GetCost()));
    }
}