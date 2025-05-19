using System.Runtime.CompilerServices;
using TxtLibrary;
using Spectre.Console;
namespace MenuLibrary;
/// <summary>
/// Реализация сортировок и фильтраций
/// </summary>
public class CatalogEditing
{
    private List<Film> films; // Для чего будем применять сортировки и фильтрации
    private readonly string[] Choices = ["1 - сортировка", "2 - фильтрация"]; // выбор действий

    delegate void Editing(); // делегат, который нам и реализует 
    // разные возможности сортировки
    // Идея в том, что пользователь добавляет сортировки и фильтрации, я из складываю в делегат, а потом его запускаю
    // каждая сортировка и фильтрация меняет приватное поле сверху, поэтому каждый из них применяется к результату предыдущего

    private Editing editions; 
    public CatalogEditing(List<Film> films) // фильмы, которые будем изменять
    {
        this.films = new List<Film>(films);
    }
    /// <summary>
    /// Добавление сортировки
    /// </summary>
    public void AddSort()
    {
        // Выбор поля сортировки
        var action = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("По какому полю хотите провсти сортировку")
                .AddChoices(["Имя", "Год", "Рейтинг"]));
        // Выбор направления сортировки
        var action2 = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Выберите порядок сортировки")
                .AddChoices(["Прямой", "Обратный"]));
        // Много много ифов, каждый из которых создает анонимный метод и добавляет к делегату, сортировка происходит
        // с помощью LINQ
        if (action == "Имя")
        {
            if (action2 == "Прямой")
            {
                editions += delegate()
                {
                    films = (from f in films
                        orderby f.Name
                        select f).ToList();
                };
            }
            else
            {
                editions += delegate()
                {
                    films = (from f in films
                        orderby f.Name descending
                        select f).ToList();
                };
            }
        }
        else if (action == "Год")
        {
            if (action2 == "Прямой")
            {
                editions += delegate()
                {
                    films = (from f in films
                        orderby f.Year
                        select f).ToList();
                };
            }
            else
            {
                editions += delegate()
                {
                    films = (from f in films
                        orderby f.Year descending
                        select f).ToList();
                };
            }
        }
        else if (action == "Рейтинг")
        {
            if (action2 == "Прямой")
            {
                editions += delegate()
                {
                    films = (from f in films
                        orderby f.Rating
                        select f).ToList();
                };
            }
            else
            {
                editions += delegate()
                {
                    films = (from f in films
                        orderby f.Rating descending
                        select f).ToList();
                };
            }
        }
    }
    /// <summary>
    /// Добавление фильтрации
    /// </summary>
    public void AddFilter()
    {
        // по какому полю
        var action = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("По какому полю хотите провсти фильтрацию")
                .AddChoices(["Жанр", "Год", "Рейтинг"]));
        Film currentFilm = new Film(); // создаем фильм для проверки вводимых данных на корректность
        // Опять много много однотипных ифов, добавляющих к делегату анонимные методы, выполняющие указанную фильтрацию
        // (с помощью LINQ)
        if (action == "Жанр")
        {
            Console.WriteLine("Введите необходимые жанры (в одну строку через ', '");
            try
            {
                currentFilm[1] = Console.ReadLine(); // проверяем, что жанры корректные
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            editions += delegate() // добавляем к делегату
            {
                films = (from g in currentFilm.Genres
                    from f in films
                    where f.Genres.Contains(g)
                    select f).ToList();
            };
        }
        
        else if (action == "Год")
        {
            Console.WriteLine("Введите год");
            try
            {
                currentFilm[2] = Console.ReadLine();
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            var action2 = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Получить все фильмы, год которых...")
                    .AddChoices(["больше или равен введенного", "равен введенному", "меньше или равен введенного"]));
            if (action2 == "больше или равен введенного")
            {
                editions += delegate()
                {
                    films = (from f in films
                        where f.Year >= currentFilm.Year
                        select f).ToList();
                };
            }
            else if (action2 == "меньше или равен введенного")
            {
                editions += delegate()
                {
                    films = (from f in films
                        where f.Year <= currentFilm.Year
                        select f).ToList();
                };
            }
            else
            {
                editions += delegate()
                {
                    films = (from f in films
                        where f.Year == currentFilm.Year
                        select f).ToList();
                };
            }
        }
        else if (action == "Рейтинг")
        {
            Console.WriteLine("Введите рейтинг");
            try
            {
                currentFilm[3] = Console.ReadLine();
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            var action2 = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Получить все фильмы, рейтинг которых...")
                    .AddChoices(["больше или равен введенного", "равен введенному", "меньше или равен введенного"]));
            if (action2 == "больше или равен введенного")
            {
                editions += delegate()
                {
                    films = (from f in films
                        where f.Rating >= currentFilm.Rating
                        select f).ToList();
                };
            }
            else if (action2 == "меньше или равен введенного")
            {
                editions += delegate()
                {
                    films = (from f in films
                        where f.Rating <= currentFilm.Rating
                        select f).ToList();
                };
            }
            else
            {
                editions += delegate()
                {
                    films = (from f in films
                        where f.Rating == currentFilm.Rating
                        select f).ToList();
                };
            }
        }
    }
    /// <summary>
    /// Управление добавлением фильтраций и сортировок
    /// </summary>
    /// <returns> возвращает измененный каталог фильмов </returns>
    public List<Film> CatalogOfEditings()
    {
        Console.WriteLine("Сколько сортировок и фильтров хотите выполнить?");
        if (!int.TryParse(Console.ReadLine(), out int res) || res <= 0)
        {
            Menu.WriteMessage("Это не положительное натуральное число", ConsoleColor.Red);
            return null;
        }
        // Добавляем сортировки и фльтрации
        for (int i = 0; i < res; i++)
        {
            var action = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Выберите действие")
                    .AddChoices(Choices));
            if (action == Choices[0])
            {
                AddSort();
            }
            else
            {
                AddFilter();
            }
        }

        if (editions == null) // проверка, что мы добавили хоть один корректный фильтр или сортировку
        {
            return films;
        }
        editions(); // выполняем
        Menu.WriteMessage("Редактирование каталога прошло успешно", ConsoleColor.Green);
        return films; // возвращаем
    }
}