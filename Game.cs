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