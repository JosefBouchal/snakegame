using System;
using System.Collections.Generic;
using System.Threading;

namespace Snake
{
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            game.Run();
        }
    }

    class Game
    {
        private int windowWidth = 32;
        private int windowHeight = 16;
        private Random random = new Random();
        public int Score { get; private set; } = 5;
        private bool isGameOver = false;
        private Snake snake;
        private Food food;
        private DateTime startTime;
        private DateTime currentTime;
        public bool DirectionChanged { get; set; } = false;

        public Game()
        {
            Console.WindowHeight = windowHeight;
            Console.WindowWidth = windowWidth;
            snake = new Snake(windowWidth / 2, windowHeight / 2, this);
            food = new Food(random.Next(1, windowWidth - 2), random.Next(1, windowHeight - 2));
        }

        public void Run()
        {
            while (!isGameOver)
            {
                Console.Clear();
                DrawBorders();
                CheckCollision();
                snake.Draw();
                food.Draw();
                HandleInput();
                snake.Move();
            }
            EndGame();
        }

        private void DrawBorders()
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

        private void CheckCollision()
        {
            if (snake.CheckCollision(windowWidth, windowHeight))
                isGameOver = true;

            if (food.IsEaten(snake.HeadX, snake.HeadY))
            {
                Score++;
                food.Respawn(random, windowWidth, windowHeight);
                snake.Grow();
            }
        }

        private void HandleInput()
        {
            startTime = DateTime.Now;
            DirectionChanged = false;
            while (true)
            {
                currentTime = DateTime.Now;
                if ((currentTime - startTime).TotalMilliseconds > 500) break;
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    snake.ChangeDirection(keyInfo);
                }
            }
        }

        private void EndGame()
        {
            Console.SetCursorPosition(windowWidth / 5, windowHeight / 2);
            Console.WriteLine("Game over, Score: " + Score);
            Console.SetCursorPosition(windowWidth / 5, windowHeight / 2 + 1);
        }
    }

    class Snake
    {
        private List<int> bodyX = new List<int>();
        private List<int> bodyY = new List<int>();
        private string currentDirection = "RIGHT";
        public int HeadX { get; private set; }
        public int HeadY { get; private set; }
        private ConsoleColor color = ConsoleColor.Red;
        private Game game;

        public Snake(int startX, int startY, Game gameInstance)
        {
            HeadX = startX;
            HeadY = startY;
            game = gameInstance;
        }

        public void Draw()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            for (int i = 0; i < bodyX.Count; i++)
            {
                Console.SetCursorPosition(bodyX[i], bodyY[i]);
                Console.Write("■");
            }
            Console.SetCursorPosition(HeadX, HeadY);
            Console.ForegroundColor = color;
            Console.Write("■");
        }

        public void Move()
        {
            bodyX.Add(HeadX);
            bodyY.Add(HeadY);
            switch (currentDirection)
            {
                case "UP":
                    HeadY--;
                    break;
                case "DOWN":
                    HeadY++;
                    break;
                case "LEFT":
                    HeadX--;
                    break;
                case "RIGHT":
                    HeadX++;
                    break;
            }
            if (bodyX.Count > game.Score)
            {
                bodyX.RemoveAt(0);
                bodyY.RemoveAt(0);
            }
        }

        public void ChangeDirection(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key == ConsoleKey.UpArrow && currentDirection != "DOWN" && !game.DirectionChanged)
            {
                currentDirection = "UP";
                game.DirectionChanged = true;
            }
            if (keyInfo.Key == ConsoleKey.DownArrow && currentDirection != "UP" && !game.DirectionChanged)
            {
                currentDirection = "DOWN";
                game.DirectionChanged = true;
            }
            if (keyInfo.Key == ConsoleKey.LeftArrow && currentDirection != "RIGHT" && !game.DirectionChanged)
            {
                currentDirection = "LEFT";
                game.DirectionChanged = true;
            }
            if (keyInfo.Key == ConsoleKey.RightArrow && currentDirection != "LEFT" && !game.DirectionChanged)
            {
                currentDirection = "RIGHT";
                game.DirectionChanged = true;
            }
        }

        public bool CheckCollision(int width, int height)
        {
            if (HeadX == width - 1 || HeadX == 0 ||
                HeadY == height - 1 || HeadY == 0)
                return true;
            for (int i = 0; i < bodyX.Count; i++)
            {
                if (bodyX[i] == HeadX && bodyY[i] == HeadY)
                    return true;
            }
            return false;
        }

        public void Grow()
        {
            bodyX.Insert(0, HeadX);
            bodyY.Insert(0, HeadY);
        }
    }

    class Food
    {
        private int x;
        private int y;

        public Food(int startX, int startY)
        {
            x = startX;
            y = startY;
        }

        public void Draw()
        {
            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("■");
        }

        public bool IsEaten(int snakeX, int snakeY)
        {
            return x == snakeX && y == snakeY;
        }

        public void Respawn(Random random, int width, int height)
        {
            x = random.Next(1, width - 2);
            y = random.Next(1, height - 2);
        }
    }
}
