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
						public static {{field.Type.FullyQualifiedName}} {{field.Name}}
						{
							get => (null as {{type.FullyQualifiedName}})!.{{field.Name}}Field();
						}
					}

					""");
			}
			else
			{
				indentWriter.WriteLines(
					$$"""
					extension({{type.FullyQualifiedName}})
					{
						public static {{field.Type.FullyQualifiedName}} {{field.Name}}
						{
							get => (null as {{type.FullyQualifiedName}})!.{{field.Name}}Field();
							set => (null as {{type.FullyQualifiedName}})!.{{field.Name}}Field() = value;
						}
					}

					""");
			}

			indentWriter.WriteLines(
				$$"""
				extension({{type.FullyQualifiedName}} self)
				{
					[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.StaticField, Name = "{{field.Name}}")]
					private extern ref {{field.Type.FullyQualifiedName}} {{field.Name}}Field();
				}

				""");
		}
		else
		{
			if (field.IsReadOnly)
			{
				indentWriter.WriteLines(
					$$"""
					extension({{type.FullyQualifiedName}} self)
					{
						public {{field.Type.FullyQualifiedName}} {{field.Name}}
						{
							get => self.{{field.Name}}Field();
						}
					}

					""");
			}
			else
			{
				indentWriter.WriteLines(
					$$"""
					extension({{type.FullyQualifiedName}} self)
					{
						public {{field.Type.FullyQualifiedName}} {{field.Name}}
						{
							get => self.{{field.Name}}Field();
							set => self.{{field.Name}}Field() = value;
						}
					}

					""");
			}

			indentWriter.WriteLines(
				$$"""
			extension({{type.FullyQualifiedName}} self)
			{
				[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.Field, Name = "{{field.Name}}")]
				private extern ref {{field.Type.FullyQualifiedName}} {{field.Name}}Field();
			}

			""");
		}
	}
}