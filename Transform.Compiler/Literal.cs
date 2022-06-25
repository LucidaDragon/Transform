using System;

namespace Transform.Compiler
{
	public class Literal : Expression
	{
		public long? SignedInt { get; }
		public ulong? UnsignedInt { get; }
		public float? Float32 { get; }
		public double? Float64 { get; }
		public string String { get; }

		public Literal(long value)
		{
			SignedInt = value;
		}

		public Literal(ulong value, bool force)
		{
			if (value <= long.MaxValue && !force) SignedInt = (long)value;
			else UnsignedInt = value;
		}

		public Literal(float value)
		{
			Float32 = value;
		}

		public Literal(double value)
		{
			Float64 = value;
		}

		public Literal(string value)
		{
			String = value;
		}

		public override TypeReference GetResultType(IEmitter emit, CodeBody parent, TypeReference[] typeHints)
		{
			if (SignedInt.HasValue) return new TypeReference(parent.OwningFunction.ParentNamespace, new string[0], "word", new TypeReference[0], 0);
			else if (UnsignedInt.HasValue) return new TypeReference(parent.OwningFunction.ParentNamespace, new string[0], "uword", new TypeReference[0], 0);
			else if (Float32.HasValue) return new TypeReference(parent.OwningFunction.ParentNamespace, new string[0], "float", new TypeReference[0], 0);
			else if (Float64.HasValue) return new TypeReference(parent.OwningFunction.ParentNamespace, new string[0], "double", new TypeReference[0], 0);
			else if (String != null) return new TypeReference(parent.OwningFunction.ParentNamespace, new string[0], "char", new TypeReference[0], 1);
			else throw new Exception("Literal has no value.");
		}

		public override void Emit(IEmitter emit, CodeBody parent, TypeReference[] typeHints)
		{
			if (SignedInt.HasValue) emit.Push(SignedInt.Value);
			else if (UnsignedInt.HasValue) emit.Push(UnsignedInt.Value);
			else if (Float32.HasValue) emit.Push(Float32.Value);
			else if (Float64.HasValue) emit.Push(Float64.Value);
			else if (String != null) emit.PushAddress(emit.CreateStringLabel(String));
			else throw new Exception("Literal has no value.");
		}

		public override bool TryGetConstant(out Literal result)
		{
			result = this;
			return true;
		}
	}
}
