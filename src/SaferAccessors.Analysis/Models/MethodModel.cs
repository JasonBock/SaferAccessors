using Microsoft.CodeAnalysis;
using SaferAccessors.Analysis.Extensions;

namespace SaferAccessors.Analysis.Models;

internal sealed record MethodModel
{
	internal MethodModel(IMethodSymbol method, TypeReferenceModelContext typeReferenceModelContext)
	{
		var compilation = typeReferenceModelContext.SemanticModel.Compilation;

		this.IsGenericMethod = method.IsGenericMethod;
		this.IsUnsafe = method.IsUnsafe();
		this.IsStatic = method.IsStatic;

		this.Constraints = method.GetConstraints(compilation);
		this.DefaultConstraints = method.GetDefaultConstraints();
		this.TypeArguments = [.. method.TypeArguments.Select(_ => typeReferenceModelContext.CreateTypeReference(_))];
		this.TypeParameters = [.. method.TypeParameters.Select(_ => typeReferenceModelContext.CreateTypeReference(_))];

		this.Name = method.Name;

		this.Parameters = [..method.Parameters.Select(
			_ => new ParameterModel(_, typeReferenceModelContext))];

		this.ReturnType = typeReferenceModelContext.CreateTypeReference(method.ReturnType);
		this.ReturnsByRef = method.ReturnsByRef;
		this.ReturnsByRefReadOnly = method.ReturnsByRefReadonly;

		this.ReturnTypeTypeArguments = method.ReturnType is INamedTypeSymbol returnType ?
			[.. returnType.TypeArguments.Select(_ => typeReferenceModelContext.CreateTypeReference(_))] : [];
	}

	internal EquatableArray<Constraints> Constraints { get; }
	internal EquatableArray<Constraints> DefaultConstraints { get; }
	internal bool IsGenericMethod { get; }
	internal bool IsStatic { get; }
	internal bool IsUnsafe { get; }
	internal string Name { get; }
	internal EquatableArray<ParameterModel> Parameters { get; }
	internal ITypeReferenceModel ReturnType { get; }
	internal EquatableArray<ITypeReferenceModel> ReturnTypeTypeArguments { get; }
	internal bool ReturnsVoid { get; }
	internal bool ReturnsByRef { get; }
	internal bool ReturnsByRefReadOnly { get; }
	internal EquatableArray<ITypeReferenceModel> TypeArguments { get; }
	internal EquatableArray<ITypeReferenceModel> TypeParameters { get; }
}