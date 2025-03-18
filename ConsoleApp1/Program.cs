using System;
using System.Collections.Generic;
using System.Threading;

namespace Snake
{
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game(40, 20); // Snadná změ na velikosti
            game.Run();
        }
    }

    enum Direction { Up, Down, Left, Right }

    struct Position
    {
        public int X { get; }
        public int Y { get; }

        public Position(int x, int y) { X = x; Y = y; }
    }

    interface IDrawable
    {
        void Draw();
    }

    class Game
    {
        private int windowWidth;
        private int windowHeight;
        private Random random = new Random();
        public int Score { get; private set; } = 5;
        private bool isGameOver = false;
        private List<Snake> snakes = new List<Snake>();
        private List<FoodBase> foodItems = new List<FoodBase>();

        public Game(int width, int height)
        {
            windowWidth = width;
            windowHeight = height;
            Console.WindowHeight = windowHeight;
            Console.WindowWidth = windowWidth;

            // Přidání jednoho nebo více hadů;
            snakes.Add(new Snake(windowWidth / 2, windowHeight / 2, this));


            // Přidání různých typů jídla
            foodItems.Add(new NormalFood(random, windowWidth, windowHeight));
            
        }

        public void Run()
        {
            while (!isGameOver)
            {
                Console.Clear();
                DrawBorders();
                CheckCollision();
                snakes.ForEach(snake => snake.Draw());
                foodItems.ForEach(food => food.Draw());
                HandleInput();
                snakes.ForEach(snake => snake.Move());
            }
            EndGame();
        }

        private void DrawBorders()
        {
            for (int i = 0; i < windowWidth; i++)
            {
                Console.SetCursorPosition(i, 0);
                Console.Write("■");
                Console.SetCursorPosition(i, windowHeight - 1);
                Console.Write("■");
            }
            for (int i = 0; i < windowHeight; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write("■");
                Console.SetCursorPosition(windowWidth - 1, i);
                Console.Write("■");
            }
        }

        private void CheckCollision()
        {
            foreach (var snake in snakes)
            {
                if (snake.CheckCollision(windowWidth, windowHeight))
                    isGameOver = true;

                foreach (var food in foodItems)
                {
                    if (food.IsEaten(snake.Head))
                    {
                        Score += food.Points;
                        food.Respawn(random, windowWidth, windowHeight);
                        snake.Grow();
                    }
                }
            }
        }

        private void HandleInput()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                foreach (var snake in snakes)
                {
                    snake.ChangeDirection(keyInfo);
                }
            }
            Thread.Sleep(400); // Rychlost hry
        }

        private void EndGame()
        {
            Console.SetCursorPosition(windowWidth / 5, windowHeight / 2);
            Console.WriteLine("Game over, Score: " + Score);
        }
    }

    class Snake : IDrawable
    {
        private List<Position> body = new List<Position>();
        public Position Head => body[^1];
        private Direction currentDirection = Direction.Right;
        private Game game;

        public Snake(int startX, int startY, Game gameInstance)
        {
            body.Add(new Position(startX, startY));
            game = gameInstance;
        }

        public void Draw()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            foreach (var segment in body)
            {
                Console.SetCursorPosition(segment.X, segment.Y);
                Console.Write("■");
            }
        }

        public void Move()
        {
            Position newHead = currentDirection switch
            {
                Direction.Up => new Position(Head.X, Head.Y - 1),
                Direction.Down => new Position(Head.X, Head.Y + 1),
                Direction.Left => new Position(Head.X - 1, Head.Y),
                Direction.Right => new Position(Head.X + 1, Head.Y),
                _ => Head
            };

            body.Add(newHead);
            if (body.Count > game.Score)
                body.RemoveAt(0);
        }

        public void ChangeDirection(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key == ConsoleKey.UpArrow && currentDirection != Direction.Down)
                currentDirection = Direction.Up;
            if (keyInfo.Key == ConsoleKey.DownArrow && currentDirection != Direction.Up)
                currentDirection = Direction.Down;
            if (keyInfo.Key == ConsoleKey.LeftArrow && currentDirection != Direction.Right)
                currentDirection = Direction.Left;
            if (keyInfo.Key == ConsoleKey.RightArrow && currentDirection != Direction.Left)
                currentDirection = Direction.Right;
        }

        public bool CheckCollision(int width, int height)
        {
            return Head.X == 0 || Head.X == width - 1 || Head.Y == 0 || Head.Y == height - 1;
        }

        public void Grow()
        {
            body.Insert(0, Head);
        }
    }

    abstract class FoodBase : IDrawable
    {
        protected Position position;
        public int Points { get; protected set; }
        protected ConsoleColor color;

        public FoodBase(Random random, int width, int height, int points, ConsoleColor color)
        {
            this.color = color;
            Points = points;
            Respawn(random, width, height);
        }

        public void Draw()
        {
            Console.SetCursorPosition(position.X, position.Y);
            Console.ForegroundColor = color;
            Console.Write("■");
        }

        public bool IsEaten(Position snakeHead)
        {
            return position.X == snakeHead.X && position.Y == snakeHead.Y;
        }

        public void Respawn(Random random, int width, int height)
        {
            position = new Position(random.Next(1, width - 2), random.Next(1, height - 2));
        }
    }

    class NormalFood : FoodBase
    {
        public NormalFood(Random random, int width, int height)
            : base(random, width, height, 1, ConsoleColor.Cyan) { }
    }
}
