package app;

public class ExperimentResult {
    public long timeForkJoinNanos;
    public long timeThreadsNanos;
    public long timeSeqNanos;
    public double diffForkJoin;
    public double diffThreads;

    public Double GetSpeedupForkJoin() {
        return timeSeqNanos / (double) timeForkJoinNanos;
    }

    public Double GetSpeedupThreads() {
        return timeSeqNanos / (double) timeThreadsNanos;
    }

    public Double GetTimeSeqSecs() {
        return timeSeqNanos / 1e9;
    }

    public Double GetTimeThreadsSecs() {
        return timeThreadsNanos / 1e9;
    }

    public Double GetTimeForkJoinSecs() {
        return timeForkJoinNanos / 1e9;
    }
}
