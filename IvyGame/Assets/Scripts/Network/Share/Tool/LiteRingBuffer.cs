using System;
using System.Collections;
using System.Collections.Generic;

namespace Game.Network
{
    /// <summary>
    /// 环形缓存池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class LiteRingBuffer<T> : IEnumerable<T>
    {
        //元素
        private readonly T[] elements;

        //开始
        private int start;
        //结束
        private int end;
        //数量
        private int count;

        //数量限制
        private readonly int capacity;

        public T this[int i] => elements[(start + i) % capacity];

        public LiteRingBuffer(int count)
        {
            elements = new T[count];
            capacity = count;
        }

        public void Add(T element)
        {
            if (count == capacity)
                throw new ArgumentException();

            elements[end] = element;
            end = (end + 1) % capacity;
            count++;
        }

        public void FastClear()
        {
            start = 0;
            end = 0;
            count = 0;
        }

        public int Count => count;
        public T First => elements[start];
        public T Last => elements[(start + count - 1) % capacity];
        public bool IsFull => count == capacity;

        public void RemoveFromStart(int count)
        {
            if (count > capacity || count > this.count)
                throw new ArgumentException();
            start = (start + count) % capacity;
            this.count -= count;
        }

        public IEnumerator<T> GetEnumerator()
        {
            int counter = start;
            while (counter != end)
            {
                yield return elements[counter];
                counter = (counter + 1) % capacity;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
