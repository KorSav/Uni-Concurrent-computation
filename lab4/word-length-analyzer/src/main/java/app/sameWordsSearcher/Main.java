package app.sameWordsSearcher;

import java.io.File;
import java.io.IOException;
import java.util.Set;

import app.Folder;

public class Main {
    public static void main(String[] args) throws IOException {
        if (args.length != 1) {
            System.out.println("Expected folder path as an only argument.");
            return;
        }
        Folder folder = Folder.fromDirectory(new File(args[0]));
        SameWordsSearcher sws = new SameWordsSearcher();
        long t0 = System.currentTimeMillis();
        Set<String> sameWords = sws.searchSameWords(folder);
        long tSpent = System.currentTimeMillis() - t0;
        System.out.println("Same words among all documents:");
        for (String word : sameWords) {
            System.out.print("%s ".formatted(word));
        }
        System.out.println();
        System.out.println("Time spent: %.3f s".formatted(tSpent / 1000.));
    }
}