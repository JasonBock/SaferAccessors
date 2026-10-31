namespace SaferAccessors;

/// <summary>
/// Used to specify a type to build unsafe accessors for.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class UnsafeAccessorMapAttribute
  : Attribute
{
	/// <summary>
	/// Creates a new <see cref="UnsafeAccessorMapAttribute"/> instance.
	/// </summary>
	/// <param name="targetType">The target type.</param>
	public UnsafeAccessorMapAttribute(Type targetType) =>
		this.TargetType = targetType ?? throw new ArgumentNullException(nameof(targetType));

	/// <summary>
	/// Gets the type to build unsafe accessors for.
	/// </summary>
	public Type TargetType { get; }
}