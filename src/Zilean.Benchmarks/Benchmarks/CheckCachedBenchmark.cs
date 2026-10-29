using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Zilean.Benchmarks.Benchmarks;

[SimpleJob]
[MemoryDiagnoser]
public class CheckCachedBenchmark
{
    private HashSet<string> _hashSet = null!;
    private List<CachedItem> _items = null!;

    [Params(10, 100, 1000)]
    public int _n;

    [GlobalSetup]
    public void Setup()
    {
        _hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        _items = new List<CachedItem>();

        for (int i = 0; i < _n; i++)
        {
            var hash = Guid.NewGuid().ToString();
            _hashSet.Add(hash);

            // Suppose half of them are found in DB
            if (i % 2 == 0)
            {
                _items.Add(new CachedItem { InfoHash = hash, IsCached = true });
            }
        }
    }

    [Benchmark(Baseline = true)]
    public List<CachedItem> Original()
    {
        var items = new List<CachedItem>(_items);
        foreach (var hash in _hashSet.Where(hash => items.All(x => !x.InfoHash.Equals(hash, StringComparison.OrdinalIgnoreCase))))
        {
            items.Add(new CachedItem
            {
                InfoHash = hash,
                IsCached = false
            });
        }
        return items;
    }

    [Benchmark]
    public List<CachedItem> Optimized()
    {
        var items = new List<CachedItem>(_items);
        var foundHashes = items.Select(x => x.InfoHash).ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var hash in _hashSet.Where(hash => !foundHashes.Contains(hash)))
        {
            items.Add(new CachedItem
            {
                InfoHash = hash,
                IsCached = false
            });
        }
        return items;
    }
}

public class CachedItem
{
    public string InfoHash { get; set; } = string.Empty;
    public bool IsCached { get; set; }
}
