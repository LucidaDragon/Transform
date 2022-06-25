using System;

namespace Transform.Compiler
{
	public class EmitterStatement : Statement
	{
		private readonly Action<IEmitter, CodeBody, ILabel> OnEmit;

		public EmitterStatement(CodeBody parent, Action<IEmitter, CodeBody, ILabel> onEmit) : base(parent) => OnEmit = onEmit;

		public override void Emit(IEmitter emit, CodeBody parent, ILabel endOfBlock) => OnEmit(emit, parent, endOfBlock);

		public override InitializedVariable[] GetLocals() => new InitializedVariable[0];
	}
}
