using MatrixMul;
using ClosedXML.Excel;

int[] threadCounts = [4, 9, 25];
int[] matrixSizes = [500, 1000, 1500, 2000, 2500, 3000];
Result StripeRes = null!;
Result FoxRes = null!;
Result SeqRes = null!;

Dictionary<string, double[][]> methodEtSizesPerTh = new() {
    {nameof(StripeRes), new double[threadCounts.Length][]},
    {nameof(FoxRes), new double[threadCounts.Length][]},
    {nameof(SeqRes), new double[1][]},
};
foreach (string methodName in methodEtSizesPerTh.Keys)
    for (int i = 0; i < threadCounts.Length; i++) {
        if (methodName == nameof(SeqRes)) {
            methodEtSizesPerTh[methodName][0] = new double[matrixSizes.Length];
            break;
        }
        methodEtSizesPerTh[methodName][i] = new double[matrixSizes.Length];
    }

// warm up
string spaces = new(' ', 100);
System.Console.WriteLine("Warming up...");
for (int ims = 0; ims < Math.Min(2, matrixSizes.Length); ims++) {
    var ms = matrixSizes[ims];
    var (A, B) = GenerateRandomMatrices(ms);
    for (int itc = 0; itc < Math.Min(2, threadCounts.Length); itc++) {
        var tc = threadCounts[itc];

        if (itc == 0) {
            System.Console.Write($"\r{spaces}\r");
            System.Console.Write($"Current algorithm: Sequential");
            SeqRes = SequentialMul.Multiply(A, B);
            methodEtSizesPerTh[nameof(SeqRes)][0][ims] = SeqRes.CalcTime.TotalSeconds;
        }

        System.Console.Write($"\r{spaces}\r");
        System.Console.Write($"Current algorithm: Stripe");
        StripeRes = StripeMul.Multiply(A, B, tc);
        methodEtSizesPerTh[nameof(StripeRes)][itc][ims] = StripeRes.CalcTime.TotalSeconds;

        System.Console.Write($"\r{spaces}\r");
        System.Console.Write($"Current algorithm: Fox");
        FoxRes = FoxMul.Multiply(A, B, tc);
        methodEtSizesPerTh[nameof(FoxRes)][itc][ims] = FoxRes.CalcTime.TotalSeconds;
        System.Console.Write($"\r{spaces}\r");
    }
}

// testing
System.Console.WriteLine("Testing...");
for (int ims = 0; ims < matrixSizes.Length; ims++) {
    var ms = matrixSizes[ims];
    var (A, B) = GenerateRandomMatrices(ms);
    for (int itc = 0; itc < threadCounts.Length; itc++) {
        var tc = threadCounts[itc];
        System.Console.WriteLine($"Test {ims + 1}.{itc + 1}: {tc} ths for {ms}-sized M");
        if (itc == 0) {
            System.Console.Write($"\r{spaces}\r");
            System.Console.Write($"Current algorithm: Sequential");
            SeqRes = SequentialMul.Multiply(A, B);
            methodEtSizesPerTh[nameof(SeqRes)][0][ims] = SeqRes.CalcTime.TotalSeconds;
        }

        System.Console.Write($"\r{spaces}\r");
        System.Console.Write($"Current algorithm: Stripe");
        StripeRes = StripeMul.Multiply(A, B, tc);
        methodEtSizesPerTh[nameof(StripeRes)][itc][ims] = StripeRes.CalcTime.TotalSeconds;

        System.Console.Write($"\r{spaces}\r");
        System.Console.Write($"Current algorithm: Fox");
        FoxRes = FoxMul.Multiply(A, B, tc);
        methodEtSizesPerTh[nameof(FoxRes)][itc][ims] = FoxRes.CalcTime.TotalSeconds;

        System.Console.Write($"\r{spaces}\r");
    }
}

System.Console.WriteLine("Calculate diffs...");
var diff_stripe = MatrixComparator.CalcMaxAbsDiff(SeqRes.M, StripeRes.M);
var diff_fox = MatrixComparator.CalcMaxAbsDiff(SeqRes.M, FoxRes.M);

System.Console.WriteLine("Saving to file...");
using (var wb = new XLWorkbook()) {
    var ws = wb.Worksheets.Add("Matrices");
    ws.Cell("A1").Value = "Size";
    ws.Range("A1:A3").Merge();
    ws.Cell("B1").Value = "Sequential\nTime, s";
    ws.Range("B1:B3").Merge();
    ws.Cell("B4").InsertData(methodEtSizesPerTh[nameof(SeqRes)][0]);
    for (int itc = 0; itc < threadCounts.Length; itc++) {
        var colFrom = 3 + itc * 4;
        var colTo = colFrom + 4;
        AddThreadGroup(ws, colFrom, colTo, threadCounts[itc]);
        ws.Cell(4, colFrom).InsertData(methodEtSizesPerTh[nameof(StripeRes)][itc]);
        ws.Range(4, colFrom + 1, 4 + matrixSizes.Length - 1, colFrom + 1)
            .FormulaR1C1 = $"RC[-{colFrom - 1}]/RC[-1]";
        ws.Cell(4, colFrom + 2).InsertData(methodEtSizesPerTh[nameof(FoxRes)][itc]);
        ws.Range(4, colFrom + 3, 4 + matrixSizes.Length - 1, colFrom + 3)
            .FormulaR1C1 = $"RC[-{colFrom + 2 - 1}]/RC[-1]";
    }
    ws.Cell("A4").InsertData(matrixSizes);
    ws.Cell($"A{5 + matrixSizes.Length}").Value = $"Diff stripe: {diff_stripe}";
    ws.Cell($"A{6 + matrixSizes.Length}").Value = $"Diff fox: {diff_fox}";
    wb.SaveAs("Result.xlsx");
}

static (double[,], double[,]) GenerateRandomMatrices(int matrixSize)
{
    double[,] matrixA = new double[matrixSize, matrixSize];
    double[,] matrixB = new double[matrixSize, matrixSize];

    Random random = new();
    for (int ir = 0; ir < matrixSize; ir++)
        for (int ic = 0; ic < matrixSize; ic++) {
            matrixA[ir, ic] = random.NextDouble();
            matrixB[ir, ic] = random.NextDouble();
        }

    return (matrixA, matrixB);
}

static void AddThreadGroup(IXLWorksheet ws, int colFrom, int colTo, int threadCount)
{
    colTo -= 1;
    ws.Cell(1, colFrom).Value = $"{threadCount} threads";
    ws.Range(1, colFrom, 1, colTo).Merge();
    var methodHeader = ws.Range(2, colFrom, 2, colTo);
    methodHeader.Cell(1, 1).Value = "Stripe";
    methodHeader.Range(1, 1, 1, 2).Merge();
    methodHeader.Cell(1, 3).Value = "Fox";
    methodHeader.Range(1, 3, 1, 4).Merge();
    var valueHeader = ws.Range(3, colFrom, 3, colTo);
    for (int i = 0; i < valueHeader.ColumnCount(); i++) {
        valueHeader.Cell(1, i + 1).Value = (i % 2 == 0) ? "Time, s" : "Speedup";
    }
}