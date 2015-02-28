using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Media;
using WMPLib;

class Snake
{
    //Bogomil 
    #region Position
    public struct Position
    {
        public int row;
        public int col;
        public Position(int row, int col)
        {
            this.row = row;
            this.col = col;
        }
    }
    #endregion
    static string obstacleSymbol = "@";
    static void Main()
    {
        Console.SetWindowSize(80, 25);
        Console.SetBufferSize(80, 25);

        Position obstaclePosition = Obstacles();
        var bomb = new Dictionary<int, List<int>>();
        //bomb.Add(obstaclePosition.row, new List<int>());
        //bomb[obstaclePosition.row].Add(obstaclePosition.col);

        bool isObstEat = false;
        string[,] screen = InitializeScreen(obstaclePosition, isObstEat, bomb);//new string[25, 80];
        int valuesCol = screen.GetLength(1) - 2;
        string placeHolder = "{0, -14}";
        //Using a string matrix you can show Score, Name, Speed, Level and Time 
        //by changing one element in the matrix to a left aligned string of the variable
        //examples:
        //Score:
        screen[2, valuesCol] = String.Format(placeHolder, 50000);
        //Name:
        screen[5, valuesCol] = String.Format(placeHolder, "Pesho");
        //Speed:
        screen[8, valuesCol] = String.Format(placeHolder, 7);
        //Level:
        screen[11, valuesCol] = String.Format(placeHolder, "Medium");
        //Time:
        screen[14, valuesCol] = String.Format(placeHolder, 360);
        //StringBuilder can also be used to build proper 14 charcter strings 
        //that are placed in the second to last col and correct row



        //printScreen(screen);

        //music will be used for background music while in a menu or in game.
        //sound will be used for playing short sounds when things happen in the game (power ups, eating stuff, hitting a wall)
        //music and sound are seperated so they can played at the same time and can have diferent volume levels
        //wmp.URL can't use paths like "..\..\Sounds\inMenu.wav" they need to be "inGame.wav"
        //sounds are played in the background until the sound finishes so the program won't have to wait for them to finish
        //a file can't be looped so a timer my need to be implemented to play the sound again after the file is over
        WindowsMediaPlayer music = new WindowsMediaPlayer();
        WindowsMediaPlayer sound = new WindowsMediaPlayer();

        //This example playes "inGame.wav", waits 10 secs, then starts playing "powerUp.wav" every 5 seconds
        //music.URL = "inGame.wav";
        //music.settings.volume = 40;
        //music.controls.play();

        //Thread.Sleep(10000);

        //sound.URL = "powerUp.wav";
        //while (true)
        //{
        //    sound.controls.play();
        //    Thread.Sleep(5000);
        //}
        Position head = new Position(1, 1);

        string face = ((char)1).ToString();

        var body = new List<Position>();
        for (int i = 0; i < 5; i++)
        {
            body.Add(new Position(0, 2 + i));
        }
        //foreach (Position point in body)
        //{
        //    screen[point.row, point.col] = "*";
        //}

        //screen[head.row, head.col] = face;
        //int i = 0;
       // Position next = DirectionOfMovement(head);
        Position currentPosition = head;
        Position next = head;

        while (true)
        {
            //Console.WriteLine("Row: {0}  col: {1}", head.row, head.col);           

            bool isObstEaten = IsObstacleEaten(currentPosition, obstaclePosition);
            if (isObstEaten)
            {
                
                if (bomb.ContainsKey(obstaclePosition.row))
                {
                    bomb[obstaclePosition.row].Add(obstaclePosition.col);
                }
                else
                {
                    bomb.Add(obstaclePosition.row, new List<int>());
                    bomb[obstaclePosition.row].Add(obstaclePosition.col);
                }

                obstaclePosition = Obstacles();
                
            }



            screen = InitializeScreen(obstaclePosition, isObstEaten, bomb);
            if (currentPosition.row > 24 || currentPosition.row < 1 || currentPosition.col > 63 || currentPosition.col < 1)
            {
                break;
            }

            screen[currentPosition.row, currentPosition.col] = face;


            body.RemoveAt(body.Count - 1);
            Position nextBodyPosition = body[0];

            foreach (Position point in body)
                screen[point.row, point.col] = face;

            body.Insert(0, next);


            Thread.Sleep(150);
            Console.Clear();

            printScreen(MultiArrayToArray(screen));
            next = DirectionOfMovement(currentPosition);
            currentPosition = next;

            bool isBombEat = bombEat(next, bomb);
            if (isBombEat)
            {
                break;
            }


            
        }

        //var result = new Dictionary<string, int>();

        //result["adasdas"] = 23213123;
        //result["fafdasd"] = 123123;
        //SaveHighScore(result);

        //ConsoleKeyInfo cki;
        //Console.TreatControlCAsInput = true;
        //Console.Clear();
        //PrintWelcomeScreen();

        //do
        //{
        //    cki = Console.ReadKey();

        //    if (cki.KeyChar == 'h')
        //    {
        //        Console.Clear();
        //        PrintHighScores();
        //    }
        //    else if (cki.KeyChar == 'n')
        //    {
        //        Console.Clear();
        //        StartNewGame();
        //    }
        //    else if (cki.KeyChar == 'e')
        //    {
        //        Console.Clear();
        //        PrintTankYouForPlayingScreen();
        //        break;
        //    }
        //    else if (cki.KeyChar == 'c')
        //    {
        //        Console.Clear();
        //        PrintWelcomeScreen();
        //    }
        //    else if (cki.KeyChar == 'p')
        //    {
        //        Console.Clear();
        //        PauseGame();
        //    }

        //} while (cki.Key != ConsoleKey.Escape);

        //Bogomil
        //Position nextPositionSnakeHead = DirectionOfMovement(head);
    }
    //Bogomil

