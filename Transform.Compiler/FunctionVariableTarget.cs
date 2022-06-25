using System.Collections.Generic;

namespace Transform.Compiler
{
	public class FunctionVariableTarget : Expression
	{
		public Function Function { get; }

		public FunctionVariableTarget(Function function)
		{
			Function = function;
		}

		public override TypeReference GetResultType(IEmitter emit, CodeBody parent, TypeReference[] typeHints)
		{
			var buffer = new List<TypeReference> { Function.ReturnValue };
			for (var i = 0; i < Function.Parameters.Count; i++) buffer.Add(Function.Parameters[i].Type);

			return new TypeReference(parent.OwningFunction.ParentNamespace, new string[0], "func", buffer.ToArray(), 0);
		}

		public override void Emit(IEmitter emit, CodeBody parent, TypeReference[] typeHints)
		{
			emit.PushAddress(emit.TryGetLabelByKey(Function.Body, out ILabel label) ? label : emit.NullLabel());
		}
	}
}
