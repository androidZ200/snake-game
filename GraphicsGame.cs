using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;

namespace змейка
{
    class GraphicsGame
    {
        private Field field;
        private Random rand = new Random();

        public Bitmap output { get; private set; }
        private Bitmap ellipseImage;
        private Rectangle SizeImage;

        private Thread mainThread;
        private Thread JumpRectangle;
        private Thread moveField;
        private Thread speedupMoveThread;
        private Thread rotateField;
        private Thread speedupRotateThread;

        private PointF beginning = new PointF(0, 0);
        private PointF speedImage = new PointF(0, 0);
        private float angle = 0;
        private float speedAngle = 0;

        public float MaxSpeedField { get; private set; } = 0.7f;
        public float MaxSpeedRotate { get; private set; } = 0.2f;

        public bool MoveImage { get; private set; } = false;
        public bool RotateImage { get; private set; } = false;

        private object LockMove = new object();
        private object LockRotate = new object();
        private object LockDrawEllipse = new object();
        private object LockDrawRectangle = new object();

        public event Action<Bitmap> outImage;

        public GraphicsGame(Field f, Action<Bitmap> output)
        {
            field = f;
            outImage = output;
            this.output = new Bitmap((int)(field.WidthImage * 1.4142f), (int)(field.WidthImage * 1.4142f));
            SizeImage = new Rectangle((int)(field.WidthImage * 0.2071f), (int)(field.WidthImage * 0.2071f), field.WidthImage, field.WidthImage);
            DrawEllipse(Color.Gray);
        }
        ~GraphicsGame()
        {
            AbortThread(ref mainThread);
            AbortThread(ref JumpRectangle);
            AbortThread(ref moveField);
            AbortThread(ref speedupMoveThread);
            AbortThread(ref rotateField);
            AbortThread(ref speedupRotateThread);
        }

        public void ChangeImage()
        {
            if (!MoveImage && !RotateImage)
            {
                DrawImage();
                outImage(output);
            }
        }
        public Bitmap GetImage()
        {
            DrawImage();
            return output;
        }
        public void DrawEllipse(Color r)
        {
            lock (LockDrawEllipse)
            {
                ellipseImage = new Bitmap((int)(field.WidthImage * 1.4142f), (int)(field.WidthImage * 1.4142f));
                Graphics g = Graphics.FromImage(ellipseImage);
                g.FillEllipse(new SolidBrush(r), 0, 0, output.Width, output.Height);
            }
        }
        public void Jump()
        {
            if (MoveImage || RotateImage)
                StartTrhead(ref JumpRectangle, JumpRectangleImage);
        }

        public void StartMove()
        {
            MoveImage = true;
            if (mainThread == null)
                StartTrhead(ref mainThread, Drawing);
            StartTrhead(ref moveField, speedMove);
            StartTrhead(ref speedupMoveThread, waidSpeedMove);
        }
        public void StopMove()
        {
            PauseMove();
            speedImage = Point.Empty;
            beginning = Point.Empty;
        }
        public void PauseMove()
        {
            AbortThread(ref moveField);
            AbortThread(ref speedupMoveThread);
            MoveImage = false;
        }

        public void StartRotate()
        {
            RotateImage = true;
            if (mainThread == null)
                StartTrhead(ref mainThread, Drawing);
            StartTrhead(ref rotateField, speedRotate);
            StartTrhead(ref speedupRotateThread, waitSpeedRotate);
        }
        public void StopRotate()
        {
            PauseRotate();
            speedAngle = 0;
            angle = 0;
        }
        public void PauseRotate()
        {
            AbortThread(ref rotateField);
            AbortThread(ref speedupRotateThread);
            RotateImage = false;
        }

