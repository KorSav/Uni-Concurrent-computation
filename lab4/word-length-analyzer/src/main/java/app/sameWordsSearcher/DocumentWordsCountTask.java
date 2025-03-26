package app.sameWordsSearcher;

import java.util.Arrays;
import java.util.HashSet;
import java.util.Set;
import java.util.concurrent.RecursiveTask;

import app.Document;

class DocumentUniqueWordsTask extends RecursiveTask<Set<String>> {
    private final Document document;

    DocumentUniqueWordsTask(Document document) {
        super();
        this.document = document;
    }

    @Override
    protected Set<String> compute() {
        Set<String> result = new HashSet<>();
        for (String line : document.getLines()) {
            result.addAll(Arrays.asList(line.trim().split("(\\s|\\p{Punct})+")));
        }
        return result;
    }
}
