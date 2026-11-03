using BenchmarkDotNet.Attributes;
using System.Reflection;

namespace SaferAccessors.Performance;

#pragma warning disable CA1822 // Mark members as static
[MemoryDiagnoser]
public class ConstructorUsage
{
	private const string Value = "Some Value";

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
	private Type contentType;
	private ConstructorInfo contentConstructor;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

	[GlobalSetup]
	public void Setup()
	{
		this.contentType = typeof(ConstructorContent);
		this.contentConstructor = this.contentType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, [typeof(string)])!;
	}

	[Benchmark(Baseline = true)]
	public string CreateViaReflection()
	{
		var contentType = typeof(ConstructorContent);
		var content = (ConstructorContent)contentType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, [typeof(string)])!.Invoke([ConstructorUsage.Value])!;
		return content.Value;
	}

	[Benchmark]
	public string CreateViaCachedReflection()
	{
		var content = (ConstructorContent)this.contentConstructor.Invoke([ConstructorUsage.Value])!;
		return content.Value;
	}

	[Benchmark]
	public string CreateViaUnsafeAccessors()
	{
		var content = ConstructorContent.New(ConstructorUsage.Value);
		return content.Value;
	}
}
