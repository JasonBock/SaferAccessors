using NUnit.Framework;

namespace SaferAccessors.Analysis.IntegrationTests.FieldAccessorGeneratorTestTypes;

#pragma warning disable CA1823

public class TestWithEditableInstance
{
	private int id = 11;

	public int RetrieveId() => this.id;
}

public class TestWithReadOnlyInstance
{
	private readonly int id = 22;

	public int RetrieveId() => this.id;
}

public class TestWithEditableStatic
{
	private static int id = 11;

	public int RetrieveId() => TestWithEditableStatic.id;
}

public static class TestWithEditableStaticDeclaredOnStaticType
{
	private static int id = 11;

	public static int RetrieveId() => TestWithEditableStaticDeclaredOnStaticType.id;
}

public class TestWithReadOnlyStatic
{
#pragma warning disable CA1802 // Use literals where appropriate
	private static readonly int id = 22;
#pragma warning restore CA1802 // Use literals where appropriate

	public int RetrieveId() => TestWithReadOnlyStatic.id;
}

public static class TestWithReadOnlyStaticDeclaredOnStaticType
{
#pragma warning disable CA1802 // Use literals where appropriate
	private static readonly int id = 22;
#pragma warning restore CA1802 // Use literals where appropriate

	public static int RetrieveId() => TestWithReadOnlyStaticDeclaredOnStaticType.id;
}

#pragma warning restore CA1823 // Avoid unused private fields

internal static class FieldAccessorGeneratorTest
{
	[Test]
	public static void RoundtripEditableInstance()
	{
		var test = new TestWithEditableInstance
		{
			id = 22
		};
		Assert.That(test.RetrieveId(), Is.EqualTo(22));
	}

	[Test]
	public static void RoundtripReadOnlyInstance()
	{
		var test = new TestWithReadOnlyInstance();
		Assert.That(test.id, Is.EqualTo(test.RetrieveId()));
	}

	[Test]
	public static void RoundtripEditableStatic()
	{
		var test = new TestWithEditableStatic();
		TestWithEditableStatic.id = 22;
		Assert.That(test.RetrieveId(), Is.EqualTo(22));
	}

	[Test]
	public static void RoundtripEditableStaticDeclaredOnStaticType()
	{
		TestWithEditableStaticDeclaredOnStaticType.id = 22;
		Assert.That(TestWithEditableStaticDeclaredOnStaticType.RetrieveId(), Is.EqualTo(22));
	}

	[Test]
	public static void RoundtripReadOnlyStatic() => 
		Assert.That(TestWithReadOnlyStaticDeclaredOnStaticType.id, Is.EqualTo(TestWithReadOnlyStaticDeclaredOnStaticType.RetrieveId()));
}