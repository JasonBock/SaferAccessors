using SaferAccessors;
using SaferAccessors.Performance;

[assembly: UnsafeAccessorMap(typeof(ConstructorContent))]

namespace SaferAccessors.Performance;

public sealed class ConstructorContent
{
	private ConstructorContent(string value) =>
		this.Value = value;

	public string Value { get; }
}