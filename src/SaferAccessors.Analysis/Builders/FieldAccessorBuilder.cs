using SaferAccessors.Analysis.Extensions;
using SaferAccessors.Analysis.Models;
using System.CodeDom.Compiler;

namespace SaferAccessors.Analysis.Builders;

internal static class FieldAccessorBuilder
{
	internal static void Build(IndentedTextWriter indentWriter, FieldModel field, ITypeReferenceModel type, ExtensionTargets extensionTargets)
	{
		if (field.IsStatic)
		{
			if (type.IsGeneric)
			{
				if (field.IsReadOnly)
				{
					indentWriter.WriteLines(
						$$"""
						extension<{{type.TypeParameterNames}}>({{type.FullyQualifiedName}})
						{
							public static {{field.Type.FullyQualifiedName}} {{field.Name}}
							{
								get => {{type.Name}}Accessors<{{type.TypeParameterNames}}>.{{field.Name}}((null as {{extensionTargets.SelfTargeting}})!);
							}
						}

						""");
				}
				else
				{
					indentWriter.WriteLines(
						$$"""
						extension<{{type.TypeParameterNames}}>({{type.FullyQualifiedName}})
						{
							public static {{field.Type.FullyQualifiedName}} {{field.Name}}
							{
								get => {{type.Name}}Accessors<{{type.TypeParameterNames}}>.{{field.Name}}((null as {{extensionTargets.SelfTargeting}})!);
								set => {{type.Name}}Accessors<{{type.TypeParameterNames}}>.{{field.Name}}((null as {{extensionTargets.SelfTargeting}})!) = value;
							}
						}

						""");
				}
			}
			else
			{
				if (field.IsReadOnly)
				{
					indentWriter.WriteLines(
						$$"""
						extension({{type.FullyQualifiedName}})
						{
							public static {{field.Type.FullyQualifiedName}} {{field.Name}}
							{
								get => {{type.Name}}Accessors.{{field.Name}}((null as {{extensionTargets.SelfTargeting}})!);
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
								get => {{type.Name}}Accessors.{{field.Name}}((null as {{extensionTargets.SelfTargeting}})!);
								set => {{type.Name}}Accessors.{{field.Name}}((null as {{extensionTargets.SelfTargeting}})!) = value;
							}
						}

						""");
				}

				indentWriter.WriteLines(
					$$"""
					private static partial class {{type.Name}}Accessors
					{
						[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.StaticField, Name = "{{field.Name}}")]
						public static extern ref {{field.Type.FullyQualifiedName}} {{field.Name}}({{extensionTargets.SelfReference}});
					}

					""");
			}
		}
		else
		{
			if (type.IsGeneric)
			{
				if (field.IsReadOnly)
				{
					indentWriter.WriteLines(
						$$"""
						extension<{{type.TypeParameterNames}}>({{type.FullyQualifiedName}} self)
						{
							public {{field.Type.FullyQualifiedName}} {{field.Name}}
							{
								get => {{type.Name}}Accessors<{{type.TypeParameterNames}}>.{{field.Name}}(self);
							}
						}

						""");
				}
				else
				{
					indentWriter.WriteLines(
						$$"""
						extension<{{type.TypeParameterNames}}>({{type.FullyQualifiedName}} self)
						{
							public {{field.Type.FullyQualifiedName}} {{field.Name}}
							{
								get => {{type.Name}}Accessors<{{type.TypeParameterNames}}>.{{field.Name}}(self);
								set => {{type.Name}}Accessors<{{type.TypeParameterNames}}>.{{field.Name}}(self) = value;
							}
						}

						""");
				}
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
								get => {{type.Name}}Accessors.{{field.Name}}(self);
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
								get => {{type.Name}}Accessors.{{field.Name}}(self);
								set => {{type.Name}}Accessors.{{field.Name}}(self) = value;
							}
						}

						""");
				}

				indentWriter.WriteLines(
					$$"""
					private static partial class {{type.Name}}Accessors
					{
						[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.Field, Name = "{{field.Name}}")]
						public static extern ref {{field.Type.FullyQualifiedName}} {{field.Name}}({{extensionTargets.SelfReference}});
					}

					""");
			}
		}
	}

	internal static void BuildGenericAccessor(IndentedTextWriter indentWriter, FieldModel field, ExtensionTargets extensionTargets)
	{
		var accessorKind = field.IsStatic ? "StaticField" : "Field";

		indentWriter.WriteLines(
			$$"""
			[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.{{accessorKind}}, Name = "{{field.Name}}")]
			public static extern ref {{field.Type.FullyQualifiedName}} {{field.Name}}({{extensionTargets.SelfReference}});
				
			""");
	}
}