using Microsoft.CodeAnalysis;
using SaferAccessors.Analysis.Models;

namespace SaferAccessors.Analysis.Extensions;

internal static class INamedTypeSymbolExtensions
{
	extension(INamedTypeSymbol self)
	{
		internal EquatableArray<Constraints> GetConstraints(Compilation compilation)
		{
			var constraints = new List<Constraints>();

			if (self.TypeParameters.Length > 0)
			{
				for (var i = 0; i < self.TypeParameters.Length; i++)
				{
					var typeParameter = self.TypeParameters[i];

					if (typeParameter.Equals(self.TypeArguments[i]))
					{
						var constraint = typeParameter.GetConstraints(compilation);

						if (constraint is not null)
						{
							constraints.Add(constraint);
						}
					}
				}
			}

			return [.. constraints];
		}

		internal bool HasOpenGenerics() =>
			self.TypeArguments.Any(_ => _.TypeKind == TypeKind.TypeParameter ||
				_ is INamedTypeSymbol argumentTypeSymbol && argumentTypeSymbol.HasOpenGenerics());
	}
}