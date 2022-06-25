using System;

namespace Transform.Compiler
{
	public class CallExpression : Expression
	{
		public Expression Source { get; }
		public Expression[] Arguments { get; }

		public CallExpression(Expression source, Expression[] args)
		{
			Source = source;
			Arguments = args;
		}

		public override TypeReference GetResultType(IEmitter emit, CodeBody parent, TypeReference[] typeHints)
		{
			var funcPointerType = Source.GetResultType(emit, parent, GetTypeHints(emit, parent, typeHints));

			if ((funcPointerType.GenericArguments.Length - 1) != Arguments.Length) throw new Exception("Argument count does not match target.");

			for (var i = 1; i < funcPointerType.GenericArguments.Length; i++)
			{
				if (!Arguments[i - 1].GetResultType(emit, parent, typeHints).AssignableTo(funcPointerType.GenericArguments[i]))
				{
					throw new Exception($"Argument {i} does not match the required type.");
				}
			}

			return funcPointerType.GenericArguments[0];
		}

		public override void Emit(IEmitter emit, CodeBody parent, TypeReference[] typeHints)
		{
			for (var i = 0; i < Arguments.Length; i++) Arguments[i].Emit(emit, parent, typeHints);
			Source.EmitAddress(emit, parent, GetTypeHints(emit, parent, typeHints));
			emit.Call();
		}

		private TypeReference[] GetTypeHints(IEmitter emit, CodeBody parent, TypeReference[] typeHints)
		{
			var hints = new TypeReference[Arguments.Length];
			for (var i = 0; i < Arguments.Length; i++) hints[i] = Arguments[i].GetResultType(emit, parent, typeHints);
			return hints;
		}
	}
}
