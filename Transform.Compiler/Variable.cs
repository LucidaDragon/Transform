namespace Transform.Compiler
{
	public class Variable
	{
		public string Name { get; }
		public TypeReference Type { get; }

		public Variable(string name, TypeReference type)
		{
			Name = name;
			Type = type;
		}

		public string GetFullName(Namespace parent)
		{
			var prefix = parent.FullName;

			return prefix == null ? Name : $"{parent.FullName}.{Name}";
		}

		public string GetFullName(Structure structure)
		{
			return $"{structure.FullName}.{Name}";
		}

		public ulong GetSize(IEmitterContext context)
		{
			return Type.ResolveSize(context);
		}

		public override string ToString()
		{
			return $"{Type.ResolveFullName()} {Name}";
		}
	}
}
