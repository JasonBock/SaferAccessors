using NUnit.Framework;

namespace SaferAccessors.Analysis.IntegrationTests.PropertyAccessorGeneratorTestTypes;

#pragma warning disable CA1823

public class TestWithInstance
{
	private int Id { get; set; }
}

public class TestWithStatic
{
	private static int Id { get; set; }
}

public static class TestWithStaticDeclaredOnStaticType
{
	private static int Id { get; set; }
}

internal static class FieldAccessorGeneratorTest
{
	[Test]
	public static void RoundtripInstance()
	{
		var test = new TestWithInstance
		{
			Id = 22
		};
		Assert.That(test.Id, Is.EqualTo(22));
	}

	[Test]
	public static void RoundtripStatic()
	{
		TestWithStatic.Id = 22;
		Assert.That(TestWithStatic.Id, Is.EqualTo(22));
	}

	[Test]
	public static void RoundtripStaticDeclaredOnStaticType()
	{
		TestWithStaticDeclaredOnStaticType.Id = 22;
		Assert.That(TestWithStaticDeclaredOnStaticType.Id, Is.EqualTo(22));
	}
}