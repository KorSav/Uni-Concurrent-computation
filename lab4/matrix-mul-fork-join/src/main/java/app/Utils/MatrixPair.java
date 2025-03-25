package app.Utils;

public class MatrixPair {
    private final double[][] matrixA;
    private final double[][] matrixB;

    public MatrixPair(double[][] matrixA, double[][] matrixB) {
        this.matrixA = matrixA;
        this.matrixB = matrixB;
    }

    public double[][] getMatrixA() {
        return matrixA;
    }

    public double[][] getMatrixB() {
        return matrixB;
    }
}