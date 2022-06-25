namespace Transform.Compiler
{
	public abstract class Statement
	{
		public CodeBody Parent { get; }

		public Statement(CodeBody parent)
		{
			Parent = parent;
		}

		public abstract InitializedVariable[] GetLocals();
		public abstract void Emit(IEmitter emit, CodeBody parent, ILabel endOfBlock);
	}
}
