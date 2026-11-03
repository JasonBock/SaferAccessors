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
		var constructors = new List<MethodModel>();
		var methods = new List<MethodModel>();
		var properties = new List<PropertyModel>();

		foreach (var member in type.GetMembers())
		{
			if (member is IFieldSymbol field &&
				field.CanBeReferencedByName && field.DeclaredAccessibility == Accessibility.Private)
			{
				fields.Add(new(field, typeReferenceModelContext));
			}
			else if (member is IMethodSymbol method && method.DeclaredAccessibility == Accessibility.Private)
			{
				if (method.CanBeReferencedByName && method.MethodKind != MethodKind.Constructor)
				{
					methods.Add(new(method, typeReferenceModelContext));
				}
				else if (method.MethodKind == MethodKind.Constructor)
				{
					constructors.Add(new(method, typeReferenceModelContext));
				}
			}
			else if (member is IPropertySymbol property && !property.IsIndexer &&
				property.CanBeReferencedByName && property.DeclaredAccessibility == Accessibility.Private)
			{
				properties.Add(new(property, typeReferenceModelContext));
			}
		}

		return new(
			new AccessorModel(typeReferenceModelContext.CreateTypeReference(type), [.. fields], [.. constructors], [.. methods], [.. properties]));
	}

	internal AccessorModel Accessor { get; }
}