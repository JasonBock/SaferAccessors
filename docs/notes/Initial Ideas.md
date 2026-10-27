OK, I have an interesting, potentially wild idea ... `SaferAccessors`

I create an attribute called `UnsafeAccessorMap`, that does this:

```c#
[assembly: UnsafeAccessorMap(typeof(Content))]

public sealed class Content
{
  private int id;
  private Guid index;

  private int Compute() => this.id ^ this.index.GetHashCode();
}
```

This would generate:

```c#
public static class ContentUnsafeAccessors
{
  [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "Compute")]
  public static extern int ComputeMethod(Content content);

  [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "id")]
  public static extern ref int IdField(Content content);

  [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "index")]
  public static extern ref Guid IndexField(Content content);
}
```

Basically, it would look for all the inaccessible members, and do the following:
* For each constructor, it would create a "Constructor" with the right parameters, 
* For each method, it would create a "{MethodName}Method" with the right return value and parameters
* For each field, it would create a "{FieldName}Field", returning it as a `ref`, and Pascal-casing the name
* For each property, it would create a "{Get/Set/Init}{PropertyName}Property/Indexer". For an indexer, the property name would be blank (maybe "This")

I'm also wondering if I can make all the accessors extension members ... I think I can, a POC shows it can work:

```c#
public static class ContentUnsafeAccessorsExtensions
{
  extension(Content self)
  {
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "Compute")]
    public extern int ComputeMethod();

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "id")]
    public extern ref int IdField();

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "index")]
    public extern ref Guid IndexField();
  }
}
```

Static methods and fields would also work.

References:
* https://andrewlock.net/exploring-dotnet-10-preview-features-9-easier-reflection-with-unsafeaccessortype/
* https://medium.com/@malarsharmila/introduction-to-the-unsafeaccessorattribute-class-in-net-8-0-d3a55ec15762