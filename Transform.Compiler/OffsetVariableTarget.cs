namespace Transform.Compiler
{
	public class OffsetVariableTarget : Expression
	{
		public Expression Target { get; }
		public ulong Offset { get; }
		public TypeReference Type { get; }
		
		public OffsetVariableTarget(Expression target, ulong offset, TypeReference type)
		{
			Target = target;
			Offset = offset;
			Type = type;
		}

		public override TypeReference GetResultType(IEmitter emit, CodeBody parent, TypeReference[] typeHints)
		{
			return Type.GetPointer();
		}

		public override void Emit(IEmitter emit, CodeBody parent, TypeReference[] typeHints)
		{
			Target.Emit(emit, parent, typeHints);
			emit.Push(Offset);
			emit.Add();
		}
	}
}
