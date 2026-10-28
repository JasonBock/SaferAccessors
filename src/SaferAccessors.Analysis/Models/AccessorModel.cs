namespace SaferAccessors.Analysis.Models;

internal sealed record AccessorModel(ITypeReferenceModel Type, EquatableArray<FieldModel> Fields);