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

var C_stripe = StripeMul.Multiply(matrixA, matrixB, 4);
var C_seq = SequentialMul.Multiply(matrixA, matrixB);
var diff = MatrixComparator.CalcMaxAbsDiff(C_seq, C_stripe);

using (var wb = new XLWorkbook()) {
    var ws = wb.Worksheets.Add("Matrices");
    for (int i = 0; i < C_stripe.Length; i++) {
        ws.Cell("A1").Value = "Stripe";
        ws.Cell($"A{i + 2}").Value = C_stripe[i / matrixSize, i % matrixSize];
        ws.Cell("B1").Value = "Sequential";
        ws.Cell($"B{i + 2}").Value = C_seq[i / matrixSize, i % matrixSize];
    };
    ws.Cell("C1").InsertData(new[] { "Diff:", diff.ToString() }, true);
    ws.Range(2, 3, 1 + matrixSize * matrixSize, 3).FormulaR1C1 = "ABS(RC[-2] - RC[-1])";
    wb.SaveAs("test.xlsx");
}
