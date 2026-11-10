using SaferAccessors.Analysis.Extensions;
using SaferAccessors.Analysis.Models;
using System.CodeDom.Compiler;

namespace SaferAccessors.Analysis.Builders;

internal static class PropertyAccessorBuilder
{
	internal static void Build(IndentedTextWriter indentWriter, PropertyModel property, ITypeReferenceModel type, ExtensionTargets extensionTargets)
	{
		if (property.IsStatic)
		{
			indentWriter.WriteLines(
				$$"""
				extension({{type.FullyQualifiedName}})
				{
					public static {{property.Type.FullyQualifiedName}} {{property.Name}}
					{
				""");
			indentWriter.Indent += 2;

			if (property.HasGet)
			{
				indentWriter.WriteLine($"get => {type.Name}Accessors.Get{property.Name}((null as {extensionTargets.SelfTargeting})!);");
			}

			if (property.HasSet)
			{
				indentWriter.WriteLine($"set => {type.Name}Accessors.Set{property.Name}((null as {extensionTargets.SelfTargeting})!, value);");
			}

			indentWriter.Indent -= 2;
			indentWriter.WriteLines(
				$$"""
					}
				}
				
				private static partial class {{type.Name}}Accessors
				{
				""");
			indentWriter.Indent++;

			if (property.HasGet)
			{
				indentWriter.WriteLines(
					$$"""
					[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.StaticMethod, Name = "get_{{property.Name}}")]
					public static extern {{property.Type.FullyQualifiedName}} Get{{property.Name}}({{extensionTargets.SelfReference}});

					""");
			}

			if (property.HasSet)
			{
				indentWriter.WriteLines(
					$$"""
					[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.StaticMethod, Name = "set_{{property.Name}}")]
					public static extern void Set{{property.Name}}({{extensionTargets.SelfReference}}, {{property.Type.FullyQualifiedName}} value);

					""");
			}

			indentWriter.Indent--;
			indentWriter.WriteLines(
				"""
				}

				""");
		}
		else
		{
			indentWriter.WriteLines(
				$$"""
				extension({{extensionTargets.SelfReference}})
				{
					public {{property.Type.FullyQualifiedName}} {{property.Name}}
					{
				""");
			indentWriter.Indent += 2;

			if (property.HasGet)
			{
				indentWriter.WriteLine($"get => {type.Name}Accessors.Get{property.Name}(self);");
			}

			if (property.HasSet)
			{
				indentWriter.WriteLine($"set => {type.Name}Accessors.Set{property.Name}(self, value);");
			}

			indentWriter.Indent -= 2;
			indentWriter.WriteLines(
				$$"""
					}
				}

				private static partial class {{type.Name}}Accessors
				{
				""");
			indentWriter.Indent++;

			if (property.HasGet)
			{
				indentWriter.WriteLines(
					$$"""
					[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.Method, Name = "get_{{property.Name}}")]
					public static extern {{property.Type.FullyQualifiedName}} Get{{property.Name}}({{extensionTargets.SelfReference}});

					""");
			}

			if (property.HasSet)
			{
				indentWriter.WriteLines(
					$$"""
					[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.Method, Name = "set_{{property.Name}}")]
					public static extern void Set{{property.Name}}({{extensionTargets.SelfReference}}, {{property.Type.FullyQualifiedName}} value);

					""");
			}

			indentWriter.Indent--;
			indentWriter.WriteLines(
				"""
				}

				""");
		}
	}
}