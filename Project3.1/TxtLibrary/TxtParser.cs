
using System.Text;
namespace TxtLibrary
{
    /// <summary>
    /// Класс для парсинга json
    /// </summary>
    public static class TxtParser
    {
        public static void WriteTxt(List<Film> films)
        {
            for (int i = 0; i < films.Count; i++)
            {
                Console.WriteLine(films[i].ConvertToLine());
            }
        }
        /// <summary>
        /// перечисление состояний для парсинга
        /// </summary>
        private enum State
        {
            Program, //Внешнаяя часть
            Key
        };
        /// <summary>
        /// Чтение
        /// </summary>
        /// <typeparam name="T"> Тип считываемого объекта </typeparam>
        /// <returns> Возвращает лист этих объектов </returns>
        public static List<Film> ReadTxt()
        {
            List<Film> ans = new List<Film>();
            ans.Add(new Film());
            int indList = 0;
            State state = State.Program;
            string current;
            while ((current = Console.ReadLine()) is not "" and not null)
            {
                string key = "";
                int indexField = 0;
                foreach (char symb in current)
                {
                    bool flag = false;
                    switch (state)
                    {
                        case State.Program: // Если мы во внешней части
                            if (symb == '[')
                            {
                                state = State.Key; // мы в ключе
                                key = "";
                            }
                            break;
                        case State.Key:
                            if (symb == ']') // ключ закончился
                            {
                                state = State.Program;
                                try
                                {
                                    ans[indList][indexField++] = key; // заполняем поле
                                    if (indexField == 4)
                                    {
                                        indList++;
                                        ans.Add(new Film()); // создаем новый фильм
                                    }
                                }
                                catch (ArgumentException)
                                {
                                    flag = true;
                                }
                                catch (IndexOutOfRangeException)
                                {
                                    return null;
                                }
                            }
                            else
                            {
                                key += symb;
                            }

                            break;
                    }

                    if (flag) break;
                }
            }
            ans.Remove(ans[indList]);
            return ans;
        }
    }
}