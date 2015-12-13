namespace PartsUnlimited.Cache
{
    public class CacheResult<T>
    {
        public CacheResult(T value)
        {
            HasValue = value != null;
            Value= value;
        }

        private CacheResult()
        {
            HasValue = false;
        }

        public bool HasValue { get; }

        public T Value { get; }

        public static CacheResult<T> Empty()
        {
            return new CacheResult<T>();
        }

    }
}