package app.Utils;

import java.util.Random;

public class MatrixUtils {
    public static MatrixPair generateRandomMatrices(int matrixSize) {
        double[][] matrixA = new double[matrixSize][matrixSize];
        double[][] matrixB = new double[matrixSize][matrixSize];

        Random random = new Random();
        for (int ir = 0; ir < matrixSize; ir++) {
            for (int ic = 0; ic < matrixSize; ic++) {
                matrixA[ir][ic] = random.nextDouble();
                matrixB[ir][ic] = random.nextDouble();
            }
        }
        return new MatrixPair(matrixA, matrixB);
    }
}
