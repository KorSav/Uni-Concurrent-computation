package queueing_system;

public class Client implements Runnable {
    private final int _processingTime;

    public Client(int processingTime) {
        _processingTime = processingTime;
    }

    @Override
    public void run() {
        try {
            Thread.sleep(_processingTime);
        } catch (InterruptedException e) {
            e.printStackTrace();
        }
    }
}
