package queueing_system;

import java.util.concurrent.ExecutionException;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.Future;

public class Main {
    static SemiBlockingQueue<Client> queue;
    static Consumer[] consumers;
    static Producer producer;
    static QueueLengthCounter counter;
    static Future<Integer> futCancelCount;
    static Future<Double> futAverageQueueSize;
    static int clientsCount = 1000;

    public static void main(String[] args) throws InterruptedException, ExecutionException {
        ExecutorService executorService = Executors
                .newFixedThreadPool(Runtime.getRuntime().availableProcessors());
        initModel();
        for (Consumer consumer : consumers) {
            executorService.submit(consumer);
        }
        futCancelCount = executorService.submit(producer);
        futAverageQueueSize = executorService.submit(counter);
        System.out.println("Cancel prob: %.1f%%; Avg queue size: %f"
                .formatted(
                        futCancelCount.get() / (double) clientsCount * 100.,
                        futAverageQueueSize.get()));
        executorService.shutdown();
    }

    private static void initModel() {
        int queueSize = 5;
        int serversCount = 3;
        double measureIntervalMillis = 10;
        int clientProcessingTimeMillis = 15;
        int meanClientCreationTimeMillis = 4;
        int biasClientCreationTimeMillis = 1;

        queue = new SemiBlockingQueue<>(queueSize);
        consumers = new Consumer[serversCount];
        for (int i = 0; i < serversCount; i++) {
            consumers[i] = new Consumer(queue);
        }
        producer = new Producer(
                queue, clientsCount,
                meanClientCreationTimeMillis,
                biasClientCreationTimeMillis,
                clientProcessingTimeMillis);
        counter = new QueueLengthCounter(queue, measureIntervalMillis);
    }
}