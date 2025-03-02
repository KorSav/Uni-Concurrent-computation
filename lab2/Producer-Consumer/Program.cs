Drop drop = new();
new Thread(new Producer(drop).Run).Start();
new Thread(new Consumer(drop).Run).Start();

public class Drop
{
    private string _message = string.Empty;
    private readonly object _messageIsPut = new();
    private bool _empty = true;

    public string Take()
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

    public void Put(string message)
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
        string[] importantInfo = [
            "Mares eat oats",
            "Does eat oats",
            "Little lambs eat ivy",
            "A kid will eat ivy too"
        ];
        Random random = new();
        for (int i = 0; i < importantInfo.Length; i++) {
            _drop.Put(importantInfo[i]);
            try {
                Thread.Sleep(random.Next(5000));
            }
            catch (ThreadInterruptedException) { }
        }
        _drop.Put("DONE");
    }
}

public class Consumer(Drop drop)
{
    private readonly Drop _drop = drop;

    public void Run()
    {
        Random random = new();
        for (string message = _drop.Take();
            !message.Equals("DONE");
            message = _drop.Take()) {
            System.Console.WriteLine($"MESSAGE RECEIVED: {message}");
            try {
                Thread.Sleep(random.Next(5000));
            }
            catch (ThreadInterruptedException) { }
        }
    }
}