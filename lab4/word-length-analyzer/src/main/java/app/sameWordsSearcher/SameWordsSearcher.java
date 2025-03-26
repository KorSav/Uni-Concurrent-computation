package app.sameWordsSearcher;

import java.util.Set;
import java.util.concurrent.ForkJoinPool;

import app.Folder;

public class SameWordsSearcher {

    public Set<String> searchSameWords(Folder folder) {
        return ForkJoinPool.commonPool().invoke(new FolderSameWordsTask(folder));
    }
}
