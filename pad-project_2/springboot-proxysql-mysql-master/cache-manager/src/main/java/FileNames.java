public enum FileNames {

    CACHE_FILE("cachedSites.txt"),
    BLOCKED_FILE("blockedSites.txt");

    public final String fileName;

    FileNames(String fileName) {
        this.fileName = fileName;
    }

    public String getFileName() {
        return fileName;
    }
}
