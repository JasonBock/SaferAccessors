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

TODO:
* All member access should only include those that have `CanBeReferencedByName` as `true`
* Field accessors should be handled to honor the `readonly` characteristic
* How does this work with `struct`s? I think there are some subtle differences ... maybe.
* Generics will need to be handled correct (type parameters on the extension type, method type parameters on the method, constraints defined, etc.)
* Should we only look for "speakable" members? For example, C# declares a managed backing field for auto properties, and we probably don't want to expose those.
* `[UnsafeAccessorType]` - we're going to need to use at some point