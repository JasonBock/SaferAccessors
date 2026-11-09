using NUnit.Framework;

namespace SaferAccessors.Analysis.IntegrationTests.MethodAccessorGeneratorTestTypes;

public class TestWithInstance
{
	private int GetValue(string key) => key.Length;
}

public class TestWithInstance<T>
{
	private int GetValue(T key) => key?.ToString()!.Length ?? 0;
}

public class TestWithStatic
{
	private static int GetValue(string key) => key.Length;
}

public class TestWithStatic<T>
{
	private static int GetValue(T key) => key?.ToString()!.Length ?? 0;
}

public static class TestWithStaticDeclaredOnStaticType
{
	private static int GetValue(string key) => key.Length;
}

public static class TestWithStaticDeclaredOnStaticType<T>
{
	private static int GetValue(T key) => key?.ToString()!.Length ?? 0;
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
	public static void InvokeInstanceOnGeneric()
	{
		var test = new TestWithInstance<string>();
		Assert.That(test.GetValue("333"), Is.EqualTo(3));
	}

	[Test]
	public static void InvokeStatic() => 
		Assert.That(TestWithStatic.GetValue("333"), Is.EqualTo(3));

	[Test]
	public static void InvokeStaticOnGeneric() =>
		Assert.That(TestWithStatic<string>.GetValue("333"), Is.EqualTo(3));

	[Test]
	public static void InvokeStaticDeclaredOnStaticType() =>
		Assert.That(TestWithStaticDeclaredOnStaticType.GetValue("333"), Is.EqualTo(3));

	[Test]
	public static void InvokeStaticDeclaredOnStaticGenericType() =>
		Assert.That(TestWithStaticDeclaredOnStaticType<string>.GetValue("333"), Is.EqualTo(3));
}