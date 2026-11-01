using Microsoft.CodeAnalysis;
using SaferAccessors.Analysis.Extensions;

namespace SaferAccessors.Analysis.Models;

internal sealed record ParameterModel
{
	internal ParameterModel(IParameterSymbol parameter, TypeReferenceModelContext typeReferenceModelContext)
	{
		this.Name = parameter.Name;
		this.RefKind = parameter.RefKind;
		this.RequiresNullableAnnotation = parameter.RequiresForcedNullableAnnotation();
		this.IsParams = parameter.IsParams;
		this.IsScoped = parameter.IsScoped();

		this.Type = typeReferenceModelContext.CreateTypeReference(parameter.Type);

		this.HasExplicitDefaultValue = parameter.HasExplicitDefaultValue;

		if (this.HasExplicitDefaultValue)
		{
			this.ExplicitDefaultValue = parameter.ExplicitDefaultValue.GetDefaultValue(
				parameter.Type, typeReferenceModelContext.SemanticModel.Compilation);
		}
	}

	internal string? ExplicitDefaultValue { get; }
	internal bool HasExplicitDefaultValue { get; }
	internal bool IsParams { get; }
	internal bool IsScoped { get; }
	internal string Name { get; }
	internal RefKind RefKind { get; }
	internal bool RequiresNullableAnnotation { get; }
	internal ITypeReferenceModel Type { get; }
}