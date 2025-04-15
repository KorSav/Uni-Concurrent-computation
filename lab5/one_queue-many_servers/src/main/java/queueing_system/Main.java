package queueing_system;

import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.Future;
import java.util.concurrent.TimeUnit;

public class Main {

    static int passesCount = 20;
    static int clientsCount = 1000;
    static int queueSize = 5;
    static int serversCount = 3;
    static double measureIntervalMillis = 10;
    static int logIntervalMillis = 200;
    static int clientProcessingTimeMillis = 15;
    static int meanClientCreationTimeMillis = 4;
    static int biasClientCreationTimeMillis = 1;

    public static void main(String[] args) throws InterruptedException, ExecutionException {
        ExecutorService executorService = Executors
                .newFixedThreadPool(Runtime.getRuntime().availableProcessors());
        ArrayList<ModelRunner> modelRunners = new ArrayList<>(passesCount);
        for (int i = 0; i < passesCount; i++) {
            modelRunners.add(new ModelRunner(
                    queueSize, serversCount,
                    measureIntervalMillis, clientProcessingTimeMillis,
                    meanClientCreationTimeMillis, biasClientCreationTimeMillis,
                    clientsCount, i + 1, logIntervalMillis));
        }
        List<Future<ModelResult>> results = executorService.invokeAll(modelRunners);
        ModelResult result = new ModelResult(0, 0, 0);
        for (var future : results) {
            result.avgQueueSize += future.get().avgQueueSize;
            result.cancelProbability += future.get().cancelProbability;
        }
        executorService.shutdown();
        executorService.awaitTermination(10, TimeUnit.SECONDS);
        System.out.println("\nModels results:");
        for (var future : results) {
            ModelResult mr = future.get();
            System.out.println("%-2d Cancel prob: %.3f; Avg queue size: %f"
                    .formatted(mr.modelNumber, mr.cancelProbability, mr.avgQueueSize));
        }
        result.avgQueueSize /= passesCount;
        result.cancelProbability /= passesCount;
        System.out.println("\nResult of modelling with %d passes"
                .formatted(passesCount));
        System.out.println("Cancel prob: %.3f; Avg queue size: %f"
                .formatted(result.cancelProbability, result.avgQueueSize));
    }
}