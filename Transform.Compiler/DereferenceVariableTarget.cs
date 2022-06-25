namespace Transform.Compiler
{
	public class DereferenceVariableTarget : Expression
	{
		public Expression Target { get; }

		public DereferenceVariableTarget(Expression target)
		{
			Target = target;
		}

		public override TypeReference GetResultType(IEmitter emit, CodeBody parent, TypeReference[] typeHints)
		{
			return Target.GetResultType(emit, parent, typeHints).Dereference();
		}

		public override void Emit(IEmitter emit, CodeBody parent, TypeReference[] typeHints)
		{
			Target.Emit(emit, parent, typeHints);
			emit.Load();
		}
	}
}
