using System.Collections.Generic;

namespace Transform.Compiler
{
	public class Function
	{
		public string FullName
		{
			get
			{
				if (ParentStructure == null)
				{
					var prefix = ParentNamespace.FullName;

					return prefix == null ? Name : $"{prefix}.{Name}";
				}
				else
				{
					return $"{ParentStructure.FullName}.{Name}";
				}
			}
		}

		public string FullNameWithParameters
		{
			get
			{
				var paramTypes = new string[Parameters.Count];
				for (var i = 0; i < paramTypes.Length; i++) paramTypes[i] = Parameters[i].Type.ResolveFullName();

				return $"{FullName}({string.Join(",", paramTypes)})";
			}
		}

		public bool IsOperator
		{
			get
			{
				for (var i = 0; i < Name.Length; i++) if (!TokenStream.IsIdentifierChar(Name[i])) return true;
				return false;
			}
		}

		public string Name { get; }
		public Namespace ParentNamespace { get; }
		public Structure ParentStructure { get; }
		public TypeReference ReturnValue { get; set; }
		public List<Variable> Parameters { get; } = new List<Variable>();
		public CodeBody Body { get; set; }
		public bool Inline { get; set; }

		public Function(Namespace parentNamespace, Structure parentStructure, string name)
		{
			Name = name;
			ParentNamespace = parentNamespace;
			ParentStructure = parentStructure;
		}

		public int GetArgumentIndex(Variable argument)
		{
			return Parameters.IndexOf(argument);
		}

		public override string ToString()
		{
			return $"{ReturnValue.ResolveFullName()} {FullNameWithParameters}";
		}
	}
}
