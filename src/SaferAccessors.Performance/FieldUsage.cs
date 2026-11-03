using BenchmarkDotNet.Attributes;
using System.Reflection;
using System.Security.Cryptography;

namespace SaferAccessors.Performance;

#pragma warning disable CA1024 // Use properties where appropriate
[MemoryDiagnoser]
public class FieldUsage
{
	private int id;
	private int value;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
	private Type contentType;
	private FieldInfo contentIdField;
	private FieldInfo contentValueField;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

	[GlobalSetup]
	public void Setup()
	{
		this.id = RandomNumberGenerator.GetInt32(10_000);
		this.value = RandomNumberGenerator.GetInt32(10_000);
		this.contentType = typeof(FieldContent);
		this.contentIdField = this.contentType.GetField("id", BindingFlags.Instance | BindingFlags.NonPublic)!;
		this.contentValueField = this.contentType.GetField("value", BindingFlags.Instance | BindingFlags.NonPublic)!;
	}

	[Benchmark(Baseline = true)]
	public int GetAndSetFieldsViaReflection()
	{
		var content = new FieldContent();
		var contentType = content.GetType();
		var contentIdField = contentType.GetField("id", BindingFlags.Instance | BindingFlags.NonPublic)!;
		var contentValueField = contentType.GetField("value", BindingFlags.Instance | BindingFlags.NonPublic)!;
		contentIdField.SetValue(content, this.id);
		contentValueField.SetValue(content, this.value);

		return (int)contentIdField.GetValue(content)! + (int)contentValueField.GetValue(content)!;
	}

	[Benchmark]
	public int GetAndSetFieldsViaCachedReflection()
	{
		var content = new FieldContent();
		this.contentIdField.SetValue(content, this.id);
		this.contentValueField.SetValue(content, this.value);

		return (int)this.contentIdField.GetValue(content)! + (int)this.contentValueField.GetValue(content)!;
	}

	[Benchmark]
	public int GetAndSetFieldsViaUnsafeAccessors()
	{
		var content = new FieldContent
		{
			id = this.id,
			value = this.value
		};

		return content.id + content.value;
	}
}
