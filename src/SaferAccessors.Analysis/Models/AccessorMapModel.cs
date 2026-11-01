using Microsoft.CodeAnalysis;

namespace SaferAccessors.Analysis.Models;

internal sealed record AccessorMapModel
{
	private AccessorMapModel(AccessorModel accessor) =>
		this.Accessor = accessor;

	internal static AccessorMapModel Create(ITypeSymbol type, TypeReferenceModelContext typeReferenceModelContext)
	{
		// TODO: What about inheritance? How does that figure in to this?

		// TODO: Should interfaces be excluded? They can have non-public DIMs.

		// TODO: We only want members that are private
		// or internal that we don't have [InternalsVisibleTo] access.
		// Handling private for now, will need to come back to this.

		// Get all the members
		var fields = new List<FieldModel>();
		var methods = new List<MethodModel>();

		foreach (var member in type.GetMembers())
		{
			if (member is IFieldSymbol field)
			{
				if (field.CanBeReferencedByName && field.DeclaredAccessibility == Accessibility.Private)
				{
					fields.Add(new(field, typeReferenceModelContext));
				}
			}
			else if (member is IMethodSymbol method)
			{
				if (method.CanBeReferencedByName && method.DeclaredAccessibility == Accessibility.Private)
				{
					methods.Add(new(method, typeReferenceModelContext));
				}
			}
		}

		return new(new AccessorModel(typeReferenceModelContext.CreateTypeReference(type), [.. fields], [.. methods]));
	}

	internal AccessorModel Accessor { get; }
}