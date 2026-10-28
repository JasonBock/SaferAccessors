using Microsoft.CodeAnalysis;
using SaferAccessors.Analysis.Builders;
using SaferAccessors.Analysis.Models;
using System.Collections.Immutable;

namespace SaferAccessors.Analysis;

[Generator]
internal sealed class SaferAccessorsGenerator
	: IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var provider = context.SyntaxProvider
			.ForAttributeWithMetadataName("SaferAccessors.UnsafeAccessorMapAttribute", (_, _) => true,
				(context, token) =>
				{
					var models = new List<AccessorModel>(context.Attributes.Length);
					var typeReferenceModelContext = new TypeReferenceModelContext(context.SemanticModel);

					foreach (var attribute in context.Attributes)
					{
						var targetType = (attribute.ConstructorArguments[0].Value as ITypeSymbol)!;
						models.Add(AccessorMapModel.Create(targetType, typeReferenceModelContext).Accessor);
					}

					return models;
				})
			.SelectMany((models, _) => models);

		context.RegisterSourceOutput(provider.Collect(),
			(context, source) => SaferAccessorsGenerator.CreateOutput(source, context));
	}

	private static void CreateOutput(ImmutableArray<AccessorModel> models, SourceProductionContext context)
	{
		foreach (var model in models.Distinct())
		{
			var builder = new AccessorBuilder(model);
			context.AddSource(builder.FileName, builder.Text);
		}
	}
}