using NUnit.Framework;

namespace SaferAccessors.Analysis.IntegrationTests.ConstructorAccessorGeneratorTestTypes;

public class Test
{
	private Test(string key, Guid id) =>
		(this.Key, this.Id) = (key, id);

	public Guid Id { get; }
	public string Key { get; }
}

public class Test<T>
{
	private Test(T key, Guid id) =>
		(this.Key, this.Id) = (key, id);

	public Guid Id { get; }
	public T Key { get; }
}

internal static class ConstructorAccessorGeneratorTest
{
	[Test]
	public static void Create()
	{
		var key = "key";
		var id = Guid.NewGuid();
		var test = Test.New("key", id);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(test.Key, Is.EqualTo(key));
			Assert.That(test.Id, Is.EqualTo(id));
		}
	}

	[Test]
	public static void CreateWithGeneric()
	{
		var key = "key";
		var id = Guid.NewGuid();
		var test = Test<string>.New("key", id);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(test.Key, Is.EqualTo(key));
			Assert.That(test.Id, Is.EqualTo(id));
		}
	}
}