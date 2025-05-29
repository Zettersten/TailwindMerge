using System.Runtime.CompilerServices;

namespace TailwindMerge.Utilities;

/// <summary>
/// High-performance thread-safe LRU (Least Recently Used) cache with minimal allocations.
/// Uses LinkedList + Dictionary for O(1) operations and .NET 9 Lock for efficient synchronization.
/// </summary>
/// <typeparam name="TKey">The type of keys in the cache</typeparam>
/// <typeparam name="TValue">The type of values in the cache</typeparam>
internal sealed class LruCache<TKey, TValue>(int capacity)
    where TKey : notnull
{
    private readonly Dictionary<TKey, LinkedListNode<CacheItem>> cache = new(capacity);
    private readonly LinkedList<CacheItem> accessOrder = new();
    private readonly Lock @lock = new();

    public int Capacity { get; } =
        capacity > 0 ? capacity : throw new ArgumentOutOfRangeException(nameof(capacity));
    public int Count => this.cache.Count;

    /// <summary>
    /// Gets a value from the cache, moving it to most recently used position.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool TryGet(TKey key, out TValue? value)
    {
        using (this.@lock.EnterScope())
        {
            if (this.cache.TryGetValue(key, out var node))
            {
                // Move to front (most recently used)
                this.accessOrder.Remove(node);
                this.accessOrder.AddFirst(node);
                value = node.Value.Value;
                return true;
            }
        }

        value = default;
        return false;
    }

    /// <summary>
    /// Gets a value from the cache, or computes and caches it if not present.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
    {
        ArgumentNullException.ThrowIfNull(valueFactory);

        using (this.@lock.EnterScope())
        {
            if (this.cache.TryGetValue(key, out var existingNode))
            {
                // Move to front (most recently used)
                this.accessOrder.Remove(existingNode);
                this.accessOrder.AddFirst(existingNode);
                return existingNode.Value.Value;
            }

            // Not found, compute and add
            var value = valueFactory(key);
            this.AddUnsafe(key, value);
            return value;
        }
    }

    /// <summary>
    /// Adds or updates a key-value pair in the cache.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Set(TKey key, TValue value)
    {
        using (this.@lock.EnterScope())
        {
            if (this.cache.TryGetValue(key, out var existingNode))
            {
                // Update existing
                existingNode.Value = new CacheItem(key, value);
                this.accessOrder.Remove(existingNode);
                this.accessOrder.AddFirst(existingNode);
                return;
            }

            this.AddUnsafe(key, value);
        }
    }

    /// <summary>
    /// Removes a key from the cache.
    /// </summary>
    internal bool Remove(TKey key)
    {
        using (this.@lock.EnterScope())
        {
            if (this.cache.Remove(key, out var node))
            {
                this.accessOrder.Remove(node);
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Clears all items from the cache.
    /// </summary>
    internal void Clear()
    {
        using (this.@lock.EnterScope())
        {
            this.cache.Clear();
            this.accessOrder.Clear();
        }
    }

    /// <summary>
    /// Gets all keys in the cache (snapshot at time of call).
    /// </summary>
    internal TKey[] GetKeys()
    {
        using (this.@lock.EnterScope())
        {
            var keys = new TKey[this.cache.Count];
            var i = 0;
            foreach (var kvp in this.cache)
            {
                keys[i++] = kvp.Key;
            }
            return keys;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void AddUnsafe(TKey key, TValue value)
    {
        // Evict LRU item if at capacity
        if (this.cache.Count >= this.Capacity)
        {
            var lru = this.accessOrder.Last!;
            this.cache.Remove(lru.Value.Key);
            this.accessOrder.RemoveLast();
        }

        // Add new item as most recently used
        var newNode = this.accessOrder.AddFirst(new CacheItem(key, value));
        this.cache[key] = newNode;
    }

    /// <summary>
    /// Cache item record to minimize allocations.
    /// </summary>
    private readonly record struct CacheItem(TKey Key, TValue Value);
}
