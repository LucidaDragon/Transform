namespace Transform.Compiler
{
	public class ArgumentVariableTarget : Expression
	{
		public Variable Argument { get; }

		public ArgumentVariableTarget(Variable argument)
		{
			Argument = argument;
		}

		public override TypeReference GetResultType(IEmitter emit, CodeBody parent, TypeReference[] typeHints)
		{
			return Argument.Type.GetPointer();
		}

		public override void Emit(IEmitter emit, CodeBody parent, TypeReference[] typeHints)
		{
			emit.PushArgumentAddress(parent.OwningFunction.GetArgumentIndex(Argument));
		}
	}
}
