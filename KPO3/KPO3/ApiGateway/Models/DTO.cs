using System.Text.Json.Serialization;

namespace ApiGateway.Models;

public class DTO
{
    [JsonPropertyName("reportId")]
    public Guid Id { get; set; }
    [JsonPropertyName("exercise")]
    public string Exercise { get; set; }
}