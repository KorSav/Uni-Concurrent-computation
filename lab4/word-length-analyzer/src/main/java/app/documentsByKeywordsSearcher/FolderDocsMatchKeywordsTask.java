package app.documentsByKeywordsSearcher;

import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.RecursiveTask;

import app.Document;
import app.Folder;

public class FolderDocsMatchKeywordsTask extends RecursiveTask<List<KeywordsMatchResult>> {
    private final Folder folder;
    private final Document keywordsDoc;

    public FolderDocsMatchKeywordsTask(Folder folder, Document keywordsDoc) {
        super();
        this.folder = folder;
        this.keywordsDoc = keywordsDoc;
    }

    @Override
    protected List<KeywordsMatchResult> compute() {
        List<DocumentMatchKeywordsTask> docsTasks = new ArrayList<>();
        List<FolderDocsMatchKeywordsTask> subFoldsTasks = new ArrayList<>();
        List<KeywordsMatchResult> result = new ArrayList<>();
        for (Document document : folder.getDocuments()) {
            docsTasks.add(new DocumentMatchKeywordsTask(document, keywordsDoc));
            docsTasks.getLast().fork();
        }
        for (Folder subFolder : folder.getSubFolders()) {
            subFoldsTasks.add(new FolderDocsMatchKeywordsTask(subFolder, keywordsDoc));
            subFoldsTasks.getLast().fork();
        }
        for (DocumentMatchKeywordsTask task : docsTasks) {
            KeywordsMatchResult mr = task.join();
            if (mr.isEmpty())
                continue;
            mr.prependName(folder.getName() + '/');
            result.add(mr);
        }
        for (FolderDocsMatchKeywordsTask task : subFoldsTasks) {
            for (KeywordsMatchResult mr : task.join()) {
                if (mr.isEmpty())
                    continue;
                mr.prependName(folder.getName() + '/');
                result.add(mr);
            }
        }
        return result;
    }

}
