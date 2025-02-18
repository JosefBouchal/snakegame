using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
//█ ■
///https://www.youtube.com/watch?v=SGZgvMwjq2U
namespace Snake
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WindowHeight = 16;
            Console.WindowWidth = 32;
            int windowWidth = Console.WindowWidth;
            int windowHeight = Console.WindowHeight;
            Random random = new Random();
            int score = 5;
            bool isGameOver = false;
            Pixel snakeHead = new Pixel();
            snakeHead.X = windowWidth / 2;
            snakeHead.Y = windowHeight / 2;
            snakeHead.Color = ConsoleColor.Red;
            string currentDirection = "RIGHT";
            List<int> snakeBodyX = new List<int>();
            List<int> snakeBodyY = new List<int>();
            int foodX = random.Next(0, windowWidth);
            int foodY = random.Next(0, windowHeight);
            DateTime startTime = DateTime.Now;
            DateTime currentTime = DateTime.Now;
            bool directionChanged = false;
            while (true)
            {
                Console.Clear();
                // Check collision with borders
                if (snakeHead.X == windowWidth - 1 || snakeHead.X == 0 ||
                    snakeHead.Y == windowHeight - 1 || snakeHead.Y == 0)
                {
                    isGameOver = true;
                }
                // Draw top border
                for (int i = 0; i < windowWidth; i++)
                {
                    Console.SetCursorPosition(i, 0);
                    Console.Write("■");
                }
                // Draw bottom border
                for (int i = 0; i < windowWidth; i++)
                {
                    Console.SetCursorPosition(i, windowHeight - 1);
                    Console.Write("■");
                }
                // Draw left border
                for (int i = 0; i < windowHeight; i++)
                {
                    Console.SetCursorPosition(0, i);
                    Console.Write("■");
                }
                // Draw right border
                for (int i = 0; i < windowHeight; i++)
                {
                    Console.SetCursorPosition(windowWidth - 1, i);
                    Console.Write("■");
                }
                // Check if snake eats the food
                Console.ForegroundColor = ConsoleColor.Green;
                if (foodX == snakeHead.X && foodY == snakeHead.Y)
                {
                    score++;
                    foodX = random.Next(1, windowWidth - 2);
                    foodY = random.Next(1, windowHeight - 2);
                }
                // Draw snake body
                for (int i = 0; i < snakeBodyX.Count; i++)
                {
                    Console.SetCursorPosition(snakeBodyX[i], snakeBodyY[i]);
                    Console.Write("■");
                    // Check collision with snake body
                    if (snakeBodyX[i] == snakeHead.X && snakeBodyY[i] == snakeHead.Y)
                    {
                        isGameOver = true;
                    }
                }
                if (isGameOver)
                {
                    break;
                }
                // Draw snake head
                Console.SetCursorPosition(snakeHead.X, snakeHead.Y);
                Console.ForegroundColor = snakeHead.Color;
                Console.Write("■");
                // Draw food
                Console.SetCursorPosition(foodX, foodY);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("■");

                // Handle input and delay movement
                startTime = DateTime.Now;
                directionChanged = false;
                while (true)
                {
                    currentTime = DateTime.Now;
                    if (currentTime.Subtract(startTime).TotalMilliseconds > 500) { break; }
                    if (Console.KeyAvailable)
                    {
                        ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                        // Update direction if a valid key is pressed and direction hasn't changed yet
                        if (keyInfo.Key.Equals(ConsoleKey.UpArrow) && currentDirection != "DOWN" && !directionChanged)
                        {
                            currentDirection = "UP";
                            directionChanged = true;
                        }
                        if (keyInfo.Key.Equals(ConsoleKey.DownArrow) && currentDirection != "UP" && !directionChanged)
                        {
                            currentDirection = "DOWN";
                            directionChanged = true;
                        }
                        if (keyInfo.Key.Equals(ConsoleKey.LeftArrow) && currentDirection != "RIGHT" && !directionChanged)
                        {
                            currentDirection = "LEFT";
                            directionChanged = true;
                        }
                        if (keyInfo.Key.Equals(ConsoleKey.RightArrow) && currentDirection != "LEFT" && !directionChanged)
                        {
                            currentDirection = "RIGHT";
                            directionChanged = true;
                        }
                    }
                }
                // Add current head position to body lists
                snakeBodyX.Add(snakeHead.X);
                snakeBodyY.Add(snakeHead.Y);
                // Update snake head position based on direction
                switch (currentDirection)
                {
                    case "UP":
                        snakeHead.Y--;
                        break;
                    case "DOWN":
                        snakeHead.Y++;
                        break;
                    case "LEFT":
                        snakeHead.X--;
                        break;
                    case "RIGHT":
                        snakeHead.X++;
                        break;
                }
                // Remove tail if the snake is longer than the score
                if (snakeBodyX.Count > score)
                {
                    snakeBodyX.RemoveAt(0);
                    snakeBodyY.RemoveAt(0);
                }
            }
            Console.SetCursorPosition(windowWidth / 5, windowHeight / 2);
            Console.WriteLine("Game over, Score: " + score);
            Console.SetCursorPosition(windowWidth / 5, windowHeight / 2 + 1);
        }
        class Pixel
        {
            public int X { get; set; }
            public int Y { get; set; }
            public ConsoleColor Color { get; set; }
        }
    }
}