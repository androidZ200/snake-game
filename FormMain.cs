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
    public partial class FormMain : Form
    {
        Game game;
        int MaxValue = 0;

        public FormMain()
        {
            InitializeComponent();
            SetStyle(ControlStyles.Selectable, false);

            game = new Game(50, 4, GameOver, Draw);
            game.ChangeLength += ChangrLength;
            pictureBox1.Image = game.GetImage();
        }
        private void start_Click(object sender, EventArgs e)
        {
            if(!game.Play)
            {
                start.Text = "Stop";
                game.Start();
            }
            else
            {
                start.Text = "Start";
                game.Stop();
            }
        }
        private void restart_Click(object sender, EventArgs e)
        {
            if (MaxValue > 60) game.NewStartLength(20);
            else if (MaxValue > 30) game.NewStartLength(10);

            start.Text = "Start";
            game.Reset();
            pictureBox1.Image = game.GetImage();
            ChangrLength(game.SnakeLength);
        }
        private void FormMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Z || e.KeyCode == Keys.Left) game.RotateMove(Game.Rotate.Left);
            else if (e.KeyCode == Keys.X || e.KeyCode == Keys.Right) game.RotateMove(Game.Rotate.Right);
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Left) game.RotateMove(Game.Rotate.Left);
            else if (keyData == Keys.Right) game.RotateMove(Game.Rotate.Right);

            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            game.Stop();
            Thread.Sleep(50);
        }

        private void GameOver(int length)
        {
            if (MaxValue > 60) game.NewStartLength(20);
            else if (MaxValue > 30) game.NewStartLength(10);

            Invoke((Action)(() => start.Text = "Start"));
            MessageBox.Show("DIE. Length: " + length);
        }
        private void Draw(Bitmap bmp)
        {
            BeginInvoke((Action)(() => pictureBox1.Image = bmp));
        }
        private void ChangrLength(int length)
        {
            MaxValue = length > MaxValue ? length : MaxValue;
            Invoke((Action)(() => BodySnaceCountText.Text = length.ToString()));
        }
    }
}