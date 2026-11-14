using NUnit.Framework;

namespace SaferAccessors.Analysis.IntegrationTests.PropertyAccessorGeneratorTestTypes;

#pragma warning disable CA1823

public class TestWithInstance
{
	private int Id { get; set; }
}

public class TestWithInstance<T>
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
	private T Id { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
}

public class TestWithStatic
{
	private static int Id { get; set; }
}

public class TestWithStatic<T>
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
	private static T Id { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
}

public static class TestWithStaticDeclaredOnStaticType
{
	private static int Id { get; set; }
}

public static class TestWithStaticDeclaredOnStaticType<T>
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
	private static T Id { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
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
	public static void RoundtripInstanceOnGenericType()
	{
		var test = new TestWithInstance<int>
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
	public static void RoundtripStaticOnGenericType()
	{
		TestWithStatic<int>.Id = 22;
		Assert.That(TestWithStatic<int>.Id, Is.EqualTo(22));
	}

	[Test]
	public static void RoundtripStaticDeclaredOnStaticType()
	{
		TestWithStaticDeclaredOnStaticType.Id = 22;
		Assert.That(TestWithStaticDeclaredOnStaticType.Id, Is.EqualTo(22));
	}

	[Test]
	public static void RoundtripStaticDeclaredOnStaticGenericType()
	{
		TestWithStaticDeclaredOnStaticType<int>.Id = 22;
		Assert.That(TestWithStaticDeclaredOnStaticType<int>.Id, Is.EqualTo(22));
	}
}