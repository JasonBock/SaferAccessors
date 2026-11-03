using SaferAccessors;
using SaferAccessors.Performance;

[assembly: UnsafeAccessorMap(typeof(FieldContent))]

namespace SaferAccessors.Performance;

public sealed class FieldContent
{
#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable CA1823 // Avoid unused private fields
#pragma warning disable CS0169
#pragma warning disable CS0649
	private int id;
	private int value;
}