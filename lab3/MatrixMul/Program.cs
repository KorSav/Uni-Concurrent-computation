using MatrixMul;
// See https://aka.ms/new-console-template for more information
const int matrixSize = 3;
double[,] matrixA = new double[matrixSize, matrixSize];
double[,] matrixB = new double[matrixSize, matrixSize];

Random random = new();
for (int ir = 0; ir < matrixSize; ir++)
    for (int ic = 0; ic < matrixSize; ic++) {
        matrixA[ir, ic] = random.NextDouble();
        matrixB[ir, ic] = random.NextDouble();
    }
StripeMul.Multiply(matrixA, matrixB, 2);
