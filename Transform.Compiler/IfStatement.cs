using System.Collections.Generic;

namespace Transform.Compiler
{
	public class IfStatement : Statement
	{
		public Expression Condition { get; }
		public CodeBody IfBody { get; }
		public CodeBody ElseBody { get; }

		public IfStatement(CodeBody parent, Expression condition, CodeBody ifBody, CodeBody elseBody) : base(parent)
		{
			Condition = condition;
			IfBody = ifBody;
			ElseBody = elseBody;
		}

		public override InitializedVariable[] GetLocals()
		{
			var buffer = new List<InitializedVariable>();
			buffer.AddRange(IfBody.GetLocals());
			buffer.AddRange(ElseBody.GetLocals());
			return buffer.ToArray();
		}

		public override void Emit(IEmitter emit, CodeBody parent, ILabel endOfBlock)
		{
			var ifBlockStart = emit.CreateLabel(null, null);
			var ifBlockEnd = emit.CreateLabel(null, null);
			var elseBlockStart = emit.CreateLabel(null, null);
			var endOfIf = emit.CreateLabel(null, null);

			Condition.Emit(emit, parent, new TypeReference[0]);
			emit.BranchIfNot(elseBlockStart);
			emit.MarkLabel(ifBlockStart);
			for (var i = 0; i < IfBody.Statements.Count; i++) IfBody.Statements[i].Emit(emit, IfBody, ifBlockEnd);
			emit.MarkLabel(ifBlockEnd);
			if (ElseBody.Statements.Count > 0) emit.Jump(endOfIf);
			emit.MarkLabel(elseBlockStart);
			for (var i = 0; i < ElseBody.Statements.Count; i++) ElseBody.Statements[i].Emit(emit, ElseBody, endOfIf);
			emit.MarkLabel(endOfIf);
		}
	}
}
