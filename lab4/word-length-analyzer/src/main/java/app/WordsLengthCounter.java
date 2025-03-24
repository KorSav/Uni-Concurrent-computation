package app;

import java.util.concurrent.ForkJoinPool;

public class WordsLengthCounter {

    String[] wordsIn(String line) {
        return line.trim().split("(\\s|\\p{Punct})+");
    }

    public Histogram getHistogram(Document document) {
        Histogram histogram = new Histogram();
        for (String line : document.getLines()) {
            for (String word : wordsIn(line)) {
                if (!word.isBlank())
                    histogram.Increment(word.length());
            }
        }
        return histogram;
    }

    public Histogram getWordsLengthHistogramParallel(Folder folder) {
        return ForkJoinPool.commonPool().invoke(new FolderWordsCountTask(folder));
    }

    public Histogram getWordsLengthHistogramSequential(Folder folder) {
        Histogram histogram = new Histogram();
        WordsLengthCounter wlc = new WordsLengthCounter();
        for (Document document : folder.getDocuments()) {
            histogram.Add(wlc.getHistogram(document));
        }
        for (Folder subFolder : folder.getSubFolders()) {
            histogram.Add(getWordsLengthHistogramSequential(subFolder));
        }
        return histogram;
    }
}
