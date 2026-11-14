using SaferAccessors.Analysis.Extensions;
using SaferAccessors.Analysis.Models;
using System.CodeDom.Compiler;

namespace SaferAccessors.Analysis.Builders;

internal static class ConstructorAccessorBuilder
{
	internal static void Build(IndentedTextWriter indentWriter, MethodModel method, ITypeReferenceModel type)
	{
		var parameters = string.Join(", ", method.Parameters.Select(parameter => $"{parameter.Type.FullyQualifiedName} {parameter.Name}"));
		var parameterNames = string.Join(", ", method.Parameters.Select(parameter => parameter.Name));

		if (type.IsGeneric)
		{
			indentWriter.WriteLines(
				$$"""
				extension<{{type.TypeParameterNames}}>({{type.FullyQualifiedName}})
				{
					public static {{type.FullyQualifiedName}} New({{parameters}}) =>
						{{type.Name}}Accessors<{{type.TypeParameterNames}}>.New({{parameterNames}});
				}
				
				""");
		}
		else
		{
			indentWriter.WriteLines(
				$$"""
				extension({{type.FullyQualifiedName}})
				{
					[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.Constructor)]
					public extern static {{type.FullyQualifiedName}} New({{parameters}});
				}
				
				""");
		}
	}

	internal static void BuildGenericAccessor(IndentedTextWriter indentWriter, MethodModel constructor, ITypeReferenceModel targetType)
	{
		var parameters = string.Join(", ", constructor.Parameters.Select(parameter => $"{parameter.Type.FullyQualifiedName} {parameter.Name}"));
		var parameterNames = string.Join(", ", constructor.Parameters.Select(parameter => parameter.Name));

		indentWriter.WriteLines(
			$$"""
			[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.Constructor)]
			public extern static {{targetType.FullyQualifiedName}} New({{parameters}});
				
			""");
	}
}