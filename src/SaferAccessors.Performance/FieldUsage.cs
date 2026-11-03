using BenchmarkDotNet.Attributes;
using System.Reflection;
using System.Security.Cryptography;

namespace SaferAccessors.Performance;

[MemoryDiagnoser]
public class FieldUsage
{
	private int id;
	private Guid index;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
	private Type contentType;
	private FieldInfo contentIdField;
	private FieldInfo contentIndexField;
	private MethodInfo contentComputeMethod;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

	[GlobalSetup]
	public void Setup()
	{
		this.id = RandomNumberGenerator.GetInt32(10_000);
		this.index = Guid.NewGuid();
		this.contentType = typeof(Content);
		this.contentIdField = this.contentType.GetField("id", BindingFlags.Instance | BindingFlags.NonPublic)!;
		this.contentIndexField = this.contentType.GetField("index", BindingFlags.Instance | BindingFlags.NonPublic)!;
		this.contentComputeMethod = this.contentType.GetMethod("Compute", BindingFlags.Instance | BindingFlags.NonPublic)!;
	}

	[Benchmark(Baseline = true)]
	public int SetFieldsAndComputeViaReflection()
	{
		var content = new Content();
		var contentType = content.GetType();
		var contentIdField = contentType.GetField("id", BindingFlags.Instance | BindingFlags.NonPublic)!;
		var contentIndexField = contentType.GetField("index", BindingFlags.Instance | BindingFlags.NonPublic)!;
		contentIdField.SetValue(content, this.id);
		contentIndexField.SetValue(content, this.index);

		return (int)contentType.GetMethod("Compute", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(content, [])!;
	}

	[Benchmark]
	public int SetFieldsAndComputeViaCachedReflection()
	{
		var content = new Content();
		this.contentIdField.SetValue(content, this.id);
		this.contentIndexField.SetValue(content, this.index);

		return (int)this.contentComputeMethod!.Invoke(content, [])!;
	}

	[Benchmark]
	public int SetFieldsAndComputeViaUnsafeAccessors()
	{
		var content = new Content
		{
			id = this.id,
			index = this.index
		};

		return content.Compute();
	}
}
