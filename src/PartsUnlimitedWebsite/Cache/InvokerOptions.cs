using System.Runtime.Remoting.Channels;
using Microsoft.AspNet.Razor.Tokenizer;

namespace PartsUnlimited.Cache
{
    public class InvokerOptions
    {
        public InvokerOptions()
        {
        }

        public InvokerOptions WithCacheOptions(PartsUnlimitedCacheOptions options)
        {
            CacheOption = options;
            return this;
        }

        public InvokerOptions WhichRemovesIfNull()
        {
            RemoveIfNull = true;
            return this;
        }

        public bool ShouldPopulateCache => CacheOption != null;
        public bool RemoveIfNull { get; private set; }
        public PartsUnlimitedCacheOptions CacheOption { get; private set; }

    }
}