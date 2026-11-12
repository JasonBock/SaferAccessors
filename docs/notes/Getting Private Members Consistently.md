# Getting Private Members Consistently (Hopefully)

There's a couple of issues going on:
* Compilations created by Roslyn (so I can't control it), are loaded with the `MetadataImportOptions` value set to `MetadataImportOptions.Public`. This means it won't load private members from references. 
* Somewhat nebulous here: it seems like if the loaded reference is a `CompilationReference`, it **will** load private members. But if it's a `PortableExecutableReference`, it doesn't.

Here's a test I wrote that helped me diagnose what was going on:

```c#
var source =
    """
    public class Holder
    {
        private int id;

        private Holder() { }

        private void ProcessData(string data) { }

        private string Data { get; set; }
    }
    """;

var sourceReferences = Shared.References.Value
    .Cast<MetadataReference>();
var sourceSyntaxTree = CSharpSyntaxTree.ParseText(source);
var sourceCompilation = CSharpCompilation.Create("Source", [sourceSyntaxTree],
    sourceReferences,
    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
var sourceReference = sourceCompilation.ToMetadataReference()!;

//using var outputStream = new MemoryStream();
//var result = sourceCompilation.Emit(outputStream);
//outputStream.Position = 0;
//var outputReference = MetadataReference.CreateFromStream(outputStream);

var code =
    """
    using SaferAccessors;

    [assembly: UnsafeAccessorMap(typeof(Holder))]
    """;

var generatedCode = "";

await TestAssistants.RunGeneratorAsync<SaferAccessorsGenerator>(code,
    [($"Holder_Accessors.g.cs", generatedCode)],
    [],
    additionalReferences: sourceReferences.Append(sourceReference));
```

In this case, `generatedCode` has the four accessors I would expect. That's because I'm using `sourceReference`, which is a `CompilationReference`. `CompilationReference` derives from `MetadataReference`. However, if I uncomment the code such that I have `outputReference`, and pass that into `additionalReferences:`, I get nothing generated. `outputReference` is a `PortableExecutableReference`, which also derives from `CompilationReference`.

So, it seems like if the type comes from a reference like an assembly (either direct or from a NuGet package), I won't see the private members. That really makes SaferAccessor more of a fun experiment, but not something that's really useful either.

There is one "trick" I'd like to try, but I fear that the performance would not be good.

```c#
context.SemanticModel.Compilation.References.ToArray().Where(_ => _.Display == targetType.ContainingAssembly.Name)
```

This is a `MetadataReference`. Specifically, it's a `CSharpCompilationReference`, which derives from `CompilationReference`. Therefore:

```c#
?ttReference is CompilationReference
```

Evaluates to `true`. When I make it in-memory, it's `MetadataImageReference`, which derives from `PortableExecutableReference`.

`targetType.ContainingAssembly.Locations[0]` for `string`, the `Kind` is `MetadataFile`. When I use `CompilationReference`, it's `SourceFile`. 

I think I'm getting somewhere. If the `Kind` is `MetadataFile` for `targetType.ContainingAssembly.Locations[0]`, then we're (probably) not going to get private information. Could we make a temporary compilation?

```c#
var tempCompilation = CSharpCompilation.Create("temp", [CSharpSyntaxTree.ParseText(string.Empty, cancellationToken: token)],
    context.SemanticModel.Compilation.References,
    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, metadataImportOptions: MetadataImportOptions.All));
targetType = tempCompilation.GetTypeByMetadataName(targetType.MetadataName)!;
```

Now, if the target is `string`, this doesn't work - `GetTypeByMetadataName()` returns `null`. I'm not sure why. If that does happen, I guess we'd have to fall back to what `targetType` originally was. I should try this with a type from a NuGet package, maybe like CSLA, because I know it has private members, like `ApplicationContext`.

```c#
targetType = tempCompilation.GetTypeByMetadataName(targetType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat))
```

**This** looks like that works when I target `Csla.ApplicationContext`. We're making progress!

**AND**, if I use `"System.String"` for the metadata name lookup, **that** works perfectly! Now I'm cooking with gas :). I'll need to figure out how to get the correct metadata name for types like `string`, `int`, etc. Also, generics have a metadata name something like "System.Collections.Generic.List<>", and it needs to be "System.Collections.Generic.List`1".

So there's things to clarify with the metadata name and lookups, but this feels like a path forward.

BTW to get a new compilation with everything imported, this **might** work as well:

```c#
var allCompilation = context.SemanticModel.Compilation.WithOptions(
    context.SemanticModel.Compilation.Options.WithMetadataImportOptions(MetadataImportOptions.All));
```

Basically taking the current compilation and creating a new one from it with different import options. Not sure if this would be faster or slower or what, but it's a little bit more succinct.

Summary:
* Test for target type from an assembly - `MetadataReference.CreateFromStream(outputStream)`
* Test for `string` being the target
* Test for a generic (e.g `List<>`) being the targetf