using SymbolsSync;

const int charsPerRowCount = 100;
const int rowsCount = 90;

char[] chars = ['\\', '/', '|'];
Thread[] ths = new Thread[chars.Length];
for (int i = 0; i < chars.Length; i++){
    CharPrinter cp = new(chars[i], charsPerRowCount, rowsCount);
    ths[i] = new(cp.Print);
    ths[i].Start();
}

foreach (var th in ths) th.Join();
