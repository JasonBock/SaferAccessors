using NUnit.Framework;

namespace SaferAccessors.Tests;

internal static class UnsafeAccessorMapAttributeTests
{
	[Test]
	public static void Create()
	{
		var type = typeof(string);
		var attribute = new UnsafeAccessorMapAttribute(type);

		Assert.That(attribute.TargetType, Is.EqualTo(type));
	}

	[Test]
	public static void CreateWhenTargetTypeIsNull() =>
		Assert.That(() => new UnsafeAccessorMapAttribute(null!), Throws.TypeOf<ArgumentNullException>());
}