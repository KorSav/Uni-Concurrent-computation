package app.Utils;

public class Result {
    private final double[][] resultMatrix;
    private final long elapsedTimeNano;

    public Result(double[][] resultMatrix, long elapsedTimeNano) {
        this.resultMatrix = resultMatrix;
        this.elapsedTimeNano = elapsedTimeNano;
    }

    public double[][] getResultMatrix() {
        return resultMatrix;
    }

    public long getElapsedTimeNano() {
        return elapsedTimeNano;
    }
}
