package app;

import java.io.File;
import java.io.IOException;

public class Main {
    public static void main(String[] args) throws IOException {
        WordsLengthCounter wlc = new WordsLengthCounter();
        Folder folder = Folder.fromDirectory(new File(args[0]));
        long t1 = System.currentTimeMillis();
        Histogram hist1 = wlc.getWordsLengthHistogramSequential(folder);
        long t2 = System.currentTimeMillis();
        Histogram hist2 = wlc.getWordsLengthHistogramParallel(folder);
        long r2 = System.currentTimeMillis() - t2;
        long r1 = t2 - t1;
        for (int wl = 1; wl < hist1.getCount() + 1; wl++) {
            System.out.println("%-2d - %7d  %-7d".formatted(wl, hist1.Get(wl), hist2.Get(wl)));
        }
        System.out.println("Seq: %d ms\nParallel: %d ms".formatted(r1, r2));
    }
}