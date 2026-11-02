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
	}

	internal string SelfTargeting { get; }
	internal string SelfReference { get; }
}