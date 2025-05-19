namespace MenuLibrary;
using TxtLibrary;
using Newtonsoft.Json.Linq;
/// <summary>
/// Выполнение запросов к сайту
/// </summary>
public class ApiWork
{
    /// <summary>
    /// Делает запрос к OMDB и получает ответ
    /// </summary>
    /// <param name="Name"> Имя фильма </param>
    /// <param name="year"> год выпуска </param>
    /// <returns> возвращает json - ответ от сайта </returns>
    public string APIRequest(string Name, string year)
    {
        HttpClient client = new HttpClient();
        string newName = Name.Replace(":", "%3A"); //спец символ в названии, должен быть щаменен
        string apiName = string.Join('+', newName.Split(' ')); //слова в названии фильма перечисляются через +
        // Делает запрос
        using HttpResponseMessage response = client.GetAsync("http://www.omdbapi.com/?apikey=8b56ee3a&t=" + apiName + "&y=" + year + "&plot=full").Result;
        string json = response.Content.ReadAsStringAsync().Result; //результат
        if (json[2] == 'R') //если файл имеет вид {"Request": false... то фильм не нашли
        {
            return null;
        }
        return json;
    }
    /// <summary>
    /// запускает метод запроса и Парсит json ответ API
    /// </summary>
    /// <param name="film"> фильм, по которому надо сделать запрос </param>
    /// <returns> Возвращает распаршенный json </returns>
    public JsonDeserializationResult Work(Film film)
    {
        string ans = APIRequest(film.Name, film.Year.ToString()); //делает запрос
        if (ans == null) // запрос не получился
        {
            return null;
        }
        // Парсим объект (пользуемся newtonJson)
        JObject googleSearch = JObject.Parse(ans);
        JsonDeserializationResult searchResult = googleSearch.ToObject<JsonDeserializationResult>();
        return searchResult;
    }
    /// <summary>
    /// Делаем из фильма - расширенный фильм с помощью информации от сайта (используется при обновлении данных)
    /// </summary>
    /// <param name="film"> фильм, по которому делаем запрос </param>
    /// <returns> расширенный фильм с новыми полями </returns>
    public UpdatedFilm UpdateFilm(Film film)
    {
        JsonDeserializationResult searchResult = Work(film);
        if (searchResult == null)
        {
            return null;
        }
        return searchResult.ConvertFilmToUpdatedWithJson(film);
    }
    /// <summary>
    /// Не преобразует фильм в расширенный, а создает фильм из запроса (используется при вводе фильма сразу из базы)
    /// </summary>
    /// <param name="film"> фильм, по которому сделали запрос </param>
    /// <returns> расширенный фильм </returns>
    public UpdatedFilm CreateFilm(Film film)
    {
        JsonDeserializationResult searchResult = Work(film);
        if (searchResult == null)
        {
            return null;
        }
        return searchResult.ConvertJsonResultToFilm();
    }
}