using System.Net.Mail;
using TxtLibrary;
using Spectre.Console;
namespace MenuLibrary;
/// <summary>
/// реализует Карусель
/// </summary>
public class Carousel
{
    
    private List<UpdatedFilm> films; //Расширенные фильмы (у них есть ссылка на постер)
    private string[] Choices = ["1 - Сдвинуться влево", "2 - Сдвинуться вправо", "3 - Выйти"]; // Возможные варианты действия
    /// <summary>
    /// Конструктор, отбирает из всех фильмов расширенные (с ссылкой на постер)
    /// </summary>
    /// <param name="films"> фильмы, из которыз отбираем </param>
    public Carousel(List<Film> films)
    {
        HttpClient client = new HttpClient(); // для получения постера из интернета
        this.films = new List<UpdatedFilm>();
        foreach (Film film in films)
        {
            if (film is UpdatedFilm) // если фильм расширенный
            {
                UpdatedFilm uFilm = (UpdatedFilm)film; // преобразуем в расширенный
                var stream = client.GetStreamAsync(uFilm.Poster).Result; // получаем постер
                uFilm.SetID(); // Присваиваем номер фильму (нужно для названия файла, чтоб хранить локально)
                using var file = File.OpenWrite(uFilm.ID + ".png"); // Открываем файл
                stream.CopyToAsync(file); // Сохраняем туда картинку
                this.films.Add(uFilm); // добавляем в список
            }
        }
    }
    /// <summary>
    /// Сам вывод карусели
    /// </summary>
    public void PrintCarousel()
    {
        if (films.Count == 0) // если нет расширенных фильмов, то и выводить нечего
        {
            Menu.WriteMessage("Нет фильмов, расширенных с помощью OMDB", ConsoleColor.Red);
            return;
        }

        int index = 0; // на каком мы фильме
        bool flag = false; // будет true когда попросят выйти из карусели
        do
        {
            Console.Clear(); // очищаем, чтобы было красиво
            try
            {
                var image = new CanvasImage(films[index].ID + ".png"); //пытаемся нарисовать
                image.MaxWidth(16);
                AnsiConsole.Write(image);
            }
            catch (SixLabors.ImageSharp.UnknownImageFormatException) 
                // Файл не загрузился (в интернете было написано, это бывает из-за прохого соединения или медленного сервера
            {
                Console.WriteLine("Не получилось загрузить этот постер, пожалуйста, двигайтесь дальше");
            }
            catch (SixLabors.ImageSharp.InvalidImageContentException) // Файл плохого качества
            {
                Console.WriteLine("Файл слишком плохого качества для его вывода");
            }
            var action = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Выберите действие")
                    .AddChoices(Choices));
            if (action == "3 - Выйти") // выходим
            {
                flag = true; // условие выхода 
            }
            else if (action == "1 - Сдвинуться влево")// уходим влево
            {
                index += (films.Count - 1);
                index %= films.Count;
            }
            else //уходим вправо
            {
                index++;
                index %= films.Count;
            }
        } while (!flag);

        foreach (UpdatedFilm uFilm in films) //удаляем все сохраненные постеры
        {
            File.Delete(uFilm.ID + ".png");
        }
    }
}