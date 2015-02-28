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
    static string foodSymbol = '♣'.ToString();//"@";
    static void Main()
    {
        Console.SetWindowSize(85, 26);
        Console.SetBufferSize(85, 26);
        Console.CursorVisible = false;

        MainMenu();
        PlayGame(2, "Much game");

        //string[,] screen;
        //var bomb = new Dictionary<int, List<int>>();
        //bomb.Add(foodPosition.row, new List<int>());
        //bomb[obstaclePosition.row].Add(obstaclePosition.col);

        //string[,] screen = InitializeScreen(foodPosition,  bomb);//new string[25, 80];
        //int valuesCol = screen.GetLength(1) - 2;
        //string placeHolder = "{0, -14}";
        ////Using a string matrix you can show Score, Name, Speed, Level and Time 
        ////by changing one element in the matrix to a left aligned string of the variable
        ////examples:
        ////Score:
        //screen[2, valuesCol] = String.Format(placeHolder, 50000);
        ////Name:
        //screen[5, valuesCol] = String.Format(placeHolder, "Pesho");
        ////Speed:
        //screen[8, valuesCol] = String.Format(placeHolder, 7);
        ////Level:
        //screen[11, valuesCol] = String.Format(placeHolder, "Medium");
        ////Time:
        //screen[14, valuesCol] = String.Format(placeHolder, 360);
        //StringBuilder can also be used to build proper 14 charcter strings 
        //that are placed in the second to last col and correct row



        //printScreen(screen);

        //music will be used for background music while in a menu or in game.
        //sound will be used for playing short sounds when things happen in the game (power ups, eating stuff, hitting a wall)
        //music and sound are seperated so they can played at the same time and can have diferent volume levels
        //wmp.URL can't use paths like "..\..\Sounds\inMenu.wav" they need to be "inGame.wav"
        //sounds are played in the background until the sound finishes so the program won't have to wait for them to finish
        //a file can't be looped so a timer my need to be implemented to play the sound again after the file is over
        //WindowsMediaPlayer music = new WindowsMediaPlayer();
        //WindowsMediaPlayer sound = new WindowsMediaPlayer();

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
        //Position head = new Position(1, 1);

        //string face = ((char)1).ToString();
        //string bodyChar = ((char)4).ToString();

        
        //Position headPos = new Position(22, 31);
        //Position pastPos = new Position(23, 31);
        //var body = new Queue<Position>();
        //body.Enqueue(pastPos);
        //Position foodPosition = Food();
        //bool eatSelf;
        //bool hitWall;

        //while (true)
        //{                
        //    if (IsFoodEaten(headPos, foodPosition))
        //    {
                
        //        //if (bomb.ContainsKey(obstaclePosition.row))
        //        //{
        //        //    bomb[obstaclePosition.row].Add(obstaclePosition.col);
        //        //}
        //        //else
        //        //{
        //        //    bomb.Add(obstaclePosition.row, new List<int>());
        //        //    bomb[obstaclePosition.row].Add(obstaclePosition.col);
        //        //}

        //        foodPosition = Food();
        //        body.Enqueue(pastPos);             
        //    }

        //    screen = InitializeScreen(foodPosition);

        //    body.Enqueue(pastPos);
        //    body.Dequeue();
            
        //    foreach (Position point in body)
        //    {
        //        screen[point.row, point.col] = bodyChar;
        //    }
        //    screen[headPos.row, headPos.col] = face;

        //    Thread.Sleep(150);
        //    Console.Clear();
        //    printScreen(screen);

        //    eatSelf = body.Contains(headPos);
        //    hitWall = headPos.row > 23 || headPos.row < 1 || headPos.col > 68 || headPos.col < 1;
        //    if (hitWall || eatSelf)
        //    {
        //        break;
        //    }

        //    //Thread.Sleep(99999);
        //    pastPos = headPos;
        //    headPos = DirectionOfMovement(headPos);

        //    //bool isBombEat = bombEat(next, bomb);
        //    //if (isBombEat)
        //    //{
        //    //    break;
        //    //}
        //}

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

    //--------------Game
    static Dictionary<string, int> PlayGame(int speed, string level)
    {
        WindowsMediaPlayer music = new WindowsMediaPlayer();
        WindowsMediaPlayer sound = new WindowsMediaPlayer();
        string face = ((char)1).ToString();
        string bodyChar = ((char)4).ToString();

        string[,] screen;
        Position headPos = new Position(22, 31);
        Position pastPos = new Position(23, 31);
        var body = new Queue<Position>();
        body.Enqueue(pastPos);
        Position foodPosition = Food();
        bool eatSelf;
        bool hitWall;

        while (true)
        {
            if (IsFoodEaten(headPos, foodPosition))
            {

                //if (bomb.ContainsKey(obstaclePosition.row))
                //{
                //    bomb[obstaclePosition.row].Add(obstaclePosition.col);
                //}
                //else
                //{
                //    bomb.Add(obstaclePosition.row, new List<int>());
                //    bomb[obstaclePosition.row].Add(obstaclePosition.col);
                //}

                foodPosition = Food();
                body.Enqueue(pastPos);
            }

            screen = InitializeScreen(foodPosition);

            body.Enqueue(pastPos);
            body.Dequeue();

            foreach (Position point in body)
            {
                screen[point.row, point.col] = bodyChar;
            }
            screen[headPos.row, headPos.col] = face;

            Thread.Sleep(150);
            Console.Clear();
            printScreen(screen);

            eatSelf = body.Contains(headPos);
            hitWall = headPos.row > 23 || headPos.row < 1 || headPos.col > 68 || headPos.col < 1;
            if (hitWall || eatSelf)
            {
                break;
            }

            //Thread.Sleep(99999);
            pastPos = headPos;
            headPos = DirectionOfMovement(headPos);

            //bool isBombEat = bombEat(next, bomb);
            //if (isBombEat)
            //{
            //    break;
            //}
        }

        return new Dictionary<string, int>();
    }

    //Bogomil
    public static int direction = 3;
    private static readonly Random rnd = new Random();
    private static readonly object syncLock = new object();

    static ConsoleKeyInfo userInput = new ConsoleKeyInfo();
    //-------------Movement
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
            userInput = CorrectKey();// Console.ReadKey();

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
            if (Console.KeyAvailable)
            { 
                correctKey = Console.ReadKey();
                if (correctKey.Key == ConsoleKey.LeftArrow || correctKey.Key == ConsoleKey.RightArrow ||
                correctKey.Key == ConsoleKey.UpArrow || correctKey.Key == ConsoleKey.DownArrow)
                {
                    return correctKey;
                }
            }
            else
            {
                return userInput;
            }       
        }    
    }

    //--------------Screen
    static string[,] InitializeScreen(Position food/*, Dictionary<int, List<int>> bomb*/)
    {
        //ScoreRow = 2;
        //NameRow = 5;
        //SpedRow = 8;
        //LevelRow = 11;
        //TimeRow = 14;
        string[,] screen = new string[25, 72];

        int totalRows = screen.GetLength(0);
        int totalCols = screen.GetLength(1);

        for (int row = 0; row < totalRows; row++)
        {
            for (int col = 0; col < totalCols; col++)
            {
                //if (bomb.ContainsKey(row) && bomb[row].Contains(col))
                //{
                //    screen[row, col] = "*";
                //}
                if (col == 0 || col == totalCols - 1 || col == totalCols - 3)
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

        screen[food.row, food.col] = foodSymbol;

        return screen;
    }

    static void printScreen(string[,] screen)
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

        Console.Write(result);
    }
   
    //-------------High Scores
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

    //-----------Food
    public static Position Food()
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

    public static bool IsFoodEaten(Position currentPosition, Position obstacle)
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

    #region BombMethod
    //public static bool bombEat(Position nextPosition, Dictionary<int, List<int>> bombPositions)
    //{
    //    bool isBombEat = false;
    //    if (bombPositions.ContainsKey(nextPosition.row) && bombPositions[nextPosition.row].Contains(nextPosition.col))
    //    {
    //        isBombEat = true;
    //    }

    //    return isBombEat;
    //}
    #endregion
    //---------------Menus
    static void PrintMainMenu() 
    {
        string basilisk =
       @"▀█████████▄     ▄████████    ▄████████  ▄█   ▄█        ▄█     ▄████████    ▄█   ▄█▄ 
  ███    ███   ███    ███   ███    ███ ███  ███       ███    ███    ███   ███ ▄███▀ 
  ███    ███   ███    ███   ███    █▀  ███▌ ███       ███▌   ███    █▀    ███▐██▀   
 ▄███▄▄▄██▀    ███    ███   ███        ███▌ ███       ███▌   ███         ▄█████▀    
▀▀███▀▀▀██▄  ▀███████████ ▀███████████ ███▌ ███       ███▌ ▀███████████ ▀▀█████▄    
  ███    ██▄   ███    ███          ███ ███  ███       ███           ███   ███▐██▄   
  ███    ███   ███    ███    ▄█    ███ ███  ███▌    ▄ ███     ▄█    ███   ███ ▀███▄ 
▄█████████▀    ███    █▀   ▄████████▀  █▀   █████▄▄██ █▀    ▄████████▀    ███   ▀█▀ 
                                            ▀                             ▀         ";
        StringBuilder menu = new StringBuilder(basilisk);
        string whiteSpace = new string(' ', 34);

        menu.Append(new string('\n', 4));
        menu.Append(whiteSpace);
        menu.Append("1. Play New Game\n");
        menu.Append(whiteSpace);
        menu.Append("2. High Scores\n");
        menu.Append(whiteSpace);
        menu.Append("3. Mute Music\n");
        menu.Append(whiteSpace);
        menu.Append("4. Exit\n");

        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.Write(menu);
        Console.ForegroundColor = ConsoleColor.Gray;
    }
    private static int MainMenu()
    {
        PrintMainMenu();

        while (true)
        {
            userInput = Console.ReadKey();

            if (userInput.Key == ConsoleKey.D1)
            {
                Console.Clear();
                return 1;
            }
            else if (userInput.Key == ConsoleKey.D2)
            {
                Console.Clear();
                return 2;
            }
            else if (userInput.Key == ConsoleKey.D3)
            {
                Console.Clear();
                return 3;
            }
            else if (userInput.Key == ConsoleKey.D4)
            {
                Console.Clear();
                return 4;
            }
            else
            {
                Console.Clear();
                PrintMainMenu();
            }
        }
    }
    private static void PrintTankYouForPlayingScreen()
    {
        Console.WriteLine("Thank you for playing!");
    }
    //Not implemented methods
    private static void StartNewGame()
    {
        // TODO: Implement game;
        Console.WriteLine("Snake game");
        Console.WriteLine("Game over!");
        AddCurrentUserScore("Kiro", CalculateResult());
        Console.Clear();
        PrintHighScores();
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
    private static void PauseGame()
    {
        throw new NotImplementedException();
    }
}