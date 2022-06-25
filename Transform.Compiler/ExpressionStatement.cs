namespace Transform.Compiler
{
	public class ExpressionStatement : Statement
	{
		public Expression Expression { get; }

		public ExpressionStatement(CodeBody parent, Expression expression) : base(parent)
		{
			Expression = expression;
		}

		public override InitializedVariable[] GetLocals() => new InitializedVariable[0];

		public override void Emit(IEmitter emit, CodeBody parent, ILabel endOfBlock)
		{
			Expression.Emit(emit, parent, new TypeReference[0]);
			if (Expression.GetResultType(emit, parent, new TypeReference[0]).ResolveSize(emit.GetContext()) > 0) emit.Pop();
		}
	}
}
