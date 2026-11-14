So where do generics come into play? On the `extension` declaration for type parameters on the target type, and method declarations if the method has type parameters.

Let's look at the first case: type parameters on the type:

```c#
using System;
using System.Collections.Generic;
using System.Linq;

var items = new List<int> { 1, 4, 6 };

Console.WriteLine(items.Combine());

public static class ListExtensions
{
    extension<T>(List<T> self)
    {
        public string Combine() => 
            string.Join(',', self.Select(item => item?.ToString()));
    }
}
```

Basically the type parameters go on the `extension` declaration, and they naturally flow through.

Constraints will have to come through as well:

```c#
using System;

var thing = new Repository<Thing>().CreateInstance();
Console.WriteLine(thing.GetType().Name);

public class Thing { }

public class Repository<T> 
    where T : class, new()
{ }

public static class RepositoryExtensions
{
    extension<T>(Repository<T> self) where T : class, new()
    {
        public T CreateInstance()
        {
            return new T(); // Allowed because of new() constraint
        }
    }
}
```

However, with `[UnsafeAccessor]`, you can't directly put the `extern` method within the `extension` directive. It won't work. A redirect is necessary:

```c#
Console.WriteLine(new Processor<string>().Calculate("Hello"));

public class Processor<T>
{
    private int Calculate(T value) => value?.GetHashCode() ?? 0;
}

public static class ProcessorExtensions
{
    extension<T>(Processor<T> self)
    {
        public int Calculate(T value) => 
            GenericProcessorExtensions<T>.Calculate(self, value);
    }

    private static partial class GenericProcessorExtensions<T>
    {
        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "Calculate")]
        public extern static int Calculate(Processor<T> instance, T value);
    }
}
```

So, if a type is generic, the extension member must redirect to a `private static` class that's `partial` that has the `extern` method, even if the member itself isn't generic.

For generic methods, if it has its' own type parameters, it works in a similar fashion:

```c#
using System;
using System.Runtime.CompilerServices;

Console.WriteLine(new Processor<string>().Calculate("Hello"));
Console.WriteLine(new Processor<string>().Accept("Hello2"));

public class Processor<T>
{
    private int Calculate(T value) => value?.GetHashCode() ?? 0;
    private int Accept<TValue>(TValue value) => value?.GetHashCode() ?? 0;
}

public static class ProcessorExtensions
{
    extension<T>(Processor<T> self)
    {
        public int Calculate(T value) => 
            GenericProcessorExtensions<T>.Calculate(self, value);

        public int Accept<TValue>(TValue value) => 
            GenericProcessorExtensions<T>.Accept<TValue>(self, value);
    }

    private static partial class GenericProcessorExtensions<T>
    {
        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "Calculate")]
        public extern static int Calculate(Processor<T> instance, T value);

        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "Accept")]
        public extern static int Accept<TValue>(Processor<T> instance, TValue value);
    }
}
```

However, fields don't need any special handling, even if the field is based on a type parameter from the type definition.

Constructors will need a redirect, similar to methods, though the wrapping `New()` method will need to be defined on the type:

```c#
var processor = Processor<string>.New("22");

public class Processor<T>
{
    private Processor(T id) => this.id = id;

    private T id;
}

public static class ProcessorExtensions
{
    extension<T>(Processor<T>)
    {
        public static Processor<T> New(T id) =>
            GenericProcessorExtensions<T>.New(id);
    }

    private static partial class GenericProcessorExtensions<T>
    {
		[UnsafeAccessor(UnsafeAccessorKind.Constructor)]
		public extern static Processor<T> New(T value);
    }
}
```

Properties need a redirect just like methods.

Here's a summary of the different members and what needs to be done **when the type is generic**:

* Methods - need to have the redirect in place shown above
* Constructor - need to have the redirect in place shown above
* Properties - need to have the redirect in place shown above
* Fields - no extra work necessary (still should have tests to cover this)

If the type is not generic and a method is generic, no redirects need to be done, we just need to make sure that the method definition includes the type parameters in the `extern` signature.

So fields are the exception, methods and properties work exactly the same with the workaround, constructors need a similar workaround but slightly different.

I'm wondering if should make **all** redirects, whether needed for generics or not, consistent. That is, there's a `private static partial class {TypeName}Accessors` that contains all redirects:

```c#
public class Test
{
  private static int GetValue(string key) => 22;
}

public static partial class TestExtensions
{
  extension(global::Test)
  {
    public static int GetValue(string key) =>
      TestAccessors.GetValue((null as Test)!, key);
  }
	
  private static partial class TestAccessors
  {
    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "GetValue")]
    public static extern int GetValue(Test self, string key);
  }
}
```

* DONE - Methods
* DONE - Properties
* DONE - Constructors
* DONE - Fields

ISSUE TODO:
* Handle the following members and add tests (unit and integration)
    * DONE - Methods
        * DONE - Generic type
            * DONE - Non-generic method
                * DONE - Instance
                * DONE - Static on instance
                * DONE - Static on static
            * DONE - Generic method
                * DONE - Instance
                * DONE - Static on instance
                * DONE - Static on static
        * DONE - Non-generic type
            * DONE - Generic method
                * DONE - Instance
                * DONE - Static on instance
                * DONE - Static on static
    * DONE - Properties
        * DONE - Instance
        * DONE - Static on instance
        * DONE - Static on static
    * DONE - Fields
        * DONE - Instance
        * DONE - Static on instance
        * DONE - Static on static
    * Constructors
* Handle types and methods that have constraints
* Properties with a public getter but private setter, need to handle that.
* .NET 11/`latest` LangVersion

FUTURE TODO:
* ISSUE CREATED - Types in referenced assemblies - Well, as I found out, non-public members - more to the point, **private** members - don't show up if the compilation is created, because the compilation created for a project is (apparently) created with `metadataImportOptions: MetadataImportOptions.All` not being the value (seems like the default is `MetadataImportOptions.Public`). That's a problem. There might be a "trick" to create a compilation that sets the option for what we're looking for. Need to play with it, and even if this is a possibility, it would probably make things much slower - we'd only want to do it if we can tell the type symbol comes from an assembly that was not loaded with metadata (if it's a `PortableExecutableReference`, it won't work, but if it's a `CompilationReference` then it looks like it will).
* Explicit interface implementation - it looks like `CanBeReferencedByName` returns `false` for those, but ... we should be able to call them? Maybe? Needs more research.
* ISSUE MADE - Method definitions - specifically, `in`, `out`, `ref`, `[Scoped]`, `ref` returns, `ref readonly` returns, what can be done here with `[UnsafeAccessor]` and what do I need to update