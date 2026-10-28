using SaferAccessors.Analysis.Extensions;
using SaferAccessors.Analysis.Models;
using System.CodeDom.Compiler;

namespace SaferAccessors.Analysis.Builders;

internal static class FieldAccessorBuilder
{
	// TODO: We need to make sure the name is Pascal-cased ... maybe.
	// That is, if the field is named "id", we want the accessor to be called "IdField".
	// If the field starts with "_", then what? It's fine to have a method called "_IdField", though it looks weird.
	internal static void Build(IndentedTextWriter indentWriter, FieldModel field) => 
		indentWriter.WriteLines(
			$$"""
			[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.Field, Name = "{{field.Name}}")]
			public static extern {{(field.IsReadOnly ? "ref " : string.Empty)}}int {{field.Name}}Field();

			""");
}