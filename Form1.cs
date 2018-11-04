using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace змейка
{
    public partial class Form1 : Form
    {
        Mode Mode = Mode.Easy;
        Random rand = new Random();
        List<Point> BodySnake = new List<Point>();
        List<byte> FutureMoves = new List<byte>();
        object Lock = new object();
        byte angleSnake = 3;
        float angleImage = 0f;
        Point beginning = new Point(0, 0);
        Color ColorSnake = Color.Black;
        Color ColorField = Color.White;
        Color ColorEat = Color.Red;
        Color ColorEllipse = Color.Gray;
        //byte NewColorR = 0, NewColorG = 0, NewColorB = 0;
        bool Game = false, Pause = false, Reverse = false;
        Field[,] field = new Field[50, 50];
        Thread t1, t2, t3, t4;

        public Form1()
        {
            InitializeComponent();
        }
        private void start_Click(object sender, EventArgs e)
        {
            if (Game)
            {
                if (Pause)
                {
                    t1 = new Thread(Move);
                    t2 = new Thread(MoveField);
                    t3 = new Thread(RotateField);
                    t4 = new Thread(FPS);
                    if (!t1.IsAlive && !t2.IsAlive && !t3.IsAlive)
                    {
                        start.Text = "stop";
                        Pause = false;
                        if (Mode == Mode.FieldMove)
                        {
                            t2.Start();
                        }
                        else if (Mode == Mode.FieldRotate)
                        {
                            t2.Start();
                            t3.Start();
                        }
                        Thread.Sleep(100);
                        t1.Start();
                        t4.Start();
                    }
                }
                else
                {
                    start.Text = "start";
                    Pause = true;
                    if (t2 != null) t2.Abort();
                    if (t3 != null) t3.Abort();
                    if (t4 != null) t4.Abort();
                }
            }
            else
            {
                Mode = Mode.Easy;
                ColorSnake = Color.Black;
                ColorField = Color.White;
                ColorEat = Color.Red;
                ColorEllipse = Color.Gray;
                Reverse = false;
                Brush p = new SolidBrush(ColorSnake);
                for (int i = 0; i < 50; i++)
                    for (int j = 0; j < 50; j++)
                        field[i, j] = Field.None;
                beginning = new Point(0, 0);
                angleImage = 0f;
                BodySnake.Clear();
                angleSnake = (byte)rand.Next(4);
                BodySnake.Add(new Point(rand.Next(50), rand.Next(50)));
                field[BodySnake[0].X, BodySnake[0].Y] = Field.Snake;
                for (int i = 0; i < 3; i++)
                {
                    int x = 0, y = 0;
                    if (angleSnake == 0) y = -1;
                    if (angleSnake == 1) x = 1;
                    if (angleSnake == 2) y = 1;
                    if (angleSnake == 3) x = -1;
                    BodySnake.Add(new Point(BodySnake[i].X + x, BodySnake[i].Y + y));
                    field[BodySnake[i + 1].X, BodySnake[i + 1].Y] = Field.Snake;
                }
                AddEat();
                FutureMoves.Clear();
                Game = true;
                Pause = false;
                start.Text = "stop";
                BodySnaceCountText.Text = "4";
                t1 = new Thread(Move);
                t4 = new Thread(FPS);
                t1.Start();
                t4.Start();
            }
        }
        private void restart_Click(object sender, EventArgs e)
        {
            GameOver();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (t1 != null) t1.Abort();
            if (t2 != null) t2.Abort();
            if (t3 != null) t3.Abort();
            if (t4 != null) t4.Abort();
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right || e.KeyCode == Keys.X)
            {
                if (!Reverse)
                    FutureMoves.Add(2);
                else
                    FutureMoves.Add(1);
            }
            else if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Z)
            {
                if (Reverse)
                    FutureMoves.Add(2);
                else
                    FutureMoves.Add(1);
            }
        }
        private Image ImageResize(Bitmap bmp, int pictureBox_Width, int pictureBox_Height)
        {
            lock (Lock)
            {
                double min = Math.Min(pictureBox_Height * 1.0 / bmp.Height, pictureBox_Width * 1.0 / bmp.Width);
                return new Bitmap(bmp, (int)(bmp.Width * min), (int)(bmp.Height * min));
            }
        }
        private Bitmap rotateImage(Bitmap input, float angle)
        {
            lock (Lock)
            {
                Bitmap result = new Bitmap(input.Width, input.Height);
                Graphics g = Graphics.FromImage(result);
                g.TranslateTransform((float)input.Width / 2, (float)input.Height / 2);
                g.RotateTransform(angle);
                g.TranslateTransform(-(float)input.Width / 2, -(float)input.Height / 2);
                g.DrawImage(input, new Point(0, 0));
                return result;
            }
        }
        private void Move()
        {
            while (!Pause && Game)
            {
                if (FutureMoves.Count <= (BodySnake.Count - 45) / 10 || FutureMoves.Count == 0)
                    FutureMoves.Add(0);
                if (FutureMoves[0] == 1)
                    angleSnake = (byte)((angleSnake + 3) % 4);
                else if (FutureMoves[0] == 2)
                    angleSnake = (byte)((angleSnake + 1) % 4);
                FutureMoves.RemoveAt(0);
                int x = 0, y = 0;
                if (angleSnake == 0) y = -1;
                if (angleSnake == 1) x = 1;
                if (angleSnake == 2) y = 1;
                if (angleSnake == 3) x = -1;
                if (field[(BodySnake[BodySnake.Count - 1].X + x + 50) % 50, (BodySnake[BodySnake.Count - 1].Y + y + 50) % 50] != Field.Snake)
                {
                    Point timePoint = BodySnake[0];
                    if (field[(BodySnake[BodySnake.Count - 1].X + x + 50) % 50, (BodySnake[BodySnake.Count - 1].Y + y + 50) % 50] != Field.Eat)
                    {
                        field[BodySnake[0].X, BodySnake[0].Y] = Field.None;
                        BodySnake.RemoveAt(0);
                    }
                    else
                    {
                        if (BodySnake.Count >= 2499) Game = false;
                        field[(BodySnake[BodySnake.Count - 1].X + x + 50) % 50, (BodySnake[BodySnake.Count - 1].Y + y + 50) % 50] = Field.Snake;
                        AddEat();
                    }
                    field[(BodySnake[BodySnake.Count - 1].X + x + 50) % 50, (BodySnake[BodySnake.Count - 1].Y + y + 50) % 50] = Field.Snake;
                    BodySnake.Add(new Point((BodySnake[BodySnake.Count - 1].X + x + 50) % 50, (BodySnake[BodySnake.Count - 1].Y + y + 50) % 50));
                    if (BodySnake.Count >= 100 && rand.Next(300) == 0)
                    {
                        if (timePoint.X - BodySnake[0].X == 1) angleSnake = 2;
                        if (timePoint.X - BodySnake[0].X == -1) angleSnake = 4;
                        if (timePoint.Y - BodySnake[0].Y == 1) angleSnake = 3;
                        if (timePoint.Y - BodySnake[0].Y == -1) angleSnake = 1;
                        BodySnake.Reverse();
                    }
                }
                else
                    Invoke(new Action(GameOver));
                if (Game)
                {
                    if (BodySnake.Count == 10 && Mode == Mode.Easy)
                    {
                        Mode = Mode.FieldMove;
                        t2 = new Thread(MoveField);
                        t2.Start();
                    }
                    else if (BodySnake.Count == 20 && Mode == Mode.FieldMove)
                    {
                        Mode = Mode.FieldRotate;
                        t3 = new Thread(RotateField);
                        t3.Start();
                    }
                    if (BodySnake.Count >= 80 && BodySnake.Count % 40 == 0)
                    {
                        Reverse = true;
                        lock (Lock)
                            ColorEllipse = Color.Black;
                    }
                    else if (BodySnake.Count >= 100 && (BodySnake.Count + 20) % 40 == 0)
                    {
                        Reverse = false;
                        lock (Lock)
                            ColorEllipse = Color.Gray;
                    }
                    if (BodySnake.Count >= 30 && rand.Next(60) == 0)
                        ChengeColor();
                    if (BodySnake.Count >= 55 && rand.Next(200) == 0)
                    {
                        int x1, y1;
                        while (true)
                        {
                            x1 = rand.Next(50);
                            y1 = rand.Next(50);
                            if (field[x1, y1] == Field.None && (Module(x1 - BodySnake[BodySnake.Count - 1].X) > 10 ||
                                Module(x1 - BodySnake[BodySnake.Count - 1].X) < 40) &&
                                (Module(y1 - BodySnake[BodySnake.Count - 1].Y) > 10 ||
                                Module(y1 - BodySnake[BodySnake.Count - 1].Y) < 40)) break;
                        }
                        field[x1, y1] = Field.Snake;
                    }
                    Invoke(new EventHandler(delegate { BodySnaceCountText.Text = Convert.ToString(BodySnake.Count); }));
                    Invoke(new EventHandler(delegate
                    {
                        FutureMovesText.Text = "";
                        for (int i = 0; i < FutureMoves.Count; i++)
                            FutureMovesText.Text += Convert.ToString(FutureMoves[i]);
                    }));
                    if (BodySnake.Count <= 6)
                        Thread.Sleep(700);
                    else if (BodySnake.Count <= 10)
                        Thread.Sleep(600);
                    else if (BodySnake.Count <= 18)
                        Thread.Sleep(400);
                    else if (BodySnake.Count <= 26)
                        Thread.Sleep(300);
                    else if (BodySnake.Count <= 40)
                        Thread.Sleep(250);
                    else
                        Thread.Sleep(200);
                }
            }
            t1.Abort();
        }
        private void MoveField()
        {
            Point beginning1 = new Point(beginning.X * 100, beginning.Y * 100);
            Point speed = new Point(0, 0);
            Point prevSpeed = new Point(0, 0);
            while (!Pause && Game)
            {
                if (BodySnake.Count >= 16)
                {
                    speed.X = rand.Next(-300, 301);
                    speed.Y = rand.Next(-300, 301);
                }
                else
                {
                    speed.X = rand.Next(-100, 101);
                    speed.Y = rand.Next(-100, 101);
                }
                while (!Pause)
                {
                    if (prevSpeed.X < speed.X) prevSpeed.X++;
                    if (prevSpeed.X > speed.X) prevSpeed.X--;
                    if (prevSpeed.Y < speed.Y) prevSpeed.Y++;
                    if (prevSpeed.Y > speed.Y) prevSpeed.Y--;
                    lock (Lock)
                    {
                        beginning1.X = (beginning1.X + prevSpeed.X + 100000) % 100000;
                        beginning1.Y = (beginning1.Y + prevSpeed.Y + 100000) % 100000;
                        beginning.X = beginning1.X / 100;
                        beginning.Y = beginning1.Y / 100;
                    }
                    Thread.Sleep(10);
                    if (prevSpeed.X == speed.X && prevSpeed.Y == speed.Y) break;
                }
                if (!Pause)
                    while (rand.Next(100) == 0)
                    {
                        lock (Lock)
                        {
                            beginning1.X = (beginning1.X + prevSpeed.X + 100000) % 100000;
                            beginning1.Y = (beginning1.Y + prevSpeed.Y + 100000) % 100000;
                            beginning.X = beginning1.X / 100;
                            beginning.Y = beginning1.Y / 100;
                        }
                        Thread.Sleep(10);
                    }
            }
            t2.Abort();
        }
        private void RotateField()
        {
            int speedRotate = 0;
            int prevSpeedRotate = 0;
            while (!Pause && Game)
            {
                if (BodySnake.Count >= 40)
                    speedRotate = rand.Next(-200, 201);
                else
                    speedRotate = rand.Next(-50, 51);
                while (!Pause)
                {
                    if (prevSpeedRotate < speedRotate) prevSpeedRotate++;
                    if (prevSpeedRotate > speedRotate) prevSpeedRotate--;
                    lock (Lock)
                        angleImage = (float)(angleImage + (prevSpeedRotate * 1.0 / 200) + 360) % 360;
                    Thread.Sleep(10);
                    if (prevSpeedRotate == speedRotate) break;
                }
                if (!Pause)
                    for (int i = 0; i < rand.Next(50, 201); i++)
                    {
                        lock (Lock)
                            angleImage = (float)(angleImage + (prevSpeedRotate * 1.0 / 200) + 360) % 360;
                        Thread.Sleep(10);
                    }
            }
            t3.Abort();
        }
        private void ChengeColor()
        {
            int t = rand.Next(3);
            if (t == 0)
            {
                if (ColorEat.R > 125)
                    ColorEat = Color.FromArgb(0, ColorEat.G, ColorEat.B);
                else
                    ColorEat = Color.FromArgb(255, ColorEat.G, ColorEat.B);
                if (ColorSnake.R > 125)
                    ColorSnake = Color.FromArgb(0, ColorSnake.G, ColorSnake.B);
                else
                    ColorSnake = Color.FromArgb(255, ColorSnake.G, ColorSnake.B);
                if (ColorField.R > 125)
                    ColorField = Color.FromArgb(0, ColorField.G, ColorField.B);
                else
                    ColorField = Color.FromArgb(255, ColorField.G, ColorField.B);
            }
            else if (t == 1)
            {
                if (ColorEat.G > 125)
                    ColorEat = Color.FromArgb(ColorEat.R, 0, ColorEat.B);
                else
                    ColorEat = Color.FromArgb(ColorEat.R, 255, ColorEat.B);
                if (ColorSnake.G > 125)
                    ColorSnake = Color.FromArgb(ColorSnake.R, 0, ColorSnake.B);
                else
                    ColorSnake = Color.FromArgb(ColorSnake.R, 255, ColorSnake.B);
                if (ColorField.G > 125)
                    ColorField = Color.FromArgb(ColorField.R, 0, ColorField.B);
                else
                    ColorField = Color.FromArgb(ColorField.R, 255, ColorField.B);
            }
            else
            {
                if (ColorEat.B > 125)
                    ColorEat = Color.FromArgb(ColorEat.R, ColorEat.G, 0);
                else
                    ColorEat = Color.FromArgb(ColorEat.R, ColorEat.G, 255);
                if (ColorSnake.B > 125)
                    ColorSnake = Color.FromArgb(ColorSnake.R, ColorSnake.G, 0);
                else
                    ColorSnake = Color.FromArgb(ColorSnake.R, ColorSnake.G, 255);
                if (ColorField.B > 125)
                    ColorField = Color.FromArgb(ColorField.R, ColorField.G, 0);
                else
                    ColorField = Color.FromArgb(ColorField.R, ColorField.G, 255);
            }
            //if (rand.Next(2) == 0)
            //    NewColorR = 0;
            //else
            //    NewColorR = 255;
            //if (rand.Next(2) == 0)
            //    NewColorG = 0;
            //else
            //    NewColorG = 255;
            //if (rand.Next(2) == 0)
            //    NewColorB = 0;
            //else
            //    NewColorB = 255;
            //while (!Pause)
            //{
            //bool c1 = false, c2 = false, c3 = false, c4 = false, c5 = false, c6 = false, c7 = false, c8 = false, c9 = false;
            //lock (Lock)
            //{
            //    if (ColorField.R > 255 - NewColorR) ColorField = Color.FromArgb(ColorField.R - 1, ColorField.G, ColorField.B);
            //    else if (ColorField.R < 255 - NewColorR) ColorField = Color.FromArgb(ColorField.R + 1, ColorField.G, ColorField.B);
            //    else c1 = true;
            //    if (ColorField.G > 255 - NewColorG) ColorField = Color.FromArgb(ColorField.R, ColorField.G - 1, ColorField.B);
            //    else if (ColorField.G < 255 - NewColorG) ColorField = Color.FromArgb(ColorField.R, ColorField.G + 1, ColorField.B);
            //    else c2 = true;
            //    if (ColorField.B > 255 - NewColorB) ColorField = Color.FromArgb(ColorField.R, ColorField.G, ColorField.B - 1);
            //    else if (ColorField.B < 255 - NewColorB) ColorField = Color.FromArgb(ColorField.R, ColorField.G, ColorField.B + 1);
            //    else c3 = true;
            //    if (ColorSnake.R > NewColorR) ColorSnake = Color.FromArgb(ColorSnake.R - 1, ColorSnake.G, ColorSnake.B);
            //    else if (ColorSnake.R < NewColorR) ColorSnake = Color.FromArgb(ColorSnake.R + 1, ColorSnake.G, ColorSnake.B);
            //    else c4 = true;
            //    if (ColorSnake.G > NewColorG) ColorSnake = Color.FromArgb(ColorSnake.R, ColorSnake.G - 1, ColorSnake.B);
            //    else if (ColorSnake.G < NewColorG) ColorSnake = Color.FromArgb(ColorSnake.R, ColorSnake.G + 1, ColorSnake.B);
            //    else c5 = true;
            //    if (ColorSnake.B > NewColorB) ColorSnake = Color.FromArgb(ColorSnake.R, ColorSnake.G, ColorSnake.B - 1);
            //    else if (ColorSnake.B < NewColorB) ColorSnake = Color.FromArgb(ColorSnake.R, ColorSnake.G, ColorSnake.B + 1);
            //    else c6 = true;
            //    if (ColorEat.R > 255 - NewColorR) ColorEat = Color.FromArgb(ColorEat.R - 1, ColorEat.G, ColorEat.B);
            //    else if (ColorEat.R < 255 - NewColorR) ColorEat = Color.FromArgb(ColorEat.R + 1, ColorEat.G, ColorEat.B);
            //    else c7 = true;
            //    if (ColorEat.G > NewColorG) ColorEat = Color.FromArgb(ColorEat.R, ColorEat.G - 1, ColorEat.B);
            //    else if (ColorEat.G < NewColorG) ColorEat = Color.FromArgb(ColorEat.R, ColorEat.G + 1, ColorEat.B);
            //    else c8 = true;
            //    if (ColorEat.B > NewColorB) ColorEat = Color.FromArgb(ColorEat.R, ColorEat.G, ColorEat.B - 1);
            //    else if (ColorEat.B < NewColorB) ColorEat = Color.FromArgb(ColorEat.R, ColorEat.G, ColorEat.B + 1);
            //    else c9 = true;
            //}
            //if (c1 && c2 && c3 && c4 && c5 && c6 && c7 && c8 && c9) break;
            //else Thread.Sleep(5);
            //}
        }
        private int Module(int num)
        {
            if (num >= 0) return num;
            else return -num;
        }
        private void GameOver()
        {
            if (t1 != null) t1.Abort();
            if (t2 != null) t2.Abort();
            if (t3 != null) t3.Abort();
            if (t4 != null) t4.Abort();
            ColorSnake = Color.Black;
            ColorField = Color.White;
            ColorEat = Color.Red;
            ColorEllipse = Color.Gray;
            Mode = Mode.Easy;
            start.Text = "start";
            Game = false;
            beginning = new Point(0, 0);
            angleImage = 0f;
            MessageBox.Show("Игра окончена. Ваша длина: " + BodySnake.Count);
        }
        private void AddEat()
        {
            while (true)
            {
                int x1 = rand.Next(50);
                int y1 = rand.Next(50);
                if (field[x1, y1] == Field.None && (Module(x1 - BodySnake[BodySnake.Count - 1].X) < 10 ||
                    Module(x1 - BodySnake[BodySnake.Count - 1].X) > 40) &&
                    (Module(y1 - BodySnake[BodySnake.Count - 1].Y) < 10 ||
                    Module(y1 - BodySnake[BodySnake.Count - 1].Y) > 40))
                {
                    field[x1, y1] = Field.Eat;
                    break;
                }
            }
        }
        private void DrawImage()
        {
            lock (Lock)
            {
                Bitmap bmp1 = new Bitmap(1000, 1000);
                Bitmap bmp2 = new Bitmap(1000, 1000);
                Bitmap bmp3 = new Bitmap(1414, 1414);
                Graphics g1 = Graphics.FromImage(bmp1);
                Graphics g2 = Graphics.FromImage(bmp2);
                Graphics g3 = Graphics.FromImage(bmp3);
                if (pictureBox1.Image != null) pictureBox1.Image.Dispose();

                g3.FillEllipse(new SolidBrush(ColorEllipse), 0, 0, 1414, 1414);
                for (int i = 0; i < 50; i++)
                    for (int j = 0; j < 50; j++)
                    {
                        Color clr = Color.Transparent;
                        if (field[i, j] == Field.None) clr = ColorField;
                        if (field[i, j] == Field.Snake) clr = ColorSnake;
                        if (field[i, j] == Field.Eat) clr = ColorEat;
                        g1.FillRectangle(new SolidBrush(clr), i * 20, j * 20, 20, 20);
                    }
                g2.DrawImage(bmp1, beginning);
                g2.DrawImage(bmp1, beginning.X - 1000, beginning.Y);
                g2.DrawImage(bmp1, beginning.X, beginning.Y - 1000);
                g2.DrawImage(bmp1, beginning.X - 1000, beginning.Y - 1000);
                g3.DrawImage(bmp2, new Point(207, 207));

                pictureBox1.Image = ImageResize(rotateImage(bmp3, angleImage), pictureBox1.Width, pictureBox1.Height);
                bmp1.Dispose();
                bmp2.Dispose();
                bmp3.Dispose();
            }
        }
        private void FPS()
        {
            while(Game && !Pause)
            {
                DrawImage();
                Thread.Sleep(20);
            }
        }
    }
    enum Mode
    {
        Easy,
        FieldMove,
        FieldRotate
    }
    enum Field
    {
        None,
        Snake,
        Eat
    }
}