package app.sameWordsSearcher;

import java.util.ArrayList;
import java.util.HashSet;
import java.util.List;
import java.util.Set;
import java.util.concurrent.RecursiveTask;

import app.Document;
import app.Folder;

public class FolderSameWordsTask extends RecursiveTask<Set<String>> {
    private final Folder folder;

    public FolderSameWordsTask(Folder folder) {
        super();
        this.folder = folder;
    }

    @Override
    protected Set<String> compute() {
        List<RecursiveTask<Set<String>>> tasks = new ArrayList<>();
        Set<String> result = new HashSet<>();
        for (Document document : folder.getDocuments()) {
            tasks.add(new DocumentUniqueWordsTask(document));
            tasks.getLast().fork();
        }
        for (Folder subFolder : folder.getSubFolders()) {
            tasks.add(new FolderSameWordsTask(subFolder));
            tasks.getLast().fork();
        }
        // all unique words
        for (RecursiveTask<Set<String>> task : tasks) {
            result.addAll(task.join());
        }
        // intersect all with each task unique words
        for (RecursiveTask<Set<String>> task : tasks) {
            result.retainAll(task.join());
        }
        return result;
    }

}
