namespace SymbolsSync;

class Sync(int charsCount, int rowsCount, int charsPerRowCount){
    public bool IsPrinted; 
    public bool IsStop => PrintedCount == charsPerRowCount * rowsCount; 

    public int PrintedCount;

    public int ICharToPrint => PrintedCount % charsCount;

    public bool IsNewline => PrintedCount % charsPerRowCount == 0;
}