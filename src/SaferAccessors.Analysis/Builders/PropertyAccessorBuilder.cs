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
				indentWriter.WriteLine($"get => (null as {extensionTargets.SelfTargeting})!.Get{property.Name}Property();");
			}

			if (property.HasSet)
			{
				indentWriter.WriteLine($"set => (null as {extensionTargets.SelfTargeting})!.Set{property.Name}Property() = value;");
			}

			indentWriter.Indent -= 2;
			indentWriter.WriteLines(
				$$"""
					}
				}
				
				extension({{extensionTargets.SelfReference}})
				{
				""");
			indentWriter.Indent++;

			if (property.HasGet)
			{
				indentWriter.WriteLines(
					$$"""
					[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.StaticMethod, Name = "get_{{property.Name}}")]
					private extern ref {{property.Type.FullyQualifiedName}} Get{{property.Name}}Property();

					""");
			}

			if (property.HasSet)
			{
				indentWriter.WriteLines(
					$$"""
					[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.StaticMethod, Name = "set_{{property.Name}}")]
					private extern ref {{property.Type.FullyQualifiedName}} Set{{property.Name}}Property();

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
				indentWriter.WriteLine($"get => self.Get{property.Name}Property();");
			}

			if (property.HasSet)
			{
				indentWriter.WriteLine($"set => self.Set{property.Name}Property(value);");
			}

			indentWriter.Indent -= 2;
			indentWriter.WriteLines(
				"""
					}

				""");
			indentWriter.Indent++;

			if (property.HasGet)
			{
				indentWriter.WriteLines(
					$$"""
					[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.Method, Name = "get_{{property.Name}}")]
					private extern {{property.Type.FullyQualifiedName}} Get{{property.Name}}Property();

					""");
			}

			if (property.HasSet)
			{
				indentWriter.WriteLines(
					$$"""
					[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.Method, Name = "set_{{property.Name}}")]
					private extern {{property.Type.FullyQualifiedName}} Set{{property.Name}}Property({{property.Type.FullyQualifiedName}} value);

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