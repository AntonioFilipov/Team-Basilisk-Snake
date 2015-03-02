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

        //Console.Clear();
        //PrintHighScores();
        //Thread.Sleep(9999999);

        int result;
        while (true)
        {
            result = MainMenu();
            if (result == 1)
            {
                direction = 3;
                PlayGame(InputName(), SpeedSelect(), LevelSelect());
            }
            else if (result == 2)
            {
                HighScores();
            }
        }
    }

    //--------------Game
    static Dictionary<string, int> PlayGame(string name = "Player", string speed = "Medium", string level = "Medium")
    {
        //var bomb = new Dictionary<int, List<int>>();
        //bomb.Add(foodPosition.row, new List<int>());
        //bomb[obstaclePosition.row].Add(obstaclePosition.col);

        WindowsMediaPlayer music = new WindowsMediaPlayer();
        WindowsMediaPlayer sound = new WindowsMediaPlayer();
        string face = "☺";//((char)1).ToString();
        string bodyChar = "♦";//((char)4).ToString();
        string obstacleChar = "¤";
        
        string[,] screen = InitializeScreen(new  Position(12, 30));
        List<Position> obstacles = GenerateObstacles(level);

        int score = 0;
        DateTime startTime = DateTime.Now;
        TimeSpan time = new TimeSpan(); 
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
        int rndResult = 0;

        TimeSpan loop = new TimeSpan(0,1,37);
        music.URL = "inGame.wav";
        music.settings.volume = 40;
        sound.settings.volume = 85;
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

                rndResult = rnd.Next(1, 3);
                if (rndResult == 1 && !foodOfClubs)
                {
                    timeClubs = DateTime.Now;
                    posClubs = Food(headPos, body, foodPositions, obstacles);
                    foodOfClubs = true;
                }

                rndResult = rnd.Next(1, 4);
                if (rndResult == 1 && !foodOfHearts)
                {
                    timeHearts = DateTime.Now;
                    posHearts = Food(headPos, body, foodPositions, obstacles);
                    foodOfHearts = true;
                }

                rndResult = rnd.Next(1, 5);
                if (rndResult == 1 && !foodOfSpades)
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
                score += 20;
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
                if (time.Seconds >= 8)
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
                if (time.Seconds >= 6)
                {
                    foodOfSpades = false;
                }
                else
                {
                    screen[posSpades.row, posSpades.col] = "♠";
                } 
            }

            screen[scorePos.row, scorePos.col] = String.Format(placeHolder, score);

            time = DateTime.Now - startTime;
            screen[timePos.row, timePos.col] = String.Format(placeHolder, (String.Format(timeHolder, time.Hours, time.Minutes, time.Seconds)));

            if (time >= loop)
            {
                if (time.Seconds % loop.Seconds == 0)
                {
                    music.controls.stop();
                    music.controls.play();
                }
            }

            screen[5, valuesCol] = name;
            screen[8, valuesCol] = speed;
            screen[11, valuesCol] = level;

            Thread.Sleep(speedTime);
            Console.Clear();
            printScreen(screen);

            hitObstacle = obstacles.Contains(headPos);
            eatSelf = body.Contains(headPos);
            hitWall = headPos.row > 23 || headPos.row < 1 || headPos.col > 68 || headPos.col < 1;
            if (hitWall || eatSelf || hitObstacle)
            {
                sound.URL = "dead.wav";
                sound.controls.play();
                Thread.Sleep(1000);
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

        music.controls.stop();
        var result = new Dictionary<string, int>();
        result.Add(name, score);
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
    public static Position Food(Position headPos, Queue<Position> body, List<Position> food, List<Position>obstacles)
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
        bool[,] mShapeOne = {{true, true, true, true, true, true }};
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
            for (int i = 0; i < 3; i++)
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
    static bool IsPositionAvaliable(int row, int col, bool[,] shape, List<Position>unavaliables)
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

    //public static bool bombEat(Position nextPosition, Dictionary<int, List<int>> bombPositions)
    //{
    //    bool isBombEat = false;
    //    if (bombPositions.ContainsKey(nextPosition.row) && bombPositions[nextPosition.row].Contains(nextPosition.col))
    //    {
    //        isBombEat = true;
    //    }

    //    return isBombEat;
    //}
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
            menu.Append("3. Mute Music\n");
            menu.Append(whiteSpace);
            menu.Append("4. Exit\n");
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
            Dictionary<string, int> scores = GetHighScores();
            int count = 1;

            foreach (var score in scores)
            {
                menu.Append(new string(' ', 28));
                menu.Append(String.Format("{0}. {1}: {2}\n", count, score.Key, score.Value));
                count++;
            }
            menu.Append(new string(' ', 28));
            menu.Append("Escape: Go Back\n");
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