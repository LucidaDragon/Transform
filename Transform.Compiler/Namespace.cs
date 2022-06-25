using System;
using System.Collections.Generic;

namespace Transform.Compiler
{
	public class Namespace
	{
		public string FullName
		{
			get
			{
				if (Parent == null)
				{
					return Name;
				}
				else
				{
					var prefix = Parent.FullName;

					return prefix == null ? Name : $"{prefix}.{Name}";
				}
			}
		}

		public string Name { get; }
		public Namespace Parent { get; }

		public List<Namespace> Namespaces { get; } = new List<Namespace>();
		public List<Function> Functions { get; } = new List<Function>();
		public List<Structure> Structures { get; } = new List<Structure>();
		public List<InitializedVariable> Variables { get; } = new List<InitializedVariable>();

		public Namespace(Namespace parent, string name)
		{
			Name = name;
			Parent = parent;
		}

		public void Add(Namespace ns)
		{
			Namespaces.Add(ns);
		}

		public void Add(Function function)
		{
			Functions.Add(function);
		}

		public void Add(Structure structure)
		{
			Structures.Add(structure);
		}

		public void Add(InitializedVariable variable)
		{
			Variables.Add(variable);
		}

		public Namespace GetNamespace(string name)
		{
			for (var i = 0; i < Namespaces.Count; i++)
			{
				if (Namespaces[i].Name == name) return Namespaces[i];
			}

			return null;
		}

		public Function GetFunction(string name)
		{
			for (var i = 0; i < Functions.Count; i++)
			{
				if (Functions[i].Name == name) return Functions[i];
			}

			return null;
		}

		public Structure GetStructure(string name)
		{
			for (var i = 0; i < Structures.Count; i++)
			{
				if (Structures[i].Name == name) return Structures[i];
			}

			return null;
		}

		public Variable GetVariable(string name)
		{
			for (var i = 0; i < Variables.Count; i++)
			{
				if (Variables[i].Name == name) return Variables[i];
			}

			return null;
		}

		public static Namespace Merge(Namespace newParent, Namespace a, Namespace b)
		{
			if (a.Name != b.Name) throw new Exception("Namespace names do not match.");
			var result = new Namespace(newParent, a.Name);
			for (var i = 0; i < a.Functions.Count; i++) result.Add(a.Functions[i]);
			for (var i = 0; i < b.Functions.Count; i++) result.Add(b.Functions[i]);
			for (var i = 0; i < a.Structures.Count; i++) result.Add(a.Structures[i]);
			for (var i = 0; i < b.Structures.Count; i++) result.Add(b.Structures[i]);
			for (var i = 0; i < a.Variables.Count; i++) result.Add(a.Variables[i]);
			for (var i = 0; i < b.Variables.Count; i++) result.Add(b.Variables[i]);
			for (var i = 0; i < a.Namespaces.Count; i++) result.Add(a.Namespaces[i]);
			for (var i = 0; i < b.Namespaces.Count; i++)
			{
				var next = false;

				for (var j = 0; j < a.Namespaces.Count; j++)
				{
					if (result.Namespaces[j].Name == b.Namespaces[i].Name)
					{
						result.Namespaces[j] = Merge(result, result.Namespaces[j], b.Namespaces[i]);
						next = true;
						break;
					}
				}

				if (!next) result.Add(b.Namespaces[i]);
			}
			return result;
		}

		public override string ToString()
		{
			return $"namespace {FullName}";
		}
	}
}
