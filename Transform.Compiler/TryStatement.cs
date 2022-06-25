using System.Collections.Generic;

namespace Transform.Compiler
{
	public class TryStatement : Statement
	{
		public CodeBody TryBody { get; }
		public CodeBody CatchBody { get; }

		public TryStatement(CodeBody parent, CodeBody tryBody, CodeBody catchBody) : base(parent)
		{
			TryBody = tryBody;
			CatchBody = catchBody;
		}

		public override InitializedVariable[] GetLocals()
		{
			var buffer = new List<InitializedVariable>();
			buffer.AddRange(TryBody.GetLocals());
			buffer.AddRange(CatchBody.GetLocals());
			return buffer.ToArray();
		}

		public override void Emit(IEmitter emit, CodeBody parent, ILabel endOfBlock)
		{
			var tryEnd = emit.CreateLabel(null, null);
			var catchStart = emit.CreateLabel(null, null);
			var catchEnd = emit.CreateLabel(null, null);

			emit.EnterTry(catchStart);
			for (var i = 0; i < TryBody.Statements.Count; i++) TryBody.Statements[i].Emit(emit, TryBody, tryEnd);
			emit.MarkLabel(tryEnd);
			emit.ExitTry();
			emit.Jump(catchEnd);
			emit.MarkLabel(catchStart);
			for (var i = 0; i < CatchBody.Statements.Count; i++) CatchBody.Statements[i].Emit(emit, CatchBody, catchEnd);
			emit.MarkLabel(catchEnd);
		}
	}
}
