package app;

public class ExperimentResult {
    public long timeSeq;
    public long timePar;
    public Histogram histSeq;
    public Histogram histPar;
    public int procCount;

    public Double GetSpeedup() {
        return timeSeq / (double) timePar;
    }

    public Double GetEfficiency() {
        return GetSpeedup() / procCount;
    }

    public Long GetCost() {
        return timePar * procCount;
    }
}
