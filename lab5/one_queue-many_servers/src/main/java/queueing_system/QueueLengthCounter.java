package queueing_system;

import java.util.ArrayList;
import java.util.concurrent.Callable;
import java.util.stream.Collectors;

public class QueueLengthCounter implements Callable<Double> {

    private ArrayList<Integer> _queueSizes;
    private ArrayList<Double> _timeSpansMillis;
    private final double _intervalMillis;
    private final SemiBlockingQueue<Client> _queue;

    public QueueLengthCounter(SemiBlockingQueue<Client> queue, double intervalMillis) {
        super();
        _intervalMillis = intervalMillis;
        _queue = queue;
        _queueSizes = new ArrayList<>();
        _timeSpansMillis = new ArrayList<>();
    }

    @Override
    public Double call() {
        while (!_queue.isTerminated()) {
            takeMeasure();
        }
        while (_queue.size() != 0) {
            takeMeasure();
        }
        return getAverageQueueSize();
    }

    public double getAverageQueueSize() {
        double timeSum = _timeSpansMillis.stream()
                .collect(Collectors.summingDouble(ts -> ts));
        double avgSize = 0;
        for (int i = 0; i < _timeSpansMillis.size(); i++) {
            avgSize += _queueSizes.get(i) * _timeSpansMillis.get(i) / timeSum;
        }
        return avgSize;
    };

    private void takeMeasure() {
        long nanos = System.nanoTime();
        try {
            Thread.sleep((long) _intervalMillis);
        } catch (InterruptedException e) {
            e.printStackTrace();
        }
        _queueSizes.add(_queue.size());
        _timeSpansMillis.add((System.nanoTime() - nanos) / 1e6);
    }
}