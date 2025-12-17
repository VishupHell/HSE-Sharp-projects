using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ApiGateway.Models;


namespace ApiGateway.Controllers;

[ApiController][Route("[controller]")]
public class HomeController : ControllerBase
{
    private HttpClient _firstService;
    private HttpClient _secondService;

    public HomeController(IHttpClientFactory httpClientFactory)
    {
        _firstService = httpClientFactory.CreateClient("FirstService");
        _secondService = httpClientFactory.CreateClient("SecondService");
    }
    
    [HttpPost]
    public IActionResult InputFile([FromForm]string studentName, [FromForm]string exercise, IFormFile file)
    {
        var ms = new MemoryStream();
        file.CopyTo(ms);
        var bytesOfFile = ms.ToArray();

        var data = new MultipartFormDataContent();

        var stream = new StreamContent(new MemoryStream(bytesOfFile));
        stream.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
        data.Add(stream, "file", file.FileName);

        data.Add(new StringContent(studentName), "studentName");
        data.Add(new StringContent(exercise), "exercise");
        var ans = _firstService.PostAsync("/home", data).Result;
        string content = ans.Content.ReadAsStringAsync().Result;
        
        var doc = JsonDocument.Parse(content);
        var root = doc.RootElement;
        var id = root.GetProperty("fileId");
        
        var path = root.GetProperty("newFilePath");

        var myId = Guid.Parse(id.GetString()!);
        
        

        data = new MultipartFormDataContent();
        
        stream = new StreamContent(new MemoryStream(bytesOfFile));
        stream.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
        data.Add(stream, "file", file.FileName);
        
        data.Add(new StringContent(myId.ToString()), "Id");
        data.Add(new StringContent(exercise.ToString()), "exercise");
        data.Add(new StringContent(path.ToString()), "filePath");
        
        var report = _secondService.PostAsync("/home", data).Result;
        
        
        var reportContent = report.Content.ReadAsStringAsync().Result;
        
        
        
        var repDoc = JsonDocument.Parse(reportContent);
        
        var repRoot = repDoc.RootElement;
        
        var repId = repRoot.GetProperty("fileId");

        var myRepId = Guid.Parse(repId.GetString()!);
        return Ok(new
        {
            fileId = myId,
        });
    }
    
    [HttpGet("reports/{exercise}")]
    public IActionResult GetExersiceFiles(string exercise)
    {
        var ans = _secondService.GetAsync($"/home/report_exercise/{exercise}").Result;
        string content = ans.Content.ReadAsStringAsync().Result;
        var dtos = JsonSerializer.Deserialize<List<DTO>>(content).Where(x => x.Exercise == exercise);
        
        return Ok(dtos);
    }
}