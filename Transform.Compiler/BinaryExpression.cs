namespace Transform.Compiler
{
	public class BinaryExpression : Expression
	{
		public Expression Left { get; }
		public string Operator { get; }
		public Expression Right { get; }

		private readonly CallExpression OperatorCall;

		public BinaryExpression(Expression left, string op, Expression right)
		{
			Left = left;
			Operator = op;
			Right = right;

			OperatorCall = new CallExpression(new VariableReference(new[] { Operator }), new[] { Left, Right });
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
