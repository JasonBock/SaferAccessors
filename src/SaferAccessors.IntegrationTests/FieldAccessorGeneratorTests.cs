using NUnit.Framework;

namespace SaferAccessors.Analysis.IntegrationTests.FieldAccessorGeneratorTestTypes;

#pragma warning disable CA1823

public class TestWithEditableInstance
{
	private int id = 11;

	public int RetrieveId() => this.id;
}

public class TestWithReadOnlyInstance
{
	private readonly int id = 22;

	public int RetrieveId() => this.id;
}

public class TestWithEditableInstance<T>
{
#pragma warning disable CS0649
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
	private T id;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
#pragma warning restore CS0649
	public T RetrieveId() => this.id;
}

public class TestWithReadOnlyInstance<T>
{
#pragma warning disable CS0649
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
	private readonly T id;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
#pragma warning restore CS0649

	public T RetrieveId() => this.id;
}

public class TestWithEditableStatic
{
	private static int id = 11;

	public int RetrieveId() => TestWithEditableStatic.id;
}

public class TestWithReadOnlyStatic
{
#pragma warning disable CA1802 // Use literals where appropriate
	private static readonly int id = 22;
#pragma warning restore CA1802 // Use literals where appropriate

	public int RetrieveId() => TestWithReadOnlyStatic.id;
}

public class TestWithEditableStatic<T>
{
#pragma warning disable CS0649
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
	private static T id;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
#pragma warning restore CS0649

	public T RetrieveId() => TestWithEditableStatic<T>.id;
}

public class TestWithReadOnlyStatic<T>
{
#pragma warning disable CS0649
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
	private static readonly T id;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
#pragma warning restore CS0649

	public T RetrieveId() => TestWithReadOnlyStatic<T>.id;
}

public static class TestWithEditableStaticDeclaredOnStaticType
{
	private static int id = 11;

	public static int RetrieveId() => TestWithEditableStaticDeclaredOnStaticType.id;
}

public static class TestWithReadOnlyStaticDeclaredOnStaticType
{
#pragma warning disable CA1802 // Use literals where appropriate
	private static readonly int id = 22;
#pragma warning restore CA1802 // Use literals where appropriate

	public static int RetrieveId() => TestWithReadOnlyStaticDeclaredOnStaticType.id;
}

public static class TestWithEditableStaticDeclaredOnStaticType<T>
{
#pragma warning disable CS0649
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
	private static T id;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
#pragma warning restore CS0649

#pragma warning disable CA1000 // Do not declare static members on generic types
	public static T RetrieveId() => TestWithEditableStaticDeclaredOnStaticType<T>.id;
#pragma warning restore CA1000 // Do not declare static members on generic types
}

public static class TestWithReadOnlyStaticDeclaredOnStaticType<T>
{
#pragma warning disable CS0649
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
	private static readonly T id;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
#pragma warning restore CS0649

#pragma warning disable CA1000 // Do not declare static members on generic types
	public static T RetrieveId() => TestWithReadOnlyStaticDeclaredOnStaticType<T>.id;
#pragma warning restore CA1000 // Do not declare static members on generic types
}

#pragma warning restore CA1823 // Avoid unused private fields

internal static class FieldAccessorGeneratorTest
{
	[Test]
	public static void RoundtripEditableInstance()
	{
		var test = new TestWithEditableInstance
		{
			id = 22
		};
		Assert.That(test.RetrieveId(), Is.EqualTo(22));
	}

	[Test]
	public static void RoundtripReadOnlyInstance()
	{
		var test = new TestWithReadOnlyInstance();
		Assert.That(test.id, Is.EqualTo(test.RetrieveId()));
	}

	[Test]
	public static void RoundtripEditableGenericInstance()
	{
		var test = new TestWithEditableInstance<int>
		{
			id = 22
		};
		Assert.That(test.RetrieveId(), Is.EqualTo(22));
	}

	[Test]
	public static void RoundtripReadOnlyGenericInstance()
	{
		var test = new TestWithReadOnlyInstance<int>();
		Assert.That(test.id, Is.EqualTo(test.RetrieveId()));
	}

	[Test]
	public static void RoundtripEditableStatic()
	{
		var test = new TestWithEditableStatic();
		TestWithEditableStatic.id = 22;
		Assert.That(test.RetrieveId(), Is.EqualTo(22));
	}

	[Test]
	public static void RoundtripEditableGenericStatic()
	{
		var test = new TestWithEditableStatic<int>();
		TestWithEditableStatic<int>.id = 22;
		Assert.That(test.RetrieveId(), Is.EqualTo(22));
	}

	[Test]
	public static void RoundtripReadOnlyGenericStatic()
	{
		var test = new TestWithReadOnlyStatic<int>();
		Assert.That(TestWithReadOnlyStatic<int>.id, Is.EqualTo(test.RetrieveId()));
	}

	[Test]
	public static void RoundtripEditableStaticDeclaredOnStaticType()
	{
		TestWithEditableStaticDeclaredOnStaticType.id = 22;
		Assert.That(TestWithEditableStaticDeclaredOnStaticType.RetrieveId(), Is.EqualTo(22));
	}

	[Test]
	public static void RoundtripReadOnlyStaticDeclaredOnStaticType() =>
		Assert.That(TestWithReadOnlyStaticDeclaredOnStaticType.id, Is.EqualTo(TestWithReadOnlyStaticDeclaredOnStaticType.RetrieveId()));

	[Test]
	public static void RoundtripEditableStaticDeclaredOnStaticGenericType()
	{
		TestWithEditableStaticDeclaredOnStaticType<int>.id = 22;
		Assert.That(TestWithEditableStaticDeclaredOnStaticType<int>.RetrieveId(), Is.EqualTo(22));
	}

	[Test]
	public static void RoundtripReadOnlyStaticDeclaredOnStaticGenericType() =>
		Assert.That(TestWithReadOnlyStaticDeclaredOnStaticType<int>.id, Is.EqualTo(TestWithReadOnlyStaticDeclaredOnStaticType<int>.RetrieveId()));
}