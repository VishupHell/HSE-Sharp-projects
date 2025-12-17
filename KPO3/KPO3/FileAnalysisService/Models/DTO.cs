using System.Text.Json.Serialization;

namespace FileAnalysisService.Models;


public class DTO
{
    [JsonPropertyName("fileId")]
    public Guid Id { get; set; }
    [JsonPropertyName("filePath")]
    public string FilePath { get; set; }
}