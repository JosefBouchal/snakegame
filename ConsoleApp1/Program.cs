using System;
using System.Collections.Generic;
using System.Threading;

namespace Snake
{
    class Program
    {
        static int windowWidth = 32;
        static int windowHeight = 16;
        static Random random = new Random();
        static int score = 5;
        static bool isGameOver = false;
        static Pixel snakeHead = new Pixel();
        static string currentDirection = "RIGHT";
        static List<int> snakeBodyX = new List<int>();
        static List<int> snakeBodyY = new List<int>();
        static int foodX, foodY;
        static bool directionChanged;
        static DateTime startTime;
        static DateTime currentTime;

        static void Main(string[] args)
        {
            InitializeGame();
            while (!isGameOver)
            {
                Console.Clear();
                DrawBorders();
                CheckCollision();
                DrawSnake();
                DrawFood();
                HandleInput();
                UpdateSnakePosition();
            }
            EndGame();
        }

        static void InitializeGame()
        {
            Console.Clear();
            Console.WindowHeight = windowHeight;
            Console.WindowWidth = windowWidth;
            snakeHead.X = windowWidth / 2;
            snakeHead.Y = windowHeight / 2;
            snakeHead.Color = ConsoleColor.Red;
            foodX = random.Next(1, windowWidth - 2);
            foodY = random.Next(1, windowHeight - 2);
        }

        static void DrawBorders()
        {
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
        }

        static void CheckCollision()
        {
            // Check collision with borders
            if (snakeHead.X == windowWidth - 1 || snakeHead.X == 0 ||
                snakeHead.Y == windowHeight - 1 || snakeHead.Y == 0)
            {
                isGameOver = true;
            }
            for (int i = 0; i < snakeBodyX.Count; i++)
            {
                // Check collision with snake body
                if (snakeBodyX[i] == snakeHead.X && snakeBodyY[i] == snakeHead.Y)
                {
                    isGameOver = true;
                }
            }
            // Check if snake eats the food
            if (foodX == snakeHead.X && foodY == snakeHead.Y)
            {
                score++;
                foodX = random.Next(1, windowWidth - 2);
                foodY = random.Next(1, windowHeight - 2);
            }
        }

        static void DrawSnake()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            // Draw snake body
            for (int i = 0; i < snakeBodyX.Count; i++)
            {
                Console.SetCursorPosition(snakeBodyX[i], snakeBodyY[i]);
                Console.Write("■");
            }
            // Draw snake head
            Console.SetCursorPosition(snakeHead.X, snakeHead.Y);
            Console.ForegroundColor = snakeHead.Color;
            Console.Write("■");
        }

        static void DrawFood()
        {
            Console.SetCursorPosition(foodX, foodY);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("■");
        }

        static void HandleInput()
        {
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
                    ChangeDirection(keyInfo);
                }
            }
        }

        static void ChangeDirection(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key == ConsoleKey.UpArrow && currentDirection != "DOWN" && !directionChanged)
            {
                currentDirection = "UP";
                directionChanged = true;
            }
            if (keyInfo.Key == ConsoleKey.DownArrow && currentDirection != "UP" && !directionChanged)
            {
                currentDirection = "DOWN";
                directionChanged = true;
            }
            if (keyInfo.Key == ConsoleKey.LeftArrow && currentDirection != "RIGHT" && !directionChanged)
            {
                currentDirection = "LEFT";
                directionChanged = true;
            }
            if (keyInfo.Key == ConsoleKey.RightArrow && currentDirection != "LEFT" && !directionChanged)
            {
                currentDirection = "RIGHT";
                directionChanged = true;
            }
        }

        static void UpdateSnakePosition()
        {
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

        static void EndGame()
        {
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
