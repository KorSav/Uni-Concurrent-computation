package app;

import java.io.File;
import java.io.IOException;
import java.util.LinkedList;
import java.util.List;

public class Folder {
    private final List<Folder> subFolders;
    private final List<Document> documents;
    private final String name;

    Folder(List<Folder> subFolders, List<Document> documents, String name) {
        this.subFolders = subFolders;
        this.documents = documents;
        this.name = name;
    }

    public List<Folder> getSubFolders() {
        return this.subFolders;
    }

    public List<Document> getDocuments() {
        return this.documents;
    }

    public String getName() {
        return name;
    }

    public static Folder fromDirectory(File dir) throws IOException {
        List<Document> documents = new LinkedList<>();
        List<Folder> subFolders = new LinkedList<>();
        for (File entry : dir.listFiles()) {
            if (entry.isDirectory()) {
                subFolders.add(Folder.fromDirectory(entry));
            } else {
                documents.add(Document.fromFile(entry));
            }
        }
        return new Folder(subFolders, documents, dir.getName());
    }
}
