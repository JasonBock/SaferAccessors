using NUnit.Framework;

namespace SaferAccessors.Analysis.IntegrationTests.MethodAccessorGeneratorTestTypes;

public class TestWithInstance
{
	private int GetValue(string key) => key.Length;
	private int Compute<T>(T value) => value?.ToString()!.Length ?? 0;
}

public class TestWithInstance<T>
{
	private int GetValue(T key) => key?.ToString()!.Length ?? 0;
	private int Compute<V>(T value, V data) =>
		(value?.ToString()!.Length ?? 0) +
		(data?.ToString()!.Length ?? 0);
}

public class TestWithStatic
{
	private static int GetValue(string key) => key.Length;
	private static int Compute<T>(T value) => value?.ToString()!.Length ?? 0;
}

public class TestWithStatic<T>
{
	private static int GetValue(T key) => key?.ToString()!.Length ?? 0;
	private static int Compute<V>(T value, V data) =>
		(value?.ToString()!.Length ?? 0) +
		(data?.ToString()!.Length ?? 0);
}

public static class TestWithStaticDeclaredOnStaticType
{
	private static int GetValue(string key) => key.Length;
	private static int Compute<T>(T value) => value?.ToString()!.Length ?? 0;
}

public static class TestWithStaticDeclaredOnStaticType<T>
{
	private static int GetValue(T key) => key?.ToString()!.Length ?? 0;
	private static int Compute<V>(T value, V data) =>
		(value?.ToString()!.Length ?? 0) +
		(data?.ToString()!.Length ?? 0);
}

internal static class MethodAccessorGeneratorTest
{
	[Test]
	public static void InvokeInstance()
	{
		var test = new TestWithInstance();
		Assert.That(test.GetValue("333"), Is.EqualTo(3));
	}

	[Test]
	public static void InvokeGenericInstance()
	{
		var test = new TestWithInstance();
		Assert.That(test.Compute("333"), Is.EqualTo(3));
	}

	[Test]
	public static void InvokeInstanceOnGeneric()
	{
		var test = new TestWithInstance<string>();
		Assert.That(test.GetValue("333"), Is.EqualTo(3));
	}

	[Test]
	public static void InvokeGenericInstanceOnGeneric()
	{
		var test = new TestWithInstance<string>();
		Assert.That(test.Compute("333", 22), Is.EqualTo(5));
	}

	[Test]
	public static void InvokeStatic() => 
		Assert.That(TestWithStatic.GetValue("333"), Is.EqualTo(3));

	[Test]
	public static void InvokeGenericStatic() =>
		Assert.That(TestWithStatic.Compute("333"), Is.EqualTo(3));

	[Test]
	public static void InvokeStaticOnGeneric() =>
		Assert.That(TestWithStatic<string>.GetValue("333"), Is.EqualTo(3));

	[Test]
	public static void InvokeGenericStaticOnGeneric() =>
		Assert.That(TestWithStatic<string>.Compute("333", 22), Is.EqualTo(5));

	[Test]
	public static void InvokeStaticDeclaredOnStaticType() =>
		Assert.That(TestWithStaticDeclaredOnStaticType.GetValue("333"), Is.EqualTo(3));

	[Test]
	public static void InvokeGenericStaticDeclaredOnStaticType() =>
		Assert.That(TestWithStaticDeclaredOnStaticType.Compute("333"), Is.EqualTo(3));

	[Test]
	public static void InvokeStaticDeclaredOnStaticGenericType() =>
		Assert.That(TestWithStaticDeclaredOnStaticType<string>.GetValue("333"), Is.EqualTo(3));

	[Test]
	public static void InvokeGenericStaticDeclaredOnStaticGenericType() =>
		Assert.That(TestWithStaticDeclaredOnStaticType<string>.Compute("333", 22), Is.EqualTo(5));
}