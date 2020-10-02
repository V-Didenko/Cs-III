using System;
using System.Collections.Generic;
using System.Text;

namespace MyLibrary
{
    using System.Collections.Generic;

    public class MyQueue<T> : ICollection, ICloneable
    {
        public int Count { get; private set; }

        public event Action<MyQueue<T>> OnEnqueue;
        public event Action<MyQueue<T>> OnDequeue;
        public event Action OnСlear;

        public MyQueue() { }

        public MyQueue(IEnumerable<T> array)
        {
            foreach (var el in array)
            {
                Enqueue(el);
            }
        }

        class Node
        {
            public Node Next { get; set; }
            public T Value { get; }
            public Node(T value)
            {
                Value = value;
            }
        }

        private Node head;
        private Node tail;

        public object Clone()
        {
            var copyQueue = new MyQueue<T>();
            foreach (var val in this)
            {
                copyQueue.Enqueue((T)val);
            }
            return copyQueue;
        }

        public void CopyTo(Array array, int index)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (Count == 0) return;
            var queueArr = ToArray();
            var length = array.Length - index > queueArr.Length ? queueArr.Length : array.Length - index;
            if (length < 0) throw new ArgumentOutOfRangeException(nameof(array));
            Array.Copy(queueArr, 0, array, index, length);
        }

        public T Peek()
        {
            if (head == null) throw new InvalidOperationException("Queue is empty");
            return head.Value;
        }

        public void Enqueue(T value)
        {
            var newNode = new Node(value);

            if (tail == null) head = newNode;
            else tail.Next = newNode;

            tail = newNode;
            Count++;
            OnEnqueue?.Invoke(this);
        }

        public T Dequeue()
        {
            if (head == null) throw new InvalidOperationException("Queue is empty");

            var res = head.Value;
            var nextHead = head.Next;
            head = nextHead;
            if (nextHead == null) tail = null;

            Count--;
            OnDequeue?.Invoke(this);
            return res;
        }

        public void Clear()
        {
            head = tail = null;
            Count = 0;
            OnСlear?.Invoke();
        }

        public T[] ToArray()
        {
            var amount = Count;
            var arr = new T[amount];
            for (var i = 0; i < amount; i++)
            {
                arr[i] = Dequeue();
            }

            return arr;
        }

        private class QueueEnumerator : IEnumerator
        {
            private MyQueue<T> queue;
            private Node current;
            public QueueEnumerator(MyQueue<T> queue)
            {
                this.queue = queue;
                current = null;
            }
            public object Current => current.Value;
            public bool MoveNext()
            {
                if (queue.head == null) return false;
                if (current == null)
                {
                    current = queue.head;
                    return true;
                }
                if (current.Next == null) return false;

                var next = current.Next;
                current = next;
                return true;
            }

            public void Reset()
            {
                current = null;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return new QueueEnumerator(this);
        }

        public override string ToString()
        {
            var result = "";
            foreach (var val in this)
            {
                result += val + " ";
            }

            return result;
        }

        public bool IsSynchronized => true;

        public object SyncRoot => true;
    }
}
