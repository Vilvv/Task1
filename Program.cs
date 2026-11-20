using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

internal class Program
{
    private static void Main(string[] args)
    {
        const int MAXTRIES = 5;
        const int MIN_LEN = 8;
        const int MAX_LEN = 30;
        int ms;

        //locale
        var locale = new Dictionary<string, List<string>>() {
        {"2", [
            "Enter player name",                                     // 0
            "Enter a word between 8 and 30 letters long:",           // 1
            "enters a word",                                         // 2
            "Incorrect sequence of letters!",                        // 3
            "This word has already appeared!",                       // 4
            "Enter delay (default - 5000ms):",                       // 5
            "Ms is set to {0}!",                                     // 6
            "Default is 5000ms",                                     // 7
            "Max tries limit! Press any key to exit...",             // 8
            "Empty input!",                                          // 9
            "Word must contain 2 letters at least!",                 // 10
            "{0} lost! Time is over!",                               // 11
            "Game Over!"                                             // 12
        ]},
        {"1", [
            "Введите имя игрока",                                    // 0
            "Введите слово от 8 до 30 букв:",                        // 1
            "вводит слово",                                          // 2
            "Неверный набор букв!",                                  // 3
            "Это слово уже было!",                                   // 4
            "Введите задержку (по умолчанию - 5000мс):",             // 5
            "Установлено {0} мс!",                                   // 6
            "Установлено по умолчанию 5000мс",                       // 7
            "Лимит попыток исчерпан! Нажмите любую клавишу для выхода...", // 8
            "Пустой ввод!",                                          // 9
            "Слово должно содержать минимум 2 буквы!",               // 10
            "{0} проиграл! Время вышло!",                            // 11
            "Игра окончена!"                                         // 12
        ]}};


        //input section
        Console.WriteLine("Choose language ( 1 - ru || 2 - eng ):");
        string lang = getLang(MAXTRIES);
        Console.Clear();

        Console.WriteLine($"{locale[lang][0]} 1: ");
        string inputName = Console.ReadLine() ?? "";
        string playerName1 = string.IsNullOrWhiteSpace(inputName) ? "player 1" : inputName;
        Console.WriteLine($"{locale[lang][0]} 2: ");
        inputName = Console.ReadLine() ?? "";
        string playerName2 = string.IsNullOrWhiteSpace(inputName) ? "player 2" : inputName;

        Console.WriteLine($"{locale[lang][1]}");
        string initialWord = Console.ReadLine()?.ToLower() ?? "";

        int i = 0;
        while ((initialWord.Length < MIN_LEN || initialWord.Length > MAX_LEN) && i < MAXTRIES)
        {
            Console.WriteLine(locale[lang][1]);
            initialWord = Console.ReadLine()?.ToLower() ?? "";
            i++;
        }

        if (!initialWord.All(char.IsLetter))
        {
            Console.WriteLine("Word must contain only letters!");
            Console.ReadKey();
            return;
        }

        if (initialWord.Length < MIN_LEN || initialWord.Length > MAX_LEN)
        {
            Console.WriteLine(locale[lang][8]);
            Console.ReadKey();
            return;
        }

        Console.WriteLine(locale[lang][5]);
        if (Int32.TryParse(Console.ReadLine(), out ms) && ms > 0)
        {
            Console.WriteLine(string.Format(locale[lang][6], ms));
        }
        else
        {
            ms = 5000;
            Console.WriteLine(locale[lang][7]);
        }

        Game game = new Game(locale, lang, ms, playerName1, playerName2, initialWord);
        game.Run();
    }

    private static string getLang(int MAXTRIES)
    {
        int i = 0;
        while (i < MAXTRIES)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            if ((keyInfo.Key == ConsoleKey.D1) || (keyInfo.Key == ConsoleKey.D2) || (keyInfo.Key == ConsoleKey.NumPad2) || (keyInfo.Key == ConsoleKey.NumPad1))
            {
                return keyInfo.KeyChar.ToString();
            }
            else
            {
                Console.WriteLine("Choose between 1 and 2!");
                i++;
            }
        }
        Console.WriteLine("Max tries limit! Eng was setted. Press any key to continue...");
        Console.ReadKey();
        return "2";
    }
}

public class Game
{
    private readonly Dictionary<string, List<string>> _locale;
    private readonly List<string> _words;
    private readonly Dictionary<char, int> _initDict;
    private readonly string _playerName1;
    private readonly string _playerName2;
    private readonly string _lang;
    private readonly int _ms;

    public Game(Dictionary<string, List<string>> locale, string lang, int ms, string p1, string p2, string initialWord)
    {
        _locale = locale;
        _lang = lang;
        _ms = ms;
        _playerName1 = p1;
        _playerName2 = p2;
        _words = new List<string> { initialWord };

        _initDict = initialWord
            .GroupBy(letter => letter)
            .ToDictionary(e => e.Key, e => e.Count());
    }

    public void Run()
    {
        bool isPlayer1Turn = true;

        while (true)
        {
            bool isTimeOver = false;

            string currentPlayer = isPlayer1Turn ? _playerName1 : _playerName2;

            Timer timer = new Timer(_ => isTimeOver = true, null, _ms, Timeout.Infinite); //timer 

            var startTime = DateTime.UtcNow;

            Console.WriteLine($"\n{currentPlayer} {_locale[_lang][2]}:");

            string? word = Console.ReadLine()?.ToLower();

            if (string.IsNullOrEmpty(word))
            {
                Console.WriteLine(_locale[_lang][9]);
                timer.Dispose();
                break;
            }

            if (word.Length < 2)
            {
                Console.WriteLine(_locale[_lang][10]);
                timer.Dispose();
                break;
            }

            if (isTimeOver)
            {
                Console.WriteLine(string.Format(_locale[_lang][11], currentPlayer));
                timer.Dispose();
                break;
            }

            if (_words.Contains(word))
            {
                Console.WriteLine(_locale[_lang][4]);
                timer.Dispose();
                break;
            }

            if (!checkWord(word, _initDict))
            {
                Console.WriteLine(_locale[_lang][3]);
                timer.Dispose();
                break;
            }

            _words.Add(word);
            Console.WriteLine($"{(DateTime.UtcNow - startTime).TotalMilliseconds} ms");
            isPlayer1Turn = !isPlayer1Turn;

            timer.Dispose();
        }

        Console.WriteLine($"\n{_locale[_lang][12]}");
    }

    private bool checkWord(string w, Dictionary<char, int> dict)
    {
        return w.GroupBy(c => c)
                .All(g => dict.TryGetValue(g.Key, out int count) && count >= g.Count());
    }
}