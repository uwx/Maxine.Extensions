# Maxine.Extensions

A collection of high-performance .NET 10 utility libraries covering extension methods, collections, virtual file systems, Lua interop, HTTP, math, key-value databases, and more.

---

## Packages

### `Maxine.Extensions` — Core utilities

The main library. Targets `net10.0` with nullable reference types and unsafe blocks enabled.

**Extension methods**
| File | What it extends |
|---|---|
| `ArrayExtensions` | Arrays (fast copy, etc.) |
| `AsyncEnumerableExtensions` | `IAsyncEnumerable<T>` |
| `BclExtensions` | Miscellaneous BCL types |
| `CancellationTokenExtensions` | `CancellationToken` |
| `DebounceExtensions` | `Action`/`Func` debouncing |
| `DictionarySlimExtensions` | `DictionarySlim<K,V>` |
| `EnumExtensions` | `Enum` |
| `EnumerableExtensions` / `LinqExtensions` | `IEnumerable<T>` |
| `LinqExtensions_GenericMath` | LINQ with generic math operators |
| `LinqExtensions_FastReverseIterate` | Reverse iteration without allocation |
| `LinqMathExtensions` | Numeric LINQ aggregates |
| `LockExtensions` | `Monitor`/`SemaphoreSlim` |
| `MsdiExtensions` / `MsdiProviderExtensions` | `IServiceCollection` / `IServiceProvider` |
| `MultiValueDictionaryExtensions` | `MultiValueDictionary<K,V>` |
| `ReflectionExtensions` | `Type`, `MethodInfo`, etc. |
| `RegexExtensions` | `Regex` |
| `SequenceExtensions` | `ReadOnlySequence<T>` |
| `SpanExtensions` | `Span<T>` / `ReadOnlySpan<T>` (including enumerators) |
| `StringBuilderExtensions` | `StringBuilder` |
| `StringExtensions` / `StringSegmentExtensions` | `string` / `StringSegment` |
| `TaskExtensions` | `Task` / `ValueTask` |
| `TypeExtensions` | `Type` |

**Collections**
| Type | Description |
|---|---|
| `SwissTable<TKey,TValue>` | High-performance open-addressing hash map (Swiss-table algorithm) |
| `ChunkedSequence<T>` | Append-only sequence backed by fixed-size chunks |
| `CooldownDictionary<TKey>` | Dictionary with per-key cooldown/expiry |
| `EnumMap<TEnum,TValue>` | Array-backed map keyed by enum |
| `KeyedCollection<TKey,TValue>` | `Collection<T>` with key lookup |
| `KeyedObjectPool<TKey,TValue>` | Per-key object pool |
| `LazyResolve<T>` | Lazy resolution from a service provider |
| `MutableArray<T>` | Mutable wrapper around a fixed array |
| `ObservableCircularBuffer<T>` | Observable fixed-capacity ring buffer |
| `ObservableOrderedDictionary<TKey,TValue>` | Observable dictionary with insertion-order enumeration |
| `SpanLinq` | LINQ-style operators on spans without heap allocations |
| `UniquePrefixTrie` | Trie for unique-prefix lookups |
| `FakeList<T>` | `IList<T>` adapter over `IReadOnlyList<T>` |
| `ArrayOfVector2` / `ArrayOfVector3` | Struct arrays for 2D/3D vectors |

**Caching**
- Strongly-typed `MemoryCache<TKey,TValue>` wrapping `IMemoryCache`
- `MemoryCacheEntryOptions<TKey,TValue>` for typed eviction callbacks

**I/O**
- `LineReader` — zero-alloc line reader over `ReadOnlySequence<byte>`
- `SpanReader` — forward-reading span helper
- `TemporaryFile` / `TemporaryDirectory` — RAII temp file/directory wrappers
- `NoDisposeStream` — prevents disposal of a wrapped stream
- `ProjectUtils` — locates project/solution root at runtime

**JSON converters** (System.Text.Json)
- `MultiValueDictionaryConverterFactory`
- `OrderedDictionaryConverterFactory`
- `TupleAsArrayConverter` (T4-generated, supports up to N-tuples as JSON arrays)

**Math utilities**
- `Angle`, `Length`, `Speed` — units with implicit conversions
- `BoundingBox`, `LatLon`, `Instant` — geometric/geographic value types
- `Math2` — extra math helpers beyond `System.Math`
- `PackedIntArray` — bit-packed integer array
- `VarInt` — variable-length integer encoding
- `WeightedRandomizer<T>` — weighted random selection
- `ThreadLocalRandom` — thread-safe random number generation
- `ArbitraryBase` / `RomanUtilities` — number formatting helpers

