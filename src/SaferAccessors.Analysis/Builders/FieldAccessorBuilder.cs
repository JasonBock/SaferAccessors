using SaferAccessors.Analysis.Extensions;
using SaferAccessors.Analysis.Models;
using System.CodeDom.Compiler;

namespace SaferAccessors.Analysis.Builders;

internal static class FieldAccessorBuilder
{
	internal static void Build(IndentedTextWriter indentWriter, FieldModel field, ITypeReferenceModel type)
	{
		if (field.IsStatic)
		{
			if (field.IsReadOnly)
			{
				indentWriter.WriteLines(
					$$"""
					extension({{type.FullyQualifiedName}})
					{
						public static {{field.Type.FullyQualifiedName}} {{field.Name}}Field() =>
							(null as {{type.FullyQualifiedName}})!.{{field.Name}}FieldUnsafe();
					}
					
					extension({{type.FullyQualifiedName}} self)
					{
						[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.Field, Name = "{{field.Name}}")]
						private extern ref {{field.Type.FullyQualifiedName}} {{field.Name}}FieldUnsafe();
					}

					""");
			}
			else
			{
				indentWriter.WriteLines(
					$$"""
					extension({{type.FullyQualifiedName}})
					{
						[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.Field, Name = "{{field.Name}}")]
						public static extern ref {{field.Type.FullyQualifiedName}} {{field.Name}}Field();
					}

					""");
			}
		}
		else
		{
			indentWriter.WriteLines(
				$$"""
				extension({{type.FullyQualifiedName}} self)
				{
				""");
			indentWriter.Indent++;

			if (field.IsReadOnly)
			{
				indentWriter.WriteLines(
					$$"""
					public {{field.Type.FullyQualifiedName}} {{field.Name}}Field() =>
						self.{{field.Name}}FieldUnsafe();
					
					[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.Field, Name = "{{field.Name}}")]
					private extern ref {{field.Type.FullyQualifiedName}} {{field.Name}}FieldUnsafe();

					""");
			}
			else
			{
				indentWriter.WriteLines(
					$$"""
					[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.Field, Name = "{{field.Name}}")]
					public extern ref {{field.Type.FullyQualifiedName}} {{field.Name}}Field();

					""");
			}

			indentWriter.Indent--;
			indentWriter.WriteLine("}");
		}
	}
}