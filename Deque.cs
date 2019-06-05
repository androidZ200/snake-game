using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace змейка
{
    public class Deque<T>
    {
        private class Item
        {
            public T value;
            public Item next = null;
            public Item prev = null;

            public Item() { }
            public Item(Item other)
            {
                value = other.value;
            }
            public Item(T value)
            {
                this.value = value;
            }
            public Item(T value, Item next) : this(value)
            {
                this.next = next;
            }
            public Item(T value, Item next, Item prev) : this(value, next)
            {
                this.prev = prev;
            }
        }
        public interface Iiterator
        {
            T Value { get; set; }
            Iiterator Next();
            Iiterator Prev();
            bool Equality(Iiterator other);
        }
        private class iterator : Iiterator
        {
            private Item item;

            public iterator(Item i)
            {
                item = i;
            }

            public T Value
            {
                get { return item.value; }
                set { item.value = value; }
            }

            public bool Equality(Iiterator other)
            {
                return item == ((iterator)other).item;
            }

            public Iiterator Next()
            {
                item = item.next;
                if (item == null) return null;
                return this;
            }
            public Iiterator Prev()
            {
                item = item.prev;
                if (item == null) return null;
                return this;
            }
        }
        private Item head = null;
        private Item tail = null;

        public int Count { get; private set; } = 0;

        public Deque() { }
        public Deque(Deque<T> other)
        {
            if (other.Count == 0) return;
            Item Current = other.head;
            Item Last = other.tail;
            head = new Item(Current);
            Item myCurr = head;
            Count = other.Count;
            do
            {
                Current = Current.next;
                myCurr.next = new Item(Current);
                myCurr.next.prev = myCurr;
                myCurr = myCurr.next;
            } while (Current != Last);
            tail = myCurr;
        }

        public Deque<T> AddHead(T value)
        {
            Item t = new Item(value, head);
            if (Count == 0)
                tail = head = t;
            else
            {
                head.prev = t;
                head = t;
            }
            Count++;
            return this;
        }
        public Deque<T> AddTail(T value)
        {
            Item t = new Item(value, null, tail);
            if (Count == 0)
                tail = head = t;
            else
            {
                tail.next = t;
                tail = t;
            }
            Count++;
            return this;
        }
        public T RemoveHead()
        {
            if (Count >= 1)
            {
                T t = head.value;
                if (Count == 1)
                {
                    head = tail = null;
                }
                else
                {
                    head = head.next;
                    head.prev = null;
                }
                Count--;
                return t;
            }
            throw new InvalidOperationException();
        }
        public T RemoveTail()
        {
            if (Count == 1) return RemoveHead();
            else if (Count > 1)
            {
                T t = tail.value;
                tail = tail.prev;
                tail.next = null;
                Count--;
                return t;
            }
            throw new InvalidOperationException();
        }
        public T Head
        {
            get
            {
                if (Count == 0) throw new InvalidOperationException();
                return head.value;
            }
        }
        public T Tail
        {
            get
            {
                if (Count == 0) throw new InvalidOperationException();
                return tail.value;
            }
        }

        public ref T at(int index)
        {
            if (index < 0 || index >= Count) throw new IndexOutOfRangeException();
            if(index < Count / 2)
            {
                int s = 0;
                Item curr = head;
                while(s++ != index) curr = curr.next;
                return ref curr.value;
            }
            else
            {
                int s = Count - 1;
                Item curr = tail;
                while (s-- != index) curr = curr.prev;
                return ref curr.value;
            }
        }
        public Deque<T> Clear()
        {
            Count = 0;
            head = tail = null;
            return this;
        }
        public bool Find(T elem, Func<T, T, bool> Comparison)
        {
            Item curr = head;
            while (curr != null)
            {
                if (Comparison(elem, curr.value)) return true;
                curr = curr.next;
            }
            return false;
        }
        public Deque<T> Reverse()
        {
            Item curr = head;
            while(curr != null)
            {
                Swap(ref curr.next, ref curr.prev);
                curr = curr.prev;
            }
            Swap(ref head, ref tail);
            return this;
        }
        public Deque<T> Copy(Deque<T> other)
        {
            return new Deque<T>(this);
        }

        public Iiterator GetHead()
        {
            return new iterator(head);
        }
        public Iiterator GetTail()
        {
            return new iterator(tail);
        }

        private void Swap<R> (ref R r1, ref R r2)
        {
            R t = r1;
            r1 = r2;
            r2 = t;
        }
    }
}
