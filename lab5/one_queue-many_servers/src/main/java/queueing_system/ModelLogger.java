package queueing_system;

public class ModelLogger implements Runnable {
    private final Producer _producer;
    private final QueueLengthCounter _counter;
    private final SemiBlockingQueue<Client> _queue;
    private final int _timeIntervalMillis;
    private final int _modelNumber;

    public ModelLogger(
            Producer producer, QueueLengthCounter counter,
            SemiBlockingQueue<Client> queue, int timeIntervalMillis,
            int modelNumber) {
        _producer = producer;
        _counter = counter;
        _queue = queue;
        _timeIntervalMillis = timeIntervalMillis;
        _modelNumber = modelNumber;
    }

    @Override
    public void run() {
        while (!_queue.isTerminated()) {
            try {
                Thread.sleep(_timeIntervalMillis);
                if (_queue.isTerminated())
                    break;
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
            System.out.println("Model-%-2d state: queue - %1d, canceled - %d"
                    .formatted(
                            _modelNumber,
                            _counter.getQueueSize(),
                            _producer.getCancelCount()));
        }
    }

}
