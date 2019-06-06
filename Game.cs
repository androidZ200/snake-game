using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;

namespace змейка
{
    class Game
    {
        public enum Rotate { Left, Up, Right };

        private Field field;
        private Random rand = new Random();
        private Thread gameThread;
        private object AddMoveLook = new object();
        private int Delayed = 0;
        private bool ReverseRotate = false;
        private Queue<Rotate> historyMoves = new Queue<Rotate>();
        private GraphicsGame graphics;
        private int StartLength;

        public bool Play { get; private set; } = false;
        public int SnakeLength { get { return field.snake.Length; } }
        public int CountMoves { get; private set; } = 0;

        public event Action<int> Death;
        public event Action<Bitmap> Graphics;
        public event Action<int> ChangeLength;

        public Game(int width, int length, Action<int> die, Action<Bitmap> graphics)
        {
            field = new Field(width, length, SkinsSnake.Rand());
            StartLength = length;
            field.Die += DieSnake;
            field.Eat += Eat;
            Death = die;
            Graphics = graphics;
            this.graphics = new GraphicsGame(field, Graphics);
        }
        ~Game()
        {
            if (gameThread != null) gameThread.Abort();
        }

        public void Start()
        {
            if (!Play && field.isLive)
            {
                Play = true;
                gameThread = new Thread(Move);
                gameThread.Start();
            }
            else if (!Play)
            {
                Reset();
                Start();
            }
        }
        public void Stop()
        {
            Play = false;
            graphics.PauseMove();
            graphics.PauseRotate();
        }
        public void Reset()
        {
            Stop();
            graphics.StopRotate();
            graphics.StopMove();
            field.Restart(StartLength);
            field.NewSnakeSkin(SkinsSnake.Rand());
            Delayed = 0;
            CountMoves = 0;
            historyMoves.Clear();
            ChangeLength(SnakeLength);
        }
        public void RotateMove(Rotate move)
        {
            if (Play)
                if (!ReverseRotate || move == Rotate.Up) RotateMovePriv(move);
                else if (move == Rotate.Left) RotateMovePriv(Rotate.Right);
                else if (move == Rotate.Right) RotateMovePriv(Rotate.Left);
        }
        public Bitmap GetImage()
        {
            return graphics.GetImage();
        }
        public void NewStartLength(int length)
        {
            StartLength = length;
        }

        private void Move()
        {
            while (Play)
            {
                SnakeRotate();
                field.Move();
                CountMoves++;
                graphics.ChangeImage();
                GC.Collect();
                if (field.isLive)
                {
                    CheckLength(field.snake.Length);
                    DalayedMove(field.snake.Length);
                }
            }
        }
        private void SnakeRotate()
        {
            Rotate t;
            lock (AddMoveLook)
            {
                while (historyMoves.Count <= Delayed) RotateMovePriv(Rotate.Up);
                t = historyMoves.Dequeue();
            }
            switch (t)
            {
                case Rotate.Left:
                    field.snake.ChangeAngle(false);
                    break;
                case Rotate.Right:
                    field.snake.ChangeAngle(true);
                    break;
            }
        }
        private void CheckLength(int length)
        {
            switch (length)
            {
                case 40:
                    Delayed = 1;
                    break;
                case 60:
                    Delayed = 2;
                    break;
                case 80:
                    Delayed = 3;
                    break;
                case 100:
                    Delayed = 4;
                    break;
            }
            if (length > 30)
                if (rand.Next(200) == 0) field.AddWall();

            if (length >= 10 && !graphics.MoveImage)
                graphics.StartMove();

            if (length >= 20 && !graphics.RotateImage)
                graphics.StartRotate();

            if (length >= 80 && rand.Next(700) == 0)
            {
                historyMoves.Clear();
                field.ReverseAndMove();
            }

            if (length >= 100)
            {
                if ((length / 10) % 2 == 0)
                {
                    if (!ReverseRotate)
                    {
                        ReverseRotate = true;
                        graphics.DrawEllipse(Color.Black);
                    }
                }
                else if (ReverseRotate)
                {
                    ReverseRotate = false;
                    graphics.DrawEllipse(Color.Gray);
                }
            }

            if (length >= 50 && rand.Next(500) == 0)
            {
                Color t = field.WallColor;
                field.WallColor = field.EatColor;
                field.EatColor = t;
            }

            if (length >= 40 && rand.Next(200) == 0)
            {
                Color[] clr = { Color.White, Color.Yellow, Color.Cyan, Color.Violet };
                field.Background = clr[rand.Next(clr.Length)];
            }

        }
        private void DalayedMove(int length)
        {
            if (length >= 40) Thread.Sleep(160);
            else if (length >= 30) Thread.Sleep(230);
            else if (length >= 20) Thread.Sleep(300);
            else if (length >= 14) Thread.Sleep(360);
            else if (length >= 8) Thread.Sleep(400);
            else Thread.Sleep(450);
        }
        private void RotateMovePriv(Rotate move)
        {
            lock (AddMoveLook)
                historyMoves.Enqueue(move);
        }
        private void DieSnake()
        {
            Stop();
            Death(field.snake.Length);
        }
        private void Eat()
        {
            if (SnakeLength >= 30)
                graphics.Jump();
            ChangeLength(field.snake.Length);
        }
    }
}
