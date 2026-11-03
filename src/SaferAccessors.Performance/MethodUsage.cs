using BenchmarkDotNet.Attributes;
using System.Reflection;

namespace SaferAccessors.Performance;

#pragma warning disable CA1822 // Mark members as static
[MemoryDiagnoser]
public class MethodUsage
{
	private const string Value = "Some Value";

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
	private Type contentType;
	private MethodInfo contentComputeMethod;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

	[GlobalSetup]
	public void Setup()
	{
		this.contentType = typeof(MethodContent);
		this.contentComputeMethod = this.contentType.GetMethod("Compute", BindingFlags.Instance | BindingFlags.NonPublic)!;
	}

	[Benchmark(Baseline = true)]
	public int ComputeViaReflection()
	{
		var content = new MethodContent();
		var contentType = content.GetType();

		return (int)contentType.GetMethod("Compute", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(content, [MethodUsage.Value])!;
	}

	[Benchmark]
	public int ComputeViaCachedReflection()
	{
		var content = new MethodContent();

		return (int)this.contentComputeMethod.Invoke(content, [MethodUsage.Value])!;
	}

	[Benchmark]
	public int ComputeViaUnsafeAccessors() => 
		new MethodContent().Compute(MethodUsage.Value);
}
