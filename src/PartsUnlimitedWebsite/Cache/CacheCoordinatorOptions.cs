namespace PartsUnlimited.Cache
{
    public class CacheCoordinatorOptions
    {
        public CacheCoordinatorOptions WithCacheOptions(PartsUnlimitedCacheOptions options)
        {
            CacheOption = options;
            return this;
        }

        public CacheCoordinatorOptions WhichRemovesIfNull()
        {
            RemoveIfNull = true;
            return this;
        }

        public CacheCoordinatorOptions WhichFailsOver()
        {
            CallFailOverOnError = true;
            return this;
        }

        public bool ShouldPopulateCache => CacheOption != null;
        public bool RemoveIfNull { get; private set; }
        public bool CallFailOverOnError { get; private set; }
        public PartsUnlimitedCacheOptions CacheOption { get; private set; }

    }
}