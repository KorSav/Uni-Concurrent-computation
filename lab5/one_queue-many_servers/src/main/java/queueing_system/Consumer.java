package queueing_system;

public class Consumer implements Runnable {

    private final SemiBlockingQueue<Client> _queue;

    public Consumer(SemiBlockingQueue<Client> queue) {
        super();
        _queue = queue;
    }

    @Override
    public void run() {
        Client client = null;
        try {
            while (true) {
                client = _queue.take();
                if (client == null) {
                    return;
                }
                client.run();
            }
        } catch (InterruptedException e) {
            e.printStackTrace();
        }
    }

}
