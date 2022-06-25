namespace Transform.Compiler
{
	public class WhileStatement : Statement
	{
		public Expression Condition { get; }
		public CodeBody Body { get; }

		public WhileStatement(CodeBody parent, Expression condition, CodeBody body) : base(parent)
		{
			Condition = condition;
			Body = body;
		}

		public override InitializedVariable[] GetLocals()
		{
			return Body.GetLocals();
		}

		public override void Emit(IEmitter emit, CodeBody parent, ILabel endOfBlock)
		{
			var whileStart = emit.CreateLabel(null, null);
			var whileEnd = emit.CreateLabel(null, null);

			emit.MarkLabel(whileStart);
			Condition.Emit(emit, parent, new TypeReference[0]);
			emit.BranchIfNot(whileEnd);
			for (var i = 0; i < Body.Statements.Count; i++) Body.Statements[i].Emit(emit, Body, whileEnd);
			emit.Jump(whileStart);
			emit.MarkLabel(whileEnd);
		}
	}
}
