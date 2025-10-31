using Maxine.Extensions;
using System.Reflection;

namespace Maxine.Extensions.Test;

[TestClass]
public class ReflectionExtensionsAdditionalTests
{
    private class TestClassWithDefaultConstructor
    {
        public int Value { get; set; }
        
        public TestClassWithDefaultConstructor()
        {
            Value = 42;
        }
    }

    private class TestClassWithParameterizedConstructor
    {
        public int Value { get; }
        
        public TestClassWithParameterizedConstructor(int value)
        {
            Value = value;
        }
    }

    private class TestClassWithMultipleConstructors
    {
        public int Value { get; set; }
        
        public TestClassWithMultipleConstructors()
        {
            Value = 0;
        }

        public TestClassWithMultipleConstructors(int value)
        {
            Value = value;
        }
    }

    private class TestClassWithMethods
    {
        public int LastResult { get; private set; }

        public void NoParameterMethod()
        {
            LastResult = 100;
        }

        public int NoParameterMethodWithReturn()
        {
            return 200;
        }

        public static int StaticNoParameterMethod()
        {
            return 300;
        }
    }

    [TestMethod]
    public void GetDefaultConstructor_WithDefaultConstructor_ReturnsConstructor()
    {
        var type = typeof(TestClassWithDefaultConstructor);

        var ctor = type.GetDefaultConstructor();

        Assert.IsNotNull(ctor);
        Assert.AreEqual(0, ctor.GetParameters().Length);
    }

    [TestMethod]
    public void GetDefaultConstructor_WithoutDefaultConstructor_ReturnsNull()
    {
        var type = typeof(TestClassWithParameterizedConstructor);

        var ctor = type.GetDefaultConstructor();

        Assert.IsNull(ctor);
    }

    [TestMethod]
    public void GetDefaultConstructor_WithMultipleConstructors_ReturnsDefaultOne()
    {
        var type = typeof(TestClassWithMultipleConstructors);

        var ctor = type.GetDefaultConstructor();

        Assert.IsNotNull(ctor);
        Assert.AreEqual(0, ctor.GetParameters().Length);
    }

    [TestMethod]
    public void ConstructorInvoke_CreatesInstance()
    {
        var type = typeof(TestClassWithDefaultConstructor);
        var ctor = type.GetDefaultConstructor();

        var instance = ctor!.Invoke();

        Assert.IsNotNull(instance);
        Assert.IsInstanceOfType(instance, typeof(TestClassWithDefaultConstructor));
        Assert.AreEqual(42, ((TestClassWithDefaultConstructor)instance).Value);
    }

    [TestMethod]
    public void MethodInvoke_InstanceMethod_ExecutesCorrectly()
    {
        var obj = new TestClassWithMethods();
        var method = typeof(TestClassWithMethods).GetMethod(nameof(TestClassWithMethods.NoParameterMethod))!;

        method.Invoke(obj);

        Assert.AreEqual(100, obj.LastResult);
    }

    [TestMethod]
    public void MethodInvoke_InstanceMethodWithReturn_ReturnsValue()
    {
        var obj = new TestClassWithMethods();
        var method = typeof(TestClassWithMethods).GetMethod(nameof(TestClassWithMethods.NoParameterMethodWithReturn))!;

        var result = method.Invoke(obj);

        Assert.AreEqual(200, result);
    }

    [TestMethod]
    public void MethodInvoke_StaticMethod_ReturnsValue()
    {
        var method = typeof(TestClassWithMethods).GetMethod(nameof(TestClassWithMethods.StaticNoParameterMethod))!;

        var result = method.Invoke(null);

        Assert.AreEqual(300, result);
    }

    [TestMethod]
    public void GetDefaultConstructor_ValueType_ReturnsNull()
    {
        var type = typeof(int);

        var ctor = type.GetDefaultConstructor();

        // Value types have a default constructor, but it's special and not returned by GetConstructor([])
        Assert.IsNull(ctor);
    }

    [TestMethod]
    public void GetDefaultConstructor_StructWithDefaultConstructor_Works()
    {
        var type = typeof(DateTime);

        var ctor = type.GetDefaultConstructor();

        // DateTime has no explicit parameterless constructor
        Assert.IsNull(ctor);
    }
}
