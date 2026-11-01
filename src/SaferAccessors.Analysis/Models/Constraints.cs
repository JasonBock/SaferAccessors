namespace SaferAccessors.Analysis.Models;

internal sealed record Constraints(string TypeName, EquatableArray<string> Values)
{
	public override string ToString() =>
		$"where {this.TypeName} : {string.Join(", ", this.Values)}";
}