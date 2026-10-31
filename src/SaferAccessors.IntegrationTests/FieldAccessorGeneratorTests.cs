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
}