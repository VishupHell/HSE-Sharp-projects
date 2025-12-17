using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FileStoringService.Models;

namespace FileStoringService.Controllers;


[ApiController][Route("[controller]")]
public class HomeController : ControllerBase
{
    private readonly FileStoringServiceDBContext _context;
    private readonly IWebHostEnvironment _environment;

    public HomeController(FileStoringServiceDBContext context, IWebHostEnvironment env)
    {
        _context = context;
        _environment = env;
    }

    [HttpPost]
    public IActionResult InputFile([FromForm]string studentName, [FromForm]string exercise, IFormFile file)
    {
        var curDir = Path.Combine(Path.Combine(_environment.ContentRootPath, "files"), exercise);
        Directory.CreateDirectory(curDir);
        
        var filePath = Path.Combine(curDir, file.FileName);
        var stream = new FileStream(filePath, FileMode.Create);
        file.CopyTo(stream);
        var homework = new Homework(studentName, filePath, file.FileName, DateTime.UtcNow, exercise, file.ContentType);
        _context.Files.Add(homework);
        _context.SaveChanges();
        return Ok(new
        {
            fileId = homework.Id,
            newFilePath = filePath
        });
    }
    
    [HttpGet]
    public IActionResult OutputFile(Guid id)
    {
        Homework result = _context.Files.Find(id);
        var path = result.FilePath;
        byte[] file = System.IO.File.ReadAllBytes(path);
        string type = result.FileType;
        string name = result.FileName;
        System.IO.File.WriteAllBytes(path, file);
        return File(file, type, name);
    }

    [HttpGet("exercise/{exercise}")]
    public IActionResult GetExersiceFiles(string exercise)
    {
        var result = _context.Files.Where(x => x.Exercise == exercise).Select(x => new
        {
            fileId = x.Id,
            filePath = x.FilePath,
        }).ToArray();
        return Ok(result);
    }
}