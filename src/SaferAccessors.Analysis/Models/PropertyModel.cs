using Microsoft.CodeAnalysis;

namespace SaferAccessors.Analysis.Models;

internal sealed record PropertyModel
{
	internal PropertyModel(IPropertySymbol property, TypeReferenceModelContext typeReferenceModelContext)
	{
		this.Name = property.Name;
		this.Type = typeReferenceModelContext.CreateTypeReference(property.Type);
		this.IsStatic = property.IsStatic;
		this.HasGet = property.GetMethod is not null && property.GetMethod.DeclaredAccessibility == Accessibility.Private;
		this.HasSet = property.SetMethod is not null && !property.SetMethod.IsInitOnly && property.SetMethod.DeclaredAccessibility == Accessibility.Private;
	}

	internal bool HasGet { get; }
	internal bool HasSet { get; }
	internal bool IsStatic { get; }
	internal string Name { get; }
	internal ITypeReferenceModel Type { get; }
}