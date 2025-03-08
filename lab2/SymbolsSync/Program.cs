using SymbolsSync;

const int charsPerRowCount = 90;
const int rowsCount = 30;

char[] chars = ['|', '\\', '/'];
Thread[] ths = new Thread[chars.Length];

System.Console.WriteLine("Version without sync:");
for (int i = 0; i < chars.Length; i++){
    CharPrinter cp = new(chars[i], charsPerRowCount, rowsCount/chars.Length);
    ths[i] = new(cp.Print);
    ths[i].Start();
}

foreach (var th in ths) th.Join();

System.Console.WriteLine("Version with sync:");
Sync sync = new(chars.Length, rowsCount, charsPerRowCount);
for (int i = 0; i < chars.Length; i++){
    CharPrinterSync cp = new(chars[i], sync, i);
    ths[i] = new(cp.Print);
    ths[i].Start();
}

foreach (var th in ths) th.Join();
