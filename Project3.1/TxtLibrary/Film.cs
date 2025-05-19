using System.IO.Enumeration;
using System.Text;
using System.Text.RegularExpressions;

namespace TxtLibrary;
/// <summary>
/// Класс, представляющий один фильм, введенный из консоли или из файла, имеет всего 4 поля
/// </summary>
public class Film
{
    
    public string Name { get; set; }// Имя
    public List<string> Genres { get; set; }// жанры
    public int Year { get; set; } // год
    public double Rating { get; set; } // рейтинг

    public virtual IEnumerable<string> AllFields() //поля для вывода
    {
        return new List<string> { "Название", "Жанр", "Год выпуска", "Рейтинг" };
    }
    /// <summary>
    /// Индексатор
    /// </summary>
    /// <param name="index"> какой индекс</param>
    /// <exception cref="ArgumentOutOfRangeException"> если индекс неверный</exception>
    /// <exception cref="ArgumentException"> если пытаются положить некорректные данные </exception>
    public virtual string this [int index]
    {
        get
        {
            if (index == 0) return Name;
            else if (index == 1) return string.Join(", ", Genres);
            else if (index == 2) return Year.ToString();
            else if (index == 3) return Rating.ToString();
            throw new ArgumentOutOfRangeException("Неверный индекс");
        }
        set
        {
            if (index == 0)
            {
                Name = value;
                return;
            }
            else if (index == 1)
            {
                Genres = value.Split(", ").ToList();
                foreach (string genre in Genres)
                {
                    if (!Regex.IsMatch(genre, @"[a-zA-Zа-яА-Я- ]+")) //Жанры состоят только из букв, пробела и -
                    {
                        Genres = null;
                        throw new ArgumentException("Некорректный жанр");
                    }
                }
                return;
            }
            else if (index == 2)
            {
                int year;
                if (!int.TryParse(value, out year) || year < 1896 || year > DateTime.Now.Year) // проверка года на корректность
                {
                    throw new ArgumentException("Некорректный год");
                }
                Year = year;
                return;
            }
            else if (index == 3) // Проверка, что у рейтинга точность не больше одной цифра после запятой
            {
                double rating;
                if (!double.TryParse(value.Replace('.', ','), out rating) || rating < 0 || rating > 10 || value.Length > 3)
                { 
                    throw new ArgumentException("Некорректный рейтинг");
                }

                Rating = rating;
                return;
            }
            throw new ArgumentOutOfRangeException("Неверный индекс");
        }
    }
    /// <summary>
    /// Переопределенный метод toString()
    /// </summary>
    /// <returns>фильм, превращенный в строку </returns>
    public override string ToString()
    {
        string ans = "";
        int ind = 0;
        foreach (string s in AllFields())
        {
            ans += s + ": ";
            ans += this[ind++];
            ans += '\n';
        }
        return ans;
    }
    /// <summary>
    /// Преобразование в линию для вывода в файл
    /// </summary>
    /// <returns></returns>
    public string ConvertToLine()
    {
        string ans = "[" + Name + "]";
        ans += "[" + string.Join(", ", Genres) + "]";
        ans += "[" + Year.ToString() + "]";
        ans += "[" + Rating.ToString() + "]";
        return ans;
    }
}