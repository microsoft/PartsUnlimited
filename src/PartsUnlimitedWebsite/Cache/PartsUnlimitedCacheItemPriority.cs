namespace PartsUnlimited.Cache
{
    /// <summary>
    /// Specifies how items are prioritized for preservation during a memory pressure triggered cleanup.
    /// </summary>
    public enum PartsUnlimitedCacheItemPriority
    {
//        Low,
        Normal,
        High,
//        NeverRemove,
    }
}