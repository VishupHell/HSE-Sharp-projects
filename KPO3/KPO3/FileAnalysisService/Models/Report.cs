namespace FileAnalysisService.Models;

public class Report
{
    public Guid Id { get; init; }
    public string FilePath { get; init; }
    public string FileType { get; init; }
    public string FileName { get; init; }
    public string Exercise { get; set; }
    
    public string? FileCloudWords { get; init; }

    public Report(Guid id, string filePath, string fileType, string fileName, string fileCloudWords, string exercise)
    {
        Id = id;
        FilePath = filePath;
        FileType = fileType;
        FileName = fileName;
        FileCloudWords = fileCloudWords;
        Exercise = exercise;
    }
    
    
}