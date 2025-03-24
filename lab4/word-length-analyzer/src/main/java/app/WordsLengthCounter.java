package app;

public class WordsLengthCounter {

    String[] wordsIn(String line) {
        return line.trim().split("(\\s|\\p{Punct})+");
    }

    Histogram getHistogram(Document document) {
        Histogram histogram = new Histogram();
        for (String line : document.getLines()) {
            for (String word : wordsIn(line)) {
                histogram.Increment(word.length());
            }
        }
        return histogram;
    }
}
