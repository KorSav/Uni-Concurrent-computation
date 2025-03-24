package app;

import java.util.Collections;
import java.util.List;

public class Histogram {
    private final List<Integer> wordLengthCounts = Collections.emptyList();

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
        UpdateSize(histogram.wordLengthCounts.size() + 1);
        for (int i = 0; i < wordLengthCounts.size(); i++) {
            int c1 = wordLengthCounts.get(i);
            int c2 = histogram.wordLengthCounts.get(i);
            wordLengthCounts.set(i, c1 + c2);
        }
    }

    private void UpdateSize(int wordLength) {
        int edgeDist = wordLengthCounts.size() - wordLength;
        if (edgeDist < 0) {
            wordLengthCounts.addAll(Collections.nCopies(-edgeDist, 0));
        }
    }
}
