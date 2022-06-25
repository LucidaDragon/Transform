using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Transform.Emitters
{
	public class C : IEmitter
	{
		private readonly Queue<Action> DataActions = new Queue<Action>();
		private readonly Queue<Action> Actions = new Queue<Action>();
		private readonly CEmit Emit = new CEmit();
		private bool DataMode = false;

		private void Do(Action action)
		{
			var frames = new StackTrace().GetFrames();
			var queue = DataMode ? DataActions : Actions;
			
			queue.Enqueue(() =>
			{
				Debug.WriteLine($"{frames[2].GetMethod().DeclaringType.FullName}.{frames[2].GetMethod().Name} -> {frames[1].GetMethod().Name}");
				action();
			});
		}

		public void Add() => Do(Emit.Add);

		public void AddF() => Do(Emit.Add);

		public void BeginData(ILabel label)
		{
			Emit.DefineVariable(label);
			Do(() => Emit.BeginData(label));
		}

		public void BeginFunction(ILabel label, ulong returnType, ulong[] parameters, ILocal[] locals)
		{
			Emit.DefineFunction(label, returnType, parameters);
			Do(() => Emit.BeginFunction(label, returnType, parameters, locals));
		}

		public void BitwiseAnd() => Do(Emit.BitwiseAnd);

		public void BitwiseNot() => Do(Emit.BitwiseNot);

		public void BitwiseOr() => Do(Emit.BitwiseOr);

		public void BitwiseXor() => Do(Emit.BitwiseXor);

		public void BooleanAnd() => Do(Emit.BooleanAnd);

		public void BooleanNot() => Do(Emit.BooleanNot);

		public void BooleanOr() => Do(Emit.BooleanOr);

		public void BooleanXor() => Do(Emit.BooleanXor);

		public void BranchIf(ILabel label) => Do(() => Emit.BranchIf(label));

		public void BranchIfNot(ILabel label) => Do(() => Emit.BranchIfNot(label));

		public void Call() => Do(Emit.Call);

		public ILabel CreateLabel(string name, object key) => Emit.CreateLabel(name, key);

		public void DivideF() => Do(Emit.DivideF);

		public void DivideI() => Do(Emit.DivideI);

		public void DivideU() => Do(Emit.DivideU);

		public void EndData() => Do(Emit.EndData);

		public void EndFunction() => Do(Emit.EndFunction);

		public void EnterTry(ILabel handler) => Do(() => Emit.EnterTry(handler));

		public void ExitTry() => Do(Emit.ExitTry);

		public void ExternalFunction(ILabel label, string name, ulong returnType, ulong[] parameters)
		{
			Emit.DefineFunction(label, returnType, parameters);
			Do(() => Emit.ExternalFunction(label, name, returnType, parameters));
		}

		public IEmitterContext GetContext() => Emit.GetContext();

		public int GetOutputCount() => Emit.GetOutputCount();

		public void IsEqual() => Do(Emit.IsEqual);

		public void IsEqualF() => Do(Emit.IsEqualF);

		public void IsGreaterThanF() => Do(Emit.IsGreaterThanF);

		public void IsGreaterThanI() => Do(Emit.IsGreaterThanI);

		public void IsGreaterThanOrEqualF() => Do(Emit.IsGreaterThanOrEqualF);

		public void IsGreaterThanOrEqualI() => Do(Emit.IsGreaterThanOrEqualI);

		public void IsGreaterThanOrEqualU() => Do(Emit.IsGreaterThanOrEqualU);

		public void IsGreaterThanU() => Do(Emit.IsGreaterThanU);

		public void IsLessThanF() => Do(Emit.IsLessThanF);

		public void IsLessThanI() => Do(Emit.IsLessThanI);

		public void IsLessThanOrEqualF() => Do(Emit.IsLessThanOrEqualF);

		public void IsLessThanOrEqualI() => Do(Emit.IsLessThanOrEqualI);

		public void IsLessThanOrEqualU() => Do(Emit.IsLessThanOrEqualU);

		public void IsLessThanU() => Do(Emit.IsLessThanU);

		public void IsNotEqual() => Do(Emit.IsNotEqual);

		public void IsNotEqualF() => Do(Emit.IsNotEqualF);

		public void Jump(ILabel label) => Do(() => Emit.Jump(label));

		public void Load() => Do(Emit.Load);

		public void MarkLabel(ILabel label) => Do(() => Emit.MarkLabel(label));

		public void Multiply() => Do(Emit.Multiply);

		public void MultiplyF() => Do(Emit.MultiplyF);

		public void Negate() => Do(Emit.Negate);

		public ILabel NullLabel() => Emit.NullLabel();

		public void Pop() => Do(Emit.Pop);

		public void Push(bool value) => Do(() => Emit.Push(value));

		public void Push(sbyte value) => Do(() => Emit.Push(value));

		public void Push(byte value) => Do(() => Emit.Push(value));

		public void Push(short value) => Do(() => Emit.Push(value));

		public void Push(ushort value) => Do(() => Emit.Push(value));

		public void Push(int value) => Do(() => Emit.Push(value));

		public void Push(uint value) => Do(() => Emit.Push(value));

		public void Push(long value) => Do(() => Emit.Push(value));

		public void Push(ulong value) => Do(() => Emit.Push(value));

		public void Push(float value) => Do(() => Emit.Push(value));

		public void Push(double value) => Do(() => Emit.Push(value));

		public void PushAddress(ILabel label) => Do(() => Emit.PushAddress(label));

		public void PushArgumentAddress(int index) => Do(() => Emit.PushArgumentAddress(index));

		public void PushLocalAddress(int index) => Do(() => Emit.PushLocalAddress(index));

		public void PushNullAddress() => Do(Emit.PushNullAddress);

		public void PushStackAddress() => Do(Emit.PushStackAddress);

		public void PushUnassigned(ulong size) => Do(() => Emit.PushUnassigned(size));

		public void QueueAction(Action action) => Do(() => Emit.QueueAction(action));

		public void RemainderI() => Do(Emit.RemainderI);

		public void RemainderU() => Do(Emit.RemainderU);

		public void Return() => Do(Emit.Return);

		public void Section(int index) => Do(() => Emit.Section(index));

		public void SetParameter(string name, string value) => Emit.SetParameter(name, value);

		public void ShiftLeft() => Do(Emit.ShiftLeft);

		public void ShiftRight() => Do(Emit.ShiftRight);

		public void Store() => Do(Emit.Store);

		public void Subtract() => Do(Emit.Subtract);

		public void SubtractF() => Do(Emit.SubtractF);

		public void Throw() => Do(Emit.Throw);

		public bool TryGetLabelByKey(object key, out ILabel result) => Emit.TryGetLabelByKey(key, out result);

		public void WriteOutput(int index, Stream stream, ref string name)
		{
			while (Actions.Count > 0) Actions.Dequeue()();
			Emit.WriteOutput(index, stream, ref name);
		}

		private class CEmit : IEmitter
		{
			private class Expression
			{
				public string Value { get; }
				public string Type { get; }

				public Expression(string value, string type)
				{
					Value = value;
					Type = type;
				}

				public Expression(Label label)
				{
					Value = label.Identifier;
					Type = label.Type;
				}

				public override string ToString()
				{
					throw new Exception("Use Expression.Value instead of implicit ToString().");
				}
			}

			public class Label : ILabel
			{
				public string Identifier { get; }

				public string Name { get; }
				public ulong Id { get; }
				public LabelKind Kind { get; set; } = LabelKind.Unmarked;
				public string Type { get; set; } = "void*";

				public Label(LabelKind kind, string type)
				{
					Kind = kind;
					Type = type;
					Id = 0;

					switch (Kind)
					{
						case LabelKind.Null:
							Identifier = "0";
							Name = "null";
							break;
						case LabelKind.Return:
							Identifier = "result";
							Name = "result";
							break;
						default:
							throw new InvalidOperationException();
					}
				}

				public Label(ulong id, LabelKind kind, string type)
				{
					Id = id;
					Kind = kind;
					Type = type;

					switch (kind)
					{
						default:
							Identifier = $"_{Id:X8}";
							break;
						case LabelKind.Argument:
							Identifier = $"arg{Id}";
							break;
						case LabelKind.Local:
							Identifier = $"l{Id}";
							break;
					}
				}

				public Label(CEmit emitter, string name)
				{
					Name = name;
					Id = emitter.NextId++;
					Identifier = $"_{Id:X8}";
				}

				public override string ToString() => Identifier;
			}

			public enum LabelKind
			{
				Unmarked,
				Variable,
				Function,
				Argument,
				Local,
				Code,
				Null,
				Return
			}

			private class Context : IEmitterContext
			{
				public ulong MemoryCharSize => 1;

				public ulong RegisterWordSize => 4;

				public ulong AddressWordSize => 8;

				public ulong IEEEFloat32Size => 4;

				public ulong IEEEFloat64Size => 8;

				public ulong SizeOfBits(ulong bits)
				{
					return bits == 0 ? 0 : bits < 8 ? 1 : bits / 8;
				}
			}

			private readonly Stack<Expression> Expressions = new Stack<Expression>();
			private readonly List<string> Statements = new List<string>();
			private readonly List<Label> Locals = new List<Label>();
			private readonly List<Label> Arguments = new List<Label>();
			private readonly List<string> Values = new List<string>();
			private readonly Dictionary<object, Label> Labels = new Dictionary<object, Label>();
			private readonly Dictionary<Label, ulong[]> FunctionTypes = new Dictionary<Label, ulong[]>();
			private readonly Queue<Action> Backlog = new Queue<Action>();

			private Label CurrentLabel;
			private ulong NextId = 0;
			private ulong NextLocalId = 0;

			private void BinaryOp(string op)
			{
				var a = Expressions.Pop();
				var b = Expressions.Pop();
				Expressions.Push(new Expression($"({a.Value}) {op} ({b.Value})", GetOperatorType(op, a.Type, b.Type)));
			}

			private void UnaryOp(string op)
			{
				var a = Expressions.Pop();
				Expressions.Push(new Expression($"{op}({a.Value})", GetOperatorType(op, a.Type, "void")));
			}

			private string GetOperatorType(string op, string aType, string bType)
			{
				switch (op)
				{
					case "<":
					case "<=":
					case ">":
					case ">=":
					case "==":
					case "!=":
					case "&&":
					case "||":
					case "!":
						return "int";
					default:
						if (IsFloat(aType) != IsFloat(bType)) return IsFloat(aType) ? aType : bType;
						var aSize = GetTypeSize(aType);
						var bSize = GetTypeSize(bType);
						return aSize > bSize || (aSize == bSize && !IsFixedInteger(aType)) ? aType : bType;
				}
			}

			private string[] GetFunctionPointerTypes(string type)
			{
				if (!IsFunctionPointer(type)) return new string[0];
				var index = type.IndexOf('(', type.IndexOf('(') + 1) + 1;
				var end = type.LastIndexOf(')');
				return type.Substring(index, end - index).Split(',');
			}

			private ulong GetArraySize(string type)
			{
				if (!IsArray(type)) return GetTypeSize(type);
				var index = type.IndexOf('[') + 1;
				var end = type.IndexOf(']');
				return ulong.Parse(type.Substring(index, end - index));
			}

			private bool IsFixedInteger(string type)
			{
				return (type.StartsWith("int") || type.StartsWith("uint")) && type.EndsWith("_t");
			}

			private bool IsFloat(string type)
			{
				return type == "float" || type == "double";
			}

			private bool IsFunctionPointer(string type)
			{
				return type.StartsWith("void (*)(") && type.EndsWith(")");
			}

			private bool IsArray(string type)
			{
				return type.IndexOf('[') >= 0 && type.LastIndexOf("]") >= 0;
			}

			private string ArrayToPointer(string type)
			{
				return $"{type.Substring(0, type.IndexOf('['))}*{type.Substring(type.LastIndexOf(']') + 1)}";
			}

			private ulong GetTypeSize(string type)
			{
				switch (type)
				{
					default:
						return (ulong)(type.EndsWith("*") ? 8 : 0);
					case "void":
						return 0;
					case "char":
					case "unsigned char":
					case "int8_t":
					case "uint8_t":
						return 1;
					case "int":
					case "unsigned int":
					case "int16_t":
					case "uint16_t":
						return 2;
					case "long":
					case "unsigned long":
					case "float":
					case "int32_t":
					case "uint32_t":
						return 4;
					case "long long":
					case "unsigned long long":
					case "double":
					case "int64_t":
					case "uint64_t":
						return 8;
				}
			}

			private void FlushStatements()
			{
				var statements = new Stack<Expression>(Expressions.Count);
				while (Expressions.Count > 0) statements.Push(Expressions.Pop());
				while (statements.Count > 0) Statements.Add($"{statements.Pop().Value};");
			}

			private void DoPendingActions()
			{
				while (Backlog.Count > 0) Backlog.Dequeue()();
			}

			public void Add() => BinaryOp("+");

			public void AddF() => BinaryOp("+");

			public void BeginData(ILabel label)
			{
				if (CurrentLabel != null) throw new InvalidOperationException();
				CurrentLabel = (Label)label;
				Statements.Add($"//Entity: {CurrentLabel.Name ?? "Anonymous/Literal"}");
				if (CurrentLabel.Kind != LabelKind.Unmarked) throw new InvalidOperationException("Label is already in use.");
				CurrentLabel.Kind = LabelKind.Variable;
				CurrentLabel.Type = "unsigned char*";
				Statements.Add($"unsigned char {CurrentLabel.Identifier}[] = {{");
			}

			public void BeginFunction(ILabel label, ulong returnType, ulong[] parameters, ILocal[] locals)
			{
				if (CurrentLabel != null) throw new InvalidOperationException();
				CurrentLabel = (Label)label;
				Statements.Add($"//Entity: {CurrentLabel.Name ?? "Anonymous/Literal"}");
				if (CurrentLabel.Kind != LabelKind.Unmarked && !FunctionTypes.TryGetValue((Label)label, out _)) throw new InvalidOperationException("Label is already in use.");
				CurrentLabel.Kind = LabelKind.Function;
				var types = new ulong[parameters.Length + 1];
				var ptrArgs = $"unsigned char[{returnType}]";
				var funcArgs = $"unsigned char result[{returnType}]";
				Arguments.Add(new Label(LabelKind.Return, ptrArgs));
				types[0] = returnType;
				for (var i = 0; i < parameters.Length; i++)
				{
					Arguments.Add(new Label((ulong)i, LabelKind.Argument, $"unsigned char[{parameters[i]}]"));
					types[i + 1] = parameters[i];
					ptrArgs += $",unsigned char[{parameters[i]}]";
					funcArgs += $", unsigned char arg{i}[{parameters[i]}]";
				}
				if (!FunctionTypes.TryGetValue((Label)label, out _)) FunctionTypes.Add((Label)label, types);
				CurrentLabel.Type = $"void (*)({ptrArgs})";
				Statements.Add($"void {CurrentLabel.Identifier}({funcArgs}) {{");
				for (var i = 0; i < locals.Length; i++)
				{
					locals[i].PushInitialValue(this);
					PushAddress(AddLocal(locals[i].Size));
					Store();
				}
			}

			private Label AddLocal(ulong size)
			{
				var id = NextLocalId++;
				Statements.Add($"unsigned char l{id}[{size}];");
				var result = new Label(id, LabelKind.Local, $"unsigned char[{size}]");
				Locals.Add(result);
				return result;
			}

			public void BitwiseAnd() => BinaryOp("&");

			public void BitwiseNot() => UnaryOp("~");

			public void BitwiseOr() => BinaryOp("|");

			public void BitwiseXor() => BinaryOp("^");

			public void BooleanAnd() => BinaryOp("&&");

			public void BooleanNot() => UnaryOp("!");

			public void BooleanOr() => BinaryOp("||");

			public void BooleanXor() => BinaryOp("!=");

			public void BranchIf(ILabel label)
			{
				var a = Expressions.Pop();
				FlushStatements();
				Statements.Add($"if ({a.Value}) goto {((Label)label).Identifier};");
			}

			public void BranchIfNot(ILabel label)
			{
				var a = Expressions.Pop();
				FlushStatements();
				Statements.Add($"if (!({a.Value})) goto {((Label)label).Identifier};");
			}

			public void Call()
			{
				var target = Expressions.Pop();
				if (!IsFunctionPointer(target.Type)) throw new InvalidOperationException("Target must be function.");
				var args = GetFunctionPointerTypes(target.Type);
				var argSizes = new ulong[args.Length];
				for (var i = 0; i < args.Length; i++) argSizes[i] = GetArraySize(args[i]);
				var resultLocal = AddLocal(argSizes[0]);
				var call = $"(({target.Type}){target.Value})({resultLocal.Identifier}";
				var argValues = new Expression[args.Length - 1];
				for (var i = args.Length - 1; i > 0; i--) argValues[i - 1] = Expressions.Pop();
				for (var i = 0; i < argValues.Length; i++) call += $", {argValues[i].Value}";
				Statements.Add(call + ");");
				if (argSizes[0] > 0) Expressions.Push(new Expression(resultLocal));
			}

			public ILabel CreateLabel(string name, object key)
			{
				var result = new Label(this, name);
				if (key != null) Labels.Add(key, result);
				return result;
			}

			public void DefineFunction(ILabel label, ulong returnType, ulong[] parameters)
			{
				if (CurrentLabel != null) throw new InvalidOperationException("Functions must be defined globally.");

				if (!FunctionTypes.TryGetValue((Label)label, out _))
				{
					var args = new string[parameters.Length + 1];
					var trampolineArgs = new string[parameters.Length + 1];
					args[0] = $"unsigned char[{returnType}]";
					trampolineArgs[0] = "result";
					for (var i = 0; i < parameters.Length; i++)
					{
						args[i + 1] = $"unsigned char[{parameters[i]}]";
						trampolineArgs[i + 1] = $"arg{i}";
					}
					Statements.Insert(0, $"//Entity: {((Label)label).Name ?? "Anonymous/Literal"}");
					Statements.Insert(1, $"void {((Label)label).Identifier}({string.Join(", ", args)});");
					((Label)label).Kind = LabelKind.Function;
					((Label)label).Type = $"void (*)({string.Join(", ", args)})";

					var types = new ulong[parameters.Length + 1];
					types[0] = returnType;
					for (var i = 0; i < parameters.Length; i++) types[i + 1] = parameters[i];
					FunctionTypes.Add((Label)label, types);
				}
			}

			public void DefineVariable(ILabel label)
			{
				if (CurrentLabel != null) throw new InvalidOperationException("Variables must be defined globally.");

				Statements.Insert(0, $"//Entity: {((Label)label).Name ?? "Anonymous/Literal"}");
				Statements.Insert(1, $"unsigned char {((Label)label).Identifier}[];");
			}

			public void DivideF() => BinaryOp("/");

			public void DivideI() => BinaryOp("/");

			public void DivideU() => BinaryOp("/");

			public void EndData()
			{
				Statements.Add(string.Join(",", Values));
				Statements.Add("};");
				Values.Clear();
				CurrentLabel = null;
				DoPendingActions();
			}

			public void EndFunction()
			{
				if (CurrentLabel == null) throw new InvalidOperationException("BeginFunction not called before EndFunction.");
				Statements.Add("}");
				Locals.Clear();
				Arguments.Clear();
				CurrentLabel = null;
				DoPendingActions();
			}

			public void EnterTry(ILabel handler)
			{
				throw new NotImplementedException();
			}

			public void ExitTry()
			{
				throw new NotImplementedException();
			}

			public void ExternalFunction(ILabel label, string name, ulong returnType, ulong[] parameters)
			{
				var args = new string[parameters.Length + 1];
				var trampolineArgs = new string[parameters.Length + 1];
				args[0] = $"unsigned char[{returnType}]";
				trampolineArgs[0] = "result";
				for (var i = 0; i < parameters.Length; i++)
				{
					args[i + 1] = $"unsigned char[{parameters[i]}]";
					trampolineArgs[i + 1] = $"arg{i}";
				}
				Statements.Insert(0, $"void {name.Replace('.', '_')}({string.Join(", ", args)});");

				BeginFunction(label, returnType, parameters, new ILocal[0]);
				Statements.Add($"{name.Replace('.', '_')}({string.Join(", ", trampolineArgs)});");
				EndFunction();
			}

			public IEmitterContext GetContext() => new Context();

			public int GetOutputCount() => 1;

			public void IsEqual() => BinaryOp("==");

			public void IsEqualF() => BinaryOp("==");

			public void IsGreaterThanF() => BinaryOp(">");

			public void IsGreaterThanI() => BinaryOp(">");

			public void IsGreaterThanOrEqualF() => BinaryOp(">=");

			public void IsGreaterThanOrEqualI() => BinaryOp(">=");

			public void IsGreaterThanOrEqualU() => BinaryOp(">=");

			public void IsGreaterThanU() => BinaryOp(">");

			public void IsLessThanF() => BinaryOp("<");

			public void IsLessThanI() => BinaryOp("<");

			public void IsLessThanOrEqualF() => BinaryOp("<=");

			public void IsLessThanOrEqualI() => BinaryOp("<=");

			public void IsLessThanOrEqualU() => BinaryOp("<=");

			public void IsLessThanU() => BinaryOp("<");

			public void IsNotEqual() => BinaryOp("!=");

			public void IsNotEqualF() => BinaryOp("!=");

			public void Jump(ILabel label)
			{
				FlushStatements();
				Statements.Add($"goto {((Label)label).Identifier};");
			}

			public void Load()
			{
				Expressions.Push(new Expression($"*(int*)({Expressions.Pop().Value})", "int"));
			}

			public void MarkLabel(ILabel label)
			{
				FlushStatements();
				var l = (Label)label;
				if (l.Kind != LabelKind.Unmarked) throw new InvalidOperationException("Label is already in use.");
				l.Kind = LabelKind.Code;
				Statements.Add($"{l.Identifier}:");
			}

			public void Multiply() => BinaryOp("*");

			public void MultiplyF() => BinaryOp("*");

			public void Negate() => UnaryOp("-");

			public ILabel NullLabel() => new Label(LabelKind.Null, "void*");

			public void Pop()
			{
				Statements.Add($"{Expressions.Pop().Value};");
			}

			public void Push(bool value)
			{
				var data = value ? "1" : "0";

				if (CurrentLabel.Kind == LabelKind.Variable) Values.Add(data);
				else Expressions.Push(new Expression(data, "int"));
			}

			public void Push(sbyte value)
			{
				if (CurrentLabel.Kind == LabelKind.Variable) Values.Add(value.ToString());
				else Expressions.Push(new Expression($"(char){value}", "char"));
			}

			public void Push(byte value)
			{
				if (CurrentLabel.Kind == LabelKind.Variable) Values.Add(value.ToString());
				else Expressions.Push(new Expression($"(unsigned char){value}", "unsigned char"));
			}

			public void Push(short value)
			{
				if (CurrentLabel.Kind == LabelKind.Variable)
				{
					Values.Add(((ushort)value & 0xFF).ToString());
					Values.Add((((ushort)value >> 8) & 0xFF).ToString());
				}
				else
				{
					Expressions.Push(new Expression($"(int){value}", "int"));
				}
			}

			public void Push(ushort value)
			{
				if (CurrentLabel.Kind == LabelKind.Variable)
				{
					Values.Add((value & 0xFF).ToString());
					Values.Add(((value >> 8) & 0xFF).ToString());
				}
				else
				{
					Expressions.Push(new Expression($"(unsigned int){value}", "unsigned int"));
				}
			}

			public void Push(int value)
			{
				if (CurrentLabel.Kind == LabelKind.Variable)
				{
					Values.Add(((uint)value & 0xFF).ToString());
					Values.Add((((uint)value >> 8) & 0xFF).ToString());
					Values.Add((((uint)value >> 16) & 0xFF).ToString());
					Values.Add((((uint)value >> 24) & 0xFF).ToString());
				}
				else
				{
					Expressions.Push(new Expression($"(long){value}", "long"));
				}
			}

			public void Push(uint value)
			{
				if (CurrentLabel.Kind == LabelKind.Variable)
				{
					Values.Add((value & 0xFF).ToString());
					Values.Add(((value >> 8) & 0xFF).ToString());
					Values.Add(((value >> 16) & 0xFF).ToString());
					Values.Add(((value >> 24) & 0xFF).ToString());
				}
				else
				{
					Expressions.Push(new Expression($"(unsigned long){value}", "unsigned long"));
				}
			}

			public void Push(long value)
			{
				if (CurrentLabel.Kind == LabelKind.Variable)
				{
					Values.Add(((ulong)value & 0xFF).ToString());
					Values.Add((((ulong)value >> 8) & 0xFF).ToString());
					Values.Add((((ulong)value >> 16) & 0xFF).ToString());
					Values.Add((((ulong)value >> 24) & 0xFF).ToString());
					Values.Add((((ulong)value >> 32) & 0xFF).ToString());
					Values.Add((((ulong)value >> 40) & 0xFF).ToString());
					Values.Add((((ulong)value >> 48) & 0xFF).ToString());
					Values.Add((((ulong)value >> 56) & 0xFF).ToString());
				}
				else
				{
					Expressions.Push(new Expression($"(long long){value}", "long long"));
				}
			}

			public void Push(ulong value)
			{
				if (CurrentLabel.Kind == LabelKind.Variable)
				{
					Values.Add((value & 0xFF).ToString());
					Values.Add(((value >> 8) & 0xFF).ToString());
					Values.Add(((value >> 16) & 0xFF).ToString());
					Values.Add(((value >> 24) & 0xFF).ToString());
					Values.Add(((value >> 32) & 0xFF).ToString());
					Values.Add(((value >> 40) & 0xFF).ToString());
					Values.Add(((value >> 48) & 0xFF).ToString());
					Values.Add(((value >> 56) & 0xFF).ToString());
				}
				else
				{
					Expressions.Push(new Expression($"(unsigned long long){value}", "unsigned long long"));
				}
			}

			public void Push(float value)
			{
				if (CurrentLabel.Kind == LabelKind.Variable)
				{
					var values = BitConverter.GetBytes(value);
					for (var i = 0; i < values.Length; i++) Push(values[i]);
				}
				else
				{
					Expressions.Push(new Expression($"(float){value}", "float"));
				}
			}

			public void Push(double value)
			{
				if (CurrentLabel.Kind == LabelKind.Variable)
				{
					var values = BitConverter.GetBytes(value);
					for (var i = 0; i < values.Length; i++) Push(values[i]);
				}
				else
				{
					Expressions.Push(new Expression($"(double){value}", "double"));
				}
			}

			public void PushAddress(ILabel label)
			{
				Expressions.Push(new Expression((Label)label));
			}

			public void PushArgumentAddress(int index)
			{
				Expressions.Push(new Expression(Arguments[index + 1]));
			}

			public void PushLocalAddress(int index)
			{
				Expressions.Push(new Expression(Locals[index]));
			}

			public void PushNullAddress()
			{
				if (CurrentLabel.Kind == LabelKind.Variable)
				{
					Values.Add("0");
				}
				else
				{
					Expressions.Push(new Expression((Label)NullLabel()));
				}
			}

			public void PushStackAddress()
			{
				throw new NotImplementedException();
			}

			public void PushUnassigned(ulong size)
			{
				for (ulong i = 0; i < size; i++) Push((byte)0);
			}

			public void QueueAction(Action action) => Backlog.Enqueue(action);

			public void RemainderI() => BinaryOp("%");

			public void RemainderU() => BinaryOp("%");

			public void Return()
			{
				if (IsArray(Arguments[0].Type) && GetArraySize(Arguments[0].Type) > 0)
				{
					Expressions.Push(new Expression(Arguments[0]));
					Store();
				}

				FlushStatements();
				Statements.Add("return;");
			}

			public void Section(int index) { }

			public void SetParameter(string name, string value)
			{
				throw new NotImplementedException();
			}

			public void ShiftLeft() => BinaryOp("<<");

			public void ShiftRight() => BinaryOp(">>");

			public void Store()
			{
				var target = Expressions.Pop();
				var value = Expressions.Pop();
				Statements.Add($"*(({(IsArray(value.Type) ? ArrayToPointer(value.Type) : value.Type)}*){target.Value}) = {value.Value};");
			}

			public void Subtract() => BinaryOp("-");

			public void SubtractF() => BinaryOp("-");

			public void Throw()
			{
				throw new NotImplementedException();
			}

			public bool TryGetLabelByKey(object key, out ILabel result)
			{
				result = null;
				if (key == null) return false;
				else if (Labels.TryGetValue(key, out Label label))
				{
					result = label;
					return true;
				}
				else
				{
					return false;
				}
			}

			public void WriteOutput(int index, Stream stream, ref string name)
			{
				if (index != 0) throw new IndexOutOfRangeException();

				for (var i = 0; i < Statements.Count; i++)
				{
					var row = Encoding.UTF8.GetBytes(Statements[i] + Environment.NewLine);
					stream.Write(row, 0, row.Length);
				}
			}
		}
	}
}
