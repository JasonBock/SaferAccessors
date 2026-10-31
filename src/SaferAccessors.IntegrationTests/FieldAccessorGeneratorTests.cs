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

public class TestWithReadOnlyStatic
{
#pragma warning disable CA1802 // Use literals where appropriate
	private static readonly int id = 22;
#pragma warning restore CA1802 // Use literals where appropriate

	public int RetrieveId() => TestWithReadOnlyStatic.id;
}

#pragma warning restore CA1823 // Avoid unused private fields

internal static class FieldAccessorGeneratorTest
{
	[Test]
	public static void RoundtripEditableInstance()
	{
		var test = new TestWithEditableInstance();
		test.idField() = 22;
		Assert.That(test.RetrieveId(), Is.EqualTo(22));
	}

	[Test]
	public static void RoundtripReadOnlyInstance()
	{
		var test = new TestWithReadOnlyInstance();
		Assert.That(test.idField(), Is.EqualTo(test.RetrieveId()));
	}

	[Test]
	public static void RoundtripEditableStatic()
	{
		var test = new TestWithEditableStatic();
		TestWithEditableStatic.idField() = 22;
		Assert.That(test.RetrieveId(), Is.EqualTo(22));
	}

	[Test]
	public static void RoundtripReadOnlyStatic()
	{
		var test = new TestWithReadOnlyStatic();
		Assert.That(TestWithReadOnlyStatic.idField(), Is.EqualTo(test.RetrieveId()));
	}
}