    public static int direction = 0;

    public static Position DirectionOfMovement(Position currentPositionSnakeHead)
    {
        int rigth = 0;
        int left = 1;
        int down = 2;
        int up = 3;
       
        Position[] directions = new Position[]
        {
                new Position(0, 1), //right
                new Position(0, -1), // left
                new Position(1, 0), // down
                new Position(-1, 0), // up
        };
        //int direction = rigth; //0 = right; 1 = left; 2 = down; 3 = up;

        if (Console.KeyAvailable)
        {
            //Въведената от потребителя посока
            ConsoleKeyInfo userInput = CorrectKey();// Console.ReadKey();

            if (userInput.Key == ConsoleKey.RightArrow)
            {
                if (direction != left)
                {
                    direction = rigth;
                }
            }
            else if (userInput.Key == ConsoleKey.LeftArrow)
            {
                if (direction != rigth)
                {
                    direction = left;
                }
            }
            else if (userInput.Key == ConsoleKey.DownArrow)
            {
                if (direction != up)
                {
                    direction = down;
                }
            }
            else if (userInput.Key == ConsoleKey.UpArrow)
            {
                if (direction != down)
                {
                    direction = up;
                }
            }
        }

        Position nextDirection = directions[direction];

        Position nextPositionSnakeHead = new Position(currentPositionSnakeHead.row + nextDirection.row,
                currentPositionSnakeHead.col + nextDirection.col);

        return nextPositionSnakeHead;

    }

    static ConsoleKeyInfo CorrectKey()
    {
        ConsoleKeyInfo correctKey = new ConsoleKeyInfo();

        while (true)
        {
            correctKey = Console.ReadKey();
            if (correctKey.Key.ToString() == "LeftArrow" || correctKey.Key.ToString() == "RightArrow" ||
                correctKey.Key.ToString() == "UpArrow" || correctKey.Key.ToString() == "DownArrow")
            {
                break;
            }
        }

        return correctKey;
    }

