using Microsoft.CodeAnalysis;

namespace SaferAccessors.Analysis.Models;

internal interface ITypeReferenceModel
	: IEquatable<ITypeReferenceModel>, IComparable<ITypeReferenceModel>
{
	internal EquatableArray<Constraints> Constraints { get; }
	internal string FullyQualifiedName { get; }
	internal bool IsGeneric { get; }
	internal bool IsStatic { get; }
	internal string Name { get; }
	internal string? Namespace { get; }
	internal SymbolKind SymbolKind { get; }
	internal EquatableArray<ITypeReferenceModel> TypeParameters { get; }
	internal string? TypeParameterNames { get; }
}