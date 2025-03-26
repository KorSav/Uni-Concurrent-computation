package app.documentsByKeywordsSearcher;

import java.io.File;
import java.io.IOException;
import java.util.List;
import java.util.concurrent.ForkJoinPool;

import app.Document;
import app.Folder;

public class Main {
    public static void main(String[] args) throws IOException {
        if (args.length != 2) {
            System.out.println("Expected 2 args:");
            System.out.println("1 - folder with docs to search");
            System.out.println("2 - document with keywords each in separate line");
            return;
        }
        Folder folder = Folder.fromDirectory(new File(args[0]));
        Document keywords = Document.fromFile(new File(args[1]));
        keywords.trimLines();
        long t0 = System.currentTimeMillis();
        List<KeywordsMatchResult> mrs = ForkJoinPool.commonPool()
                .invoke(new FolderDocsMatchKeywordsTask(folder, keywords));
        long tSpent = System.currentTimeMillis() - t0;
        System.out.println("Documents matching keywords (%d/%d):"
                .formatted(mrs.size(), folder.getAllDocsCount()));
        mrs.sort((mr1, mr2) -> mr2.getMatchedCount() - mr1.getMatchedCount());
        for (KeywordsMatchResult mr : mrs) {
            System.out.print("%s (%d/%d): ".formatted(
                    mr.getDocumentName(),
                    mr.getMatchedCount(),
                    keywords.getLines().size()));
            System.out.println(mr.getMatched(", "));
        }
        System.out.println("Time spent: %.3f s".formatted(tSpent / 1000.));
    }
}