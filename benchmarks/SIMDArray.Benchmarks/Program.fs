module Program

open System.Linq
open System.Linq.Expressions
open System
open System.Threading.Tasks
open System.Threading
open System.Numerics
open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running
open BenchmarkDotNet.Configs
open BenchmarkDotNet.Jobs
open SIMDArrayUtils
open BenchmarkDotNet.Diagnostics.Windows
open System.Collections.Generic

module Array =
    let inline zeroCreateUnchecked (count: int) = Array.zeroCreate count

    let inline subUnchecked startIndex count (array: 'T[]) = Array.sub array startIndex count

// Almost every array function calls this, so mock it with
// the exact same code
let inline checkNonNull argName arg =
    match box arg with
    | null -> nullArg argName
    | _ -> ()

let empty = [||]

let inline indexNotFound () = raise (Exception())

let partition f (array: _[]) =
    checkNonNull "array" array
    let res = Array.zeroCreateUnchecked array.Length
    let mutable upCount = 0
    let mutable downCount = array.Length - 1

    for x in array do
        if f x then
            res.[upCount] <- x
            upCount <- upCount + 1
        else
            res.[downCount] <- x
            downCount <- downCount - 1

    let res1 = Array.subUnchecked 0 upCount res
    let res2 = Array.zeroCreateUnchecked (array.Length - upCount)

    downCount <- array.Length - 1

    for i = 0 to res2.Length - 1 do
        res2.[i] <- res.[downCount]
        downCount <- downCount - 1

    res1, res2

[<GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)>]
[<CategoriesColumn>]
[<MemoryDiagnoser>]
type CoreBenchmark() =
    let mutable arrayInt = [||]
    let mutable arrayInt2 = [||]
    let mutable arrayFloat = [||]
    let mutable arrayFloat2 = [||]

    [<Params(100, 1000, 1000000)>]
    member val public Length = 0 with get, set

    member val public Half = Int32.MaxValue / 2

    [<GlobalSetup>]
    member self.SetupData() =
        let r = Random(self.Length)
        arrayInt <- Array.init self.Length (fun i -> r.Next())
        arrayInt2 <- Array.init self.Length (fun i -> r.Next())
        arrayFloat <- Array.init self.Length (fun i -> r.NextDouble())
        arrayFloat2 <- Array.init self.Length (fun i -> r.NextDouble())

    [<BenchmarkCategory("MapSquare", "Int")>]
    [<Benchmark(Baseline = true)>]
    member self.MapSquare() = arrayInt |> Array.map (fun x -> x * x)

    [<BenchmarkCategory("MapSquare", "Int")>]
    [<Benchmark>]
    member self.MapSquareSIMD() =
        arrayInt |> Array.SIMD.map (fun x -> x * x) (fun x -> x * x)

    [<BenchmarkCategory("MapSquare", "Float")>]
    [<Benchmark(Baseline = true)>]
    member self.MapSquareFloat() =
        arrayFloat |> Array.map (fun x -> x * x)

    [<BenchmarkCategory("MapSquare", "Float")>]
    [<Benchmark>]
    member self.MapSquareSIMDFloat() =
        arrayFloat |> Array.SIMD.map (fun x -> x * x) (fun x -> x * x)

    [<BenchmarkCategory("Dot")>]
    [<Benchmark(Baseline = true)>]
    member self.Dot() =
        arrayInt |> Array.fold2 (fun a x y -> a + x * y) 0 arrayInt2

    [<BenchmarkCategory("Dot")>]
    [<Benchmark>]
    member self.DotSIMD() = arrayInt |> Array.SIMD.dot arrayInt2

    [<BenchmarkCategory("Max", "Int")>]
    [<Benchmark(Baseline = true)>]
    member self.Max() = arrayInt |> Array.max

    [<BenchmarkCategory("Max", "Int")>]
    [<Benchmark>]
    member self.MaxSIMD() = arrayInt |> Array.SIMD.max

    [<BenchmarkCategory("Max", "Float")>]
    [<Benchmark(Baseline = true)>]
    member self.MaxFloat() = arrayFloat |> Array.max

    [<BenchmarkCategory("Max", "Float")>]
    [<Benchmark>]
    member self.MaxSIMDFloat() = arrayFloat |> Array.SIMD.max

    [<BenchmarkCategory("MaxBy")>]
    [<Benchmark(Baseline = true)>]
    member self.MaxBy() =
        arrayInt |> Array.maxBy (fun x -> x * x)

    [<BenchmarkCategory("MaxBy")>]
    [<Benchmark>]
    member self.MaxBySIMD() =
        arrayInt |> Array.SIMD.maxBy (fun x -> x * x) (fun x -> x * x)

    [<BenchmarkCategory("Min")>]
    [<Benchmark(Baseline = true)>]
    member self.Min() = arrayInt |> Array.min

    [<BenchmarkCategory("Min")>]
    [<Benchmark>]
    member self.MinSIMD() = arrayInt |> Array.SIMD.min

    [<BenchmarkCategory("MinBy")>]
    [<Benchmark(Baseline = true)>]
    member self.MinBy() =
        arrayInt |> Array.minBy (fun x -> x - 82)

    [<BenchmarkCategory("MinBy")>]
    [<Benchmark>]
    member self.MinBySIMD() =
        arrayInt |> Array.SIMD.minBy (fun x -> x - Vector<int>(82)) (fun x -> x - 82)

    [<BenchmarkCategory("Map", "Int")>]
    [<Benchmark(Baseline = true)>]
    member self.Map() =
        arrayInt |> Array.map (fun x -> x + 2 * x)

    [<BenchmarkCategory("Map", "Int")>]
    [<Benchmark>]
    member self.MapSIMD() =
        arrayInt |> Array.SIMD.map (fun x -> x + 2 * x) (fun x -> x + 2 * x)

    [<BenchmarkCategory("Map", "Float")>]
    [<Benchmark(Baseline = true)>]
    member self.MapFloat() =
        arrayFloat |> Array.map (fun x -> x + 2. * x)

    [<BenchmarkCategory("Map", "Float")>]
    [<Benchmark>]
    member self.MapSIMDFloat() =
        arrayFloat |> Array.SIMD.map (fun x -> x + 2. * x) (fun x -> x + 2. * x)

    [<BenchmarkCategory("Fold")>]
    [<Benchmark(Baseline = true)>]
    member self.Fold() =
        (0, arrayInt) ||> Array.fold (fun acc x -> x + acc)

    [<BenchmarkCategory("Fold")>]
    [<Benchmark>]
    member self.FoldSIMD() =
        let inline fn acc x = x + acc
        (0, arrayInt) ||> Array.SIMD.fold fn fn (+)

    [<BenchmarkCategory("Partition")>]
    [<Benchmark(Baseline = true)>]
    member self.Partition() =
        arrayInt |> Array.partition (fun x -> x > self.Half)

    [<BenchmarkCategory("Partition")>]
    [<Benchmark>]
    member self.PartitionPerformance() =
        arrayInt |> Array.Performance.partitionUnordered (fun x -> x > self.Half)

    [<BenchmarkCategory("Filter", "Int")>]
    [<Benchmark(Baseline = true)>]
    member self.Filter() =
        arrayInt |> Array.filter (fun x -> x % 2 = 0)

    [<BenchmarkCategory("Filter", "Int")>]
    [<Benchmark>]
    member self.FilterPerformance() =
        arrayInt |> Array.Performance.filterSimplePredicate (fun x -> x % 2 = 0)

    [<BenchmarkCategory("Filter", "Float")>]
    [<Benchmark(Baseline = true)>]
    member self.FilterFloat() =
        arrayFloat |> Array.filter (fun x -> x % 2. = 0)

    [<BenchmarkCategory("Filter", "Float")>]
    [<Benchmark>]
    member self.FilterPerformanceFloat() =
        arrayFloat |> Array.Performance.filterSimplePredicate (fun x -> x % 2. = 0)

[<EntryPoint>]
let main _argv =
    let _ = BenchmarkRunner.Run<CoreBenchmark>()
    0
