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
var C_stripe = StripeMul.Multiply(matrixA, matrixB, 2);
var C_seq = SequentialMul.Multiply(matrixA, matrixB);
System.Console.WriteLine($"Diff: {MatrixComparator.CalcMaxAbsDiff(C_seq, C_stripe)}");

foreach (var v in C_stripe) System.Console.Write($"{v} ");
System.Console.WriteLine();
foreach (var v in C_seq) System.Console.Write($"{v} ");