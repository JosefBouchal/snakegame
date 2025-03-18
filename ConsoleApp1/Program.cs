using System;
using System.Collections.Generic;
using System.Threading;

namespace Snake
{
    class Program
    {
        // Hlavní metoda, která spustí hru
        static void Main(string[] args)
        {
            Game game = new Game(40, 20); // Nastavení velikosti hrací plochy
            game.Run();
        }
    }

    // Směry pohybu hada
    enum Direction { Up, Down, Left, Right }

    // Struktura pro uložení souřadnic na hrací ploše
    struct Position
    {
        public int X { get; }
        public int Y { get; }

        public Position(int x, int y) { X = x; Y = y; }
    }

    // Rozhraní pro objekty, které lze vykreslit
    interface IDrawable
    {
        void Draw();
    }

    // Hlavní třída hry, která řídí celou logiku
    class Game
    {
        private int windowWidth;
        private int windowHeight;
        private Random random = new Random();
        public int Score { get; private set; } = 5;
        private bool isGameOver = false;
        private List<Snake> snakes = new List<Snake>();
        private List<FoodBase> foodItems = new List<FoodBase>();

        // Konstruktor nastavuje velikost okna a inicializuje hada a jídlo
        public Game(int width, int height)
        {
            windowWidth = width;
            windowHeight = height;
            Console.WindowHeight = windowHeight;
            Console.WindowWidth = windowWidth;

            // Přidání jednoho nebo více hadů
            snakes.Add(new Snake(windowWidth / 2, windowHeight / 2, this));

            // Přidání různých typů jídla
            foodItems.Add(new NormalFood(random, windowWidth, windowHeight));
        }

        // Hlavní smyčka hry
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

        // Vykreslí hranice hrací plochy
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

        // Kontroluje kolize hada s okraji nebo jídlem
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

        // Zpracovává vstupy z klávesnice
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

        // Zobrazení konce hry
        private void EndGame()
        {
            Console.SetCursorPosition(windowWidth / 5, windowHeight / 2);
            Console.WriteLine("Game over, Score: " + Score);
        }
    }

    // Třída hada, který se pohybuje a interaguje s jídlem
    class Snake : IDrawable
    {
        private List<Position> body = new List<Position>();
        public Position Head => body[^1]; // Poslední prvek v seznamu představuje hlavu hada
        private Direction currentDirection = Direction.Right;
        private Game game;

        // Konstruktor nastavuje počáteční pozici hada
        public Snake(int startX, int startY, Game gameInstance)
        {
            body.Add(new Position(startX, startY));
            game = gameInstance;
        }

        // Vykreslí hada na konzoli
        public void Draw()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            foreach (var segment in body)
            {
                Console.SetCursorPosition(segment.X, segment.Y);
                Console.Write("■");
            }
        }

        // Posune hada v aktuálním směru
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

        // Změní směr pohybu na základě vstupu z klávesnice
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

        // Kontroluje, zda had narazil do stěny
        public bool CheckCollision(int width, int height)
        {
            return Head.X == 0 || Head.X == width - 1 || Head.Y == 0 || Head.Y == height - 1;
        }

        // Přidá nový segment těla (had roste)
        public void Grow()
        {
            body.Insert(0, Head);
        }
    }

    // Abstraktní třída pro jídlo
    abstract class FoodBase : IDrawable
    {
        protected Position position;
        public int Points { get; protected set; }
        protected ConsoleColor color;

        // Konstruktor nastavuje barvu a hodnotu jídla
        public FoodBase(Random random, int width, int height, int points, ConsoleColor color)
        {
            this.color = color;
            Points = points;
            Respawn(random, width, height);
        }

        // Vykreslí jídlo na konzoli
        public void Draw()
        {
            Console.SetCursorPosition(position.X, position.Y);
            Console.ForegroundColor = color;
            Console.Write("■");
        }

        // Zkontroluje, zda had snědl jídlo
        public bool IsEaten(Position snakeHead)
        {
            return position.X == snakeHead.X && position.Y == snakeHead.Y;
        }

        // Přemístí jídlo na novou náhodnou pozici
        public void Respawn(Random random, int width, int height)
        {
            position = new Position(random.Next(1, width - 2), random.Next(1, height - 2));
        }
    }

    // Normální jídlo, které dává 1 bod
    class NormalFood : FoodBase
    {
        public NormalFood(Random random, int width, int height)
            : base(random, width, height, 1, ConsoleColor.Cyan) { }
    }
}
