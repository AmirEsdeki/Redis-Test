namespace Redis_Test
{
    public class DataContainer<T>
    {
        private readonly T _data;
        private readonly string _message;

        public DataContainer(T data, string message)
        {
            _data = data;
            _message = message;
        }

        public T Data => _data;
        public string Message => _message;
    }
}
