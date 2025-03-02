using static Constants;

Drop drop = new();
new Thread(new Producer(drop).Run).Start();
new Thread(new Consumer(drop).Run).Start();

public static class Constants
{
    public const int NumbersCount = 1000;
}

public class Drop
{
    private int _message;
    private readonly object _messageIsPut = new();
    private bool _empty = true;

    public int Take()
    {
        lock (_messageIsPut) {
            while (_empty) {
                try {
                    Monitor.Wait(_messageIsPut);
                }
                catch (ThreadInterruptedException) { }
            }
            _empty = true;
            Monitor.PulseAll(_messageIsPut);
            return _message;
        }
    }

    public void Put(int message)
    {
        lock (_messageIsPut) {
            while (!_empty) {
                try {
                    Monitor.Wait(_messageIsPut);
                }
                catch (ThreadInterruptedException) { }
            }
            _empty = false;
            _message = message;
            Monitor.PulseAll(_messageIsPut);
        }
    }
}

public class Producer(Drop drop)
{
    private readonly Drop _drop = drop;

    public void Run()
    {
        Random random = new();
        for (int i = 0; i < NumbersCount; i++) {
            _drop.Put(i);
            try {
                Thread.Sleep(random.Next(15));
            }
            catch (ThreadInterruptedException) { }
        }
        _drop.Put(-1);
    }
}

public class Consumer(Drop drop)
{
    private readonly Drop _drop = drop;

    public void Run()
    {
        Random random = new();
        for (int message = _drop.Take();
            message != -1;
            message = _drop.Take()) {
            System.Console.WriteLine($"MESSAGE RECEIVED: {message}");
            try {
                Thread.Sleep(random.Next(15));
            }
            catch (ThreadInterruptedException) { }
        }
    }
}