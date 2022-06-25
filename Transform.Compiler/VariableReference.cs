using System;
using System.Collections.Generic;

namespace Transform.Compiler
{
	public class VariableReference : Expression
	{
		public string[] Path { get; }

		public VariableReference(string[] path)
		{
			Path = path;
		}

		public override TypeReference GetResultType(IEmitter emit, CodeBody parent, TypeReference[] typeHints)
		{
			return new DereferenceVariableTarget(Resolve(emit, parent, typeHints, 0)).GetResultType(emit, parent, typeHints);
		}

		public override void Emit(IEmitter emit, CodeBody parent, TypeReference[] typeHints)
		{
			EmitAddress(emit, parent, typeHints);
			emit.Load();
		}

		public override void EmitAddress(IEmitter emit, CodeBody parent, TypeReference[] typeHints)
		{
			Resolve(emit, parent, typeHints, 0).Emit(emit, parent, typeHints);
		}

		private Expression Resolve(IEmitter emit, CodeBody parent, TypeReference[] typeHints, int index)
		{
			for (var i = 0; i < parent.OwningFunction.Parameters.Count; i++)
			{
				if (parent.OwningFunction.Parameters[i].Name == Path[index])
				{
					Expression result = new ArgumentVariableTarget(parent.OwningFunction.Parameters[i]);

					if ((index + 1) < Path.Length)
					{
						var type = ResolveType(parent.OwningFunction.Parameters[i].Type, ref result);
						return Resolve(emit.GetContext(), type, typeHints, result, index + 1);
					}
					else
					{
						return result;
					}
				}
			}

			var block = parent;
			while (block != null)
			{
				for (var i = 0; i < block.Locals.Count; i++)
				{
					if (block.Locals[i].Name == Path[index])
					{
						Expression result = new LocalVariableTarget(block.Locals[i]);

						if ((index + 1) < Path.Length)
						{
							var type = ResolveType(block.Locals[i].Type, ref result);
							return Resolve(emit.GetContext(), type, typeHints, result, index + 1);
						}
						else
						{
							return result;
						}
					}
				}

				block = block.ParentBody;
			}

			if (parent.OwningFunction.ParentStructure != null && parent.OwningFunction.Parameters.Count > 0)
			{
				var structure = parent.OwningFunction.ParentStructure;

				for (var i = 0; i < structure.Fields.Count; i++)
				{
					if (structure.Fields[i].Name == Path[index])
					{
						var thisRef = new ArgumentVariableTarget(parent.OwningFunction.Parameters[0]);
						return Resolve(emit.GetContext(), structure, typeHints, thisRef, index);
					}
				}
			}

			var possibleFunctions = new List<Function>();

			Namespace lastNs = null;
			var currentNs = parent.OwningFunction.ParentNamespace;
			while (currentNs != null)
			{
				if (TryResolveVariable(emit, currentNs, typeHints, index, out Expression result)) return result;

				for (var i = 0; i < currentNs.Functions.Count; i++)
				{
					if (currentNs.Functions[i].Name == Path[index])
					{
						possibleFunctions.Add(currentNs.Functions[i]);
					}
				}

				lastNs = currentNs;
				currentNs = currentNs.Parent;
			}

			if ((index + 1) < Path.Length) return Resolve(emit, lastNs, typeHints, index);

			MatchFunctions(possibleFunctions, typeHints);

			if (possibleFunctions.Count > 0) return new FunctionVariableTarget(possibleFunctions[0]);
			else throw new Exception("Undefined identifier.");
		}

		private Expression Resolve(IEmitter emit, Namespace parent, TypeReference[] typeHints, int index)
		{
			if (TryResolveVariable(emit, parent, typeHints, index, out Expression result)) return result;

			if ((index + 1) < Path.Length)
			{
				for (var i = 0; i < parent.Namespaces.Count; i++)
				{
					if (parent.Namespaces[i].Name == Path[index])
					{
						return Resolve(emit, parent.Namespaces[i], typeHints, index + 1);
					}
				}

				throw new Exception($"No namespace named \"{Path[index]}\" in \"{parent.FullName}\".");
			}
			else
			{
				if (TryResolveFunction(parent, typeHints, index, out result)) return result;
				else throw new Exception("Undefined identifier.");
			}
		}

		private Expression Resolve(IEmitterContext context, Structure parent, TypeReference[] typeHints, Expression from, int index)
		{
			ulong offset = 0;

			for (var i = 0; i < parent.Fields.Count; i++)
			{
				if (parent.Fields[i].Name == Path[index])
				{
					var result = new OffsetVariableTarget(from, offset, parent.Fields[i].Type);

					if ((index + 1) < Path.Length)
					{
						var type = ResolveType(parent.Fields[i].Type, ref from);
						return Resolve(context, type, typeHints, result, index + 1);
					}
					else
					{
						return result;
					}
				}

				offset += parent.Fields[i].GetSize(context);
			}

			throw new Exception("Field not defined.");
		}

		private bool TryResolveFunction(Namespace parent, TypeReference[] typeHints, int index, out Expression result)
		{
			var possibleFunctions = new List<Function>();

			for (var i = 0; i < parent.Functions.Count; i++)
			{
				if (parent.Functions[i].Name == Path[index])
				{
					possibleFunctions.Add(parent.Functions[i]);
				}
			}

			MatchFunctions(possibleFunctions, typeHints);

			result = null;
			if (possibleFunctions.Count == 1) result = new FunctionVariableTarget(possibleFunctions[0]);
			return result != null;
		}

		private bool TryResolveVariable(IEmitter emit, Namespace parent, TypeReference[] typeHints, int index, out Expression result)
		{
			for (var i = 0; i < parent.Variables.Count; i++)
			{
				if (parent.Variables[i].Name == Path[index])
				{
					if (!emit.TryGetLabelByKey(parent.Variables[i].Value, out ILabel label)) throw new Exception("Target object has not been labeled.");

					result = new AddressVariableTarget(label, parent.Variables[i].Type);

					if ((index + 1) < Path.Length)
					{
						var type = ResolveType(parent.Variables[i].Type, ref result);
						result = Resolve(emit.GetContext(), type, typeHints, result, index + 1);
					}

					return true;
				}
			}

			result = null;
			return false;
		}

		private Structure ResolveType(TypeReference typeRef, ref Expression from)
		{
			for (var j = 0; j < typeRef.Indirection; j++) from = new DereferenceVariableTarget(from);

			var type = typeRef.ResolveUnderlyingType();
			if (type == null) throw new Exception("Undefined type.");

			return type;
		}

		private void MatchFunctions(List<Function> possibleFunctions, TypeReference[] typeHints)
		{
			for (var i = 0; i < possibleFunctions.Count; i++)
			{
				var func = possibleFunctions[i];

				if (func.Parameters.Count != typeHints.Length)
				{
					possibleFunctions.RemoveAt(i);
					i--;
				}
				else
				{
					for (var j = 0; j < func.Parameters.Count; j++)
					{
						if (!typeHints[j].AssignableTo(func.Parameters[j].Type))
						{
							possibleFunctions.RemoveAt(i);
							i--;
							break;
						}
					}
				}
			}
		}

		public override string ToString()
		{
			return string.Join(".", Path);
		}
	}
}
