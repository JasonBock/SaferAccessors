using NUnit.Framework;

namespace SaferAccessors.Analysis.IntegrationTests.MethodAccessorGeneratorTestTypes;

public class TestWithInstance
{
	private int GetValue(string key) => key.Length;
}

public class TestWithStatic
{
	private static int GetValue(string key) => key.Length;
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
	public static void InvokeStatic() => 
		Assert.That(TestWithStatic.GetValue("333"), Is.EqualTo(3));
}