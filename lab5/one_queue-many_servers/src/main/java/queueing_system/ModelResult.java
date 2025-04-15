package queueing_system;

public class ModelResult {
    public double cancelProbability;
    public double avgQueueSize;
    public int modelNumber;

    public ModelResult(double cancelProbability, double avgQueueSize, int modelNumber) {
        this.cancelProbability = cancelProbability;
        this.avgQueueSize = avgQueueSize;
        this.modelNumber = modelNumber;
    }
}
