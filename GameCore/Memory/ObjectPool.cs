using System;
using System.Collections.Generic;

namespace GameCore.Memory
{
    public class ObjectPool<T>
    {
        private const string EXCEPTION_POP_INVALID  = "There are no elements in the pool left";
        private const string EXCEPTION_INC_RANGE    = "Maximum size of the pool must be greater than 0";
        private const string EXCEPTION_PUSH_NULL    = "Cannot add a null reference";
        private const string EXCEPTION_PUSH_MEMORY  = "The pool has reached capacity";

        private PoolNode<T> _head;
        private int _size;
        private int _max;

        public ObjectPool(int max)
        {
            _size = 0;
            _max = max;
        }

        public int Size()
        {
            return _size;
        }

        public int Max()
        {
            return _max;
        }

        public bool Full()
        {
            return _max == _size;
        }

        public T Pop()
        {
            if (_size == 0)
                throw new InvalidOperationException(EXCEPTION_POP_INVALID);

            _size--;
            PoolNode<T> head = _head;
            _head = head.GetNext();

            return head.GetValue();
        }

        public void Increase(int max)
        {
            if (max <= 0)
                throw new ArgumentOutOfRangeException(EXCEPTION_INC_RANGE);

            _max = max;
        }

        public void Push(T value)
        {
            if (value == null)
                throw new ArgumentNullException(EXCEPTION_PUSH_NULL);

            if (_max == _size)
                throw new OutOfMemoryException(EXCEPTION_PUSH_MEMORY);
            
            _size++;
            PoolNode<T> node = new PoolNode<T>(_head, value);
            _head = node;
        }
    }
}
