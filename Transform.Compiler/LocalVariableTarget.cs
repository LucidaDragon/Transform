namespace Transform.Compiler
{
	public class LocalVariableTarget : Expression
	{
		public InitializedVariable Local { get; }

		public LocalVariableTarget(InitializedVariable local)
		{
			Local = local;
		}

		public override TypeReference GetResultType(IEmitter emit, CodeBody parent, TypeReference[] typeHints)
		{
			return Local.Type.GetPointer();
		}

		public override void Emit(IEmitter emit, CodeBody parent, TypeReference[] typeHints)
		{
			emit.PushLocalAddress(parent.GetLocalIndex(Local));
		}
	}
}
