using System.Buffers;
using MessagePack;
using MessagePack.Resolvers;
using Maxine.Extensions.MessagePack;

namespace Maxine.Extensions.Test.MessagePack;

[TestClass]
public class CycleResolverTests
{
    [MessagePackObject(AllowPrivate = true)]
    internal class TestClass
    {
        [Key(0)] public int Value { get; set; }
        [Key(1)] public string? Name { get; set; }
        [Key(2)] public TestClass? Reference { get; set; }
    }

    [MessagePackObject(AllowPrivate = true)]
    internal class CircularReferenceClass
    {
        [Key(0)] public int Id { get; set; }
        [Key(1)] public CircularReferenceClass? Parent { get; set; }
        [Key(2)] public CircularReferenceClass? Child { get; set; }
    }

    [MessagePackObject(AllowPrivate = true)]
    internal class ComplexObject
    {
        [Key(0)] public List<string>? Items { get; set; }
        [Key(1)] public Dictionary<string, int>? Data { get; set; }
    }

    [TestMethod]
    public void Create_ReturnsNewInstance()
    {
        var resolver = CycleResolver.Create(StandardResolver.Instance);

        Assert.IsNotNull(resolver);
        Assert.IsInstanceOfType<CycleResolver<StandardResolver>>(resolver);
    }

    [TestMethod]
    public void GetFormatter_ForValueType_ReturnsDelegatedFormatter()
    {
        var resolver = CycleResolver.Create(StandardResolver.Instance);
        
        var formatter = resolver.GetFormatter<int>();

        Assert.IsNotNull(formatter);
    }

    [TestMethod]
    public void GetFormatter_ForReferenceType_ReturnsDedupingFormatter()
    {
        var resolver = CycleResolver.Create(StandardResolver.Instance);
        
        var formatter = resolver.GetFormatter<string>();

        Assert.IsNotNull(formatter);
    }

