using System;

namespace Transform.Compiler
{
	public class IndexerExpression : Expression
	{
		public Expression Source { get; }
		public Expression[] Indicies { get; }

		public IndexerExpression(Expression source, Expression[] indicies)
		{
			Source = source;
			Indicies = indicies;
		}

		public override TypeReference GetResultType(IEmitter emit, CodeBody parent, TypeReference[] typeHints)
		{
			return Source.GetResultType(emit, parent, typeHints).Dereference();
		}

		public override void Emit(IEmitter emit, CodeBody parent, TypeReference[] typeHints)
		{
			EmitAddress(emit, parent, typeHints);
			emit.Load();
		}

		public override void EmitAddress(IEmitter emit, CodeBody parent, TypeReference[] typeHints)
		{
			if (Indicies.Length != 1) throw new NotImplementedException("Multidimensional indicies are planned but not yet implemented.");

			var type = Source.GetResultType(emit, parent, typeHints);

			if (type.Indirection <= 0) throw new Exception("Indexers can only be applied to pointers.");

			var size = type.Dereference().ResolveSize(emit.GetContext());

			Indicies[0].Emit(emit, parent, typeHints);
			emit.Push(size);
			emit.Multiply();
			Source.EmitAddress(emit, parent, typeHints);
			emit.Add();
		}
	}
}
