using System.Collections.Generic;

namespace Transform.Compiler
{
	public class ForStatement : Statement
	{
		public InitializedVariable Initializer { get; }
		public Expression Condition { get; }
		public Expression Iteration { get; }
		public CodeBody Body { get; }

		public ForStatement(CodeBody parent, InitializedVariable initializer, Expression condition, Expression iteration, CodeBody body) : base(parent)
		{
			Initializer = initializer;
			Condition = condition;
			Iteration = iteration;
			Body = body;
		}

		public override InitializedVariable[] GetLocals()
		{
			var buffer = new List<InitializedVariable>();
			if (Initializer != null) buffer.Add(Initializer);
			buffer.AddRange(Body.GetLocals());
			return buffer.ToArray();
		}

		public override void Emit(IEmitter emit, CodeBody parent, ILabel endOfBlock)
		{
			Initializer.Value.Emit(emit, parent, new TypeReference[0]);
			emit.PushLocalAddress(parent.GetLocalIndex(Initializer));
			emit.Store();

			var forStart = emit.CreateLabel(null, null);
			var forEnd = emit.CreateLabel(null, null);
			emit.MarkLabel(forStart);
			Condition.Emit(emit, parent, new TypeReference[0]);
			emit.BranchIfNot(forEnd);
			for (var i = 0; i < Body.Statements.Count; i++) Body.Statements[i].Emit(emit, Body, forEnd);
			new ExpressionStatement(parent, Iteration).Emit(emit, parent, forEnd);
			emit.Jump(forStart);
			emit.MarkLabel(forEnd);
		}
	}
}
