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
var StripeRes = StripeMul.Multiply(matrixA, matrixB, 9);
System.Console.WriteLine("Fox mul...");
var FoxRes = FoxMul.Multiply(matrixA, matrixB, 9);
System.Console.WriteLine("Sequential mul...");
var SeqRes = SequentialMul.Multiply(matrixA, matrixB);
System.Console.WriteLine("Calculate diffs...");
var diff_stripe = MatrixComparator.CalcMaxAbsDiff(SeqRes.M, StripeRes.M);
var diff_fox = MatrixComparator.CalcMaxAbsDiff(SeqRes.M, FoxRes.M);
System.Console.WriteLine("Saving to file...");

using (var wb = new XLWorkbook()) {
    var ws = wb.Worksheets.Add("Matrices");
    ws.Cell("A1").Value = $"Diff stripe: {diff_stripe}";
    ws.Cell("A2").Value = $"Diff fox: {diff_fox}";
    wb.SaveAs("test.xlsx");
}