**Memory utilities**
- `ValueStringBuilder`, `ValueArrayBuilder<T>`, `ValueBytesBuilder` — stack-based builders to avoid allocation
- `NativeMemoryHandle` / `NativeMemoryStream` — unmanaged memory wrappers
- `UnmanagedBufferAllocator` — pooled unmanaged buffer allocator
- `OwnedMemoryStream` / `NonAllocatingPool<T>` — stream/pool utilities
- `BitHelpers` — bit manipulation helpers

**Other**
- `BenchmarkTimer` — `using`-scope stopwatch that prints elapsed time
- `HashUtilities` — FNV and other hash helpers
- `Skip32Cipher` — lightweight 32-bit block cipher
- `Nibble` — 4-bit value type
- `AsciiComparer` — fast ASCII string comparer
- `PooledArrayBufferWriter<T>` — `IBufferWriter<T>` backed by a pooled array
- `ValueTaskAsyncLock` — async lock returning `ValueTask`
- `UrlUtil` — URL manipulation helpers
- `StartupTask` — fire-and-forget startup task abstraction

---

### `Maxine.Extensions.Mathematics` — 3D/2D math

A comprehensive math library for graphics and game-related workloads, derived from the [Stride](https://www.stride3d.net/) engine's math library.

**Types**
- Vectors: `Vector2`, `Vector3`, `Vector4`, `Double2/3/4`, `Int2/3/4`, `UInt4`, `Half2/3/4`
- Matrix/Quaternion: `Matrix`, `Quaternion`
- Color: `Color`, `Color3`, `Color4`, `ColorBGRA`, `ColorHSV`
- Geometry: `Plane`, `Ray`, `BoundingBox`, `BoundingSphere`, `BoundingFrustum`, `Rectangle`, `RectangleF`, `Point`, `Size2/2F/3`
- Helpers: `MathUtil`, `CollisionHelper`, `VectorExtensions`, `ColorExtensions`, `HalfUtils`, `GuillotinePacker`, `SphericalHarmonics`
- Angles: `AngleSingle` (radians/degrees/gradians), `AngleType`

---

### `Maxine.Extensions.Lua` — Lua interop

Bindings to the Lua C API for all major Lua versions and LuaJIT, compiled against the native libraries directly.

**Version packages**

| Package | Lua version |
|---|---|
| `Maxine.Extensions.Lua50` | Lua 5.0 |
| `Maxine.Extensions.Lua51` | Lua 5.1 |
| `Maxine.Extensions.Lua52` | Lua 5.2 |
| `Maxine.Extensions.Lua53` | Lua 5.3 |
| `Maxine.Extensions.Lua54` | Lua 5.4 |
| `Maxine.Extensions.Lua55` | Lua 5.5 |
| `Maxine.Extensions.LuaJIT` | LuaJIT 2.1 |

Each version package exposes the raw C API via unmanaged function pointers (`delegate* unmanaged[Cdecl]`).

**High-level wrappers** (in `Maxine.Extensions.Lua`)

`LuaValue` — abstract base for Lua values that can be pushed/popped from the stack:
- `LuaBoolean`, `LuaNumber`, `LuaString`, `LuaLightUserdata`
- `LuaTable` — implements `IDictionary<LuaValue, LuaValue>` with full iteration
- `LuaFunction` — callable via `Call(params ReadOnlySpan<LuaValue>)`, throws on Lua error
- `LuaUserdata`, `LuaThread`

**NLua / KeraLua integration**
- `Maxine.Extensions.Lua54.KeraLua` / `Maxine.Extensions.Lua55.KeraLua` — KeraLua-based wrappers
- `Maxine.Extensions.Lua54.NLua` / `Maxine.Extensions.Lua55.NLua` — NLua-based wrappers

**`Maxine.Extensions.Lua.Generator`**
Source generator that produces P/Invoke bindings from Lua headers using ClangSharp's `PInvokeGenerator`. Includes pre-generated sources for all supported versions.

---

### `Maxine.VFS` — Virtual File System

An `IFileSystem` abstraction that unifies access to multiple storage backends.

**Implementations**
| Class | Backed by |
|---|---|
| `IoFileSystem` | `System.IO` (real disk) |
| `MemoryFileSystem` | In-memory dictionary |
| `HttpFileSystem` | HTTP GET requests |
| `ZipFileSystem` | ZIP archives |
| `MountingFileSystem` | Multiple file systems mounted at paths |
| `FallbackFileSystem` | Primary + fallback file system |
| `WritableFallbackFileSystem` | Read-only primary, writable fallback |
| `RelativeReadOnlyFileSystem` | Read-only view scoped to a subdirectory |
| `NullFileSystem` | Discards all writes, returns nothing on reads |

**Path handling** — `IPath` interface with two implementations:
- `IPath.IoPath` — delegates to `System.IO.Path`
- `IPath.MemoryPath` — portable forward-slash paths with `..` resolution

---

### `Maxine.Fetch` — HTTP client

A `fetch()`-inspired HTTP API modeled after the browser's [Fetch API](https://developer.mozilla.org/en-US/docs/Web/API/Fetch_API).

```csharp
var response = await Fetch.FetchAsync("https://example.com", new RequestNoUri
{
    Method = HttpMethod.Post,
    Body = RequestBody.Json(new { hello = "world" })
});

if (response.Ok)
{
    var text = await response.Text();
    var obj  = await response.Json<MyType>();
}
```

**Key types**
- `Fetch.FetchAsync(...)` — static entry point
- `Request` / `RequestNoUri` — request builder (extends `HttpRequestMessage`)
- `RequestBody` — fluent body factory; accepts `string`, `byte[]`, `Stream`, JSON, or multipart `FormData`
- `Response` — wraps `HttpResponseMessage`; exposes `Ok`, `Status`, `Headers`, `Body()`, `Bytes()`, `Text()`, `Json<T>()`

---

### `Maxine.Extensions.KeyValue` — Key-value database

A typed abstraction over embedded key-value stores with pluggable serialization.

**Backends**
- `RocksKeyValueDb` — backed by [RocksDB](https://rocksdb.org/)
- `ZoneTreeKeyValueDb` — backed by [ZoneTree](https://github.com/koculu/ZoneTree)

**Serialization formats** (per table, for both keys and values)
- Default (primitive/BCL types)
- MessagePack
- FlatBuffers (via FlatSharp)

**Fluent builder API**
```csharp
IKeyValueDb db = new RocksKeyValueDb("path/to/db");

IKeyValues<string, MyData> table = db
    .Table("users")
    .WithKey<string>()
    .WithMsgpackValue<MyData>();
```

---

### `Maxine.Extensions.EntityFrameworkCore` — EF Core helpers

Utilities for Entity Framework Core:
- `ValueConverterEx` — base class for custom value converters with less boilerplate
- `ConversionAttribute` — attribute to declare value converter on a property
- `CustomModelAttributeExtensions` — `IModelBuilder` extensions to apply `ConversionAttribute` automatically

---

### `Maxine.Extensions.Expressions` — LINQ expression utilities

A fork/adaptation of [Mono.Linq.Expressions](https://github.com/jbevain/mono.linq.expressions). Provides:
- Custom expression nodes: `DoWhileExpression`, `ForExpression`, `ForEachExpression`, `WhileExpression`, `UsingExpression`
- `PredicateBuilder` — `And`/`Or` predicate composition
- `ExpressionExtensions` / `FluentExtensions` — fluent expression building helpers
- `CSharp` / `CSharpWriter` — round-trip expression-to-C# text rendering
- `DelegateConverter` — compile expressions to typed delegates

---

### `Maxine.Extensions.MessagePack` — MessagePack extensions

MessagePack serialization helpers:
- `InlineArrayResolver` / `InlineArrays` — T4-generated formatters for `System.Runtime.CompilerServices.InlineArray` types
- `DedupingResolver` — deduplication of identical objects during serialization
- `CycleResolver` — cycle detection to avoid infinite serialization loops
- `UnsafeUnmanagedStructListFormatter` — zero-copy formatter for lists of unmanaged structs

---

### `Maxine.Extensions.SkiaSharp` — SkiaSharp helpers

Extensions for [SkiaSharp](https://github.com/mono/SkiaSharp):
- `BitmapUtils` — `SKBitmap` conversion helpers
- `CustomSKShaper` — extended text shaping utilities
- `Graphics/` — additional drawing helpers

---

### `Maxine.LogViewer` — WPF log viewer

A lightweight real-time log viewer for WPF applications.

**`Maxine.LogViewer.Sink`** (`net10.0-windows`)
- `WpfLoggerProvider` — `ILoggerProvider` that routes log entries to an in-process broker
- `IWpfLogBroker` — interface for forwarding structured log entries to a UI
- `WpfLoggerProviderConfigurationExtensions` — `ILoggingBuilder.AddWpfLogger()` extension

**`Maxine.LogViewer.Desktop`** (`net10.0-windows`)
- Standalone WPF window (`MainWindow`) displaying scrollable, filterable log output
- `CircularBufferView` — live circular-buffer backed `ItemsControl`
- `LogFormatter` / `StringMatching` — formatting and substring-highlight helpers

---

## Requirements

- .NET 10 SDK
- Windows required for `Maxine.LogViewer.*` (WPF)
- Native Lua shared libraries required for `Maxine.Extensions.Lua*` packages

## License

See [LICENSE.Stride.txt](LICENSE.Stride.txt) for the Stride-derived mathematics code. Other packages are not separately licensed in this repository.
