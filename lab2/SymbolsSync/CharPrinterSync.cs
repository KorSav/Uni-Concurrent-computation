namespace SymbolsSync;

class CharPrinterSync(char chr, Sync sync, int iOrder)
{
    public void Print()
    {
        while (!sync.IsStop) {
            lock (sync) {
                while (sync.ICharToPrint != iOrder) {
                    Monitor.Wait(sync);
                    if (sync.IsStop) return;
                }

                System.Console.Write(chr);

                sync.PrintedCount++;
                if (sync.IsNewline)
                    System.Console.WriteLine();
                Monitor.PulseAll(sync);
            }
        }
    }
}