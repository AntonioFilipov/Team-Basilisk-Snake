using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Snake
{
    static void Main(string[] args)
    {

        char[,] screen = new char[25, 75];

        int[] headPos = { 10, 40 };

        for (int row = 0; row < screen.GetLength(0); row++)
        {
            for (int col = 0; col < screen.GetLength(1); col++)
            {
                screen[row, col] = '.';
            }
        }

        printScreen(screen);


        ConsoleKeyInfo cki;
        Console.TreatControlCAsInput = true;
        Console.Clear();
        PrintWelcomeScreen();

        do
        {
            cki = Console.ReadKey();

            if (cki.KeyChar == 'h')
            {
                Console.Clear();
                PrintHighScores();
            }
            else if (cki.KeyChar == 'n')
            {
                Console.Clear();
                StartNewGame();
            }
            else if (cki.KeyChar == 'e')
            {
                Console.Clear();
                PrintTankYouForPlayingScreen();
                break;
            }
            else if (cki.KeyChar == 'c')
            {
                Console.Clear();
                PrintWelcomeScreen();
            }
            else if (cki.KeyChar == 'p')
            {
                Console.Clear();
                PauseGame();
            }

        } while (cki.Key != ConsoleKey.Escape);
    }

    static void printScreen(char[,] screen)
    {
        for (int row = 0; row < screen.GetLength(0); row++)
        {
            for (int col = 0; col < screen.GetLength(1); col++)
            {
                Console.Write(screen[row, col]);
            }
            Console.WriteLine();
        }
    }

    private static void PauseGame()
    {
        throw new NotImplementedException();
    }

    private static string GetUserName()
    {
        // TODO: fix bug with empty entry
        Console.WriteLine("Enter username: ");
        string userName = Console.ReadLine();
        var name = userName.Where(ch => char.IsLetter(ch));
        return name.ToString();
    }

    private static int CalculateResult()
    {
        // TODO: Implement real logic;
        Random rand = new Random();
        return rand.Next(100, 1000);
    }

    private static void PrintTankYouForPlayingScreen()
    {
        Console.WriteLine("Thank you for playing!");
    }

    private static void StartNewGame()
    {
        // TODO: Implement game;
        Console.WriteLine("Snake game");
        Console.WriteLine("Game over!");
        AddCurrentUserScore("Kiro", CalculateResult());
        Console.Clear();
        PrintHighScores();
    }

    private static void PrintWelcomeScreen()
    {
        Console.WriteLine(new String('-', 15) + "Welcome screen" + new String('-', 15));
        Console.WriteLine();
        Console.WriteLine("\t\tN => New Game");
        Console.WriteLine("\t\tP => Pause Game");
        Console.WriteLine("\t\tC => Controllers info");
        Console.WriteLine("\t\tH => High-scores");
        Console.WriteLine("\t\tE => Exit");
        Console.WriteLine();
        Console.WriteLine(new String('-', 44));
    }
    private static void AddCurrentUserScore(string input, int score)
    {
        string userName = input.Trim();

        var highScores = GetHighScores();

        if (highScores.ContainsKey(userName))
        {
            highScores[userName] = score;
        }
        else
        {
            highScores.Add(userName, score);
        }

        SaveHighScore(highScores);
    }

    private static void SaveHighScore(Dictionary<string, int> currentScores)
    {
        using (StreamWriter sw = new StreamWriter("..\\..\\HighScores.txt"))
        {
            foreach (var item in currentScores)
            {
                sw.WriteLine(item.Key + ", " + item.Value);
            }
        }
    }

    public static Dictionary<string, int> GetHighScores()
    {
        string line = "";
        string path = "..\\..\\HighScores.txt";
        char[] delimiter = new char[] { ',', ' ', ':', '!' };
        var output = new Dictionary<string, int>();
        if (File.Exists(path))
        {
            using (StreamReader sr = new StreamReader(path))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    string[] currentLine = line.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                    output.Add(currentLine[0].Trim(), int.Parse(currentLine[1]));
                }
            }
        }
        else
        {
            SaveHighScore(output);
        }

        return output;
    }

    public static void PrintHighScores()
    {
        Console.WriteLine(new String('-', 15) + "Highscores" + new String('-', 15));
        Console.WriteLine();
        var items = from pair in GetHighScores()
                    orderby pair.Value descending
                    select pair;
        int counter = 0;
        foreach (var pair in items)
        {
            Console.WriteLine("\t{0} : {1,-8} {2,8}", ++counter, pair.Key, pair.Value);
        }

        Console.WriteLine();
        Console.WriteLine(new String('-', 40));
    }
}
