using Microsoft.CodeAnalysis;

namespace SaferAccessors.Analysis.Models;

internal interface ITypeReferenceModel
	: IEquatable<ITypeReferenceModel>, IComparable<ITypeReferenceModel>
{
	internal string FullyQualifiedName { get; }
	internal string Name { get; }
	internal string? Namespace { get; }
	public SymbolKind SymbolKind { get; }
}