namespace Transform.Compiler
{
	public class InitializedVariable : Variable
	{
		public Expression Value { get; }

		public InitializedVariable(string name, TypeReference type, Expression value) : base(name, type)
		{
			Value = value;
		}
	}
}
