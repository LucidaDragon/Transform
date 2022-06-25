namespace Transform.Compiler
{
	public class ReturnStatement : Statement
	{
		public Expression Expression { get; }

		public ReturnStatement(CodeBody parent, Expression expression) : base(parent)
		{
			Expression = expression;
		}

		public override InitializedVariable[] GetLocals()
		{
			return new InitializedVariable[0];
		}

		public override void Emit(IEmitter emit, CodeBody parent, ILabel endOfBlock)
		{
			Expression.Emit(emit, parent, new TypeReference[0]);
			emit.Return();
		}
	}
}
