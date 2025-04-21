package queueing_system;

import java.util.Random;
import java.util.concurrent.Callable;

public class Producer implements Callable<Integer> {

    private final SemiBlockingQueue<Client> _queue;
    private final int _max_client_count;
    private int _cur_client_count = 0;
    private int _cancel_count = 0;
    private final Random random = new Random();
    private final int _bias;
    private final int _mean;
    private final int _clientPTime;

    public Producer(
            SemiBlockingQueue<Client> queue, int clients_count,
            int meanTimeMillis, int biasTimeMillis,
            int clientProcessingTimeMillis) {
        super();
        _queue = queue;
        _max_client_count = clients_count;
        _mean = meanTimeMillis;
        _bias = biasTimeMillis;
        _clientPTime = clientProcessingTimeMillis;
    }

    @Override
    public Integer call() throws InterruptedException {
        for (; _cur_client_count < _max_client_count; _cur_client_count++) {
            Thread.sleep(getWaitTime());
            Client c = new Client(_clientPTime);
            boolean isPut = _queue.put(c);
            if (!isPut) {
                ++_cancel_count;
            }
        }
        _queue.terminate();
        return _cancel_count;
    }

    private int getWaitTime() {
        int noise = random.nextInt(_bias * 2) - _bias;
        return noise + _mean;
    }

    public int getCancelCount() {
        return _cancel_count;
    }
}