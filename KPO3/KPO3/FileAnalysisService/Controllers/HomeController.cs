using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using FileAnalysisService.Models;

namespace FileAnalysisService.Controllers;

[ApiController][Route("[controller]")]
public class HomeController : ControllerBase
{
    private readonly FileAnalysisServiceDBContext _context;
    private readonly IWebHostEnvironment _environment;
    
    private HttpClient _firstService;

    public HomeController(FileAnalysisServiceDBContext context, IWebHostEnvironment env, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _environment = env;
        _firstService = httpClientFactory.CreateClient("FirstService");
    }
    
    [HttpPost]
    public IActionResult InputFile([FromForm]Guid id, [FromForm]string exercise, [FromForm]string FilePath, IFormFile file)
    {
        var data = new MultipartFormDataContent();
        data.Add(new StringContent(exercise), "exercise");
        var ans = _firstService.GetAsync($"/home/exercise/{exercise}").Result;
        string content = ans.Content.ReadAsStringAsync().Result;
        var dtos = JsonSerializer.Deserialize<List<DTO>>(content);

        bool flag = false;
        bool mainFlag = false;
        
        var curResp = _firstService.GetAsync($"/home/{id}").Result;
        var curBytes = curResp.Content.ReadAsByteArrayAsync().Result;
        
        foreach (var dto in dtos)
        {
            flag = false;
            var newCurResp = _firstService.GetAsync($"/home/{dto.Id}").Result;
            var newCurBytes = newCurResp.Content.ReadAsByteArrayAsync().Result;
            if (newCurBytes.Length != curBytes.Length)
            {
                continue;
            }

            for (int i = 0; i < newCurBytes.Length; i++)
            {
                if (newCurBytes[i] != curBytes[i])
                {
                    flag = true;
                    break;
                }
            }

            if (!flag)
            {
                mainFlag = true;
                break;
            }
        }
        var curDir = Path.Combine(Path.Combine(_environment.ContentRootPath, "reports"), exercise);
        Directory.CreateDirectory(curDir);
        
        var filePath = Path.Combine(curDir, "report_" + file.FileName);
        var stream = new FileStream(filePath, FileMode.Create);
        
        byte[] buffer = Encoding.Default.GetBytes("Percent of origin: " + ((mainFlag) ? "0" : ">0"));
        stream.Write(buffer, 0, buffer.Length);
        
        var report = new Report(id, filePath, file.ContentType, file.FileName, null, exercise);
        _context.Files.Add(report);
        _context.SaveChanges();
        return Ok(new
        {
            fileId = report.Id
        });
    }
    [HttpGet]
    public IActionResult OutputFile(Guid id)
    {
        Report result = _context.Files.Find(id);
        var path = result.FilePath;
        byte[] file = System.IO.File.ReadAllBytes(path);
        string type = result.FileType;
        string name = result.FileName;
        System.IO.File.WriteAllBytes(path, file);
        return File(file, type, name);
    }
    
    [HttpGet("report_exercise/{exercise}")]
    public IActionResult GetExersiceFiles(string exercise)
    {
        var result = _context.Files.Where(x => x.Exercise == exercise).Select(x => new
        {
            reportId = x.Id,
            exercise = x.Exercise,
        }).ToArray();
        return Ok(result);
    }
}