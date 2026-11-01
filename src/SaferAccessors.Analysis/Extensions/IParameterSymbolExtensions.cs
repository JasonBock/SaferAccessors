using Microsoft.CodeAnalysis;

namespace SaferAccessors.Analysis.Extensions;

internal static class IParameterSymbolExtensions
{
	extension(IParameterSymbol self)
	{
		internal bool IsScoped() =>
			(self.Type.IsRefLikeType && !self.IsParams && self.ScopedKind == ScopedKind.ScopedValue) ||
			(self.Type.IsValueType && self.RefKind == RefKind.Ref && self.ScopedKind == ScopedKind.ScopedRef);

		internal bool RequiresForcedNullableAnnotation() =>
			self.HasExplicitDefaultValue && self.ExplicitDefaultValue is null &&
				self.Type.TypeKind != TypeKind.TypeParameter &&
				!self.Type.IsValueType && self.NullableAnnotation != NullableAnnotation.Annotated;
	}
}