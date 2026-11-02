using NUnit.Framework;

namespace SaferAccessors.Analysis.IntegrationTests.PropertyAccessorGeneratorTestTypes;

#pragma warning disable CA1823

public class TestWithGetInstance
{
	public TestWithGetInstance(int id) => 
		this.Id = id;

	private int Id { get; }
}

internal static class FieldAccessorGeneratorTest
{
	[Test]
	public static void GetInstance()
	{
		var test = new TestWithGetInstance(22);
		Assert.That(test.Id, Is.EqualTo(22));
	}
}