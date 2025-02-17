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
    public int Value { get; private set; } = 0;

    public void Increment()
    {
        Value++;
    }

    public void Decrement()
    {
        Value--;
    }
}