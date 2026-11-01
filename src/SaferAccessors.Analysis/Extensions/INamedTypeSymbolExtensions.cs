using Microsoft.CodeAnalysis;

namespace SaferAccessors.Analysis.Extensions;

internal static class INamedTypeSymbolExtensions
{
	extension(INamedTypeSymbol self)
	{
		internal bool HasOpenGenerics() =>
			self.TypeArguments.Any(_ => _.TypeKind == TypeKind.TypeParameter ||
				_ is INamedTypeSymbol argumentTypeSymbol && argumentTypeSymbol.HasOpenGenerics());
	}
}