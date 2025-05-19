using System.Xml;

namespace MenuLibrary;
using Spectre.Console;
using TxtLibrary;
/// <summary>
/// Класс для реализации Breakdown Chart
/// </summary>
public class MyChart
{
    private readonly string[] Choices = ["1) Жанры", "2) Рейтинг"]; //Выбор возможных действий
    //Цвета для покраски квадративков в BChart
    private readonly Color[] AllColors = [Color.Green, Color.Red, Color.Blue, Color.Yellow, Color.LightGreen, Color.Purple, Color.Aqua, Color.Navy, Color.DarkBlue, Color.DarkMagenta, Color.White];
    private List<Film> films; //фильмы, по которым и будем выводить статистику
    /// <summary>
    /// Конструктор для определения поля films
    /// </summary>
    /// <param name="films"> Лист фильмов, статистику по которым будем собирать</param>
    public MyChart(List<Film> films)
    {
        this.films = films;
    }
    /// <summary>
    /// Определиться, статистику по какому полю юудем выводить
    /// </summary>
    public void MakeChoise()
    {
        // Обычный TUI из SC
        var field = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Выберите поле, по которому надо сделать распределение")
                .AddChoices(Choices));
        if (field == Choices[0]) CollectStats(false);
        else CollectStats(true);
    }
    /// <summary>
    /// Делает словарь со статистикой
    /// </summary>
    /// <param name="value"> статистика по каким полям
    ///                      true - рейтинг
    ///                      false - жанр</param>
    public void CollectStats(bool value)
    {
        Dictionary<string,int> stats = new Dictionary<string,int>();
        if (value == false) // сбор по жанрам
        {
            List<string> keys = new List<string>();
            foreach (Film film in films) // проходимся по фильмам
            {
                foreach (string genre in film.Genres) // проходимся по жанрам каждого фильма
                {
                    if (stats.ContainsKey(genre)) // в словаре уже есть такой ключ
                    {
                        stats[genre]++; // увеличиваем счетчик
                    }
                    else
                    {
                        keys.Add(genre); // добавляем ключ
                        stats.Add(genre, 1);
                    }
                }
            }
            SortGenes(stats, keys); // подготавливаем к BChart
        }
        else
        {
            for (int i = 0; i < 10; i++) // проходимся по всем отрезкам рейтинга
            {
                stats.Add(i + "-" + (i + 1), 0);
            }
            stats.Add("10", 0);
            foreach (Film film in films)
            {
                int rate = (int)film.Rating;
                if (rate == 10) // если рейтинг 10 (это отдельная группа)
                {
                    stats["10"]++;
                }
                else
                {
                    stats[rate.ToString() + '-' + (rate + 1)]++; // превращаем число в его отрезок и прибавляем счетчик
                }
            }
            List<(string, int, Color)> chartFields = new List<(string Label, int Value, Color color)>(); // Переделываем в формат для BChart
            for (int i = 0; i < 10; i++)
            {
                string curKey = i + "-" + (i + 1);
                chartFields.Add((curKey, stats[curKey], AllColors[i]));
            }
            chartFields.Add(("10", stats["10"], AllColors[10]));
            DrawChart(chartFields);
        }
    }
    /// <summary>
    /// Сортирует жанры, чтоб взять 11 самых популярных
    /// </summary>
    /// <param name="stats"> Словарь, где ключ - жанр, а значение - в скольких фильмах</param>
    /// <param name="genres"> Список всех ключей словаря </param>
    public void SortGenes(Dictionary<string, int> stats, List<string> genres)
    {
        Comparison<string> comparison = (a, b) => stats[b] - stats[a]; // лямбда - выражение для сортировки по значению в словаре
        genres.Sort(comparison); // сортируем
        List<(string, int, Color)> chartFields = new List<(string Label, int Value, Color color)>(); // переделываем в формат для BChart
        for (int i = 0; i < Math.Min(genres.Count, 11); i++)
        {
            chartFields.Add((genres[i], stats[genres[i]], AllColors[i])); // Добавляем только 11 самых популярных
        }
        DrawChart(chartFields);
    }
    /// <summary>
    /// рисует BChart
    /// </summary>
    /// <param name="items"> лист элементов BChart</param>
    public void DrawChart(List<(string Label, int Value, Color color)> items)
    {
        // отрисовка
        AnsiConsole.Write(new BreakdownChart()
            .FullSize()
            .AddItems(items, (item) => new BreakdownChartItem(
                item.Label, item.Value, item.color)));
    }
}