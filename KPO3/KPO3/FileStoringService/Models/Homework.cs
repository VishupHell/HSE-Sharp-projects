namespace FileStoringService.Models;

public class Homework
{
    public Guid Id { get; init; }
    public string StudentName { get; init; }
    public string Exercise { get; init; }
    public string FilePath { get; init; }
    public string FileName { get; init; }
    public DateTime Time { get; init; }
    public string FileType { get; init; }

    public Homework(string studentName, string filePath, string fileName, DateTime time, string exercise, string fileType)
    {
        StudentName = studentName;
        FilePath = filePath;
        FileName = fileName;
        Exercise = exercise;
        Time = time;
        Id = Guid.NewGuid();
        FileType = fileType;
    }
}