using SaferAccessors;
using SaferAccessors.Performance;

[assembly: UnsafeAccessorMap(typeof(PropertyContent))]

namespace SaferAccessors.Performance;

public sealed class PropertyContent
{
	private int Id { get; set; }
	private int Value { get; set; }
}