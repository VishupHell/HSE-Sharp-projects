
namespace TxtLibrary;

/// <summary>
/// Класс, представляющий расширенный фильм (наследник фильма)
/// </summary>
public class UpdatedFilm : Film
{
    private static int Count;

    static UpdatedFilm()
    {
        Count = 0; // счетчик, чтобы присваивать расширенным фильмам разные номера и созранять из постеры локально
    }
    public string Director { get; set; }
    public string Actors { get; set; }
    public string Plot { get; set; }
    public string Poster { get; set; }

    public List<(string, double)> Ratings { get; set; }
    public double AverageRating { get; set; }
    
    public int ID { get; set; }

    public void SetID() // присвоить фильму ID
    {
        ID = Count;
        Count++;
    }
    
    public override IEnumerable<string> AllFields() // Перечисление полей для  вывода в строчку
    {
        return new List<string> { "Название", "Жанр", "Год выпуска", "Средний рейтинг по платформам", "Директор", "Актеры", "Сюжет"};
    }

    public override string this[int index] // индексатор имеет только свойство get, так как он используется только для вывода
    {
        get
        {
            if (index == 0) return Name;
            else if (index == 1) return string.Join(", ", Genres);
            else if (index == 2) return Year.ToString();
            else if (index == 3) return AverageRating.ToString();
            else if (index == 4) return Director;
            else if (index == 5) return Actors;
            else if (index == 6) return Plot;
            throw new ArgumentOutOfRangeException("Неверный индекс");
        }
    }
    /// <summary>
    /// Вывод в строчку
    /// </summary>
    /// <returns></returns>
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
}