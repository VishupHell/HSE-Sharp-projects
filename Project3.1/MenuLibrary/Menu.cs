using System;
using TxtLibrary;
using Spectre.Console;
namespace MenuLibrary;
public class Menu
{
    private readonly string[] Choices =
    {
        "1 - Добавить данные", "2 - Вывод каталога", "3 - Изменить рейтинг фильма", "4 - Удалить фильм", 
        "5 - Вывести диаграмму", "6 - Провести фильтрации и сортировки", "7 - Обновить все полученные данные", 
        "8 - Карусель обложек", "9 - Получить гистограмму распределения рейтингов", "10 - Завершить работу программы"
    };
    /// <summary>
    /// Работа с пользователем
    /// </summary>
    public void UserWork()
    {
        List<Film> films = null;
        bool endProgram = false;
        do {
            if (films != null && films.Count == 0)
            {
                films = null;
            }
            var action = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Выберите действие")
                    .AddChoices(Choices));
            int indexAction = Array.FindIndex(Choices, x => x == action);
            switch (indexAction)
            {
                case 0:
                {
                    // Ввод
                    MyTextReader currentReader = new MyTextReader();
                    List<Film> newFilms = currentReader.ReadText();
                    if (newFilms != null)
                    {
                        WriteMessage("Ввод прошел успешно", ConsoleColor.Green);
                        if (films == null)
                        {
                            films = newFilms;
                        }
                        else
                        {
                            films.AddRange(newFilms);
                        }
                    }

                    if (films != null && films.Count == 0)
                    {
                        films = null;
                    }
                }
                break;
                case 1:
                {
                    if (films == null)
                    {
                        WriteMessage("Вы еще не завершили корректно пункт 1", ConsoleColor.Red);
                        break;
                    }
                    MyTextWriter currentWriter = new MyTextWriter(films);
                    currentWriter.MakeChoise();
                }
                    break;
                case 2 or 3:// удаление через TUI и редактирование рейтинга
                {
                    if (films == null)
                    {
                        WriteMessage("Вы еще не завершили корректно пункт 1", ConsoleColor.Red);
                        break;
                    }
                    string[] stringFilms = new string[films.Count];
                    for (int i = 0; i < films.Count; i++)
                    {
                        stringFilms[i] = films[i].ToString();
                    }

                    string title;
                    if (indexAction == 2)
                    {
                        title = "Выберите фильм для изменения его рейтинга";
                    }
                    else
                    {
                        title = "Выберите фильм для его удаления";
                    }
                    var film = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .PageSize(3)
                            .Title(title)
                            .AddChoices(stringFilms));
                    for (int i = 0; i < stringFilms.Length; i++)
                    {
                        
                        
                        if (film == stringFilms[i])
                        {
                            if (indexAction == 3)
                            {
                                films.RemoveAt(i);
                                break;
                            }
                            Console.WriteLine("Введите новый рейтинг");
                            try
                            {
                                films[i][3] = Console.ReadLine();
                            }
                            catch (ArgumentException e)
                            {
                                WriteMessage(e.Message, ConsoleColor.Red);
                                break;
                            }
                        }
                    }
                }
                    break;
                case 4:
                {
                    if (films == null)
                    {
                        WriteMessage("Вы еще не завершили корректно пункт 1", ConsoleColor.Red);
                        break;
                    }
                    MyChart chart = new MyChart(films);
                    chart.MakeChoise();
                }
                    break;
                case 5:
                {
                    if (films == null)
                    {
                        WriteMessage("Вы еще не завершили корректно пункт 1", ConsoleColor.Red);
                        break;
                    }
                    CatalogEditing edit = new CatalogEditing(films);
                    List<Film> newFilms = edit.CatalogOfEditings();
                    if (newFilms == null)
                    {
                        break;
                    }
                    MyTextWriter currentWriter = new MyTextWriter(newFilms);
                    currentWriter.MakeChoise();
                    var action2 = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("Хотите заменить каталог на новый?")
                            .AddChoices(["Да", "Нет"]));
                    if (action2 == "Да")
                    {
                        films = newFilms;
                    }
                }
                    break;
                case 6:
                {
                    if (films == null)
                    {
                        WriteMessage("Вы еще не завершили корректно пункт 1", ConsoleColor.Red);
                        break;
                    }
                    ApiWork apiWork = new ApiWork();
                    for (int i = 0; i < films.Count; i++)
                    {
                        UpdatedFilm uFilm = apiWork.UpdateFilm(films[i]);
                        if (uFilm != null)
                        {
                            films[i] = uFilm;
                        }
                    }
                }
                    break;
                case 7:
                {
                    if (films == null)
                    {
                        WriteMessage("Вы еще не завершили корректно пункт 1", ConsoleColor.Red);
                        break;
                    }
                    Carousel curCarousel = new Carousel(films);
                    curCarousel.PrintCarousel();
                }
                    break;
                case 8:
                {
                    if (films == null)
                    {
                        WriteMessage("Вы еще не завершили корректно пункт 1", ConsoleColor.Red);
                        break;
                    }
                    MyPlot currentPlot = new MyPlot(films);
                    currentPlot.CreatePlot();
                }
                    break;
                case 9:
                {
                    endProgram = true;
                    Console.WriteLine("Завершаю программу...");
                }
                break;
            }
            Console.WriteLine();
        }
        while (!endProgram);
    }
    /// <summary>
    /// Вывод сообщения
    /// </summary>
    /// <param name="message"> сообщение </param>
    /// <param name="color"> цвет вывода</param>
    internal static void WriteMessage(string message, ConsoleColor color)
    {
        Console.WriteLine();
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ForegroundColor = ConsoleColor.Gray;
    }

    internal void ParodyCleaning(List<Film> films)
    {
        Dictionary<string, List<int>> filmsCount = new Dictionary<string, List<int>>();
        for (int i = 0; i < films.Count; i++)
        {
            if (filmsCount.ContainsKey(films[i].Name))
            {
                filmsCount[films[i].Name].Add(i);
            }
            filmsCount.Add(films[i].Name, [i]);
        }
        
    }
}