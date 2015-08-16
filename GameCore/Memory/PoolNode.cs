namespace GameCore.Memory
{
    internal class PoolNode<T>
    {
        private PoolNode<T> _next;
        private T _value;

        public PoolNode()
        {
            _next = null;
            _value = default(T);
        }

        public PoolNode(PoolNode<T> next, T value)
        {
            _next = next;
            _value = value;
        }

        public PoolNode<T> GetNext()
        {
            return _next;
        }

        public T GetValue()
        {
            return _value;
        }

        public void SetNext(PoolNode<T> next)
        {
            _next = next;
        }

        public void SetValue(T value)
        {
            _value = value;
        }
    }
}
