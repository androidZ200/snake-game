using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace змейка
{
    abstract class SkinsSnake
    {
        private static Random rand = new Random();
        public static Func<int, Color> Rand()
        {
            Func<int, Color>[] skins = { FullBlack, FullGreen, GrassSnake, AgkistrodonContortrix, BlackRed, BlackYelloy,
            BlackBlue, LampropeltisTriangulumElapsoides, GreenLines, GrayWaves};
            return skins[rand.Next(skins.Length)];
        }

        public static Color FullBlack(int index)
        {
            return Color.Black;
        }
        public static Color FullGreen(int index)
        {
            return Color.DarkGreen;
        }
        public static Color GrassSnake(int index)
        {
            if (index == 1) return Color.Gold;
            return Color.FromArgb(60, 60, 60);
        }
        public static Color AgkistrodonContortrix(int index)
        {
            if (index % 2 == 1) return Color.Gold;
            return Color.Chocolate;
        }
        public static Color BlackRed(int index)
        {
            if (index % 7 == 1) return Color.Red;
            return Color.Black;
        }
        public static Color BlackYelloy(int index)
        {
            if (index % 2 == 1) return Color.Black;
            return Color.Goldenrod;
        }
        public static Color BlackBlue(int index)
        {
            if (index % 2 == 0) return Color.Black;
            return Color.Blue;
        }
        public static Color LampropeltisTriangulumElapsoides(int index)
        {
            int t = index % 6;
            if (t == 0 || t == 2) return Color.Black;
            else if (t == 1) return Color.Azure;
            else return Color.Maroon;
        }
        public static Color GreenLines(int index)
        {
            if (index / 4 % 2 == 0) return Color.DarkGreen;
            return Color.Green;
        }
        public static Color GrayWaves(int index)
        {
            int t = -(int)(Math.Cos(index * 0.5) * 50 - 50);
            return Color.FromArgb(t, t, t);
        }
    }
}
