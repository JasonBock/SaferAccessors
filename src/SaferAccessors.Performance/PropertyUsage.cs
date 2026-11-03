using BenchmarkDotNet.Attributes;
using System.Reflection;
using System.Security.Cryptography;

namespace SaferAccessors.Performance;

#pragma warning disable CA1024 // Use properties where appropriate
[MemoryDiagnoser]
public class PropertyUsage
{
	private int id;
	private int value;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
	private Type contentType;
	private PropertyInfo contentIdProperty;
	private PropertyInfo contentValueProperty;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

	[GlobalSetup]
	public void Setup()
	{
		this.id = RandomNumberGenerator.GetInt32(10_000);
		this.value = RandomNumberGenerator.GetInt32(10_000);
		this.contentType = typeof(PropertyContent);
		this.contentIdProperty = this.contentType.GetProperty("Id", BindingFlags.Instance | BindingFlags.NonPublic)!;
		this.contentValueProperty = this.contentType.GetProperty("Value", BindingFlags.Instance | BindingFlags.NonPublic)!;
	}

	[Benchmark(Baseline = true)]
	public int GetAndSetPropertiesViaReflection()
	{
		var content = new PropertyContent();
		var contentType = content.GetType();
		var contentIdProperty = contentType.GetProperty("Id", BindingFlags.Instance | BindingFlags.NonPublic)!;
		var contentValueProperty = contentType.GetProperty("Value", BindingFlags.Instance | BindingFlags.NonPublic)!;
		contentIdProperty.SetValue(content, this.id);
		contentValueProperty.SetValue(content, this.value);

		return (int)contentIdProperty.GetValue(content)! + (int)contentValueProperty.GetValue(content)!;
	}

	[Benchmark]
	public int GetAndSetPropertiesViaCachedReflection()
	{
		var content = new PropertyContent();
		this.contentIdProperty.SetValue(content, this.id);
		this.contentValueProperty.SetValue(content, this.value);

		return (int)this.contentIdProperty.GetValue(content)! + (int)this.contentValueProperty.GetValue(content)!;
	}

	[Benchmark]
	public int GetAndSetPropertiesViaUnsafeAccessors()
	{
		var content = new PropertyContent
		{
			Id = this.id,
			Value = this.value
		};

		return content.Id + content.Value;
	}
}
