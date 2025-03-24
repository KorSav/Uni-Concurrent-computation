package app;

import java.util.LinkedList;
import java.util.List;
import java.util.concurrent.RecursiveTask;

class FolderWordsCountTask extends RecursiveTask<Histogram> {
    private final Folder folder;

    public FolderWordsCountTask(Folder folder) {
        super();
        this.folder = folder;
    }

    @Override
    protected Histogram compute() {
        Histogram histogram = new Histogram();
        List<RecursiveTask<Histogram>> forks = new LinkedList<>();
        for (Folder subFolder : folder.getSubFolders()) {
            FolderWordsCountTask task = new FolderWordsCountTask(subFolder);
            forks.add(task);
            task.fork();
        }
        for (Document document : folder.getDocuments()) {
            DocumentWordsCountTask task = new DocumentWordsCountTask(document);
            forks.add(task);
            task.fork();
        }
        for (RecursiveTask<Histogram> task : forks) {
            histogram.Add(task.join());
        }
        return histogram;
    }
}
