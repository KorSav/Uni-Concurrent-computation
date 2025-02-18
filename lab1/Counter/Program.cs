var counter = new Counter();
var thread1 = new Thread(() => {
    for (int i = 0; i < 100_000; i++)
        counter.Increment();
});
var thread2 = new Thread(() => {
    for (int i = 0; i < 100_000; i++)
        counter.Decrement();
});

thread1.Start();
thread2.Start();

thread1.Join();
thread2.Join();

System.Console.WriteLine($"Counter value: {counter.Value}");

class Counter
{
    private readonly Lock _valueLock = new();
    public int Value { get; private set; } = 0;

    public void Increment()
    {
        lock (_valueLock) {
            Value++;
        }
    }

    public void Decrement()
    {
        lock (_valueLock) {
            Value--;
        }
    }
}