using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Zilean.Benchmarks.Benchmarks;

[MemoryDiagnoser]
public class MissingHashesCheck
{
    private HashSet<string> _hashSet = null!;
    private List<CachedItem> _items = null!;

    [Params(100, 500)]
    public int HashesCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _hashSet = new HashSet<string>();
        _items = new List<CachedItem>();
        for (int i = 0; i < HashesCount; i++)
        {
            var hash = Guid.NewGuid().ToString("N");
            _hashSet.Add(hash);
            if (i % 2 == 0) // 50% found
            {
                _items.Add(new CachedItem { InfoHash = hash });
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
            });
        }
        return items;
    }

    [Benchmark]
    public List<CachedItem> Optimized()
    {
        var items = new List<CachedItem>(_items);
        var foundHashes = new HashSet<string>(items.Select(x => x.InfoHash), StringComparer.OrdinalIgnoreCase);
        foreach (var hash in _hashSet.Where(hash => !foundHashes.Contains(hash)))
        {
            items.Add(new CachedItem
            {
                InfoHash = hash,
            });
        }
        return items;
    }

    public class CachedItem
    {
        public string InfoHash { get; set; } = null!;
    }
}
