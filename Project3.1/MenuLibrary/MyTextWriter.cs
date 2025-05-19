using TxtLibrary;
using Spectre.Console;
namespace MenuLibrary;
/// <summary>
/// Класс, предоставляющий все способы вывода данных
/// </summary>
public class MyTextWriter
{
    // возможные способы
    private readonly string[] Choices = ["1 - Вывод в виде списка", "2 - Вывод в виде таблицы", "3 - запись в файл", "4 - отменить операцию"];
    public List<Film> films;

    public MyTextWriter(List<Film> films) // конструктор
    {
        this.films = films;
    }
    /// <summary>
    /// Сделать выбор, какой вывод
    /// </summary>
    public void MakeChoise()
    {
        var action = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Как хотите получить данные?")
                .AddChoices(Choices));
        if (action == Choices[2])// запись в файл
        {
            WriteText();
        }
        else if (action == Choices[0]) //В консоль
        {
            Console.WriteLine("Каталог фильмов");
            string h = new string('-', Console.BufferWidth); // разделение фильмов
            Console.WriteLine(h);
            foreach (Film film in films)
            {
                Console.WriteLine(film); // вывод фильма, причем у расширенного фильма свой переопределенный toString()
                Console.WriteLine(h);
            }
        }
        else if (action == Choices[1])//в виде таблице
        {
            var action2 = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Какую таблицу вывести?")
                    .AddChoices(["1 - краткую", "2 - развернутую"]));
            if (action2 == "1 - краткую") // вывод на основе стандартных 4 полей
            {
                PrintTable(false);
            }
            else // вывод в виде расщиренного фильма
            {
                PrintTable(true);
            }
        }
    }
    /// <summary>
    /// вывод таблицы
    /// </summary>
    /// <param name="flag"> какой тип таблицы
    ///                     false - краткая
    ///                     true - расширенная</param>
    public void PrintTable(bool flag)
    {
        var table = new Table();
        // добавляем колонки
        table.AddColumn("Title");
        table.AddColumn("Genres");
        table.AddColumn("Year");
        if (flag)
        {
            // если расширенная версия, то колонок больше
            table.AddColumn("ARating");
            table.AddColumn("Director");
            table.AddColumn("Actors");
        }
        else
        {
            table.AddColumn("Rating");
        }
        foreach (Film film in films) // добавляем строчки, одна строчка - один фильм
        {
            if (film is UpdatedFilm && flag) // если фильм расширенный и надо выводить в расширенном формате
            {
                table.AddRow(film[0], film[1], film[2], film[3], film[4], film[5]);
            }
            else
            {
                if (flag) // если формат расширенный, а фильм нет, то выводим прочерки
                {
                    table.AddRow(film[0], film[1], film[2], film[3], "-", "-");
                }
                else
                {
                    // запись для всех нерасширенных фильмов
                    table.AddRow(film[0], film[1], film[2], film[3]);
                }
            }
        }
        AnsiConsole.Write(table);
    }
    /// <summary>
    /// Перенаправление потоков вывода, если необходимо и сам вывод
    /// </summary>
    /// <param name="value"> Нужно ли перенаправлять поток </param>
    public void WriteText()
    {
        StreamWork streamWork = new StreamWork();
        Console.WriteLine("Введите ссылку для расположения там файла для записи данных (без .txt)");
        string path = Console.ReadLine();
        Menu.WriteMessage("Строка получена", ConsoleColor.Green);
        if (streamWork.StreamOutputStart(path)) // пытаемся перенаправить поток
        {
            TxtParser.WriteTxt(films); // выводим поток
            streamWork.StreamOutputEnd(); // устанавливаем стандартный вывод
        }
    }
    
}