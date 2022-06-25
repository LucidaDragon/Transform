using System;

namespace Transform.Compiler
{
	public abstract class Expression
	{
		public virtual bool TryGetConstant(out Literal result)
		{
			result = null;
			return false;
		}

		public abstract TypeReference GetResultType(IEmitter emit, CodeBody parent, TypeReference[] typeHints);

		public abstract void Emit(IEmitter emit, CodeBody parent, TypeReference[] typeHints);

		public virtual void EmitAddress(IEmitter emit, CodeBody parent, TypeReference[] typeHints)
		{
			throw new NotSupportedException("Expression can not be referenced by an address.");
		}
	}
}
