package app.documentsByKeywordsSearcher;

import java.util.HashSet;
import java.util.Set;

public class KeywordsMatchResult {
    private Set<String> keywordsMatch;
    private String documentName;

    public String getDocumentName() {
        return documentName;
    }

    public boolean isEmpty() {
        return keywordsMatch.isEmpty();
    }

    public void prependName(String folderName) {
        documentName = folderName + documentName;
    }

    public KeywordsMatchResult(String documentName) {
        this.documentName = documentName;
        this.keywordsMatch = new HashSet<>();
    }

    public void addMatched(String keyword) {
        keywordsMatch.add(keyword);
    }

    public int getMatchedCount() {
        return keywordsMatch.size();
    }

    public String getMatched(String sep) {
        return String.join(sep, keywordsMatch);
    }
}
