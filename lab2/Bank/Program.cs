const int NACCOUNTS = 10;
const int INITIAL_BALANCE = 10_000;

var b = new Bank(NACCOUNTS, INITIAL_BALANCE);
for (int i = 0; i < NACCOUNTS; i++) {
    var transferer = new Transferer(b, i, INITIAL_BALANCE);
    var thread = new Thread(() => transferer.Transfer(b.TransferWithLock)) {
        Priority = ThreadPriority.Normal + i % 2
    };
    thread.Start();
}

class Bank
{
    public static readonly int NTEST = 10_000;
    private readonly int[] _accounts;
    private long _ntransacts;

    public delegate void Transfer(int from, int to, int amount);

    private readonly Lock _lockTransact = new();

    public Bank(int n, int initialBalance)
    {
        _accounts = new int[n];
        for (int i = 0; i < n; i++) {
            _accounts[i] = initialBalance;
        }
        _ntransacts = 0;
    }

    public void TransferWithLock(int from, int to, int amount)
    {
        lock (_lockTransact) {
            _accounts[from] -= amount;
            _accounts[to] += amount;
            _ntransacts++;
        }
        if (_ntransacts % NTEST == 0) {
            TestWithLock();
        }
    }

    public void TestWithLock()
    {
        int sum = 0;
        lock (_lockTransact) {
            for (int i = 0; i < _accounts.Length; i++) {
                sum += _accounts[i];
            }
        }
        System.Console.WriteLine($"Transactions: {_ntransacts} Sum: {sum}");
    }

    public int Size()
    {
        return _accounts.Length;
    }
}

class Transferer(Bank b, int from, int max)
{
    private readonly Bank _bank = b;
    private readonly int _fromAccount = from;
    private readonly int _maxAmount = max;
    private static readonly int REPS = 1000;

    public void Transfer(Bank.Transfer bankTransfer)
    {
        while (true) {
            for (int i = 0; i < REPS; i++) {
                Random rand = new();
                int toAccount = (int)(_bank.Size() * rand.NextSingle());
                int amount = (int)(_maxAmount * rand.NextSingle() / REPS);
                bankTransfer(_fromAccount, toAccount, amount);
            }
        }
    }
}