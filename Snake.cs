using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace змейка
{
    class Snake<T>
    {
        public enum Rotate { Up, Right, Down, Left }

        public Deque<T> Body { get; private set; } = new Deque<T>();
        public Rotate angle { get; private set; }
        private T last;
        private Func<int, Color> skin = (x => Color.Black);
        private Func<T, Rotate, T> nextMove;
        private Func<T, T, Rotate> GetAngle;

        public Snake(T head, Rotate angle, int length, 
            Func<T, Rotate, T> nextMove, Func<T, T, Rotate> GetAngle)
        {
            this.angle = angle;
            this.nextMove = nextMove;
            this.GetAngle = GetAngle;
            for (int i = 0; i < length; i++)
            {
                Body.AddTail(head);
                head = nextMove(head, (Rotate)(((int)angle + 2) % 4));
            }
        }
        public Snake(T head, Rotate angle, int length, 
            Func<T, Rotate, T> nextMove, Func<T, T, Rotate> GetAngle, Func<int, Color> Skin)
            : this(head, angle, length, nextMove, GetAngle)
        {
            skin = Skin;
        }

        public Snake<T> Move()
        {
            last = Body.RemoveTail();
            return MoveAndEat();
        }
        public Snake<T> Move(T nextHead)
        {
            Body.AddHead(nextHead);
            last = Body.RemoveTail();
            return this;
        }
        public Snake<T> ReversAndMove()
        {
            angle = GetAngle(Body.Tail, last);
            Body.AddTail(last);
            last = Body.RemoveHead();
            Body.Reverse();
            return this;
        }
        public Snake<T> MoveAndEat()
        {
            Body.AddHead(nextMove(Body.Head, angle));
            return this;
        }
        public Snake<T> ChangeAngle(bool isRight)
        {
            if (isRight) angle++;
            else angle--;
            if ((int)angle >= 4) angle = Rotate.Up;
            if (angle < 0) angle = Rotate.Left;
            return this;
        }
        public Snake<T> NewSkin(Func<int, Color> skin)
        {
            this.skin = skin;
            return this;
        }

        public T GetBody(int index)
        {
            return Body.at(index);
        }
        public int Length
        {
            get { return Body.Count; }
        }
        public Color GetColor(int index)
        {
            return skin(index);
        }
    }
}
