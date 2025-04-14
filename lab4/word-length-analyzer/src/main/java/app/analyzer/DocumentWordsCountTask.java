package app.analyzer;

import java.util.concurrent.RecursiveTask;

import app.Document;

class DocumentWordsCountTask extends RecursiveTask<Histogram> {
    private final Document document;

    DocumentWordsCountTask(Document document) {
        super();
        this.document = document;
    }

    @Override
    protected Histogram compute() {
        WordsLengthCounter wlc = new WordsLengthCounter();
        return wlc.getHistogram(document);
    }
}
