using System.ComponentModel.Design;
using Spectre.Console;
using TxtLibrary;

namespace MenuLibrary;
/// <summary>
/// Из чего состоит json-ответ нашего api запроса
/// </summary>
public class JsonDeserializationResult
{
    public string Title { get; set; } // название
    public int Year { get; set; } // год 
    public string Genre { get; set; } // жанр
    public string Director { get; set; } // Директор
    public string Actors { get; set; } // актеры
    public string Plot { get; set; } // сюдет (развернутый)
    public string Poster { get; set; } // ссылка на постер
    public RatingBases[] Ratings { get; set; } // рейтинги с разных сайтов
    
    public List<(string, double)> ConvertRatingBasesToList() //Превращаем рейтинги из внутренней структуры в список кортежей для более удобного чтения
    {
        List<(string, double)> ratings = new List<(string, double)>();
        foreach (RatingBases r in Ratings)
        {
            // У каждого сайта свой формат записи рейтинга
            double ans = 0;
            if (r.Source == "Rotten Tomatoes")
            {
                ans = int.Parse(r.Value.Substring(0, r.Value.Length - 1)) / 10;
            }
            else if (r.Source == "Metacritic")
            {
                ans = int.Parse(r.Value.Substring(0, r.Value.Length - 4)) / 10;
            }
            else if (r.Source == "Internet Movie Database")
            {
                ans = double.Parse(r.Value.Substring(0, r.Value.Length - 3).Replace('.', ','));
            }
            ratings.Add((r.Source, ans));
        }
        return ratings;
    }
    /// <summary>
    /// Создаем новый фильм из ответа
    /// </summary>
    /// <returns> расширенный фильм </returns>
    public UpdatedFilm ConvertJsonResultToFilm()
    {
        UpdatedFilm uFilm = new UpdatedFilm();
        uFilm.Name = Title;
        uFilm.Year = Year;
        uFilm.Genres = new List<string>(Genre.Split(','));
        uFilm.Ratings = ConvertRatingBasesToList();
        double avRating = 0;
        foreach ((string Name, double rate) in uFilm.Ratings)
        {
            avRating += rate;
        }
        uFilm.AverageRating = double.Round(avRating / 3, 1); // средний рейтинг по всем платформам
        uFilm.Rating = uFilm.AverageRating;
        uFilm.Poster = Poster;
        uFilm.Director = Director;
        uFilm.Actors = Actors;
        uFilm.Plot = Plot;
        return uFilm;
    }
    
    /// <summary>
    /// Расширяем фильм с помощью полученных данных
    /// </summary>
    /// <param name="film"> что расширяем </param>
    /// <returns> расширенный фильм</returns>
    public UpdatedFilm ConvertFilmToUpdatedWithJson(Film film)
    {
        UpdatedFilm uFilm = new UpdatedFilm();
        uFilm.Name = Title;
        uFilm.Year = Year;
        uFilm.Genres = film.Genres;
        foreach (string genres in Genre.Split(", "))
        {
            if (!uFilm.Genres.Contains(genres)) // добавляем новые жанры, если их не было
            {
                uFilm.Genres.Add(genres); // старые жанры не убираем, чтоб пользователь мог сам влиять на жанры
            }
        }
        uFilm.Ratings = ConvertRatingBasesToList();
        double avRating = 0;
        foreach ((string Name, double rate) in uFilm.Ratings)
        {
            avRating += rate;
        }
        uFilm.AverageRating = double.Round(avRating / 3, 1);
        uFilm.Rating = film.Rating; //старый (введенный пользователем) рейтинг оставляем, так как есть возможность пользователю самому менять рейтинг
        uFilm.Poster = Poster;
        uFilm.Director = Director;
        uFilm.Actors = Actors;
        uFilm.Plot = Plot;
        return uFilm;
    }
}
/// <summary>
/// Вложенная структура для рейтингов с других платформ
/// </summary>
public class RatingBases
{
    public string Source { get; set; }
    public string Value { get; set; }

    public override string ToString()
    {
        string ans = "Source: " + Source + "\n" + "Value: " + Value;
        return ans;
    }
}