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
    Thread logThread;
    int clientsCount;
    int modelNumber;
    int logIntervalMillis;

    public ModelRunner(
            int queueSize, int serversCount,
            double measureIntervalMillis,
            int clientProcessingTimeMillis,
            int meanClientCreationTimeMillis,
            int biasClientCreationTimeMillis,
            int clientsCount, int modelNumber, int logIntervalMillis) {
        this.clientsCount = clientsCount;
        this.modelNumber = modelNumber;
        this.logIntervalMillis = logIntervalMillis;
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
        logThread = new Thread(
                new ModelLogger(
                        producer, counter,
                        queue, this.logIntervalMillis, modelNumber));
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
        logThread.start();
        double cancelProb = futCancelCount.get() / (double) clientsCount;
        double avgQueueSize = futAverageQueueSize.get();
        executorService.shutdown();
        return new ModelResult(cancelProb, avgQueueSize, modelNumber);
    }
}
