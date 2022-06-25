using System;
using System.Collections.Generic;

namespace Transform.Compiler
{
	public class PackageCompiler
	{
		public List<Package> Packages { get; } = new List<Package>();
		public bool Reflection { get; set; } = true;

		public PackageCompiler() { }

		public PackageCompiler(IEnumerable<Package> packages) { Packages.AddRange(packages); }

		public void Add(Package package)
		{
			Packages.Add(package);
		}

		public void Emit(IEmitter emit)
		{
			for (var i = 0; i < Packages.Count; i++)
			{
				emit.Section(i);
				EmitPackage(emit, Packages[i]);
			}
		}

		private void EmitPackage(IEmitter emit, Package package)
		{
			CreateNamespaceLabels(emit, package.Global);
			EmitNamespace(emit, package.Global);
		}

		private static void CreateNamespaceLabels(IEmitter emit, Namespace ns)
		{
			emit.CreateLabel(ns.FullName, ns);
			for (var i = 0; i < ns.Namespaces.Count; i++) CreateNamespaceLabels(emit, ns.Namespaces[i]);
			for (var i = 0; i < ns.Structures.Count; i++) CreateStructureLabels(emit, ns.Structures[i]);
			for (var i = 0; i < ns.Functions.Count; i++) CreateFunctionLabels(emit, ns.Functions[i]);
			for (var i = 0; i < ns.Variables.Count; i++) CreateInitializedVariableLabels(emit, ns.Variables[i]);
		}

		private static void CreateStructureLabels(IEmitter emit, Structure structure)
		{
			emit.CreateLabel(structure.FullName, structure);
			for (var i = 0; i < structure.Fields.Count; i++) CreateVariableLabels(emit, structure.Fields[i]);
			for (var i = 0; i < structure.Methods.Count; i++) CreateFunctionLabels(emit, structure.Methods[i]);
		}

		private static void CreateFunctionLabels(IEmitter emit, Function function)
		{
			emit.CreateLabel(function.FullNameWithParameters, function);
			CreateTypeReferenceLabels(emit, function.ReturnValue);
			for (var i = 0; i < function.Parameters.Count; i++) CreateVariableLabels(emit, function.Parameters[i]);
			emit.CreateLabel($"*{function.FullNameWithParameters}", function.Body);
		}

		private static void CreateInitializedVariableLabels(IEmitter emit, InitializedVariable variable)
		{
			CreateVariableLabels(emit, variable);
			emit.CreateLabel(null, variable.Value);
		}

		private static void CreateVariableLabels(IEmitter emit, Variable variable)
		{
			emit.CreateLabel(null, variable);
			CreateTypeReferenceLabels(emit, variable.Type);
		}

		private static void CreateTypeReferenceLabels(IEmitter emit, TypeReference reference)
		{
			emit.CreateLabel(null, reference);
			for (var i = 0; i < reference.GenericArguments.Length; i++) CreateTypeReferenceLabels(emit, reference.GenericArguments[i]);
		}

		private void EmitNamespace(IEmitter emit, Namespace ns)
		{
			if (Reflection)
			{
				if (!emit.TryGetLabelByKey(ns, out ILabel target)) return;

				ILabel name, fullName, parent, childNamespaces, childStructures, childFunctions, childVariables;

				if (ns.Parent == null)
				{
					name = emit.NullLabel();
					fullName = emit.NullLabel();
					parent = emit.NullLabel();
				}
				else
				{
					name = emit.CreateStringLabel(ns.Name);
					fullName = emit.CreateStringLabel(ns.FullName);
					if (!emit.TryGetLabelByKey(ns.Parent, out parent)) parent = emit.NullLabel();
				}

				childNamespaces = EmitArray(emit, ns.Namespaces, (index, child) => emit.PushAddress(emit.TryGetLabelByKey(child, out ILabel label) ? label : emit.NullLabel()));
				childStructures = EmitArray(emit, ns.Structures, (index, child) => emit.PushAddress(emit.TryGetLabelByKey(child, out ILabel label) ? label : emit.NullLabel()));
				childFunctions = EmitArray(emit, ns.Functions, (index, child) => emit.PushAddress(emit.TryGetLabelByKey(child, out ILabel label) ? label : emit.NullLabel()));
				childVariables = EmitArray(emit, ns.Variables, (index, child) => emit.PushAddress(emit.TryGetLabelByKey(child, out ILabel label) ? label : emit.NullLabel()));

				emit.BeginData(target);
				emit.PushAddress(name);
				emit.PushAddress(fullName);
				emit.PushAddress(parent);
				emit.PushAddress(childNamespaces);
				emit.PushAddress(childStructures);
				emit.PushAddress(childFunctions);
				emit.PushAddress(childVariables);
				emit.EndData();
			}

			for (var i = 0; i < ns.Namespaces.Count; i++) EmitNamespace(emit, ns.Namespaces[i]);
			for (var i = 0; i < ns.Structures.Count; i++) EmitStructure(emit, ns.Structures[i]);
			for (var i = 0; i < ns.Functions.Count; i++) EmitFunction(emit, ns.Functions[i]);
			for (var i = 0; i < ns.Variables.Count; i++) EmitInitializedVariable(emit, ns.Variables[i]);
		}

