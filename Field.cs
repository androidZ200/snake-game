using System;
using System.Collections.Generic;
using System.Drawing;

namespace змейка
{
    class Field
    {
        private Random rand = new Random();
        public Snake<int> snake { get; set; }
        public HashSet<int> wall { get; private set; }
        public int eat { get; private set; }
        public int Width { get; private set; }
        public int WidthImage { get; private set; }
        public bool isLive { get; private set; } = true;

        private Bitmap image;
        private bool isChangeField = true;
        private object DrawingLock = new object();

        private Color _Background = Color.White;
        private Color _EatColor = Color.DarkRed;
        private Color _WallColor = Color.Black;
        public Color Background { get { return _Background; } set { lock (DrawingLock) _Background = value; isChangeField = true; } }
        public Color EatColor { get { return _EatColor; } set { lock (DrawingLock) _EatColor = value; isChangeField = true; } }
        public Color WallColor { get { return _WallColor; } set { lock (DrawingLock) _WallColor = value; isChangeField = true; } }

        public event Action Eat;
        public event Action Die;

        public Field(int Width, int startLength)
        {
            this.Width = Width;
            snake = new Snake<int>(rand.Next(Width * Width),
                (Snake<int>.Rotate)rand.Next(4), startLength, NextCell, GetRotate);
            WidthImage = Width * 20;
            wall = new HashSet<int>();
            GenerateEat();
        }
        public Field(int Width, int startLength, Func<int, Color> SkinSnake) : this(Width, startLength)
        {
            snake.NewSkin(SkinSnake);
        }

        public void Move()
        {
            int coordinate = NextCell(snake.Body.Head, snake.angle);
            if (eat == coordinate) { snake.MoveAndEat(); GenerateEat(); Eat(); }
            else if (snake.Body.Find(coordinate, Comparison)) { isLive = false; Die(); }
            else if (wall.Contains(coordinate)) { isLive = false; Die(); }
            else snake.Move(coordinate);
            lock (DrawingLock)
                isChangeField = true;
        }
        public void ReverseAndMove()
        {
            snake.ReversAndMove();
            if (eat == snake.Body.Head) { GenerateEat(); Eat(); }
            lock (DrawingLock)
                isChangeField = true;
        }
        public void Restart(int startLength)
        {
            snake = new Snake<int>(rand.Next(Width * Width),
                (Snake<int>.Rotate)rand.Next(4), startLength, NextCell, GetRotate);
            wall.Clear();
            GenerateEat();
            _Background = Color.White;
            _EatColor = Color.DarkRed;
            _WallColor = Color.Black;
            isLive = true;
            isChangeField = true;
        }
        public void NewSnakeSkin(Func<int, Color> skin)
        {
            snake.NewSkin(skin);
        }
        public void AddWall()
        {
            for (int i = 0; i < 100; i++)
            {
                int t = rand.Next(Width * Width);
                if (!snake.Body.Find(t, Comparison) && GetLength(t, snake.Body.Head) > 10 && t != eat)
                {
                    wall.Add(t);
                    break;
                }
            }
        }
        public Bitmap Image()
        {
            lock (DrawingLock)
                if (isChangeField)
                {
                    image = new Bitmap(Width * 20, Width * 20);
                    Graphics g = Graphics.FromImage(image);
                    g.Clear(Background);
                    Deque<int>.Iiterator curr = snake.Body.GetHead();
                    int index = 0;
                    do
                    {
                        g.FillRectangle(new SolidBrush(snake.GetColor(index++)), curr.Value % Width * 20,
                            curr.Value / Width * 20, 19, 19);
                    } while (curr.Next() != null);
                    g.FillRectangle(new SolidBrush(EatColor), eat % Width * 20, eat / Width * 20, 19, 19);
                    foreach (var x in wall)
                        g.FillRectangle(new SolidBrush(WallColor), x % Width * 20, x / Width * 20, 19, 19);
                    isChangeField = false;
                    return image;
                }
                else return image;
        }

        private int NextCell(int currenCell, Snake<int>.Rotate rotate)
        {
            switch (rotate)
            {
                case Snake<int>.Rotate.Up:
                    currenCell -= Width;
                    if (currenCell < 0) currenCell += Width * Width;
                    return currenCell;

                case Snake<int>.Rotate.Right:
                    if (currenCell % Width == Width - 1) return currenCell - Width + 1;
                    else return currenCell + 1;

                case Snake<int>.Rotate.Down:
                    currenCell += Width;
                    if (currenCell >= Width * Width) currenCell -= Width * Width;
                    return currenCell;

                case Snake<int>.Rotate.Left:
                    if (currenCell % Width == 0) return currenCell + Width - 1;
                    else return currenCell - 1;

                default:
                    return currenCell;
            }
        }
        private Snake<int>.Rotate GetRotate(int currentCell, int nextCell)
        {
            int delta = nextCell - currentCell;
            if (delta == 1 || delta == -Width + 1) return Snake<int>.Rotate.Right;
            else if (delta == -1 || delta == Width - 1) return Snake<int>.Rotate.Left;
            else if (delta == Width || delta == Width * (Width - 1)) return Snake<int>.Rotate.Down;
            else return Snake<int>.Rotate.Up;
        }
        private bool Comparison(int a, int b)
        {
            return a == b;
        }
        private int GetLength(int a, int b)
        {
            int a1 = a % Width;
            int b1 = b % Width;
            int length = Math.Abs(a1 - b1) > Width - Math.Abs(a1 - b1) ?
                Width - Math.Abs(a1 - b1) : Math.Abs(a1 - b1);
            a1 = a / Width;
            b1 = b / Width;
            length += Math.Abs(a1 - b1) > Width - Math.Abs(a1 - b1) ?
                Width - Math.Abs(a1 - b1) : Math.Abs(a1 - b1);
            return length;
        }
        private void GenerateEat()
        {
            for (int i = 2; ; i++)
            {
                int t = rand.Next(Width * Width);
                if (!snake.Body.Find(t, Comparison) && GetLength(t, snake.Body.Head) < i && !wall.Contains(t))
                {
                    eat = t;
                    break;
                }
            }
        }
    }
}