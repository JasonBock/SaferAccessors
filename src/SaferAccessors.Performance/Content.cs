using SaferAccessors;
using SaferAccessors.Performance;

[assembly: UnsafeAccessorMap(typeof(Content))]

namespace SaferAccessors.Performance;

public sealed class Content
{
#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable CA1823 // Avoid unused private fields
#pragma warning disable CS0649
	private int id;
	private Guid index;

	public int Generate() => this.id ^ this.index.GetHashCode();

	private int Compute() => this.id ^ this.index.GetHashCode();
}