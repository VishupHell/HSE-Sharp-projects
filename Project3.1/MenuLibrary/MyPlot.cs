namespace MenuLibrary;
using TxtLibrary;
using ScottPlot;
/// <summary>
/// рисует график распределения по рейтингу
/// </summary>
public class MyPlot
{
    private List<Film> films; // Фильмы, по данным которых будем строить гистограмму

    public MyPlot(List<Film> films) // конструктор
    {
        this.films = films;
    }
    /// <summary>
    /// Метод создания гистограммы
    /// </summary>
    public void CreatePlot()
    {
        double[] xs = [ 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 ]; // ось Х
        double[] ys = new double[11]; // ось Y 
        foreach (Film film in films)
        {
            int rate = (int)film.Rating;
            // Собираем статистику распределения по рейтингам
            if (rate == 10) // если рейтинг 10 (это отдельная группа)
            {
                ys[10]++;
            }
            else
            {
                ys[rate]++; // превращаем число в его отрезок и прибавляем счетчик
            }
        }
        var plt = new Plot(); // создаем поле для рисования
        plt.Axes.Margins(bottom: 0); // начало с 0
        plt.Add.Bars(xs, ys); //рисуем значения
        plt.SavePng("histogram.png", 400, 300); // сохраняем гистограмму
        Menu.WriteMessage("Гистограмма загружена", ConsoleColor.Green);
    }
}