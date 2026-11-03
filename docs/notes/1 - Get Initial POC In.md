Maybe the right naming "shape" should be this:

```c#
public static class ContentExtensions
{
    extension(Content self)
    {
        public static extern ref UnsafeidField();
        public static extern void UnsafeCalculateMethod(); 
    }
}
```

Basically, put "Unsafe" in the `extern` method name to make it clear that this is an "unsafe" call. Or maybe switch the order:

```c#
public static class ContentExtensions
{
    extension(Content self)
    {
        public static extern ref idUnsafeField();
        public static extern void CalculateUnsafeMethod(); 
    }
}
```

Also, for `static` members, we can do a small, neat trick:

```c#
using System;
using System.Runtime.CompilerServices;

Console.WriteLine(Content.ComputeMethod());

public static class ContentExtensions
{
    extension(Content)
    {
        public static int ComputeMethod() => 
            (null as Content)!.ComputeMethodUnsafe();
    }

    extension(Content c)
    {
        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "Compute")]
        private extern int ComputeMethodUnsafe();
    }

    extension(Content)
    {
    }
}

public sealed class Content
{
	private static int Compute() => 22;
}
```

Basically, make the `extern` method `private` and create a `static` extension method that handles passing in `null` for the `extern` method. Calling static members is a bit klutzy with `[UnsafeAccessor]`, but we can hide that and make it cleaner (IMO) with a static extension method.

Note that the empty `extension` block is there just to show that you can have multiple of those within a class, and the compiler handles that with no issue.

I'm also debating what to do with `readonly` fields:

```c#
public sealed class Content
{
    private int idEdit;
    private readonly int id = 42;
}
```

This isn't just a C# keyword:

```
.class public auto ansi sealed beforefieldinit Content
    extends [System.Runtime]System.Object
{
    // Fields
    .field private int32 idEdit
    .field private initonly int32 id
}
```

Note the `initonly` keyword, which means that this is something enforceable at the CLR level. That said, field accessors **must** use `ref` returns for fields.

We could do another "trick":

```c#
var content = new Content();
Console.WriteLine(content.GetIdField());
Console.WriteLine(content.GetIdEditField());

//content.GetIdField() = 22;
content.GetIdEditField() = 66;

Console.WriteLine(content.GetIdField());
Console.WriteLine(content.GetIdEditField());

public sealed class Content
{
    private int idEdit;
    private readonly int id = 42;
}

public static class ContentUnsafeAccessors
{
    extension(Content self)
    {
        public int GetIdField() =>
            self.GetIdFieldUnsafe();

        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "id")]
        private extern ref int GetIdFieldUnsafe();

        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "idEdit")]
        public extern ref int GetIdEditField();
    }
}
```

We expose an accessor that doesn't return by `ref`, and it forwards the call to the real accessor. This allows the user to read the field, but it can't be changed.

Also, just figured out how to use this with a static class:

```c#
using System;
using System.Runtime.CompilerServices;

Console.WriteLine(Content.Calculate("333"));

public static class Content
{
    private static int Calculate(string key) => key.Length;
}

public static class ContentExtensions
{
    extension(Content)
    {
        public static int Calculate(string key) =>
            (null as object)!.CalculateMethod(key);
    }

	extension([UnsafeAccessorType("Content")] object? self)
	{
		[UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "Calculate")]
		private extern int CalculateMethod(string key);
	}    
}
```

This is one of many cases where `[UnsafeAccessorType]` is needed.

Constructors ... I think they should just be an extension method called `New(...)` and it's directly exposed, no indirect:

```c#
var thing = Thing.New("Hello");
Console.WriteLine(thing.Value);

public class Thing
{
    private Thing(string value) =>
        this.Value = value;

    public string Value { get; }
}

public static class ThingExtensions
{
    extension(Thing)
    {
        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        public extern static Thing New(string value);        
    }
}
```

As far as I know, there is no proposal/work on the horizon for extension constructors in C#. It's been mentioned, but I couldn't find anything even remotely concrete. My **guess** would be that it would have the same syntax:

```c#
var thing = new Thing("Hello");
```

So it wouldn't be too hard to switch to this though some simple regex match and replace.

TODO:
* DONE - With the static extension method redirect, it has to be done for both editable and readonly, because you have to pass `null` in as the first parameter for the `extern`
* DONE - Should we only look for "speakable" members? For example, C# declares a managed backing field for auto properties, and we probably don't want to expose those.
* DONE - All member access should only include those that have `CanBeReferencedByName` as `true`
* DONE - Field accessors should be handled to honor the `readonly` characteristic
* DONE - Static types are problematic with the indirect I'm trying to do with static members, because you can't do `(null as StaticType)!...`
* Properties - static properties are horribly broken, need to revisit
* Constructors - how to make it work

FUTURE:
* Performance - Create a Benchmark.NET-based app that shows the difference between Reflection and SaferAccessor
* Inheritance - need to figure out how this actually works and what members we do, or do not, want to pull.
* How does this work with `struct`s? I think there are some subtle differences ... maybe.
* Generics will need to be handled correct (type parameters on the extension type, method type parameters on the method, constraints defined, etc.)
* `[UnsafeAccessorType]` - we're going to need to use this more