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
    static string foodSymbol = '■'.ToString();//"@";
    static void Main()
    {
        Console.SetWindowSize(85, 26);
        Console.SetBufferSize(85, 26);
        Console.CursorVisible = false;
        WindowsMediaPlayer menuMusic = new WindowsMediaPlayer();
        menuMusic.URL = "inMenu.wav";
        menuMusic.settings.setMode("loop", true);
        menuMusic.settings.volume = 50;
        menuMusic.controls.play();

        int result = 0;
        string name = null;
        string speed = null;
        string level = null;  
        while (true)
        {
            result = MainMenu();
            if (result == 1)
            {
                direction = 3;
                name = InputName();
                speed = SpeedSelect();
                level = LevelSelect();
                menuMusic.controls.stop();
                PlayGame(name, speed, level);
                menuMusic.controls.play();
                GameOver();
            }
            else if (result == 2)
            {
                HighScores();
            }
            else if (result == 3)
            {
                menuMusic.settings.mute = !menuMusic.settings.mute;
            }
            else if (result == 4)
            {
                Manual();
            }
            else if (result == 5)
            {
                result = AreYouSure();
                if (result == 1)
                {
                    menuMusic.close();
                    return;
                }
            }
        }
    }

    //--------------Game
    static void PlayGame(string name = "Player", string speed = "Medium", string level = "Medium")
    {
        //var bomb = new Dictionary<int, List<int>>();
        //bomb.Add(foodPosition.row, new List<int>());
        //bomb[obstaclePosition.row].Add(obstaclePosition.col);

        WindowsMediaPlayer music = new WindowsMediaPlayer();
        WindowsMediaPlayer sound = new WindowsMediaPlayer();
        string face = "☺";//((char)1).ToString();
        string bodyChar = "♦";//((char)4).ToString();
        string obstacleChar = "¤";

        string[,] screen = InitializeScreen(new Position(12, 30));
        List<Position> obstacles = GenerateObstacles(level);

        int score = 0;
        DateTime startTime = DateTime.Now;
        TimeSpan time = new TimeSpan();
        DateTime pauseStart = new DateTime();
        TimeSpan pauseTime = new TimeSpan();
        string placeHolder = "{0, -14}";
        string timeHolder = "{0}:{1}:{2}";
        int valuesCol = screen.GetLength(1) - 2;
        Position scorePos = new Position(2, valuesCol);
        Position timePos = new Position(14, valuesCol);
        name = String.Format(placeHolder, name);
        int speedTime = 0;
        switch (speed)
        {
            case "Slow":
                speedTime = 220;
                break;
            case "Medium":
                speedTime = 150;
                break;
            case "Fast":
                speedTime = 80;
                break;
            default:
                break;
        }
        speed = String.Format(placeHolder, speed);
        level = String.Format(placeHolder, level);

        Position headPos = new Position(22, 1);
        Position pastPos = new Position(23, 1);
        var body = new Queue<Position>();
        body.Enqueue(pastPos);

        DateTime timeClubs = new DateTime();
        DateTime timeHearts = new DateTime();
        DateTime timeSpades = new DateTime();
        var foodPositions = new List<Position>();
        Position foodPosition = Food(headPos, body, foodPositions, obstacles);
        Position posClubs = new Position();
        Position posHearts = new Position();
        Position posSpades = new Position();
        bool eatSelf;
        bool hitWall;
        bool hitObstacle;
        bool foodOfClubs = false;
        bool foodOfHearts = false;
        bool foodOfSpades = false;
        bool stopGame = false;
        int result = 0;

        music.settings.setMode("loop", true);
        //TimeSpan loop = new TimeSpan(0,1,37);
        music.URL = "inGame.wav";
        music.settings.volume = 25;
        sound.settings.volume = 65;
        music.controls.play();
        while (true)
        {
            foodPositions = new List<Position>() { foodPosition, posClubs, posHearts, posSpades };
            //Thread.Sleep(999999);
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

                sound.URL = "eat.wav";
                sound.controls.play();
                foodPosition = Food(headPos, body, foodPositions, obstacles);
                body.Enqueue(pastPos);
                score += 5;

                result = rnd.Next(1, 3);
                if (result == 1 && !foodOfClubs)
                {
                    timeClubs = DateTime.Now;
                    posClubs = Food(headPos, body, foodPositions, obstacles);
                    foodOfClubs = true;
                }

                result = rnd.Next(1, 4);
                if (result == 1 && !foodOfHearts)
                {
                    timeHearts = DateTime.Now;
                    posHearts = Food(headPos, body, foodPositions, obstacles);
                    foodOfHearts = true;
                }

                result = rnd.Next(1, 5);
                if (result == 1 && !foodOfSpades)
                {
                    timeSpades = DateTime.Now;
                    posSpades = Food(headPos, body, foodPositions, obstacles);
                    foodOfSpades = true;
                }
            }
            if (IsFoodEaten(headPos, posClubs) && foodOfClubs)
            {
                foodOfClubs = false;
                sound.URL = "eat.wav";
                sound.controls.play();
                body.Enqueue(pastPos);
                score += 10;
            }
            if (IsFoodEaten(headPos, posHearts) && foodOfHearts)
            {
                foodOfHearts = false;
                sound.URL = "eat.wav";
                sound.controls.play();
                body.Enqueue(pastPos);
                score += 15;
            }
            if (IsFoodEaten(headPos, posSpades) && foodOfSpades)
            {
                foodOfSpades = false;
                sound.URL = "eat.wav";
                sound.controls.play();
                body.Enqueue(pastPos);
                score += 25;
            }

            screen = InitializeScreen(foodPosition);

            body.Enqueue(pastPos);
            body.Dequeue();

            foreach (var obstacle in obstacles)
            {
                screen[obstacle.row, obstacle.col] = obstacleChar;
            }
            foreach (Position point in body)
            {
                screen[point.row, point.col] = bodyChar;
            }
            screen[headPos.row, headPos.col] = face;

            if (foodOfClubs)
            {
                time = DateTime.Now - timeClubs;
                if (time.Seconds >= 10)
                {
                    foodOfClubs = false;
                }
                else
                {
                    screen[posClubs.row, posClubs.col] = "♣";
                }
            }
            if (foodOfHearts)
            {
                time = DateTime.Now - timeHearts;
                if (time.Seconds >= 7)
                {
                    foodOfHearts = false;
                }
                else
                {
                    screen[posHearts.row, posHearts.col] = "♥";
                }
            }
            if (foodOfSpades)
            {
                time = DateTime.Now - timeSpades;
                if (time.Seconds >= 5)
                {
                    foodOfSpades = false;
                }
                else
                {
                    screen[posSpades.row, posSpades.col] = "♠";
                }
            }

            screen[scorePos.row, scorePos.col] = String.Format(placeHolder, score);

            time = DateTime.Now - startTime - pauseTime;
            screen[timePos.row, timePos.col] = String.Format(placeHolder, (String.Format(timeHolder, time.Hours, time.Minutes, time.Seconds)));

            screen[5, valuesCol] = name;
            screen[8, valuesCol] = speed;
            screen[11, valuesCol] = level;

            Thread.Sleep(speedTime);

            if (Console.KeyAvailable)
            {
                pauseStart = DateTime.Now;
                userInput = CorrectKey();

                if (userInput.Key == ConsoleKey.D1)
                {
                    music.controls.pause();
                    printScreen(screen);
                    while (true)
                    {
                        if (Console.KeyAvailable)
                        {
                            if (Console.ReadKey().Key == ConsoleKey.D1)
                            {
                                music.controls.play();
                                printScreen(screen);
                                break;
                            }
                            else
                            {
                                printScreen(screen);
                            }
                        }
                    }
                }
                else if (userInput.Key == ConsoleKey.D2)
                {
                    music.settings.mute = !music.settings.mute;
                }
                else if (userInput.Key == ConsoleKey.D3)
                {
                    sound.settings.mute = !sound.settings.mute;
                }
                else if (userInput.Key == ConsoleKey.D4)
                {
                    music.controls.pause();
                    result = AreYouSure();

                    if (result == 1)
                    {
                        stopGame = true;
                    }
                    else if (result == 2)
                    {
                        music.controls.play();
                        printScreen(screen);
                    }
                }

                pauseTime += DateTime.Now - pauseStart;
            }
            pastPos = headPos;
            headPos = DirectionOfMovement(headPos);

            if (stopGame)
            {
                break;
            }

            printScreen(screen);

            hitObstacle = obstacles.Contains(pastPos);
            eatSelf = body.Contains(pastPos);
            hitWall = pastPos.row > 23 || pastPos.row < 1 || pastPos.col > 68 || pastPos.col < 1;
            if (hitWall || eatSelf || hitObstacle)
            {
                sound.URL = "dead.wav";
                sound.controls.play();
                Thread.Sleep(1000);
                break;
            }

            //Thread.Sleep(99999);


            //bool isBombEat = bombEat(next, bomb);
            //if (isBombEat)
            //{
            //    break;
            //}
        }

        music.close();
        //var endGameResult = new Dictionary<string, int>();
        level = level.TrimEnd();
        speed = speed.TrimEnd();
        name = name.TrimEnd();
        if (level == "Medium")
        {
            score = (int)(score * 1.5);
        }
        else if (level == "Hard")
        {
            score = score * 2;
        }

        if (speed == "Medium")
        {
            score = (int)(score * 1.5);
        }
        else if (speed == "Fast")
        {
            score = score * 2;
        }
        //endGameResult.Add(name, score);
        //return new Dictionary<string, int>();
        AddCurrentUserScore(name, score);
        Console.In.Dispose();
        return;
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


        //Въведената от потребителя посока
        //userInput = CorrectKey();// Console.ReadKey();

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
                    correctKey.Key == ConsoleKey.UpArrow || correctKey.Key == ConsoleKey.DownArrow ||
                    correctKey.Key == ConsoleKey.D1 || correctKey.Key == ConsoleKey.D2 ||
                    correctKey.Key == ConsoleKey.D3 || correctKey.Key == ConsoleKey.D4)
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
                        screen[row, col] = String.Format("{0, -14}", "1:Start/Pause");
                    }
                    else if (row == 18)
                    {
                        screen[row, col] = String.Format("{0, -14}", "2:Music On/Off");
                    }
                    else if (row == 19)
                    {
                        screen[row, col] = String.Format("{0, -14}", "3:Sound On/Off");
                    }
                    else if (row == 20)
                    {
                        screen[row, col] = String.Format("{0, -14}", "4:Quit");
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
        Console.Clear();
        Console.Write(result);
    }

    //-------------High Scores
    public static Dictionary<string, int> GetHighScores()
    {
        string line = null;
        string path = "HighScores.txt";
        char[] delimiter = new char[] { '-'/*',', ' ', ':', '!'*/ };
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
            SaveHighScores(output);
        }

        return output;
    }

    private static void AddCurrentUserScore(string userName, int score)
    {
        var highScores = GetHighScores();

        if (highScores.ContainsKey(userName))
        {
            if (highScores[userName] < score)
            {
                highScores[userName] = score;
            }
        }
        else
        {
            highScores.Add(userName, score);
        }

        var result = new Dictionary<string, int>();
        foreach (var scr in highScores.OrderByDescending(i => i.Value))
        {
            result.Add(scr.Key, scr.Value);
        }

        if (highScores.Count > 5)
        {
            int smallestValue = int.MaxValue;
            string smallestValueKey = null;

            foreach (var scr in result)
            {
                if (scr.Value < smallestValue)
                {
                    smallestValue = scr.Value;
                    smallestValueKey = scr.Key;
                }
            }

            result.Remove(smallestValueKey);
        }

        SaveHighScores(result);
    }

    private static void SaveHighScores(Dictionary<string, int> currentScores)
    {
        StringBuilder builder = new StringBuilder();
        using (StreamWriter sw = new StreamWriter("HighScores.txt"))
        {
            foreach (var item in currentScores)
            {
                builder.Append(item.Key);
                builder.Append('-');
                builder.Append(item.Value);
                sw.WriteLine(builder.ToString());
                builder.Clear();
            }
        }
    }

    //-----------Food
    public static Position Food(Position headPos, Queue<Position> body, List<Position> food, List<Position> obstacles)
    {
        List<Position> board = new List<Position>();
        for (int row = 1; row <= 23; row++)
        {
            for (int col = 1; col <= 68; col++)
            {
                board.Add(new Position(row, col));
            }
        }

        //var a = board.RemoveAll(x => body.Contains(x) || x.Equals(headPos));

        board.Remove(headPos);
        foreach (var pos in body)
        {
            board.Remove(pos);
        }
        foreach (var pos in food)
        {
            board.Remove(pos);
        }
        foreach (var pos in obstacles)
        {
            board.Remove(pos);
        }
        //Random rnd = new Random();
        //int row = 1;
        //int col = 1;
        //Position result = new Position();

        return board[rnd.Next(0, board.Count)];

        //lock (syncLock)
        //{
        //    do
        //    {

        //        result.row = rnd.Next(1, 23);
        //        result.col = rnd.Next(1, 68);
        //    }
        //    while (result.Equals(headPos) && body.Contains(result));

        //    //Position food = new Position(row, col);
        //    return result;
        //}
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

    //-----------Obstacles
    static List<Position> GenerateObstacles(string level = "Medium")
    {
        #region MediumShapes
        bool[,] mShapeOne = { { true, true, true, true, true, true } };
        bool[,] mShapeTwo = {   {true},
                                {true},
                                {true},
                                {true},
                                {true},
                                {true}
                            };
        bool[,] mShapeThree = {
                                {true, true, true},
                                {true, true, true},
                                {true, true, true}
                            };
        #endregion
        #region HardShapes
        bool[,] hShapeOne = {
                                {true, true, true, true, true, true},
                                {true, true, true, true, true, true},
                                {true, true, true, true, true, true},
                            };
        bool[,] hShapeTwo = {
                                {true, false, false, false, false, false},
                                {true, false, false, false, false, false},
                                {true, false, false, false, false, false},
                                {true, false, false, false, false, false},
                                {true, false, false, false, false, false},
                                {true, true,  true,  true,  true,  true}
                            };

        bool[,] hShapeThree = {
                                {false, false, false, false, false, true},
                                {false, false, false, false, false, true},
                                {false, false, false, false, false, true},
                                {false, false, false, false, false, true},
                                {false, false, false, false, false, true},
                                {true,  true,  true,  true,  true,  true}
                            };

        bool[,] hShapeFour = {
                                {true, true, true, true, true, true, true},
                                {false, false, false, true, false, false, false},
                                {false, false, false, true, false, false, false},
                                {false, false, false, true, false, false, false},
                                {false, false, false, true, false, false, false},
                                {false, false, false, true, false, false, false},
                                {false, false, false, true, false, false, false}
                            };

        bool[,] hShapeFive = {
                                {false, false, false, true, false, false, false},
                                {false, false, false, true, false, false, false},
                                {false, false, false, true, false, false, false},
                                {true, true, true, true, true, true, true},
                                {false, false, false, true, false, false, false},
                                {false, false, false, true, false, false, false},
                                {false, false, false, true, false, false, false}
                            };
        #endregion

        int choose = 0;
        var result = new List<Position>();
        List<Position> positions = new List<Position>();
        if (level == "Medium")
        {
            for (int i = 0; i < 3; i++)
            {
                choose = rnd.Next(1, 4);
                if (choose == 1)
                {
                    positions.Clear();
                    for (int row = 1; row <= 23; row++)
                    {
                        for (int col = 2; col <= 63; col++)
                        {
                            if (IsPositionAvaliable(row, col, mShapeOne, result))
                            {
                                positions.Add(new Position(row, col));
                            }

                        }
                    }

                    choose = rnd.Next(0, positions.Count);

                    for (int row = 0; row < mShapeOne.GetLength(0); row++)
                    {
                        for (int col = 0; col < mShapeOne.GetLength(1); col++)
                        {
                            if (mShapeOne[row, col] == true)
                            {
                                result.Add(new Position(positions[choose].row + row, positions[choose].col + col));
                            }
                        }
                    }

                }
                else if (choose == 2)
                {
                    positions.Clear();
                    for (int row = 1; row <= 18; row++)
                    {
                        for (int col = 2; col <= 68; col++)
                        {
                            if (IsPositionAvaliable(row, col, mShapeTwo, result))
                            {
                                positions.Add(new Position(row, col));
                            }
                        }
                    }

                    choose = rnd.Next(0, positions.Count);

                    for (int row = 0; row < mShapeTwo.GetLength(0); row++)
                    {
                        for (int col = 0; col < mShapeTwo.GetLength(1); col++)
                        {
                            if (mShapeTwo[row, col] == true)
                            {
                                result.Add(new Position(positions[choose].row + row, positions[choose].col + col));
                            }
                        }
                    }
                }
                else if (choose == 3)
                {
                    positions.Clear();
                    for (int row = 1; row <= 21; row++)
                    {
                        for (int col = 2; col <= 66; col++)
                        {
                            if (IsPositionAvaliable(row, col, mShapeThree, result))
                            {
                                positions.Add(new Position(row, col));
                            }
                        }
                    }

                    choose = rnd.Next(0, positions.Count);

                    for (int row = 0; row < mShapeThree.GetLength(0); row++)
                    {
                        for (int col = 0; col < mShapeThree.GetLength(1); col++)
                        {
                            if (mShapeThree[row, col] == true)
                            {
                                result.Add(new Position(positions[choose].row + row, positions[choose].col + col));
                            }
                        }
                    }
                }
            }

        }
        else if (level == "Hard")
        {
            for (int i = 0; i < 4; i++)
            {
                choose = rnd.Next(1, 6);
                if (choose == 1)
                {
                    positions.Clear();
                    for (int row = 1; row <= 21; row++)
                    {
                        for (int col = 2; col <= 63; col++)
                        {
                            if (IsPositionAvaliable(row, col, hShapeOne, result))
                            {
                                positions.Add(new Position(row, col));
                            }

                        }
                    }

                    choose = rnd.Next(0, positions.Count);

                    for (int row = 0; row < hShapeOne.GetLength(0); row++)
                    {
                        for (int col = 0; col < hShapeOne.GetLength(1); col++)
                        {
                            if (hShapeOne[row, col] == true)
                            {
                                result.Add(new Position(positions[choose].row + row, positions[choose].col + col));
                            }
                        }
                    }
                }
                else if (choose == 2)
                {
                    positions.Clear();
                    for (int row = 1; row <= 18; row++)
                    {
                        for (int col = 2; col <= 63; col++)
                        {
                            if (IsPositionAvaliable(row, col, hShapeTwo, result))
                            {
                                positions.Add(new Position(row, col));
                            }

                        }
                    }

                    choose = rnd.Next(0, positions.Count);

                    for (int row = 0; row < hShapeTwo.GetLength(0); row++)
                    {
                        for (int col = 0; col < hShapeTwo.GetLength(1); col++)
                        {
                            if (hShapeTwo[row, col] == true)
                            {
                                result.Add(new Position(positions[choose].row + row, positions[choose].col + col));
                            }
                        }
                    }
                }
                else if (choose == 3)
                {
                    positions.Clear();
                    for (int row = 1; row <= 18; row++)
                    {
                        for (int col = 2; col <= 63; col++)
                        {
                            if (IsPositionAvaliable(row, col, hShapeThree, result))
                            {
                                positions.Add(new Position(row, col));
                            }

                        }
                    }

                    choose = rnd.Next(0, positions.Count);

                    for (int row = 0; row < hShapeThree.GetLength(0); row++)
                    {
                        for (int col = 0; col < hShapeThree.GetLength(1); col++)
                        {
                            if (hShapeThree[row, col] == true)
                            {
                                result.Add(new Position(positions[choose].row + row, positions[choose].col + col));
                            }
                        }
                    }
                }
                else if (choose == 4)
                {
                    positions.Clear();
                    for (int row = 1; row <= 17; row++)
                    {
                        for (int col = 2; col <= 62; col++)
                        {
                            if (IsPositionAvaliable(row, col, hShapeFour, result))
                            {
                                positions.Add(new Position(row, col));
                            }

                        }
                    }

                    choose = rnd.Next(0, positions.Count);

                    for (int row = 0; row < hShapeFour.GetLength(0); row++)
                    {
                        for (int col = 0; col < hShapeFour.GetLength(1); col++)
                        {
                            if (hShapeFour[row, col] == true)
                            {
                                result.Add(new Position(positions[choose].row + row, positions[choose].col + col));
                            }
                        }
                    }
                }
                else if (choose == 5)
                {
                    positions.Clear();
                    for (int row = 1; row <= 17; row++)
                    {
                        for (int col = 2; col <= 62; col++)
                        {
                            if (IsPositionAvaliable(row, col, hShapeFive, result))
                            {
                                positions.Add(new Position(row, col));
                            }

                        }
                    }

                    choose = rnd.Next(0, positions.Count);

                    for (int row = 0; row < hShapeFive.GetLength(0); row++)
                    {
                        for (int col = 0; col < hShapeFive.GetLength(1); col++)
                        {
                            if (hShapeFive[row, col] == true)
                            {
                                result.Add(new Position(positions[choose].row + row, positions[choose].col + col));
                            }
                        }
                    }
                }
            }
        }

        return result;
    }
    static bool IsPositionAvaliable(int row, int col, bool[,] shape, List<Position> unavaliables)
    {
        for (int rowAdd = 0; rowAdd < shape.GetLength(0); rowAdd++)
        {
            for (int colAdd = 0; colAdd < shape.GetLength(1); colAdd++)
            {
                if (shape[rowAdd, colAdd] == true)
                {
                    if (unavaliables.Contains(new Position(row + rowAdd, col + colAdd)))
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }
    #region bombs
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
    static void PrintMenu(string menuSelect)
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

        if (menuSelect == "Main")
        {
            menu.Append(whiteSpace);
            menu.Append("1. Play New Game\n");
            menu.Append(whiteSpace);
            menu.Append("2. High Scores\n");
            menu.Append(whiteSpace);
            menu.Append("3. Music On/Off\n");
            menu.Append(whiteSpace);
            menu.Append("4. Manual\n");
            menu.Append(whiteSpace);
            menu.Append("5. Exit\n");
        }
        else if (menuSelect == "NameSelect")
        {
            menu.Append(new string(' ', 28));
            menu.Append("Input name: ");
        }
        else if (menuSelect == "LevelSelect")
        {
            menu.Append(whiteSpace);
            menu.Append("Level:\n");
            menu.Append(whiteSpace);
            menu.Append("1. Easy\n");
            menu.Append(whiteSpace);
            menu.Append("2. Medium\n");
            menu.Append(whiteSpace);
            menu.Append("3. Hard\n");
        }
        else if (menuSelect == "SpeedSelect")
        {
            menu.Append(whiteSpace);
            menu.Append("Speed:\n");
            menu.Append(whiteSpace);
            menu.Append("1. Slow\n");
            menu.Append(whiteSpace);
            menu.Append("2. Medium\n");
            menu.Append(whiteSpace);
            menu.Append("3. Fast\n");
        }
        else if (menuSelect == "HighScores")
        {
            whiteSpace = new string(' ', 30);
            Dictionary<string, int> scores = GetHighScores();
            int count = 1;

            foreach (var score in scores)
            {
                menu.Append(whiteSpace);
                menu.Append(String.Format("{0}. {1}: {2}\n", count, score.Key, score.Value));
                count++;
            }
            menu.Append(whiteSpace);
            menu.Append("Escape: Go Back\n");
        }
        else if (menuSelect == "Manual")
        {
            whiteSpace = new string(' ', 24);
            menu.Append(whiteSpace);
            menu.Append("The Basilisk: ☺♦♦♦♦\n");
            menu.Append(whiteSpace);
            menu.Append("Normal food: ■ (5 points)\n");
            menu.Append(whiteSpace);
            menu.Append("Food of Clubs: ♣ (10 points, 10 seconds)\n");
            menu.Append(whiteSpace);
            menu.Append("Food of Hearts: ♥(15 points, 7 seconds)\n");
            menu.Append(whiteSpace);
            menu.Append("Food of Spades: ♠(25 points, 5 seconds)\n");
            menu.Append(whiteSpace);
            menu.Append("Obstacles: ¤\n");
            menu.Append(whiteSpace);
            menu.Append("Difficulty: Easy - No obstacles, 1.0 x Score\n");
            menu.Append(whiteSpace);
            menu.Append("Difficulty: Medium - Small obstacles, 1.5 x Score\n");
            menu.Append(whiteSpace);
            menu.Append("Difficulty: Hard - Big obstacles, 2.0 x Score\n");
            menu.Append(whiteSpace);
            menu.Append("Speed: Slow - 1.0 x Score\n");
            menu.Append(whiteSpace);
            menu.Append("Speed: Medium - 1.5 x Score\n");
            menu.Append(whiteSpace);
            menu.Append("Speed: Fast - 2.0 x Score\n");
            menu.Append(whiteSpace);
            menu.Append("Escape: Go Back\n");
        }
        else if (menuSelect == "AreYouSure")
        {
            menu.Append(whiteSpace);
            menu.Append("Are you sure?\n");
            menu.Append(whiteSpace);
            menu.Append("1. Yes\n");
            menu.Append(whiteSpace);
            menu.Append("2. No\n");
        }
        else if (menuSelect == "GameOver")
        {
            string game = @"                  ▄██████▄     ▄████████   ▄▄▄▄███▄▄▄▄      ▄████████ 
                 ███    ███   ███    ███ ▄██▀▀▀███▀▀▀██▄   ███    ███ 
                 ███    █▀    ███    ███ ███   ███   ███   ███    █▀  
                ▄███          ███    ███ ███   ███   ███  ▄███▄▄▄     
               ▀▀███ ████▄  ▀███████████ ███   ███   ███ ▀▀███▀▀▀     
                 ███    ███   ███    ███ ███   ███   ███   ███    █▄  
                 ███    ███   ███    ███ ███   ███   ███   ███    ███ 
                 ████████▀    ███    █▀   ▀█   ███   █▀    ██████████ 
                                                                      ";
            string over = @"                     ▄██████▄   ▄█    █▄     ▄████████    ▄████████ 
                    ███    ███ ███    ███   ███    ███   ███    ███ 
                    ███    ███ ███    ███   ███    █▀    ███    ███ 
                    ███    ███ ███    ███  ▄███▄▄▄      ▄███▄▄▄▄██▀ 
                    ███    ███ ███    ███ ▀▀███▀▀▀     ▀▀███▀▀▀▀▀   
                    ███    ███ ███    ███   ███    █▄  ▀███████████ 
                    ███    ███ ███    ███   ███    ███   ███    ███ 
                     ▀██████▀   ▀██████▀    ██████████   ███    ███ 
                                                         ███    ███ ";

            menu.Clear();
            menu.Append(game);
            menu.Append("\n\n");
            menu.Append(over);
        }

        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.Write(menu);
        Console.ForegroundColor = ConsoleColor.Gray;
    }
    private static int MainMenu()
    {
        Console.Clear();
        PrintMenu("Main");

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
            else if (userInput.Key == ConsoleKey.D5)
            {
                Console.Clear();
                return 5;
            }
            else
            {
                Console.Clear();
                PrintMenu("Main");
            }
        }
    }
    private static string InputName()
    {
        Console.Clear();
        PrintMenu("NameSelect");
        Console.ForegroundColor = ConsoleColor.DarkRed;
        string name = Console.ReadLine();
        Console.ForegroundColor = ConsoleColor.Gray;

        name = name.Trim();
        if (name.Length > 14)
        {
            name = name.Substring(0, 14);
        }

        if (String.IsNullOrEmpty(name))
        {
            name = "Player";
        }

        return name;
    }
    private static string LevelSelect()
    {
        Console.Clear();
        PrintMenu("LevelSelect");

        while (true)
        {
            userInput = Console.ReadKey();

            if (userInput.Key == ConsoleKey.D1)
            {
                Console.Clear();
                return "Easy";
            }
            else if (userInput.Key == ConsoleKey.D2)
            {
                Console.Clear();
                return "Medium";
            }
            else if (userInput.Key == ConsoleKey.D3)
            {
                Console.Clear();
                return "Hard";
            }
            else
            {
                Console.Clear();
                PrintMenu("LevelSelect");
            }
        }
    }
    private static string SpeedSelect()
    {
        Console.Clear();
        PrintMenu("SpeedSelect");

        while (true)
        {
            userInput = Console.ReadKey();

            if (userInput.Key == ConsoleKey.D1)
            {
                Console.Clear();
                return "Slow";
            }
            else if (userInput.Key == ConsoleKey.D2)
            {
                Console.Clear();
                return "Medium";
            }
            else if (userInput.Key == ConsoleKey.D3)
            {
                Console.Clear();
                return "Fast";
            }
            else
            {
                Console.Clear();
                PrintMenu("SpeedSelect");
            }
        }
    }
    private static void HighScores()
    {
        Console.Clear();
        PrintMenu("HighScores");

        while (true)
        {
            userInput = Console.ReadKey();

            if (userInput.Key == ConsoleKey.Escape)
            {
                return;
            }
            else
            {
                Console.Clear();
                PrintMenu("HighScores");
            }
        }
    }
    private static void Manual()
    {
        Console.Clear();
        PrintMenu("Manual");

        while (true)
        {
            userInput = Console.ReadKey();

            if (userInput.Key == ConsoleKey.Escape)
            {
                return;
            }
            else
            {
                Console.Clear();
                PrintMenu("Manual");
            }
        }
    }
    private static int AreYouSure()
    {
        Console.Clear();
        PrintMenu("AreYouSure");

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
            else
            {
                Console.Clear();
                PrintMenu("AreYouSure");
            }
        }
    }
    private static void GameOver()
    {
        Console.Clear();
        PrintMenu("GameOver");

        Thread.Sleep(2000);

        return;
    }
}