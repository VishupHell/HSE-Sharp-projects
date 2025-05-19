using TxtLibrary;
using Spectre.Console;
namespace MenuLibrary;
/// <summary>
/// Класс, представляющий все возможные способы получения данных
/// </summary>
public class MyTextReader
{
    // способы получения
    private readonly string[] Choices = ["1 - через консоль", "2 - через файл", "3 - загрузить из IMDB", "4 - отменить ввод"];
    /// <summary>
    /// Метод, организуюший все возможные способы ввода данных
    /// </summary>
    /// <returns> прочитанный лист фильмов </returns>
    public List<Film> ReadText()
    {
        List<Film> films;
        var action = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select input type")
                .AddChoices(Choices));
        if (action == Choices[0]) // ввод через консоль
        {
            Console.WriteLine("Вы выбрали ввод через консоль, пожалуйста, введите файл");
            Console.WriteLine("Формат записи:");
            Console.WriteLine("[Name][Genres (', ')][Year][Rating]");
            films = TxtParser.ReadTxt(); // запуск чтения
            if (films == null)
            {
                Menu.WriteMessage("Файл не соответствует требуемой структуре", ConsoleColor.Red);
            }
            return films;
        }
        if (action == Choices[3]) return null; // выход из чтения
        if (action == Choices[2]) // ввод непосредственно сразу из базы по названию
        {
            ApiWork apiWork = new ApiWork();
            
            Console.WriteLine("Введите название фильма");
            Film film = new Film();
            film[0] = Console.ReadLine();
            Console.WriteLine("Введите год выпуска фильма (для более точного поиска)");
            try
            {
                film[2] = Console.ReadLine(); // вводим год и проверяем на корректность
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e);
                Menu.WriteMessage("Отменяю операцию", ConsoleColor.Red);
                return null;
            }
            // Создаем фильм из ответа
            UpdatedFilm uFilm = apiWork.CreateFilm(film);
            if (uFilm == null)
            {
                Menu.WriteMessage("Такого фильма в базе нет", ConsoleColor.Red);
                return null;
            }
            return new List<Film>() { uFilm };
        }
        // ввод из файла
        Console.WriteLine("Формат записи:");
        Console.WriteLine("[Name][Genres (', ')][Year][Rating]");
        StreamWork streamWork = new StreamWork(); // все про потоки
        Console.WriteLine("Введите путь до файла, в котором хранятся данные");
        string path = Console.ReadLine();
        if (streamWork.StreamInputStart(path)) // перенаправляем поток и если с потоками все хорошо
        {
            films = TxtParser.ReadTxt(); // читаем
            streamWork.StreamInputEnd(); // перенаправляем на стандартный
            if (films == null)
            {
                Menu.WriteMessage("Файл не соответствует требуемой структуре", ConsoleColor.Red);
            }
            return films;
        }
        return null;
    }
    
}