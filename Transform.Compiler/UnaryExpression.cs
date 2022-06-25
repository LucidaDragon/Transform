namespace Transform.Compiler
{
	public class UnaryExpression : Expression
	{
		public string Operator { get; }
		public Expression Expression { get; }

		private readonly CallExpression OperatorCall;

		public UnaryExpression(string op, Expression expression)
		{
			Operator = op;
			Expression = expression;

			OperatorCall = new CallExpression(new VariableReference(new[] { Operator }), new[] { Expression });
		}

		public override TypeReference GetResultType(IEmitter emit, CodeBody parent, TypeReference[] typeHints)
		{
			return OperatorCall.GetResultType(emit, parent, typeHints);
		}

		public override void Emit(IEmitter emit, CodeBody parent, TypeReference[] typeHints)
		{
			OperatorCall.Emit(emit, parent, typeHints);
		}
	}
}
