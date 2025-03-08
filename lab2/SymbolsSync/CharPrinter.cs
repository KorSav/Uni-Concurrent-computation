namespace SymbolsSync;

class CharPrinter(char chr, int countPerRow, int countRows)
{
    public void Print()
    {
        for (int ir = 0; ir < countRows; ir++) {
            for (int ipr = 0; ipr < countPerRow; ipr++) {
                System.Console.Write(chr);
            }
            System.Console.WriteLine();
        }
    }
}