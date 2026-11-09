using SaferAccessors.Analysis.Extensions;
using SaferAccessors.Analysis.Models;
using System.CodeDom.Compiler;
using System.Reflection.Metadata;

namespace SaferAccessors.Analysis.Builders;

internal static class MethodAccessorBuilder
{
	internal static void Build(IndentedTextWriter indentWriter, MethodModel method, ITypeReferenceModel type, ExtensionTargets extensionTargets)
	{
		var parameters = string.Join(", ", method.Parameters.Select(parameter => $"{parameter.Type.FullyQualifiedName} {parameter.Name}"));
		var parameterNames = string.Join(", ", method.Parameters.Select(parameter => parameter.Name));

		if (method.IsStatic)
		{
			if (type.IsGeneric)
			{
				indentWriter.WriteLines(
					$$"""
					extension<{{type.TypeParameterNames}}>({{type.FullyQualifiedName}})
					{
						public static {{method.ReturnType.FullyQualifiedName}} {{method.Name}}({{parameters}}) =>
							{{type.Name}}Accessors<{{type.TypeParameterNames}}>.{{method.Name}}((null as {{extensionTargets.SelfTargeting}})!, {{parameterNames}});
					}

					""");
			}
			else
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
		}
		else
		{
			if (type.IsGeneric)
			{
				indentWriter.WriteLines(
					$$"""
					extension<{{type.TypeParameterNames}}>({{type.FullyQualifiedName}} self)
					{
						public {{method.ReturnType.FullyQualifiedName}} {{method.Name}}({{parameters}}) =>
							{{type.Name}}Accessors<{{type.TypeParameterNames}}>.{{method.Name}}(self, {{parameterNames}});
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

	internal static void BuildGenericAccessor(IndentedTextWriter indentWriter, MethodModel method, ITypeReferenceModel type, ExtensionTargets extensionTargets)
	{
		var accessorKind = method.IsStatic ? "StaticMethod" : "Method";
		var parameters = string.Join(", ", method.Parameters.Select(parameter => $"{parameter.Type.FullyQualifiedName} {parameter.Name}"));
		var targetType = type.IsStatic ? "object?" : type.FullyQualifiedName;

		indentWriter.WriteLines(
			$$"""
			[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.{{accessorKind}}, Name = "{{method.Name}}")]
			public static extern {{method.ReturnType.FullyQualifiedName}} {{method.Name}}({{extensionTargets.SelfReference}}, {{parameters}});
				
			""");
	}
}