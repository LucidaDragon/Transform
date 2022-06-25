using System.Collections.Generic;

namespace Transform.Compiler
{
	public class Structure
	{
		public string FullName
		{
			get
			{
				var prefix = Parent.FullName;
				var suffix = GenericParameters.Count == 0 ? "" : $"`{GenericParameters.Count}";
				return prefix == null ? $"{Name}{suffix}" : $"{prefix}.{Name}{suffix}";
			}
		}

		public string Name { get; }
		public Namespace Parent { get; }
		public ulong Padding { get; set; } = 0;
		public List<Variable> GenericParameters { get; } = new List<Variable>();
		public TypeReference BaseType { get; set; }
		public List<Variable> Fields { get; } = new List<Variable>();
		public List<Function> Methods { get; } = new List<Function>();

		public Structure(Namespace parent, string name)
		{
			Name = name;
			Parent = parent;
		}

		public ulong GetSize(IEmitterContext context)
		{
			var size = (BaseType == null ? 0 : BaseType.ResolveSize(context)) + Padding;
			for (var i = 0; i < Fields.Count; i++) size += Fields[i].Type.ResolveSize(context);
			return size;
		}

		public TypeReference GetReference(TypeReference[] genericArguments, int indirection)
		{
			return new TypeReference(this, genericArguments, indirection);
		}

		public override string ToString()
		{
			return $"struct {FullName}";
		}
	}
}
