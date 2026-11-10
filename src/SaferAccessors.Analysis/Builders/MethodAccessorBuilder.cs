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
		var methodTypeParameterNames =
			method.TypeParameterNames is not null ?
			$"<{method.TypeParameterNames}>" :
			null;

		if (method.IsStatic)
		{
			if (type.IsGeneric)
			{
				indentWriter.WriteLines(
					$$"""
					extension<{{type.TypeParameterNames}}>({{type.FullyQualifiedName}})
					{
						public static {{method.ReturnType.FullyQualifiedName}} {{method.Name}}{{methodTypeParameterNames}}({{parameters}}) =>
							{{type.Name}}Accessors<{{type.TypeParameterNames}}>.{{method.Name}}{{methodTypeParameterNames}}((null as {{extensionTargets.SelfTargeting}})!, {{parameterNames}});
					}

					""");
			}
			else
			{
				indentWriter.WriteLines(
					$$"""
					extension({{type.FullyQualifiedName}})
					{
						public static {{method.ReturnType.FullyQualifiedName}} {{method.Name}}{{methodTypeParameterNames}}({{parameters}}) =>
							{{type.Name}}Accessors.{{method.Name}}{{methodTypeParameterNames}}((null as {{extensionTargets.SelfTargeting}})!, {{parameterNames}});
					}

					private static partial class {{type.Name}}Accessors
					{
						[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.StaticMethod, Name = "{{method.Name}}")]
						public static extern {{method.ReturnType.FullyQualifiedName}} {{method.Name}}{{methodTypeParameterNames}}({{extensionTargets.SelfReference}}, {{parameters}});
					}

					""");
			}
		}
		else
		{
			if (type.IsGeneric)
			{
				indentWriter.WriteLines(
					$$"""
					extension<{{type.TypeParameterNames}}>({{type.FullyQualifiedName}} self)
					{
						public {{method.ReturnType.FullyQualifiedName}} {{method.Name}}{{methodTypeParameterNames}}({{parameters}}) =>
							{{type.Name}}Accessors<{{type.TypeParameterNames}}>.{{method.Name}}{{methodTypeParameterNames}}(self, {{parameterNames}});
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
						public extern {{method.ReturnType.FullyQualifiedName}} {{method.Name}}{{methodTypeParameterNames}}({{parameters}});
					}
				
					""");
			}
		}
	}

	internal static void BuildGenericAccessor(IndentedTextWriter indentWriter, MethodModel method, ExtensionTargets extensionTargets)
	{
		var accessorKind = method.IsStatic ? "StaticMethod" : "Method";
		var parameters = string.Join(", ", method.Parameters.Select(parameter => $"{parameter.Type.FullyQualifiedName} {parameter.Name}"));
		var methodTypeParameterNames =
			method.TypeParameterNames is not null ?
			$"<{method.TypeParameterNames}>" :
			null;

		indentWriter.WriteLines(
			$$"""
			[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.{{accessorKind}}, Name = "{{method.Name}}")]
			public static extern {{method.ReturnType.FullyQualifiedName}} {{method.Name}}{{methodTypeParameterNames}}({{extensionTargets.SelfReference}}, {{parameters}});
				
			""");
	}
}