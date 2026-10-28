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

		// TODO: We may want to be more precise here with the members we process.
		// For example, a type that has an internal method, and that type
		// exists in an assembly that has [InternalsVisibleTo] for the current compilation,
		// it can "see" it with no issue. So we wouldn't really need to generate
		// an unsafe accessor for it.

		// Get all the fields
		var fields = new List<FieldModel>();

		foreach (var field in type.GetMembers().OfType<IFieldSymbol>())
		{
			if (field.DeclaredAccessibility != Accessibility.Public)
			{
				fields.Add(new(field, typeReferenceModelContext));
			}
		}

		return new(new AccessorModel(typeReferenceModelContext.CreateTypeReference(type), [.. fields]));
	}

	internal AccessorModel Accessor { get; }
}