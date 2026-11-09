using SaferAccessors.Analysis.Models;

namespace SaferAccessors.Analysis.Builders;

internal sealed class ExtensionTargets
{
	internal ExtensionTargets(ITypeReferenceModel typeReferenceModel)
	{
		var unsafeTypeNamespace =
			!string.IsNullOrWhiteSpace(typeReferenceModel.Namespace) ?
				$"{typeReferenceModel.Namespace}." :
				"";
		var unsafeTypeName = typeReferenceModel.Name;

		this.SelfTargeting =
			typeReferenceModel.IsStatic ?
				"object" :
				typeReferenceModel.FullyQualifiedName;
		this.SelfReference =
			typeReferenceModel.IsStatic ?
				$"""[global::System.Runtime.CompilerServices.UnsafeAccessorType("{unsafeTypeNamespace}{unsafeTypeName}")] object? self""" :
				$"{typeReferenceModel.FullyQualifiedName} self";

		if (typeReferenceModel.IsStatic)
		{
			var typeParameterCount = 
				typeReferenceModel.TypeParameters.Length > 0 ? 
					$"`{typeReferenceModel.TypeParameters.Length}" : 
					"";
			var typeParameters =
				typeReferenceModel.TypeParameters.Length > 0 ? 
					$"[[{string.Join(", ", Enumerable.Range(0, typeReferenceModel.TypeParameters.Length).Select(index => $"!{index}"))}]]" :
					"";
			this.SelfReference = $"""[global::System.Runtime.CompilerServices.UnsafeAccessorType("{unsafeTypeNamespace}{unsafeTypeName}{typeParameterCount}{typeParameters}")] object? self""";
		}
		else
		{
			this.SelfReference = $"{typeReferenceModel.FullyQualifiedName} self";
		}
	}

	internal string SelfTargeting { get; }
	internal string SelfReference { get; }
}