    [TestMethod]
    public void SerializeDeserialize_Null_ReturnsNull()
    {
        var resolver = CycleResolver.Create(StandardResolver.Instance);
        var options = MessagePackSerializerOptions.Standard.WithResolver(resolver);

        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        var formatter = resolver.GetFormatter<TestClass>();
        
        formatter!.Serialize(ref writer, null!, options);
        writer.Flush();

        var reader = new MessagePackReader(buffer.WrittenMemory);
        var result = formatter.Deserialize(ref reader, options);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void SerializeDeserialize_SimpleObject_RoundTripsCorrectly()
    {
        var resolver = CycleResolver.Create(StandardResolver.Instance);
        var options = MessagePackSerializerOptions.Standard.WithResolver(resolver);

        var original = new TestClass { Value = 42, Name = "Test" };
        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        var formatter = resolver.GetFormatter<TestClass>();
        
        formatter!.Serialize(ref writer, original, options);
        writer.Flush();

        var reader = new MessagePackReader(buffer.WrittenMemory);
        var result = formatter.Deserialize(ref reader, options);

        Assert.IsNotNull(result);
        Assert.AreEqual(original.Value, result!.Value);
        Assert.AreEqual(original.Name, result.Name);
    }

    [TestMethod]
    public void Serialize_DuplicateReference_WritesObjectOnceAndReferenceForDuplicate()
    {
        var resolver = CycleResolver.Create(StandardResolver.Instance);
        var options = MessagePackSerializerOptions.Standard.WithResolver(resolver);

        var sharedObject = new TestClass { Value = 100, Name = "Shared" };
        var container = new[] { sharedObject, sharedObject }; // Same reference twice

        var serialized = MessagePackSerializer.Serialize(container, options);
        
        // The second occurrence should be smaller (just a reference)
        Assert.IsNotNull(serialized);
        Assert.IsTrue(serialized.Length > 0);
    }

    [TestMethod]
    public void Deserialize_DuplicateReference_RestoresSameInstance()
    {
        var resolver = CycleResolver.Create(StandardResolver.Instance);
        var options = MessagePackSerializerOptions.Standard.WithResolver(resolver);

        var sharedObject = new TestClass { Value = 100, Name = "Shared" };
        var container = new[] { sharedObject, sharedObject }; // Same reference twice

        var serialized = MessagePackSerializer.Serialize(container, options);
        var deserialized = MessagePackSerializer.Deserialize<TestClass[]>(serialized, options);

        Assert.IsNotNull(deserialized);
        Assert.AreEqual(2, deserialized!.Length);
        Assert.AreSame(deserialized[0], deserialized[1]); // Should be same instance
        Assert.AreEqual(100, deserialized[0]!.Value);
        Assert.AreEqual("Shared", deserialized[0]!.Name);
    }

    [TestMethod]
    public void SerializeDeserialize_CircularReference_HandlesCorrectly()
    {
        var resolver = CycleResolver.Create(StandardResolver.Instance);
        var options = MessagePackSerializerOptions.Standard.WithResolver(resolver);

        var parent = new CircularReferenceClass { Id = 1 };
        var child = new CircularReferenceClass { Id = 2, Parent = parent };
        parent.Child = child;

        var serialized = MessagePackSerializer.Serialize(parent, options);
        var deserialized = MessagePackSerializer.Deserialize<CircularReferenceClass>(serialized, options);

        Assert.IsNotNull(deserialized);
        Assert.AreEqual(1, deserialized!.Id);
        Assert.IsNotNull(deserialized.Child);
        Assert.AreEqual(2, deserialized.Child!.Id);
        Assert.AreSame(deserialized, deserialized.Child!.Parent); // Circular reference preserved
    }

    [TestMethod]
    public void SerializeDeserialize_MultipleDifferentObjects_EachStoredSeparately()
    {
        var resolver = CycleResolver.Create(StandardResolver.Instance);
        var options = MessagePackSerializerOptions.Standard.WithResolver(resolver);

        var obj1 = new TestClass { Value = 1, Name = "First" };
        var obj2 = new TestClass { Value = 2, Name = "Second" };
        var obj3 = new TestClass { Value = 3, Name = "Third" };
        var container = new[] { obj1, obj2, obj3 };

        var serialized = MessagePackSerializer.Serialize(container, options);
        var deserialized = MessagePackSerializer.Deserialize<TestClass[]>(serialized, options);

        Assert.IsNotNull(deserialized);
        Assert.AreEqual(3, deserialized!.Length);
        Assert.AreEqual(1, deserialized[0]!.Value);
        Assert.AreEqual("First", deserialized[0]!.Name);
        Assert.AreEqual(2, deserialized[1]!.Value);
        Assert.AreEqual("Second", deserialized[1]!.Name);
        Assert.AreEqual(3, deserialized[2]!.Value);
        Assert.AreEqual("Third", deserialized[2]!.Name);
    }

    [TestMethod]
    public void SerializeDeserialize_MixedDuplicateAndUnique_CorrectlyDifferentiates()
    {
        var resolver = CycleResolver.Create(StandardResolver.Instance);
        var options = MessagePackSerializerOptions.Standard.WithResolver(resolver);

        var shared = new TestClass { Value = 100, Name = "Shared" };
        var unique1 = new TestClass { Value = 1, Name = "Unique1" };
        var unique2 = new TestClass { Value = 2, Name = "Unique2" };
        var container = new[] { shared, unique1, shared, unique2, shared };

        var serialized = MessagePackSerializer.Serialize(container, options);
        var deserialized = MessagePackSerializer.Deserialize<TestClass[]>(serialized, options);

        Assert.IsNotNull(deserialized);
        Assert.AreEqual(5, deserialized!.Length);
        Assert.AreSame(deserialized[0], deserialized[2]);
        Assert.AreSame(deserialized[0], deserialized[4]);
        Assert.AreNotSame(deserialized[0], deserialized[1]);
        Assert.AreNotSame(deserialized[0], deserialized[3]);
        Assert.AreEqual(100, deserialized[0]!.Value);
        Assert.AreEqual(1, deserialized[1]!.Value);
        Assert.AreEqual(2, deserialized[3]!.Value);
    }

    [TestMethod]
    public void SerializeDeserialize_StringDeduplication_WorksCorrectly()
    {
        var resolver = CycleResolver.Create(StandardResolver.Instance);
        var options = MessagePackSerializerOptions.Standard.WithResolver(resolver);

        var sharedString = "SharedString";
        var container = new[] { sharedString, sharedString, sharedString };

        var serialized = MessagePackSerializer.Serialize(container, options);
        var deserialized = MessagePackSerializer.Deserialize<string[]>(serialized, options);

        Assert.IsNotNull(deserialized);
        Assert.AreEqual(3, deserialized!.Length);
        Assert.AreSame(deserialized[0], deserialized[1]);
        Assert.AreSame(deserialized[0], deserialized[2]);
        Assert.AreEqual("SharedString", deserialized[0]);
    }

    [TestMethod]
    public void SerializeDeserialize_ComplexObject_WorksCorrectly()
    {
        var resolver = CycleResolver.Create(StandardResolver.Instance);
        var options = MessagePackSerializerOptions.Standard.WithResolver(resolver);

        var sharedList = new List<string> { "A", "B", "C" };
        var obj1 = new ComplexObject { Items = sharedList, Data = new Dictionary<string, int> { ["key1"] = 1 } };
        var obj2 = new ComplexObject { Items = sharedList, Data = new Dictionary<string, int> { ["key2"] = 2 } };
        var container = new[] { obj1, obj2 };

        var serialized = MessagePackSerializer.Serialize(container, options);
        var deserialized = MessagePackSerializer.Deserialize<ComplexObject[]>(serialized, options);

        Assert.IsNotNull(deserialized);
        Assert.AreEqual(2, deserialized!.Length);
        Assert.AreSame(deserialized[0]!.Items, deserialized[1]!.Items); // Shared list reference preserved
        Assert.AreEqual(3, deserialized[0]!.Items!.Count);
        Assert.AreEqual("A", deserialized[0]!.Items![0]);
    }

    [TestMethod]
    public void SerializeDeserialize_NestedDuplication_WorksCorrectly()
    {
        var resolver = CycleResolver.Create(StandardResolver.Instance);
        var options = MessagePackSerializerOptions.Standard.WithResolver(resolver);

        var leaf = new TestClass { Value = 999, Name = "Leaf" };
        var branch1 = new TestClass { Value = 1, Name = "Branch1", Reference = leaf };
        var branch2 = new TestClass { Value = 2, Name = "Branch2", Reference = leaf };
        var root = new TestClass { Value = 0, Name = "Root", Reference = branch1 };
        var container = new[] { root, branch2 };

        var serialized = MessagePackSerializer.Serialize(container, options);
        var deserialized = MessagePackSerializer.Deserialize<TestClass[]>(serialized, options);

        Assert.IsNotNull(deserialized);
        Assert.AreEqual(2, deserialized!.Length);
        Assert.AreEqual("Root", deserialized[0]!.Name);
        Assert.AreEqual("Branch2", deserialized[1]!.Name);
        Assert.IsNotNull(deserialized[0]!.Reference);
        Assert.IsNotNull(deserialized[1]!.Reference);
        Assert.AreSame(deserialized[0]!.Reference!.Reference, deserialized[1]!.Reference); // Shared leaf
    }

    [TestMethod]
    public void SerializeDeserialize_EmptyArray_WorksCorrectly()
    {
        var resolver = CycleResolver.Create(StandardResolver.Instance);
        var options = MessagePackSerializerOptions.Standard.WithResolver(resolver);

        var container = Array.Empty<TestClass>();

        var serialized = MessagePackSerializer.Serialize(container, options);
        var deserialized = MessagePackSerializer.Deserialize<TestClass[]>(serialized, options);

        Assert.IsNotNull(deserialized);
        Assert.AreEqual(0, deserialized!.Length);
    }

    [TestMethod]
    public void SerializeDeserialize_ArrayWithNulls_WorksCorrectly()
    {
        var resolver = CycleResolver.Create(StandardResolver.Instance);
        var options = MessagePackSerializerOptions.Standard.WithResolver(resolver);

        var obj = new TestClass { Value = 42, Name = "Test" };
        var container = new TestClass?[] { obj, null, obj, null };

        var serialized = MessagePackSerializer.Serialize(container, options);
        var deserialized = MessagePackSerializer.Deserialize<TestClass?[]>(serialized, options);

        Assert.IsNotNull(deserialized);
        Assert.AreEqual(4, deserialized!.Length);
        Assert.IsNotNull(deserialized[0]);
        Assert.IsNull(deserialized[1]);
        Assert.IsNotNull(deserialized[2]);
        Assert.IsNull(deserialized[3]);
        Assert.AreSame(deserialized[0], deserialized[2]);
    }

    [TestMethod]
    public void GetFormatter_SameType_ReturnsCachedFormatter()
    {
        var resolver = CycleResolver.Create(StandardResolver.Instance);
        
        var formatter1 = resolver.GetFormatter<TestClass>();
        var formatter2 = resolver.GetFormatter<TestClass>();

        Assert.AreSame(formatter1, formatter2);
    }

    [TestMethod]
    public void SerializeDeserialize_ValueTypes_NotDeduplicated()
    {
        var resolver = CycleResolver.Create(StandardResolver.Instance);
        var options = MessagePackSerializerOptions.Standard.WithResolver(resolver);

        var container = new[] { 42, 42, 42 };

        var serialized = MessagePackSerializer.Serialize(container, options);
        var deserialized = MessagePackSerializer.Deserialize<int[]>(serialized, options);

        Assert.IsNotNull(deserialized);
        Assert.AreEqual(3, deserialized!.Length);
        Assert.AreEqual(42, deserialized[0]);
        Assert.AreEqual(42, deserialized[1]);
        Assert.AreEqual(42, deserialized[2]);
    }

    [TestMethod]
    public void SerializeDeserialize_LargeNumberOfDuplicates_PerformanceTest()
    {
        var resolver = CycleResolver.Create(StandardResolver.Instance);
        var options = MessagePackSerializerOptions.Standard.WithResolver(resolver);

        var shared = new TestClass { Value = 42, Name = "Shared" };
        var container = new TestClass[1000];
        for (int i = 0; i < container.Length; i++)
        {
            container[i] = shared;
        }

        var serialized = MessagePackSerializer.Serialize(container, options);
        var deserialized = MessagePackSerializer.Deserialize<TestClass[]>(serialized, options);

        Assert.IsNotNull(deserialized);
        Assert.AreEqual(1000, deserialized!.Length);
        
        // Verify all references point to same object
        for (int i = 1; i < deserialized.Length; i++)
        {
            Assert.AreSame(deserialized[0], deserialized[i]);
        }

        // Verify the serialized size is much smaller than without deduplication
        var optionsWithoutDedup = MessagePackSerializerOptions.Standard.WithResolver(StandardResolver.Instance);
        var serializedWithoutDedup = MessagePackSerializer.Serialize(container, optionsWithoutDedup);
        
        Assert.IsTrue(serialized.Length < serializedWithoutDedup.Length);
    }

    [TestMethod]
    public void SerializeDeserialize_SelfReferencingObject_HandlesCorrectly()
    {
        var resolver = CycleResolver.Create(StandardResolver.Instance);
        var options = MessagePackSerializerOptions.Standard.WithResolver(resolver);

        var obj = new TestClass { Value = 42, Name = "Self" };
        obj.Reference = obj; // Self reference

        var serialized = MessagePackSerializer.Serialize(obj, options);
        var deserialized = MessagePackSerializer.Deserialize<TestClass>(serialized, options);

        Assert.IsNotNull(deserialized);
        Assert.AreEqual(42, deserialized!.Value);
        Assert.AreEqual("Self", deserialized.Name);
        Assert.AreSame(deserialized, deserialized.Reference); // Self reference preserved
    }
}

