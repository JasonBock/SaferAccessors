namespace SaferAccessors.Analysis.Models;

internal sealed record AccessorModel(ITypeReferenceModel Type, 
	EquatableArray<FieldModel> Fields, EquatableArray<MethodModel> Constructors, 
	EquatableArray<MethodModel> Methods, EquatableArray<PropertyModel> Properties);