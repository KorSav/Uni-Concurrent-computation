package app.analyzer;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

public class Histogram {
    private final List<Integer> wordLengthCounts = new ArrayList<>();

    public Integer getCount() {
        return wordLengthCounts.size();
    }

    public void Increment(int wordLength) {
        UpdateSize(wordLength);
        int wordIndex = wordLength - 1;
        int oldValue = wordLengthCounts.get(wordIndex);
        wordLengthCounts.set(wordIndex, oldValue + 1);
    }

    public void Add(Histogram histogram) {
        UpdateSize(histogram.wordLengthCounts.size());
        for (int i = 0; i < wordLengthCounts.size(); i++) {
            int c1 = wordLengthCounts.get(i);
            int c2 = 0;
            if (i < histogram.wordLengthCounts.size())
                c2 = histogram.wordLengthCounts.get(i);
            wordLengthCounts.set(i, c1 + c2);
        }
    }

    public Double CalculateMean() {
        double total = 0;
        int amount = 0;
        for (int i = 0; i < wordLengthCounts.size(); i++) {
            amount += wordLengthCounts.get(i);
            total += wordLengthCounts.get(i) * (i + 1);
        }
        return total / amount;
    }

    public Double CalculateVariance() {
        double sum = 0;
        int amount = 0;
        double mean = CalculateMean();
        for (int i = 0; i < wordLengthCounts.size(); i++) {
            amount += wordLengthCounts.get(i);
            sum += Math.pow(i + 1 - mean, 2) * wordLengthCounts.get(i);
        }
        return sum / amount;
    }

    public Integer Get(int wordLength) {
        return wordLengthCounts.get(wordLength - 1);
    }

    private void UpdateSize(int wordLength) {
        int edgeDist = wordLengthCounts.size() - wordLength;
        if (edgeDist < 0) {
            wordLengthCounts.addAll(Collections.nCopies(-edgeDist, 0));
        }
    }
}
