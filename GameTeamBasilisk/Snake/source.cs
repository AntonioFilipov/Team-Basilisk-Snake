using System;

class Snake
{
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

    static void Main()
    {
        char[,] screen = new char[25,75];

        int[] headPos = { 10, 40 };

        for (int row = 0; row < screen.GetLength(0); row++)
        {
            for (int col = 0; col < screen.GetLength(1); col++)
            {
                screen[row, col] = '.';
            }
        }

        printScreen(screen);
    }
}