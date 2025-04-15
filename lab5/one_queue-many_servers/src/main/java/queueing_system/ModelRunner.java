package queueing_system;

import java.util.concurrent.Callable;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.Future;

public class ModelRunner implements Callable<ModelResult> {

    SemiBlockingQueue<Client> queue;
    Consumer[] consumers;
    Producer producer;
    QueueLengthCounter counter;
    Future<Integer> futCancelCount;
    Future<Double> futAverageQueueSize;
    int clientsCount = 1000;

    public ModelRunner(
            int queueSize, int serversCount,
            double measureIntervalMillis,
            int clientProcessingTimeMillis,
            int meanClientCreationTimeMillis,
            int biasClientCreationTimeMillis) {
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

    @Override
    public ModelResult call() throws Exception {
        ExecutorService executorService = Executors
                .newFixedThreadPool(Runtime.getRuntime().availableProcessors());
        for (Consumer consumer : consumers) {
            executorService.submit(consumer);
        }
        futCancelCount = executorService.submit(producer);
        futAverageQueueSize = executorService.submit(counter);
        double cancelProb = futCancelCount.get() / (double) clientsCount * 100;
        double avgQueueSize = futAverageQueueSize.get();
        System.out.println("Cancel prob: %.1f%%; Avg queue size: %f"
                .formatted(cancelProb, avgQueueSize));
        executorService.shutdown();
        return new ModelResult(cancelProb, avgQueueSize);
    }
}
