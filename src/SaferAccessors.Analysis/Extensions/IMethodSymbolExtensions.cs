using Microsoft.CodeAnalysis;
using SaferAccessors.Analysis.Models;
using SaferAccessors.Extensions;
using System.Collections.Immutable;

namespace SaferAccessors.Analysis.Extensions;

internal static class IMethodSymbolExtensions
{
	extension(IMethodSymbol self)
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

		/// <summary>
		/// This is needed because if a method has a generic parameter that is used 
		/// either with a method parameter or the return value and declares the type to be nullable,
		/// the override must create a <c>where T : default</c> constraint.
		/// Otherwise, CS0508 and CS0453 will occur.
		/// </summary>
		/// <param name="self">The target method.</param>
		/// <returns>A list of default constraints.</returns>
		internal EquatableArray<Constraints> GetDefaultConstraints()
		{
			static bool IsAnnotated(ITypeSymbol type, ITypeParameterSymbol typeParameter) =>
				(type.Equals(typeParameter) && type.NullableAnnotation == NullableAnnotation.Annotated) ||
					type.IsOpenGeneric() && ((type as INamedTypeSymbol)?.TypeArguments.Any(_ => IsAnnotated(_, typeParameter)) ?? false);

			if (self.TypeParameters.Length > 0)
			{
				var constraints = new List<Constraints>();

				foreach (var typeParameter in self.TypeParameters)
				{
					// TODO: This is starting to get convoluted.
					// Arguably, it would be good to have one method that gets
					// all constraints for a method, rather than doing it via
					// GetConstraints() and GetDefaultConstraints().
					if (typeParameter.HasReferenceTypeConstraint)
					{
						constraints.Add(new Constraints(typeParameter.GetName(), ["class"]));
					}
					else if (typeParameter.HasValueTypeConstraint)
					{
						constraints.Add(new Constraints(typeParameter.GetName(), ["struct"]));
					}
					else if (self.Parameters.Any(_ => IsAnnotated(_.Type, typeParameter)) ||
						(!self.ReturnsVoid && IsAnnotated(self.ReturnType, typeParameter)))
					{
						constraints.Add(new Constraints(typeParameter.GetName(), ["default"]));
					}
				}

				return [.. constraints];
			}
			else
			{
				return ImmutableArray<Constraints>.Empty;
			}
		}

		internal bool IsUnsafe() =>
			self.Parameters.Any(_ => _.Type.IsPointer()) || (!self.ReturnsVoid && self.ReturnType.IsPointer());
	}
}