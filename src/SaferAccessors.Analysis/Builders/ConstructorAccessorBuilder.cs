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