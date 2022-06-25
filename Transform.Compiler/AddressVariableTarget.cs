namespace Transform.Compiler
{
	public class AddressVariableTarget : Expression
	{
		public ILabel Label { get; }
		public TypeReference Type { get; }

		public AddressVariableTarget(ILabel label, TypeReference type)
		{
			Label = label;
			Type = type;
		}

		public override TypeReference GetResultType(IEmitter emit, CodeBody parent, TypeReference[] typeHints)
		{
			return Type.GetPointer();
		}

		public override void Emit(IEmitter emit, CodeBody parent, TypeReference[] typeHints)
		{
			emit.PushAddress(Label);
		}
	}
}
