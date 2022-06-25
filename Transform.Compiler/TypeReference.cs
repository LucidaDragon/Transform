namespace Transform.Compiler
{
	public class TypeReference
	{
		public Structure Owner { get; }
		public Namespace From { get; }
		public string[] Path { get; }
		public string Name { get; }
		public TypeReference[] GenericArguments { get; }
		public int Indirection { get; }

		public TypeReference(Structure structure) : this(structure, 0) { }

		public TypeReference(Structure structure, int indirection) : this(structure, new TypeReference[0], indirection) { }

		public TypeReference(Structure structure, TypeReference[] genericArguments, int indirection)
		{
			var components = structure.FullName.Split('.');
			From = structure.Parent;
			Path = new string[components.Length - 1];
			for (var i = 0; i < (components.Length - 1); i++) Path[i] = components[i];
			Name = structure.Name;
			GenericArguments = genericArguments;
			Indirection = indirection;
		}

		public TypeReference(Namespace from, string name) : this(from, new string[0], name) { }

		public TypeReference(Namespace from, string[] path, string name) : this(from, path, name, 0) { }

		public TypeReference(Namespace from, string name, int indirection) : this(from, new string[0], name, indirection) { }
		
		public TypeReference(Namespace from, Structure owner, string name) : this(from, owner, new string[0], name, new TypeReference[0], 0) { }

		public TypeReference(Namespace from, string[] path, string name, int indirection) : this(from, path, name, new TypeReference[0], indirection) { }

		public TypeReference(Namespace from, string[] path, string name, TypeReference[] genericArguments, int indirection) : this(from, null, path, name, genericArguments, indirection) { }

		public TypeReference(Namespace from, Structure owner, string[] path, string name, TypeReference[] genericArguments, int indirection)
		{
			From = from;
			Owner = owner;
			Path = path;
			Name = name;
			GenericArguments = genericArguments;
			Indirection = indirection;
		}

		public TypeReference GetPointer()
		{
			return new TypeReference(From, Path, Name, GenericArguments, Indirection + 1);
		}

		public TypeReference Dereference()
		{
			return new TypeReference(From, Path, Name, GenericArguments, Indirection - 1);
		}

		public string ResolveFullName()
		{
			return $"{ResolveUnderlyingFullName()}{"".PadRight(Indirection, '*')}";
		}

		public string ResolveUnderlyingFullName()
		{
			var suffix = (GenericArguments.Length == 0 ? "" : $"`{GenericArguments.Length}");

			if (Path.Length == 0)
			{
				var type = ResolveUnderlyingType();

				return type == null ? $"{Name}{suffix}" : type.FullName;
			}
			else
			{
				return $"{string.Join(".", Path)}.{Name}{suffix}";
			}
		}

		public ulong ResolveSize(IEmitterContext context)
		{
			if (Indirection > 0) return context.AddressWordSize;

			var type = ResolveUnderlyingType();

			return type == null ? context.RegisterWordSize : type.GetSize(context);
		}

		public Structure ResolveUnderlyingType()
		{
			var current = From;
			while (current != null)
			{
				for (var i = 0; i < current.Structures.Count; i++)
				{
					var type = current.Structures[i];

					if (type.Name == Name && type.GenericParameters.Count == GenericArguments.Length)
					{
						return type;
					}
				}

				current = current.Parent;
			}

			return null;
		}

		public bool AssignableTo(TypeReference reference, bool inherit = true)
		{
			if (ResolveFullName() == reference.ResolveFullName())
			{
				if (GenericArguments.Length != reference.GenericArguments.Length) return false;

				for (var i = 0; i < GenericArguments.Length; i++)
				{
					if (GenericArguments[i].AssignableTo(reference.GenericArguments[i], false)) return false;
				}

				return true;
			}
			else if (inherit && Indirection == reference.Indirection)
			{
				var myType = ResolveUnderlyingType();

				if (myType.BaseType != null && myType.BaseType.AssignableTo(reference)) return true;
			}

			return false;
		}

		public override string ToString()
		{
			return ResolveFullName();
		}
	}
}
