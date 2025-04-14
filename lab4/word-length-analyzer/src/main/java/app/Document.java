package app;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileReader;
import java.io.IOException;
import java.util.LinkedList;
import java.util.List;

public class Document {
    private final List<String> lines;
    private final String name;

    Document(List<String> lines, String name) {
        this.lines = lines;
        this.name = name;
    }

    public String getName() {
        return name;
    }

    public List<String> getLines() {
        return this.lines;
    }

    public void trimLines() {
        for (int i = 0; i < lines.size(); i++) {
            String line = lines.get(i);
            lines.set(i, line.trim());
        }
    }

    public static Document fromFile(File file) throws IOException {
        List<String> lines = new LinkedList<>();
        try (BufferedReader reader = new BufferedReader(new FileReader(file))) {
            String line = reader.readLine();
            while (line != null) {
                lines.add(line);
                line = reader.readLine();
            }
        }
        return new Document(lines, file.getName());
    }
}
