using Microsoft.CodeAnalysis;

namespace SaferAccessors.Analysis.Models;

internal sealed record FieldModel
{
	internal FieldModel(IFieldSymbol field, TypeReferenceModelContext typeReferenceModelContext)
	{
		this.Name = field.Name;
		this.Type = typeReferenceModelContext.CreateTypeReference(field.Type);
		this.IsReadOnly = field.IsReadOnly;
		this.IsStatic = field.IsStatic;
	}

	internal bool IsReadOnly { get; }
	internal bool IsStatic { get; }
	internal string Name { get; }
	internal ITypeReferenceModel Type { get; }
}