		private void EmitStructure(IEmitter emit, Structure structure)
		{
			if (Reflection)
			{
				if (!emit.TryGetLabelByKey(structure, out ILabel target)) return;

				var name = emit.CreateStringLabel(structure.Name);
				var fullName = emit.CreateStringLabel(structure.FullName);
				var baseType = structure.BaseType != null && emit.TryGetLabelByKey(structure.BaseType, out ILabel parentLabel) ? parentLabel : emit.NullLabel();
				var size = structure.GetSize(emit.GetContext());
				var fields = EmitArray(emit, structure.Fields, (index, field) => emit.PushAddress(emit.TryGetLabelByKey(field, out ILabel label) ? label : emit.NullLabel()));
				var methods = EmitArray(emit, structure.Methods, (index, method) => emit.PushAddress(emit.TryGetLabelByKey(method, out ILabel label) ? label : emit.NullLabel()));

				emit.BeginData(target);
				emit.PushAddress(name);
				emit.PushAddress(fullName);
				emit.PushAddress(baseType);
				emit.Push(size);
				emit.PushAddress(fields);
				emit.PushAddress(methods);
				emit.EndData();
			}

			for (var i = 0; i < structure.Fields.Count; i++) EmitVariable(emit, structure.Fields[i]);
			for (var i = 0; i < structure.Methods.Count; i++) EmitFunction(emit, structure.Methods[i]);
		}

		private void EmitFunction(IEmitter emit, Function function)
		{
			if (Reflection)
			{
				if (!emit.TryGetLabelByKey(function, out ILabel target)) return;

				var name = emit.CreateStringLabel(function.Name);
				var fullName = emit.CreateStringLabel(function.FullName);
				var signature = emit.CreateStringLabel(function.FullNameWithParameters);
				var returnValue = emit.TryGetLabelByKey(function.ReturnValue, out ILabel returnLabel) ? returnLabel : emit.NullLabel();
				var parameterArray = EmitArray(emit, function.Parameters, (index, param) => emit.PushAddress(emit.TryGetLabelByKey(param, out ILabel label) ? label : emit.NullLabel()));

				emit.BeginData(target);
				emit.PushAddress(name);
				emit.PushAddress(fullName);
				emit.PushAddress(signature);
				emit.PushAddress(returnValue);
				emit.PushAddress(parameterArray);
				emit.EndData();
			}

			EmitTypeReference(emit, function.ReturnValue);
			for (var i = 0; i < function.Parameters.Count; i++) EmitVariable(emit, function.Parameters[i]);

			if (!emit.TryGetLabelByKey(function.Body, out ILabel codeStart)) throw new InvalidOperationException("Missing body label.");

			var parameters = new ulong[function.Parameters.Count];
			for (var i = 0; i < parameters.Length; i++) parameters[i] = function.Parameters[i].GetSize(emit.GetContext());

			if (function.Body.IsExternal)
			{
				emit.ExternalFunction(codeStart, function.FullName, function.ReturnValue.ResolveSize(emit.GetContext()), parameters);
			}
			else
			{
				var locals = function.Body.GetLocals();
				var localInfos = new ILocal[locals.Length];
				for (var i = 0; i < locals.Length; i++) localInfos[i] = new LocalInfo(locals[i].GetSize(emit.GetContext()), locals[i].Value, function.Body);

				var returnSize = function.ReturnValue.ResolveSize(emit.GetContext());
				emit.BeginFunction(codeStart, returnSize, parameters, localInfos);
				for (var i = 0; i < function.Body.Statements.Count; i++) function.Body.Statements[i].Emit(emit, function.Body, null);
				emit.EndFunction();
			}
		}

