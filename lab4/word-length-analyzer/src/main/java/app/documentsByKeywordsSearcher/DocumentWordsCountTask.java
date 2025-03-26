package app.documentsByKeywordsSearcher;

import java.util.concurrent.RecursiveTask;

import app.Document;

class DocumentMatchKeywordsTask extends RecursiveTask<KeywordsMatchResult> {
    private final Document document;
    private final Document keywordsDoc;

    DocumentMatchKeywordsTask(Document document, Document keywordsDoc) {
        super();
        this.document = document;
        this.keywordsDoc = keywordsDoc;
    }

    @Override
    protected KeywordsMatchResult compute() {
        KeywordsMatchResult mr = new KeywordsMatchResult(document.getName());
        for (String line : document.getLines()) {
            for (String keyword : keywordsDoc.getLines()) {
                if (line.contains(keyword))
                    mr.addMatched(keyword);
            }
        }
        return mr;
    }
}
