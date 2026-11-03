## Table of Contents
- [Introduction](#introduction)
- [Background and History](#background-and-history)
- [Usage](#usage)
- [Performance](#performance)
- [Conclusion](#conclusion)
  
# Introduction

In this document, I'll review `SaferAccessors` and how it works. I'll discuss Reflection in .NET, how it works, why unsafe accessors are an improvement, and where source generators fit in. I'll then demonstrate how SaferAccessors work. I'll also show performance results, illustrating the benefit of using SaferAccessors for Reflection scenarios.

# Background and History

The first paragraph in [this article](https://learn.microsoft.com/en-us/dotnet/fundamentals/reflection/overview) about Reflection sums things up nicely:

> The classes in the `System.Reflection` namespace, together with `System.Type`, enable you to obtain information about loaded assemblies and the types defined within them, such as classes, interfaces, and value types (that is, structures and enumerations). You can also use reflection to create type instances at runtime, and to invoke and access them.

Reflection provides you a way to find all of the members on a type and use them however you want. Reflection also lets you use inaccessible members from a type as the following code sample demonstrates:

```c#
public sealed class MethodContent
{
  private int Compute(string value) => value.Length;
}

var content = new MethodContent();
var contentType = content.GetType();

return (int)contentType.GetMethod("Compute", BindingFlags.Instance | BindingFlags.NonPublic)!
  .Invoke(content, ["Hello"])!;
```

Even though `Compute()` is `private`, Reflection lets you find that method via `GetMethod()`, and then you can call it through `Invoke()`. In this example, `Compute()` return 5 - the length of the `"Hello"` string.

To be clear, developers should steer away from using Reflection for non-public members. Anything that is not accessible is free for the developer to change as they see fit without it being considered a breaking change. If you start using these members, your code can easily break for a number of reasons:

* The developer could change the name of the member. In the previous example, if the developer changed the name from "Compute" to "Calculate", your Reflection code would fail as it wouldn't find a method named "Compute".
* The developer could remove the member entirely for whatever reason.
* The developer could add parameters to the method, or change the return type, or change it from an instance method to being `static`.

Moreover, Reflection is slower. Calling a method through a `MethodInfo` instance, or setting a property through a `PropertyInfo` instance, will always be slower and will potentially allocate more memory than performing the same action directly. As you'll see in the Performance section, this difference is in the nanoseconds range and just a handful of bytes, but for hot paths, this can add up.

Using Reflection to use non-public members should be a very small exception in a code base. It should only be done when no other option is available. Avoid this path and stick with the common approach of using accessible members. That said, if you need to use Reflection, there is a better way:

```c#
var content = new Content();
ContentUnsafeAccessors.GetIdField(content) = this.id;
ContentUnsafeAccessors.GetIndexField(content) = this.index;

Console.WriteLine(ContentUnsafeAccessors.ComputeMethod(content));

public sealed class Content
{
  private int id;
  private Guid index;

  private int Compute() => this.id ^ this.index.GetHashCode();
}

public static class ContentUnsafeAccessors
{
  [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "Compute")]
  public static extern int ComputeMethod(Content content);

  [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "id")]
  public static extern ref int GetIdField(Content content);

  [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "index")]
  public static extern ref Guid GetIndexField(Content content);
}
```

In this case, the private fields and method are exposed through `extern` methods, attributed with [`[UnsafeAccessor]`](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.unsafeaccessorattribute). This mechanism makes member usage as fast as if a user was using them directly. This is definitely an improvement over the Reflection APIs, but there's still room for improvement:

* If a member targeted by `[UnsafeAccessor]` via the `Name` value changes its' name, the code will break at runtime, not at compile-time.
* There are some subtle rules surrounding `[UnsafeAccessor]` that may not be obvious at first - e.g. field accessors **must** have their methods return by `ref`, even if the field is marked as `readonly`.
* While `[UnsafeAccessor]` can only exist on a method, there are arguably better ways to expose these accessors using modern C# language features - specifically, [extension members](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/extension).
* It's all still a manual process - you have to create the class that holds the accessor methods and you have to declare them yourself. Furthermore, you need to dig in, either by looking at the source code of the type, or through some other introspection tools, to determine what's not public in the first place.

Enter SaferAccessors. This library holds a source generator that will examine a targeted type, and produce extension members for **all** non-public members that you can easily use. Furthermore, the source generator automatically updates the generated code if the targeted type changes member names or signatures. Let's go through how this library works.

# Usage

First, you need to reference the [NuGet package](https://www.nuget.org/packages/SaferAccessors) in your project. Next, you need to add an `assembly`-level attribute named `[UnsafeAccessorMap]` in your code, passing in the type that contains the members you want to expose:

```c#
[assembly: UnsafeAccessorMap(typeof(MethodContent))]

public sealed class MethodContent
{
  private int Compute(string value) => value.Length;
}
```

Now, you can call `Compute()` with one line of code:

```c#
var content = new MethodContent();
Console.WriteLine(content.Compute("Hello"));
```

SaferAccessors generates code that looks something like this:

```c#
public static class MethodContentExtensions
{
  extension(global::MethodContent self)
  {
    [global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.Method, Name = "Compute")]
    public extern int Compute(string value);
  }
}
```

If `Compute` was renamed to `Calculate`, the generated code would change as well. This means that the code that's using the `Compute()` extension method will no longer work, but that's discoverable at compile-time, **not** runtime. That's a big improvement.

Also, SaferAccessors takes advantage of the `extension` definition such that non-public static members look like they exist on the type:

```c#
[assembly: UnsafeAccessorMap(typeof(TestWithStatic))]

Console.WriteLine(TestWithStatic.GetValue("Key"));

public class TestWithStatic
{
  private static int GetValue(string key) => key.Length;
}
```

This is what the generated code looks like with `TestWithStatic` as a target:

```c#
public static class TestWithStaticExtensions
{
  extension(global::TestWithStatic)
  {
    public static int GetValue(string key) =>
      (null as global::TestWithStatic)!.GetValueMethod(key);
  }
	
  extension(global::TestWithStatic self)
  {
    [global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.StaticMethod, Name = "GetValue")]
    private extern int GetValueMethod(string key);
  }	
}
```

This may look a little weird because even a `static` member needs the first parameter to be of the target type, but since it's `static`, you can pass in `null`. The generator creates the necessary redirect so the extension member is `static`, but the `extern` requirements are satisfied.

Extension methods have existed since C# 3.0, but with C# 14, more members have been added for extension - namely, properties and operators. We can use that to handle properties and fields in a way that looks like normal C# code. For example, here's how a property works:

```c#
[assembly: UnsafeAccessorMap(typeof(PropertyContent))]

var content = new PropertyContent
{
  Id = 10,
  Value = 20
};

Console.WriteLine(content.Id + content.Value);

public sealed class PropertyContent
{
  private int Id { get; set; }
  private int Value { get; set; }
}
```

The generated code creates the extension property and forwards the calls to the correct `extern` methods:

```c#
public static class PropertyContentExtensions
{
  extension(global::PropertyContent self)
  {
    public int Id
    {
      get => self.GetIdProperty();
      set => self.SetIdProperty(value);
    }
	
    [global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.Method, Name = "get_Id")]
    private extern int GetIdProperty();
		
    [global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.Method, Name = "set_Id")]
    private extern void SetIdProperty(int value);
  }
	
  extension(global::PropertyContent self)
  {
    public int Value
    {
      get => self.GetValueProperty();
      set => self.SetValueProperty(value);
    }
	
    [global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.Method, Name = "get_Value")]
    private extern int GetValueProperty();
		
    [global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.Method, Name = "set_Value")]
    private extern void SetValueProperty(int value);	
    }	
}
```

Fields are handled like properties, so the syntax will look the same. If the field is declared as `readonly`, only a `get` accessor will be generated.

Constructors are also supported in SaferAccessors. There is no "extension constructor" feature yet in C#, so SaferAccessors will generate a `New()` static extension method for the targeted type:

```c#
[assembly: UnsafeAccessorMap(typeof(ConstructorContent))]

var content = ConstructorContent.New("Hello");
Console.WriteLine(content.Value);

public sealed class ConstructorContent
{
  private ConstructorContent(string value) =>
    this.Value = value;

  public string Value { get; }
}
```

If extension constructors are ever added, the assumption is that it would such that it would end up looking like `new ConstructorContent(...)`. SaferAccessors would be updated accordingly, and it would be fairly trivial to update existing code to use the new syntax.

> NOTE: In C# 15, extension indexers will be added. Once that language version is officially released in mid-November 2026, indexers will be supported in SaferAccessors.

# Performance

The SaferAccessors.Performance project contains a number of [Benchmark.NET](https://benchmarkdotnet.org/)-based tests to compare Reflection with SaferAccessors. The results.md file contains all the results of each test class - here's the results from the method tests:

```
| Method                     | Mean       | Error     | StdDev    | Median     | Ratio | Gen0   | Allocated | Alloc Ratio |
|--------------------------- |-----------:|----------:|----------:|-----------:|------:|-------:|----------:|------------:|
| ComputeViaReflection       | 21.0773 ns | 0.1664 ns | 0.1557 ns | 21.0672 ns | 1.000 | 0.0046 |      80 B |        1.00 |
| ComputeViaCachedReflection | 10.8356 ns | 0.1550 ns | 0.1450 ns | 10.8313 ns | 0.514 | 0.0046 |      80 B |        1.00 |
| ComputeViaUnsafeAccessors  |  0.0010 ns | 0.0018 ns | 0.0017 ns |  0.0000 ns | 0.000 |      - |         - |        0.00 |
```

It's clear that the accessor approach is much better than Reflection, even if you cache Reflection objects like `MethodInfo` and `PropertyInfo`. As mentioned at the beginning of this article, the times are in the nanosecond range, and the allocation size is a small amount of bytes. But with the ease and safety of SaferAccessors, there's simply no need to use Reflection APIs for member usage anymore!

# Conclusion

You've now seen how `SaferAccessors` can be used to make Reflection faster and safer. Remember to peruse the tests within `SaferAccessors.IntegrationTests` in case you get stuck. If you'd like, feel free to submit a PR to update this document to improve its contents. If you run into any issues, please submit an issue. Happy coding!