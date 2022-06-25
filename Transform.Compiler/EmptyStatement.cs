namespace Transform.Compiler
{
	public class EmptyStatement : Statement
	{
		public EmptyStatement(CodeBody parent) : base(parent) { }

		public override InitializedVariable[] GetLocals() => new InitializedVariable[0];

		public override void Emit(IEmitter emit, CodeBody parent, ILabel endOfBlock) { }
	}
}