    static string[,] InitializeScreen(Position obstacle, bool isObstEaten, Dictionary<int, List<int>> bomb)
    {
        //ScoreRow = 2;
        //NameRow = 5;
        //SpedRow = 8;
        //LevelRow = 11;
        //TimeRow = 14;
        string[,] screen = new string[25, 67];

        int totalRows = screen.GetLength(0);
        int totalCols = screen.GetLength(1);

        for (int row = 0; row < totalRows; row++)
        {
            for (int col = 0; col < totalCols; col++)
            {
                if (bomb.ContainsKey(row) && bomb[row].Contains(col))
                {
                    screen[row, col] = "*";
                }
                else if (col == 0 || col == totalCols - 1 || col == totalCols - 3)
                {
                    screen[row, col] = "|";
                }
                else if ((row == 0 || row == totalRows - 1) && (col > 0 && col < totalCols - 3))
                {
                    screen[row, col] = "-";
                }
                else if (col == totalCols - 2)
                {
                    if (row == 0)
                    {
                        screen[row, col] = "-----INFO-----";
                    }
                    else if (row == 1) //2
                    {
                        screen[row, col] = String.Format("{0, -14}", "Score:");
                    }
                    else if (row == 4)//5
                    {
                        screen[row, col] = String.Format("{0, -14}", "Name:");
                    }
                    else if (row == 7)//8
                    {
                        screen[row, col] = String.Format("{0, -14}", "Speed:");
                    }
                    else if (row == 10)//11
                    {
                        screen[row, col] = String.Format("{0, -14}", "Level:");
                    }
                    else if (row == 13)//14
                    {
                        screen[row, col] = String.Format("{0, -14}", "Time:");
                    }
                    else if (row == 16)
                    {
                        screen[row, col] = String.Format("{0, -14}", "Options:");
                    }
                    else if (row == 17)
                    {
                        screen[row, col] = String.Format("{0, -14}", "1: Pause Game");
                    }
                    else if (row == 18)
                    {
                        screen[row, col] = String.Format("{0, -14}", "2: Start Game");
                    }
                    else if (row == 19)
                    {
                        screen[row, col] = String.Format("{0, -14}", "3: Mute Music");
                    }
                    else if (row == 20)
                    {
                        screen[row, col] = String.Format("{0, -14}", "4: Mute Sounds");
                    }
                    else if (row == 21)
                    {
                        screen[row, col] = String.Format("{0, -14}", "5: Exit Game");
                    }
                    else if (row == totalRows - 1)
                    {
                        screen[row, col] = "--------------";
                    }
                    else
                    {
                        screen[row, col] = String.Format("{0, -14}", "");
                    }
                }
                else
                {
                    screen[row, col] = " ";
                }
            }
        }

        screen[obstacle.row, obstacle.col] = obstacleSymbol;

        return screen;
    }

    static StringBuilder MultiArrayToArray(string[,] screen)
    {
        StringBuilder result = new StringBuilder();
        for (int row = 0; row < screen.GetLength(0); row++)
        {
            for (int col = 0; col < screen.GetLength(1); col++)
            {
                //Console.Write(screen[row, col]);
                result.Append(screen[row, col]);
            }
            // result.Append('\n');
        }
        return result;
    }

    static void printScreen(StringBuilder str)
    {
        //bool notMax = screen.GetLength(1) < 67;
        //for (int row = 0; row < screen.GetLength(0); row++)
        //{
        //    for (int col = 0; col < screen.GetLength(1); col++)
        //    {
        //        Console.Write(screen[row, col]);
        //    }
        //    if (notMax)
        //    {
        //        Console.WriteLine();
        //    }
        //}

        //StringBuilder result = new StringBuilder();

        //for (int row = 0; row < screen.GetLength(0); row++)
        //{
        //    for (int col = 0; col < screen.GetLength(1); col++)
        //    {
        //        //Console.Write(screen[row, col]);
        //        result.Append(screen[row,col]);
        //    }
        //   // result.Append('\n');
        //}

        Console.Write(str);
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

    private static readonly Random rnd = new Random();
    private static readonly object syncLock = new object();
    public static Position Obstacles()
    {
        //Random rnd = new Random();
        List<int> randomNumbersRow = new List<int>();
        List<int> randomNumbersCol = new List<int>();
        int row = 1;
        int col = 1;

        lock (syncLock)
        {
            do
            {
                row = rnd.Next(1, 24);
            }
            while (randomNumbersRow.Contains(row));

            do
            {
                col = rnd.Next(1, 64);
            }
            while (randomNumbersCol.Contains(col));

            Position obstacle = new Position(row, col);
            return obstacle;
        }
        //Position obstacle = new Position(10, 10);
       
    }

    public static bool IsObstacleEaten(Position currentPosition, Position obstacle)
    {
        bool isEaten = false;
        //nextSnakeHead = DirectionOfMovement(nextSnakeHead);
        if (currentPosition.row == obstacle.row && currentPosition.col == obstacle.col)
        {
            isEaten = true;
        }
        //Console.WriteLine("{0} {1}", nextSnakeHead.row, obstacle.row);
        return isEaten;
    }

    public static bool bombEat(Position nextPosition, Dictionary<int, List<int>> bombPositions)
    {
        bool isBombEat = false;
        if (bombPositions.ContainsKey(nextPosition.row) && bombPositions[nextPosition.row].Contains(nextPosition.col))
        {
            isBombEat = true;
        }

        return isBombEat;
    }

    

}
