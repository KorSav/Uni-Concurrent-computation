package queueing_system;

public class ModelResult {
    public double cancelProbability;
    public double avgQueueSize;

    public ModelResult(double cancelProbability, double avgQueueSize) {
        this.cancelProbability = cancelProbability;
        this.avgQueueSize = avgQueueSize;
    }
}
