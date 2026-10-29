```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Xeon Processor 2.30GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.103
  [Host]     : .NET 10.0.3 (10.0.326.7603), X64 RyuJIT AVX2
  DefaultJob : .NET 10.0.3 (10.0.326.7603), X64 RyuJIT AVX2


```
| Method           | IterationCount | Mean           | Error        | StdDev       | Ratio | Gen0   | Gen1   | Allocated | Alloc Ratio |
|----------------- |--------------- |---------------:|-------------:|-------------:|------:|-------:|-------:|----------:|------------:|
| **Original**         | **10**             |     **1,255.9 ns** |      **6.93 ns** |      **6.15 ns** |  **1.00** | **0.0610** |      **-** |   **1.44 KB** |        **1.00** |
| OptimizedHashSet | 10             |       981.1 ns |      9.89 ns |      9.25 ns |  0.78 | 0.0439 |      - |   1.01 KB |        0.70 |
| OptimizedExcept  | 10             |     1,157.8 ns |     10.54 ns |      9.34 ns |  0.92 | 0.0572 |      - |   1.34 KB |        0.93 |
|                  |                |                |              |              |       |        |        |           |             |
| **Original**         | **100**            |    **46,433.1 ns** |    **192.90 ns** |    **161.08 ns** |  **1.00** | **0.4883** |      **-** |  **11.98 KB** |        **1.00** |
| OptimizedHashSet | 100            |     7,194.6 ns |    140.24 ns |    161.50 ns |  0.15 | 0.2899 |      - |   6.77 KB |        0.56 |
| OptimizedExcept  | 100            |     9,130.1 ns |    177.02 ns |    181.79 ns |  0.20 | 0.4578 |      - |  10.61 KB |        0.89 |
|                  |                |                |              |              |       |        |        |           |             |
| **Original**         | **1000**           | **4,102,666.1 ns** | **19,531.23 ns** | **15,248.70 ns** |  **1.00** |      **-** |      **-** | **117.46 KB** |        **1.00** |
| OptimizedHashSet | 1000           |    73,032.0 ns |    539.24 ns |    504.41 ns |  0.02 | 2.8076 | 0.4883 |  65.26 KB |        0.56 |
| OptimizedExcept  | 1000           |    89,319.2 ns |    847.52 ns |    707.72 ns |  0.02 | 4.3945 | 0.8545 | 102.97 KB |        0.88 |