        private void speedMove()
        {
            while (MoveImage)
            {
                lock (LockMove)
                {
                    beginning.X += speedImage.X;
                    beginning.Y += speedImage.Y;
                    if (beginning.X < 0) beginning.X += field.WidthImage;
                    if (beginning.X >= field.WidthImage) beginning.X -= field.WidthImage;
                    if (beginning.Y < 0) beginning.Y += field.WidthImage;
                    if (beginning.Y >= field.WidthImage) beginning.Y -= field.WidthImage;
                }

                Thread.Sleep(10);
            }
        }
        private void speedRotate()
        {
            while (RotateImage)
            {
                lock (LockRotate)
                {
                    angle += speedAngle;
                    //float p = (float)(2 * Math.PI);
                    //if (angle < 0) angle += p;
                    //else if (angle >= p) angle -= p;
                }

                Thread.Sleep(10);
            }
        }
        private void speedupMove(PointF newSpeed, int delayed)
        {
            while (speedImage != newSpeed)
            {
                lock (LockMove)
                {
                    float t = speedImage.X - newSpeed.X;
                    if (Math.Abs(t) <= 0.1f) speedImage.X = newSpeed.X;
                    else if (t < 0) speedImage.X += 0.1f;
                    else speedImage.X -= 0.1f;

                    t = speedImage.Y - newSpeed.Y;
                    if (Math.Abs(t) <= 0.1f) speedImage.Y = newSpeed.Y;
                    else if (t < 0) speedImage.Y += 0.1f;
                    else speedImage.Y -= 0.1f;
                }

                Thread.Sleep(delayed);
            }
        }
        private void speedupRotate(float newAngle, int delayed)
        {
            while (speedAngle != newAngle)
            {
                lock (LockRotate)
                {
                    float t = newAngle - speedAngle;
                    if (Math.Abs(t) < 0.01f) speedAngle = newAngle;
                    else if (t > 0) speedAngle += 0.01f;
                    else speedAngle -= 0.01f;
                }

                Thread.Sleep(delayed);
            }
        }
        private void waidSpeedMove()
        {
            while (MoveImage)
            {
                Thread.Sleep(rand.Next(1000, 6000));
                speedupMove(new PointF(Rand(MaxSpeedField), Rand(MaxSpeedField)), rand.Next(300, 600));
            }
        }
        private void waitSpeedRotate()
        {
            while (RotateImage)
            {
                Thread.Sleep(rand.Next(1000, 6000));
                speedupRotate(Rand(MaxSpeedRotate), rand.Next(250, 500));
            }
        }

        private void Drawing()
        {
            while (MoveImage || RotateImage)
            {
                DrawImage();
                outImage(output);
            }
            AbortThread(ref mainThread);
        }
        private void DrawImage()
        {
            Bitmap t;
            lock (LockDrawEllipse)
                t = new Bitmap(ellipseImage.Width, ellipseImage.Height);
            Graphics g = Graphics.FromImage(t);
            lock (LockDrawEllipse)
                g.DrawImage(ellipseImage, 0, 0);

            Bitmap MoveingImage = new Bitmap(field.WidthImage, field.WidthImage);
            Graphics s = Graphics.FromImage(MoveingImage);
            Bitmap copy = field.Image();
            lock (LockMove)
            {
                Point p = new Point((int)beginning.X, (int)beginning.Y);
                s.DrawImage(copy, p.X, p.Y);
                s.DrawImage(copy, p.X - field.WidthImage, p.Y);
                s.DrawImage(copy, p.X, p.Y - field.WidthImage);
                s.DrawImage(copy, p.X - field.WidthImage, p.Y - field.WidthImage);
            }

            lock (LockDrawRectangle)
                g.DrawImage(MoveingImage, SizeImage);
            lock (LockRotate)
                output = rotateImage(t, angle);
        }
        private Bitmap rotateImage(Bitmap input, float angle)
        {
            Bitmap result = new Bitmap(input.Width, input.Height);
            Graphics g = Graphics.FromImage(result);
            g.TranslateTransform((float)input.Width / 2, (float)input.Height / 2);
            g.RotateTransform(angle);
            g.TranslateTransform(-(float)input.Width / 2, -(float)input.Height / 2);
            g.DrawImage(input, new Point(0, 0));
            return result;
        }
        private float Rand(float lim)
        {
            return (float)rand.NextDouble() * lim * 2 - lim;
        }
        private void JumpRectangleImage()
        {
            Rectangle copy = SizeImage;
            for (int i = 0; i <= 50; i++)
            {
                int x = (int)(Math.Sin(i * Math.PI / 50) * 50);
                lock (LockDrawRectangle)
                {
                    SizeImage.X = copy.X - x;
                    SizeImage.Y = copy.Y - x;
                    SizeImage.Width = copy.Width + 2 * x;
                    SizeImage.Height = copy.Height + 2 * x;
                }
                Thread.Sleep(10);
            }
            SizeImage = copy;
            AbortThread(ref JumpRectangle);
        }

        private void StartTrhead(ref Thread t, ThreadStart func)
        {
            if (t == null)
            {
                t = new Thread(func);
                t.Start();
            }
        }
        private void AbortThread(ref Thread t)
        {
            Thread t1 = t;
            t = null;
            if (t1 != null) t1.Abort();
        }
    }
}