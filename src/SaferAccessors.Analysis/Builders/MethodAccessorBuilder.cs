using SaferAccessors.Analysis.Extensions;
using SaferAccessors.Analysis.Models;
using System.CodeDom.Compiler;

namespace SaferAccessors.Analysis.Builders;

internal static class MethodAccessorBuilder
{
	internal static void Build(IndentedTextWriter indentWriter, MethodModel method, ITypeReferenceModel type, ExtensionTargets extensionTargets)
	{
		var parameters = string.Join(", ", method.Parameters.Select(parameter => $"{parameter.Type.FullyQualifiedName} {parameter.Name}"));
		var parameterNames = string.Join(", ", method.Parameters.Select(parameter => parameter.Name));

		if (method.IsStatic)
		{
			indentWriter.WriteLines(
				$$"""
				extension({{type.FullyQualifiedName}})
				{
					public static {{method.ReturnType.FullyQualifiedName}} {{method.Name}}({{parameters}}) =>
						(null as {{extensionTargets.SelfTargeting}})!.{{method.Name}}Method({{parameterNames}});
				}

				extension({{extensionTargets.SelfReference}})
				{
					[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.StaticMethod, Name = "{{method.Name}}")]
					private extern {{method.ReturnType.FullyQualifiedName}} {{method.Name}}Method({{parameters}});
				}
				
				""");
		}
		else
		{
			indentWriter.WriteLines(
				$$"""
				extension({{type.FullyQualifiedName}} self)
				{
					[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.Method, Name = "{{method.Name}}")]
					public extern {{method.ReturnType.FullyQualifiedName}} {{method.Name}}({{parameters}});
				}
				
				""");
		}
	}
}