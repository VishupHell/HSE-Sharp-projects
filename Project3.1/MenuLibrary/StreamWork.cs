namespace MenuLibrary;
using System.IO;
/// <summary>
/// Класс для перенаправления потоков
/// </summary>
public class StreamWork
{
    public StreamReader sr;
    public StreamWriter sw;
    /// <summary>
    /// Перенаправляем ввод
    /// </summary>
    /// <param name="path"> куда перенаправляем </param>
    /// <returns> получилось ли перенаправить</returns>
    public bool StreamInputStart(string path)
    {
        try
        {
            sr = new StreamReader(path);
            Console.SetIn(sr);
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            Menu.WriteMessage("Отказано в доступе", ConsoleColor.Red);
        }
        catch (ArgumentNullException)
        {
            Menu.WriteMessage("Вы не ввели строку", ConsoleColor.Red);
        }
        catch (ArgumentException)
        {
            Menu.WriteMessage("Вы не ввели строку", ConsoleColor.Red);
        }
        catch (FileNotFoundException)
        {
            Menu.WriteMessage("Файл не найден", ConsoleColor.Red);
        }
        catch (DirectoryNotFoundException)
        {
            Menu.WriteMessage("Укаэан недопустимый путь", ConsoleColor.Red);
        }
        catch (IOException)
        {
            Menu.WriteMessage("Недопустимый формат пути", ConsoleColor.Red);
        }
        return false;
    }
    /// <summary>
    /// возвращаем ввод обратно
    /// </summary>
    public void StreamInputEnd()
    {
        sr.Dispose();
        Console.SetIn(new StreamReader(Console.OpenStandardInput(), Console.InputEncoding));
    }
    /// <summary>
    /// Перенаправляем вывод
    /// </summary>
    /// <param name="path"> куда перенаправляем </param>
    /// <returns> получилось ли перенаправить </returns>
    public bool StreamOutputStart(string path)
    {
        try
        {
            sw = new StreamWriter(path + ".txt", false);
            Console.SetOut(sw);
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            Menu.WriteMessage("Отказано в доступе", ConsoleColor.Red);
        }
        catch (ArgumentException)
        {
            Menu.WriteMessage("Некорректная строка", ConsoleColor.Red);
        }
        catch (DirectoryNotFoundException)
        {
            Menu.WriteMessage("Укаэан недопустимый путь", ConsoleColor.Red);
        }
        catch (IOException)
        {
            Menu.WriteMessage("Недопустимый формат пути", ConsoleColor.Red);
        }
        return false;
    }
    /// <summary>
    /// Возвращаем вывод обратно
    /// </summary>
    public void StreamOutputEnd()
    {
        sw.Dispose();
        StreamWriter standart = new StreamWriter(Console.OpenStandardOutput(), Console.OutputEncoding);
        standart.AutoFlush = true;
        Console.SetOut(standart);
    }
}