		private void EmitInitializedVariable(IEmitter emit, InitializedVariable variable)
		{
			EmitVariable(emit, variable);

			if (emit.TryGetLabelByKey(variable.Value, out ILabel label))
			{
				if (!variable.Value.TryGetConstant(out Literal value)) throw new Exception("Variable value is not constant.");

				emit.BeginData(label);
				EmitLiteral(emit, value);
				emit.EndData();
			}
		}

		private static void EmitLiteral(IEmitter emit, Literal literal)
		{
			literal.Emit(emit, null, new TypeReference[0]);
		}

		private void EmitVariable(IEmitter emit, Variable variable)
		{
			if (Reflection)
			{
				if (!emit.TryGetLabelByKey(variable, out ILabel target)) return;

				var name = emit.CreateStringLabel(variable.Name);
				var type = emit.TryGetLabelByKey(variable.Type, out ILabel label) ? label : emit.NullLabel();

				emit.BeginData(target);
				emit.PushAddress(name);
				emit.PushAddress(type);
				emit.EndData();

				EmitTypeReference(emit, variable.Type);
			}
		}

		private void EmitTypeReference(IEmitter emit, TypeReference reference)
		{
			if (Reflection)
			{
				if (!emit.TryGetLabelByKey(reference, out ILabel target)) return;

				var typeRef = reference.ResolveUnderlyingType();
				var underlyingType = typeRef == null ? emit.NullLabel() : emit.TryGetLabelByKey(typeRef, out ILabel label) ? label : emit.NullLabel();
				var generics = EmitArray(emit, reference.GenericArguments, (index, arg) => emit.PushAddress(emit.TryGetLabelByKey(arg, out ILabel argLabel) ? argLabel : emit.NullLabel()));
				var indirection = reference.Indirection;

				emit.BeginData(target);
				emit.PushAddress(underlyingType);
				emit.PushAddress(generics);
				emit.Push(indirection);
				emit.EndData();
			}
		}

		private static ILabel EmitArray<T>(IEmitter emit, IReadOnlyList<T> items, Action<long, T> emitItem)
		{
			return EmitArray(emit, items.ToArray(), emitItem);
		}

		private static ILabel EmitArray<T>(IEmitter emit, T[] items, Action<long, T> emitItem)
		{
			var label = emit.CreateLabel(null, null);

			emit.QueueAction(() =>
			{
				emit.BeginData(label);
				emit.Push(items.LongLength);
				emit.PushAddress(EmitCArray(emit, items, emitItem));
				emit.EndData();
			});

			return label;
		}

		private static ILabel EmitCArray<T>(IEmitter emit, IReadOnlyList<T> items, Action<long, T> emitItem)
		{
			return EmitCArray(emit, items.ToArray(), emitItem);
		}

		private static ILabel EmitCArray<T>(IEmitter emit, T[] items, Action<long, T> emitItem)
		{
			var label = emit.CreateLabel(null, null);

			emit.QueueAction(() =>
			{
				emit.BeginData(label);
				for (long i = 0; i < items.LongLength; i++) emitItem(i, items[i]);
				emit.EndData();
			});

			return label;
		}

		private class LocalInfo : ILocal
		{
			public ulong Size { get; }
			public Expression Expression { get; }
			public CodeBody Parent { get; }

			public LocalInfo(ulong size, Expression expression, CodeBody parent)
			{
				Size = size;
				Expression = expression;
				Parent = parent;
			}

			public void PushInitialValue(IEmitter emit)
			{
				Expression.Emit(emit, Parent, new TypeReference[0]);
			}
		}
	}
}
