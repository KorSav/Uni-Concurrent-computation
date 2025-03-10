using MatrixMul;
using ClosedXML.Excel;

const int matrixSize = 500;

double[,] matrixA = new double[matrixSize, matrixSize];
double[,] matrixB = new double[matrixSize, matrixSize];

Random random = new();
for (int ir = 0; ir < matrixSize; ir++)
    for (int ic = 0; ic < matrixSize; ic++) {
        matrixA[ir, ic] = random.NextDouble();
        matrixB[ir, ic] = random.NextDouble();
    }

System.Console.WriteLine("Stripe mul...");
var C_stripe = StripeMul.Multiply(matrixA, matrixB, 9);
System.Console.WriteLine("Fox mul...");
var C_fox = FoxMul.Multiply(matrixA, matrixB, 9);
System.Console.WriteLine("Sequential mul...");
var C_seq = SequentialMul.Multiply(matrixA, matrixB);
System.Console.WriteLine("Calculate diffs...");
var diff_stripe = MatrixComparator.CalcMaxAbsDiff(C_seq, C_stripe);
var diff_fox = MatrixComparator.CalcMaxAbsDiff(C_seq, C_fox);
System.Console.WriteLine("Saving to file...");

using (var wb = new XLWorkbook()) {
    var ws = wb.Worksheets.Add("Matrices");
    ws.Cell("A1").Value = $"Diff stripe: {diff_stripe}";
    ws.Cell("A2").Value = $"Diff fox: {diff_fox}";
    wb.SaveAs("test.xlsx");
}
