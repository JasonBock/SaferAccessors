using SaferAccessors;
using SaferAccessors.Performance;

[assembly: UnsafeAccessorMap(typeof(MethodContent))]

namespace SaferAccessors.Performance;

public sealed class MethodContent
{
#pragma warning disable CA1822 // Mark members as static
	private int Compute(string value) => value.Length;
#pragma warning restore CA1822 // Mark members as static
}