using System;
using System.Collections.Generic;
using System.IO;

namespace Transform.Compiler
{
	public class Package
	{
		public Namespace Global { get; } = new Namespace(null, null);

		public Package(IEmitterContext context)
		{
			Function createOperator(Namespace owner, string op, Action<IEmitter, CodeBody, ILabel> onEmit)
			{
				var result = new Function(owner, null, op);
				result.Body = new CodeBody(result, null);
				result.Body.Statements.Add(new EmitterStatement(result.Body, onEmit));
				return result;
			}

			Global.Add(new Structure(Global, "char") { Padding = context.MemoryCharSize });
			Global.Add(new Structure(Global, "uchar") { Padding = context.MemoryCharSize });
			Global.Add(new Structure(Global, "word") { Padding = context.RegisterWordSize });
			Global.Add(new Structure(Global, "uword") { Padding = context.RegisterWordSize });
			Global.Add(new Structure(Global, "i8") { Padding = context.SizeOfBits(8) });
			Global.Add(new Structure(Global, "u8") { Padding = context.SizeOfBits(8) });
			Global.Add(new Structure(Global, "i16") { Padding = context.SizeOfBits(16) });
			Global.Add(new Structure(Global, "u16") { Padding = context.SizeOfBits(16) });
			Global.Add(new Structure(Global, "i32") { Padding = context.SizeOfBits(32) });
			Global.Add(new Structure(Global, "u32") { Padding = context.SizeOfBits(32) });
			Global.Add(new Structure(Global, "i64") { Padding = context.SizeOfBits(64) });
			Global.Add(new Structure(Global, "u64") { Padding = context.SizeOfBits(64) });

			for (var i = 0; i < Global.Structures.Count; i++)
			{
				var current = Global.Structures[i];
				var ops = new[]
				{
					createOperator(Global, "+", (emit, body, end) => emit.Add()),
					createOperator(Global, "-", (emit, body, end) => emit.Subtract()),
					createOperator(Global, "*", (emit, body, end) => emit.Multiply()),
					current.Name[0] == 'u' ? createOperator(Global, "/", (emit, body, end) => emit.DivideU()) : createOperator(Global, "/", (emit, body, end) => emit.DivideI()),
					current.Name[0] == 'u' ? createOperator(Global, "%", (emit, body, end) => emit.RemainderU()) : createOperator(Global, "%", (emit, body, end) => emit.RemainderI()),
					createOperator(Global, "<<", (emit, body, end) => emit.ShiftLeft()),
					createOperator(Global, ">>", (emit, body, end) => emit.ShiftRight()),
					createOperator(Global, "&", (emit, body, end) => emit.BitwiseAnd()),
					createOperator(Global, "|", (emit, body, end) => emit.BitwiseOr()),
					createOperator(Global, "^", (emit, body, end) => emit.BitwiseXor()),
					createOperator(Global, "&&", (emit, body, end) => emit.BooleanAnd()),
					createOperator(Global, "||", (emit, body, end) => emit.BooleanOr()),
					createOperator(Global, "==", (emit, body, end) => emit.IsEqual()),
					createOperator(Global, "!=", (emit, body, end) => emit.IsNotEqual()),
					current.Name[0] == 'u' ? createOperator(Global, "<", (emit, body, end) => emit.IsLessThanU()) : createOperator(Global, "<", (emit, body, end) => emit.IsLessThanI()),
					current.Name[0] == 'u' ? createOperator(Global, ">", (emit, body, end) => emit.IsGreaterThanU()) : createOperator(Global, ">", (emit, body, end) => emit.IsGreaterThanI()),
					current.Name[0] == 'u' ? createOperator(Global, "<=", (emit, body, end) => emit.IsLessThanOrEqualU()) : createOperator(Global, "<=", (emit, body, end) => emit.IsLessThanOrEqualI()),
					current.Name[0] == 'u' ? createOperator(Global, ">=", (emit, body, end) => emit.IsGreaterThanOrEqualU()) : createOperator(Global, ">=", (emit, body, end) => emit.IsGreaterThanOrEqualI()),
				};

				for (var j = 0; j < ops.Length; j++)
				{
					ops[j].Parameters.Add(new Variable("a", new TypeReference(Global.Structures[i])));
					ops[j].Parameters.Add(new Variable("b", new TypeReference(Global.Structures[i])));
					ops[j].ReturnValue = new TypeReference(Global.Structures[i]);
					ops[j].Body.Statements.Insert(0, new EmitterStatement(ops[j].Body, (emit, body, end) =>
					{
						emit.PushArgumentAddress(0);
						emit.Load();
						emit.PushArgumentAddress(1);
						emit.Load();
					}));
					ops[j].Body.Statements.Add(new EmitterStatement(ops[j].Body, (emit, body, end) =>
					{
						emit.Return();
					}));
					Global.Add(ops[j]);
				}
			}

			Global.Add(new Structure(Global, "float") { Padding = context.IEEEFloat32Size });
			Global.Add(new Structure(Global, "double") { Padding = context.IEEEFloat64Size });

			for (var i = 0; i < 2; i++)
			{
				var type = Global.Structures[(Global.Structures.Count - 1) - i];
				var ops = new[]
				{
					createOperator(Global, "+", (emit, body, end) => emit.AddF()),
					createOperator(Global, "-", (emit, body, end) => emit.SubtractF()),
					createOperator(Global, "*", (emit, body, end) => emit.MultiplyF()),
					createOperator(Global, "/", (emit, body, end) => emit.DivideF()),
					createOperator(Global, "==", (emit, body, end) => emit.IsEqualF()),
					createOperator(Global, "!=", (emit, body, end) => emit.IsNotEqualF()),
					createOperator(Global, "<", (emit, body, end) => emit.IsLessThanF()),
					createOperator(Global, ">", (emit, body, end) => emit.IsGreaterThanF()),
					createOperator(Global, "<=", (emit, body, end) => emit.IsLessThanOrEqualF()),
					createOperator(Global, ">=", (emit, body, end) => emit.IsGreaterThanOrEqualF())
				};

				for (var j = 0; j < ops.Length; j++)
				{
					ops[j].Parameters.Add(new Variable("a", new TypeReference(type)));
					ops[j].Parameters.Add(new Variable("b", new TypeReference(type)));
					ops[j].ReturnValue = new TypeReference(type);
					ops[j].Body.Statements.Insert(0, new EmitterStatement(ops[j].Body, (emit, body, end) =>
					{
						emit.PushArgumentAddress(0);
						emit.Load();
						emit.PushArgumentAddress(1);
						emit.Load();
					}));
					ops[j].Body.Statements.Add(new EmitterStatement(ops[j].Body, (emit, body, end) =>
					{
						emit.Return();
					}));
					Global.Add(ops[j]);
				}
			}

			Global.Add(new Structure(Global, "void") { Padding = 0 });

			for (var i = 0; i < 32; i++)
			{
				var func = new Structure(Global, "func") { Padding = context.AddressWordSize };
				func.GenericParameters.Add(new Variable("TReturn", null));
				for (var j = 0; j < i; j++) func.GenericParameters.Add(new Variable($"T{j + 1}", null));
				Global.Add(func);
			}
		}

		public void AddArrayType()
		{
			var array = new Structure(Global, "Array");
			array.GenericParameters.Add(new Variable("T", new TypeReference(Global, "void", 1)));
			array.Fields.Add(new Variable("Length", new TypeReference(Global, "uword")));
			array.Fields.Add(new Variable("Items", new TypeReference(Global, array, "T")));

			Global.Add(array);
		}

		public void AddReflectionTypes()
		{
			var reflection = new Namespace(Global, "Reflection");
			var namespaceInfo = new Structure(reflection, "Namespace");
			var typeInfo = new Structure(reflection, "Type");
			var functionInfo = new Structure(reflection, "Function");
			var variableInfo = new Structure(reflection, "Variable");
			var typeReference = new Structure(reflection, "TypeReference");

			namespaceInfo.Fields.Add(new Variable("Name", new TypeReference(reflection, "char", 1)));
			namespaceInfo.Fields.Add(new Variable("FullName", new TypeReference(reflection, "char", 1)));
			namespaceInfo.Fields.Add(new Variable("Parent", new TypeReference(namespaceInfo, 1)));
			namespaceInfo.Fields.Add(new Variable("Namespaces", new TypeReference(reflection, new string[0], "Array", new[] { new TypeReference(namespaceInfo, 1) }, 1)));
			namespaceInfo.Fields.Add(new Variable("Types", new TypeReference(reflection, new string[0], "Array", new[] { new TypeReference(typeInfo, 1) }, 1)));
			namespaceInfo.Fields.Add(new Variable("Functions", new TypeReference(reflection, new string[0], "Array", new[] { new TypeReference(functionInfo, 1) }, 1)));
			namespaceInfo.Fields.Add(new Variable("Variables", new TypeReference(reflection, new string[0], "Array", new[] { new TypeReference(variableInfo, 1) }, 1)));

			typeInfo.Fields.Add(new Variable("Name", new TypeReference(reflection, "char", 1)));
			typeInfo.Fields.Add(new Variable("FullName", new TypeReference(reflection, "char", 1)));
			typeInfo.Fields.Add(new Variable("BaseType", new TypeReference(typeInfo, 1)));
			typeInfo.Fields.Add(new Variable("Size", new TypeReference(reflection, "uword")));
			typeInfo.Fields.Add(new Variable("Fields", new TypeReference(reflection, new string[0], "Array", new[] { new TypeReference(variableInfo, 1) }, 1)));
			typeInfo.Fields.Add(new Variable("Methods", new TypeReference(reflection, new string[0], "Array", new[] { new TypeReference(functionInfo, 1) }, 1)));

			functionInfo.Fields.Add(new Variable("Name", new TypeReference(reflection, "char", 1)));
			functionInfo.Fields.Add(new Variable("FullName", new TypeReference(reflection, "char", 1)));
			functionInfo.Fields.Add(new Variable("Signature", new TypeReference(reflection, "char", 1)));
			functionInfo.Fields.Add(new Variable("ReturnType", new TypeReference(typeReference, 1)));
			functionInfo.Fields.Add(new Variable("Parameters", new TypeReference(reflection, new string[0], "Array", new[] { new TypeReference(variableInfo, 1) }, 1)));

			variableInfo.Fields.Add(new Variable("Name", new TypeReference(reflection, "char", 1)));
			variableInfo.Fields.Add(new Variable("Type", new TypeReference(typeReference, 1)));

			typeReference.Fields.Add(new Variable("UnderlyingType", new TypeReference(typeInfo, 1)));
			typeReference.Fields.Add(new Variable("GenericArguments", new TypeReference(reflection, new string[0], "Array", new[] { new TypeReference(typeReference, 1) }, 1)));
			typeReference.Fields.Add(new Variable("Indirection", new TypeReference(reflection, "word")));

			reflection.Add(namespaceInfo);
			reflection.Add(typeInfo);
			reflection.Add(functionInfo);
			reflection.Add(variableInfo);
			reflection.Add(typeReference);

			Global.Add(reflection);
		}

		public void AddStandardLibrary()
		{
			var std = new Namespace(Global, "Standard");

			var printf = new Function(std, null, "Print") { ReturnValue = new TypeReference(std, "void") };
			printf.Parameters.Add(new Variable("str", new TypeReference(std, "char", 1)));
			printf.Body = new CodeBody(printf, null, true);
			std.Add(printf);

			var math = new Namespace(std, "Math");
			var sqrt = new Function(std, null, "Sqrt") { ReturnValue = new TypeReference(math, "float") };
			sqrt.Parameters.Add(new Variable("value", new TypeReference(math, "float")));
			sqrt.Body = new CodeBody(sqrt, null, true);
			math.Add(sqrt);
			std.Add(math);

			Global.Add(std);
		}

		public void AddSource(Stream stream)
		{
			var tokens = new TokenStream(stream);

			tokens.ConsumeWhitespace();

			while (!tokens.EOF)
			{
				if (TryParseNamespace(tokens, Global, out Namespace ns))
				{
					Global.Add(ns);
				}
				else if (TryParseStructure(tokens, Global, out Structure structure))
				{
					Global.Add(structure);
				}
				else if (TryParseOperator(tokens, Global, out Function op))
				{
					Global.Add(op);
				}
				else if (TryParseLocal(tokens, Global, out InitializedVariable variable))
				{
					Global.Add(variable);
				}
				else if (TryParseFunction(tokens, Global, null, out Function function))
				{
					Global.Add(function);
				}
				else
				{
					throw new Exception("Error!");
				}

				tokens.ConsumeWhitespace();
			}

			MergeNamespaces();
		}

		private void MergeNamespaces()
		{
			for (var i = 0; i < Global.Namespaces.Count; i++)
			{
				for (var j = i + 1; j < Global.Namespaces.Count; j++)
				{
					if (Global.Namespaces[i].Name == Global.Namespaces[j].Name)
					{
						Global.Namespaces[i] = Namespace.Merge(Global, Global.Namespaces[i], Global.Namespaces[j]);
						Global.Namespaces.RemoveAt(j);
						j--;
					}
				}
			}
		}

		private bool TryParseNamespace(TokenStream tokens, Namespace parent, out Namespace result)
		{
			result = null;
			var tokenState = tokens.Save();
			tokens.ConsumeWhitespace();
			if (!tokens.TryReadExact("namespace") || !tokens.ConsumeWhitespace() || !tokens.TryReadIdentifier(out string name))
			{
				tokens.Restore(tokenState);
				return false;
			}

			var path = new List<string> { name };

			while (tokens.TryReadExact("."))
			{
				if (!tokens.TryReadIdentifier(out name))
				{
					tokens.Restore(tokenState);
					return false;
				}

				path.Add(name);
			}

			var current = new Namespace(parent, path[0]);
			for (var i = 1; i < path.Count; i++)
			{
				var next = new Namespace(current, path[i]);
				current.Add(next);
				current = next;
			}
			result = current;

			tokens.ConsumeWhitespace();

			if (!tokens.TryReadExact("{"))
			{
				tokens.Restore(tokenState);
				return false;
			}

			tokens.ConsumeWhitespace();

			while (!tokens.TryReadExact("}"))
			{
				if (tokens.PeekChar() == '\0')
				{
					tokens.Restore(tokenState);
					return false;
				}

				if (TryParseNamespace(tokens, result, out Namespace ns))
				{
					result.Add(ns);
				}
				else if (TryParseStructure(tokens, result, out Structure structure))
				{
					result.Add(structure);
				}
				else if (TryParseOperator(tokens, result, out Function op))
				{
					result.Add(op);
				}
				else if (TryParseLocal(tokens, parent, out InitializedVariable variable))
				{
					result.Add(variable);
				}
				else if (TryParseFunction(tokens, result, null, out Function function))
				{
					result.Add(function);
				}
				else
				{
					tokens.Restore(tokenState);
					return false;
				}

				tokens.ConsumeWhitespace();
			}

			return true;
		}

		private static bool TryParseStructure(TokenStream tokens, Namespace parent, out Structure result)
		{
			result = null;
			var tokenState = tokens.Save();

			tokens.ConsumeWhitespace();

			if (!tokens.TryReadExact("struct") || !tokens.ConsumeWhitespace() || !tokens.TryReadIdentifier(out string name))
			{
				tokens.Restore(tokenState);
				return false;
			}

			tokens.ConsumeWhitespace();

			result = new Structure(parent, name);

			if (tokens.TryReadExact("<"))
			{
				while (true)
				{
					tokens.ConsumeWhitespace();

					if (!tokens.TryReadIdentifier(out string typeParam))
					{
						tokens.Restore(tokenState);
						return false;
					}

					tokens.ConsumeWhitespace();

					TypeReference baseType = null;

					if (tokens.TryReadExact(":"))
					{
						tokens.ConsumeWhitespace();

						if (!TryParseTypeReference(tokens, parent, out baseType))
						{
							tokens.Restore(tokenState);
							return false;
						}
					}

					result.GenericParameters.Add(new Variable(typeParam, baseType));

					tokens.ConsumeWhitespace();

					if (tokens.TryReadExact(">")) break;
					else if (!tokens.TryReadExact(","))
					{
						tokens.Restore(tokenState);
						return false;
					}
				}

				tokens.ConsumeWhitespace();
			}

			if (tokens.TryReadExact(":"))
			{
				tokens.ConsumeWhitespace();

				if (!TryParseTypeReference(tokens, parent, out TypeReference parentType))
				{
					tokens.Restore(tokenState);
					return false;
				}

				result.BaseType = parentType;

				tokens.ConsumeWhitespace();
			}

			if (!TryParseTypeBody(tokens, result, out Variable[] fields, out Function[] methods))
			{
				tokens.Restore(tokenState);
				return false;
			}

			result.Fields.AddRange(fields);
			result.Methods.AddRange(methods);

			return true;
		}

		private static bool TryParseFunction(TokenStream tokens, Namespace parentNamespace, Structure parentStructure, out Function result)
		{
			result = null;
			var tokenState = tokens.Save();

			tokens.ConsumeWhitespace();

			if (!TryParseTypeReference(tokens, parentNamespace, out TypeReference returnType))
			{
				tokens.Restore(tokenState);
				return false;
			}

			tokens.ConsumeWhitespace();

			if (!tokens.TryReadIdentifier(out string name))
			{
				tokens.Restore(tokenState);
				return false;
			}

			tokens.ConsumeWhitespace();

			if (!tokens.TryReadExact("("))
			{
				tokens.Restore(tokenState);
				return false;
			}

			tokens.ConsumeWhitespace();

			result = new Function(parentNamespace, parentStructure, name);

			if (!tokens.TryReadExact(")"))
			{
				while (true)
				{
					if (!TryParseTypeReference(tokens, parentNamespace, out TypeReference type))
					{
						tokens.Restore(tokenState);
						return false;
					}

					tokens.ConsumeWhitespace();

					if (!tokens.TryReadIdentifier(out string paramName))
					{
						tokens.Restore(tokenState);
						return false;
					}

					result.Parameters.Add(new Variable(paramName, type));

					tokens.ConsumeWhitespace();

					if (tokens.TryReadExact(")"))
					{
						break;
					}
					else if (!tokens.TryReadExact(","))
					{
						tokens.Restore(tokenState);
						return false;
					}

					tokens.ConsumeWhitespace();
				}
			}

			if (!TryParseCodeBody(tokens, result, null, out CodeBody body))
			{
				tokens.Restore(tokenState);
				return false;
			}

			result.ReturnValue = returnType;
			result.Body = body;

			return true;
		}

		private static bool TryParseOperator(TokenStream tokens, Namespace parent, out Function result)
		{
			result = null;
			var tokenState = tokens.Save();

			tokens.ConsumeWhitespace();

			if (!TryParseTypeReference(tokens, parent, out TypeReference returnType))
			{
				tokens.Restore(tokenState);
				return false;
			}

			tokens.ConsumeWhitespace();

			if (!tokens.TryReadExact("operator") || !tokens.ConsumeWhitespace())
			{
				tokens.Restore(tokenState);
				return false;
			}

			if (!tokens.TryReadOperator(out string name))
			{
				tokens.Restore(tokenState);
				return false;
			}

			tokens.ConsumeWhitespace();

			if (!tokens.TryReadExact("("))
			{
				tokens.Restore(tokenState);
				return false;
			}

			tokens.ConsumeWhitespace();

			result = new Function(parent, null, name);

			if (!tokens.TryReadExact(")"))
			{
				while (true)
				{
					if (!TryParseTypeReference(tokens, parent, out TypeReference type))
					{
						tokens.Restore(tokenState);
						return false;
					}

					tokens.ConsumeWhitespace();

					if (!tokens.TryReadIdentifier(out string paramName))
					{
						tokens.Restore(tokenState);
						return false;
					}

					result.Parameters.Add(new Variable(paramName, type));

					tokens.ConsumeWhitespace();

					if (tokens.TryReadExact(")"))
					{
						break;
					}
					else if (!tokens.TryReadExact(","))
					{
						tokens.Restore(tokenState);
						return false;
					}

					tokens.ConsumeWhitespace();
				}
			}

			if (!TryParseCodeBody(tokens, result, null, out CodeBody body))
			{
				tokens.Restore(tokenState);
				return false;
			}

			result.ReturnValue = returnType;
			result.Body = body;

			return true;
		}

		private static bool TryParseTypeReference(TokenStream tokens, Namespace parent, out TypeReference result)
		{
			result = null;
			var tokenState = tokens.Save();
			if (!tokens.TryReadIdentifier(out string name))
			{
				tokens.Restore(tokenState);
				return false;
			}

			var path = new List<string> { name };

			while (tokens.TryReadExact("."))
			{
				if (!tokens.TryReadIdentifier(out name))
				{
					tokens.Restore(tokenState);
					return false;
				}

				path.Add(name);
			}

			tokens.ConsumeWhitespace();

			var genericArgs = new List<TypeReference>();

			if (tokens.TryReadExact("<"))
			{
				while (true)
				{
					tokens.ConsumeWhitespace();

					if (!TryParseTypeReference(tokens, parent, out TypeReference typeArg))
					{
						tokens.Restore(tokenState);
						return false;
					}

					genericArgs.Add(typeArg);

					tokens.ConsumeWhitespace();

					if (tokens.TryReadExact(">")) break;
					else if (!tokens.TryReadExact(","))
					{
						tokens.Restore(tokenState);
						return false;
					}
				}

				tokens.ConsumeWhitespace();
			}

			var indirection = 0;
			while (tokens.TryReadExact("*"))
			{
				tokens.ConsumeWhitespace();
				indirection += 1;
			}

			path.RemoveAt(path.Count - 1);

			result = new TypeReference(parent, path.ToArray(), name, genericArgs.ToArray(), indirection);
			return true;
		}

		private static bool TryParseTypeBody(TokenStream tokens, Structure parent, out Variable[] fields, out Function[] methods)
		{
			fields = null;
			methods = null;
			var tokenState = tokens.Save();

			tokens.ConsumeWhitespace();

			if (!tokens.TryReadExact("{"))
			{
				tokens.Restore(tokenState);
				return false;
			}

			tokens.ConsumeWhitespace();

			var fieldBuffer = new List<Variable>();
			var methodBuffer = new List<Function>();

			while (!tokens.TryReadExact("}"))
			{
				if (TryParseField(tokens, parent.Parent, out Variable field))
				{
					fieldBuffer.Add(field);
				}
				else if (TryParseFunction(tokens, parent.Parent, parent, out Function method))
				{
					method.Parameters.Add(new Variable("this", parent.GetReference(new TypeReference[0], 1)));
					methodBuffer.Add(method);
				}
				else
				{
					tokens.Restore(tokenState);
					return false;
				}

				tokens.ConsumeWhitespace();
			}

			fields = fieldBuffer.ToArray();
			methods = methodBuffer.ToArray();
			return true;
		}

		private static bool TryParseCodeBody(TokenStream tokens, Function owningFunction, CodeBody parentBody, out CodeBody result)
		{
			result = null;
			var tokenState = tokens.Save();

			tokens.ConsumeWhitespace();

			if (!tokens.TryReadExact("{"))
			{
				tokens.Restore(tokenState);
				return false;
			}

			tokens.ConsumeWhitespace();

			result = new CodeBody(owningFunction, parentBody);

			while (!tokens.TryReadExact("}"))
			{
				if (TryParseLocal(tokens, owningFunction.ParentNamespace, out InitializedVariable local))
				{
					result.Locals.Add(local);
				}
				else if (TryParseStatement(tokens, owningFunction, result, out Statement statement))
				{
					result.Statements.Add(statement);
				}
				else
				{
					tokens.Restore(tokenState);
					return false;
				}

				tokens.ConsumeWhitespace();
			}

			return true;
		}

		private static bool TryParseField(TokenStream tokens, Namespace parent, out Variable result)
		{
			result = null;
			var tokenState = tokens.Save();

			tokens.ConsumeWhitespace();

			if (!TryParseTypeReference(tokens, parent, out TypeReference type))
			{
				tokens.Restore(tokenState);
				return false;
			}

			tokens.ConsumeWhitespace();

			if (!tokens.TryReadIdentifier(out string name))
			{
				tokens.Restore(tokenState);
				return false;
			}

			tokens.ConsumeWhitespace();

			if (!tokens.TryReadExact(";"))
			{
				tokens.Restore(tokenState);
				return false;
			}

			result = new Variable(name, type);
			return true;
		}

		private static bool TryParseLocal(TokenStream tokens, Namespace parent, out InitializedVariable result)
		{
			result = null;
			var tokenState = tokens.Save();

			tokens.ConsumeWhitespace();

			if (!TryParseTypeReference(tokens, parent, out TypeReference type))
			{
				tokens.Restore(tokenState);
				return false;
			}

			tokens.ConsumeWhitespace();

			if (!tokens.TryReadIdentifier(out string name))
			{
				tokens.Restore(tokenState);
				return false;
			}

			tokens.ConsumeWhitespace();

			if (!tokens.TryReadExact("=") || !TryParseExpression(tokens, false, out Expression value))
			{
				tokens.Restore(tokenState);
				return false;
			}

			tokens.ConsumeWhitespace();

			if (!tokens.TryReadExact(";"))
			{
				tokens.Restore(tokenState);
				return false;
			}

			result = new InitializedVariable(name, type, value);
			return true;
		}

		private static bool TryParseStatement(TokenStream tokens, Function owningFunction, CodeBody parentBody, out Statement result)
		{
			if (TryParseEmptyStatement(tokens, parentBody, out result)) return true;
			else if (TryParseIfStatement(tokens, owningFunction, parentBody, out result)) return true;
			else if (TryParseWhileStatement(tokens, owningFunction, parentBody, out result)) return true;
			else if (TryParseForStatement(tokens, owningFunction, parentBody, out result)) return true;
			else if (TryParseTryStatement(tokens, owningFunction, parentBody, out result)) return true;
			else if (TryParseReturnStatement(tokens, parentBody, out result)) return true;
			else if (TryParseExpressionStatement(tokens, parentBody, out result)) return true;
			else return false;
		}

		private static bool TryParseEmptyStatement(TokenStream tokens, CodeBody parent, out Statement result)
		{
			result = null;
			var tokenState = tokens.Save();

			tokens.ConsumeWhitespace();

			if (tokens.TryReadExact(";"))
			{
				result = new EmptyStatement(parent);
				return true;
			}
			else
			{
				tokens.Restore(tokenState);
				return false;
			}
		}

		private static bool TryParseReturnStatement(TokenStream tokens, CodeBody parent, out Statement result)
		{
			result = null;
			var tokenState = tokens.Save();

			tokens.ConsumeWhitespace();

			if (!tokens.TryReadExact("return") || !TryParseExpression(tokens, false, out Expression expression))
			{
				tokens.Restore(tokenState);
				return false;
			}

			tokens.ConsumeWhitespace();

			if (!tokens.TryReadExact(";"))
			{
				tokens.Restore(tokenState);
				return false;
			}

			result = new ReturnStatement(parent, expression);
			return true;
		}

		private static bool TryParseIfStatement(TokenStream tokens, Function owningFunction, CodeBody parentBody, out Statement result)
		{
			result = null;
			var tokenState = tokens.Save();

			tokens.ConsumeWhitespace();

			if (!tokens.TryReadExact("if"))
			{
				tokens.Restore(tokenState);
				return false;
			}

			tokens.ConsumeWhitespace();

			if (!tokens.TryReadExact("(") || !TryParseExpression(tokens, false, out Expression condition))
			{
				tokens.Restore(tokenState);
				return false;
			}

			tokens.ConsumeWhitespace();

			if (!tokens.TryReadExact(")"))
			{
				tokens.Restore(tokenState);
				return false;
			}

			tokens.ConsumeWhitespace();

			if (!TryParseCodeBody(tokens, owningFunction, parentBody, out CodeBody ifBody))
			{
				tokens.Restore(tokenState);
				return false;
			}

			tokens.ConsumeWhitespace();

			CodeBody elseBody = new CodeBody(owningFunction, parentBody);

			if (tokens.TryReadExact("else") && (tokens.PeekChar() == '{' || tokens.ConsumeWhitespace()))
			{
				if (TryParseIfStatement(tokens, owningFunction, parentBody, out Statement nextIf))
				{
					elseBody.Statements.Add(nextIf);
				}
				else if (!TryParseCodeBody(tokens, owningFunction, parentBody, out elseBody))
				{
					tokens.Restore(tokenState);
					return false;
				}
			}

			result = new IfStatement(parentBody, condition, ifBody, elseBody);
			return true;
		}

		private static bool TryParseWhileStatement(TokenStream tokens, Function owningFunction, CodeBody parentBody, out Statement result)
		{
			result = null;
			var tokenState = tokens.Save();

			tokens.ConsumeWhitespace();

			if (!tokens.TryReadExact("while"))
			{
				tokens.Restore(tokenState);
				return false;
			}

			tokens.ConsumeWhitespace();

			if (!tokens.TryReadExact("(") || !TryParseExpression(tokens, false, out Expression condition))
			{
				tokens.Restore(tokenState);
				return false;
			}

			tokens.ConsumeWhitespace();

			if (!tokens.TryReadExact(")"))
			{
				tokens.Restore(tokenState);
				return false;
			}

			tokens.ConsumeWhitespace();

			if (!TryParseCodeBody(tokens, owningFunction, parentBody, out CodeBody body))
			{
				tokens.Restore(tokenState);
				return false;
			}

			result = new WhileStatement(parentBody, condition, body);
			return true;
		}

		private static bool TryParseForStatement(TokenStream tokens, Function owningFunction, CodeBody parentBody, out Statement result)
		{
			result = null;
			var tokenState = tokens.Save();

			tokens.ConsumeWhitespace();

			if (!tokens.TryReadExact("for"))
			{
				tokens.Restore(tokenState);
				return false;
			}

			tokens.ConsumeWhitespace();

			if (!tokens.TryReadExact("("))
			{
				tokens.Restore(tokenState);
				return false;
			}

			tokens.ConsumeWhitespace();

			InitializedVariable initializer;

			if (tokens.TryReadExact(";"))
			{
				initializer = null;
			}
			else if (!TryParseLocal(tokens, owningFunction.ParentNamespace, out initializer))
			{
				tokens.Restore(tokenState);
				return false;
			}

			tokens.ConsumeWhitespace();

			Expression condition;

			if (tokens.TryReadExact(";"))
			{
				condition = new Literal(0);
			}
			else if (!TryParseExpression(tokens, false, out condition))
			{
				tokens.Restore(tokenState);
				return false;
			}
			else
			{
				tokens.ConsumeWhitespace();

				if (!tokens.TryReadExact(";"))
				{
					tokens.Restore(tokenState);
					return false;
				}
			}

			if (!TryParseExpression(tokens, false, out Expression iteration))
			{
				tokens.Restore(tokenState);
				return false;
			}

			if (!tokens.TryReadExact(")"))
			{
				tokens.Restore(tokenState);
				return false;
			}

			tokens.ConsumeWhitespace();

			if (!TryParseCodeBody(tokens, owningFunction, parentBody, out CodeBody body))
			{
				tokens.Restore(tokenState);
				return false;
			}

			result = new ForStatement(parentBody, initializer, condition, iteration, body);
			return true;
		}

		private static bool TryParseTryStatement(TokenStream tokens, Function owningFunction, CodeBody parentBody, out Statement result)
		{
			result = null;
			var tokenState = tokens.Save();

			tokens.ConsumeWhitespace();

			if (!tokens.TryReadExact("try"))
			{
				tokens.Restore(tokenState);
				return false;
			}

			tokens.ConsumeWhitespace();

			if (!TryParseCodeBody(tokens, owningFunction, parentBody, out CodeBody tryBody))
			{
				tokens.Restore(tokenState);
				return false;
			}

			tokens.ConsumeWhitespace();

			if (!tokens.TryReadExact("catch"))
			{
				tokens.Restore(tokenState);
				return false;
			}

			tokens.ConsumeWhitespace();

			if (!TryParseCodeBody(tokens, owningFunction, parentBody, out CodeBody catchBody))
			{
				tokens.Restore(tokenState);
				return false;
			}

			result = new TryStatement(parentBody, tryBody, catchBody);
			return true;
		}

		private static bool TryParseExpressionStatement(TokenStream tokens, CodeBody parent, out Statement result)
		{
			result = null;
			var tokenState = tokens.Save();

			tokens.ConsumeWhitespace();

			if (!TryParseExpression(tokens, false, out Expression expression))
			{
				tokens.Restore(tokenState);
				return false;
			}

			tokens.ConsumeWhitespace();

			if (!tokens.TryReadExact(";"))
			{
				tokens.Restore(tokenState);
				return false;
			}

			result = new ExpressionStatement(parent, expression);
			return true;
		}

		private static bool TryParseExpression(TokenStream tokens, bool stopShort, out Expression result)
		{
			result = null;
			var tokenState = tokens.Save();

			tokens.ConsumeWhitespace();

			Expression left;

			if (tokens.TryReadExact("("))
			{
				if (!TryParseExpression(tokens, false, out left))
				{
					tokens.Restore(tokenState);
					return false;
				}

				tokens.ConsumeWhitespace();

				if (!tokens.TryReadExact(")"))
				{
					tokens.Restore(tokenState);
					return false;
				}
			}
			else if (TryParseLiteral(tokens, out Literal literal))
			{
				left = literal;
			}
			else if (tokens.TryReadExact("-") && TryParseExpression(tokens, true, out Expression negated))
			{
				left = new UnaryExpression("-", negated);
			}
			else if (tokens.TryReadExact("~") && TryParseExpression(tokens, true, out Expression inverted))
			{
				left = new UnaryExpression("~", inverted);
			}
			else if (tokens.TryReadExact("!") && TryParseExpression(tokens, true, out Expression notted))
			{
				left = new UnaryExpression("!", notted);
			}
			else if (tokens.TryReadExact("&") && TryParseExpression(tokens, true, out Expression referenced))
			{
				left = new UnaryExpression("&", referenced);
			}
			else if (TryParseDottedName(tokens, out string[] path))
			{
				left = new VariableReference(path);
			}
			else
			{
				tokens.Restore(tokenState);
				return false;
			}

			if (stopShort)
			{
				result = left;
				return true;
			}

			while (TryParseSubExpression(tokens, left, out result)) left = result;
			result = left;

			return true;
		}

		private static bool TryParseSubExpression(TokenStream tokens, Expression left, out Expression result)
		{
			result = null;
			var tokenState = tokens.Save();

			tokens.ConsumeWhitespace();

			if (tokens.TryReadExact("["))
			{
				tokens.ConsumeWhitespace();

				if (!TryParseExpressionList(tokens, out Expression[] args) || args.Length == 0)
				{
					tokens.Restore(tokenState);
					return false;
				}

				tokens.ConsumeWhitespace();

				if (!tokens.TryReadExact("]"))
				{
					tokens.Restore(tokenState);
					return false;
				}

				result = new IndexerExpression(left, args);
				return true;
			}
			else if (tokens.TryReadExact("("))
			{
				tokens.ConsumeWhitespace();

				if (!TryParseExpressionList(tokens, out Expression[] args))
				{
					tokens.Restore(tokenState);
					return false;
				}

				tokens.ConsumeWhitespace();

				if (!tokens.TryReadExact(")"))
				{
					tokens.Restore(tokenState);
					return false;
				}

				result = new CallExpression(left, args);
				return true;
			}
			else
			{
				var operators = new[] { "+=", "-=", "*=", "/=", "%=", "<<=", ">>=", "&&=", "||=", "&=", "|=", "^=", "+", "-", "*", "/", "%", "<<", ">>", "&&", "||", "!=", "==", "<=", ">=", "<", ">", "=", "&", "|", "^" };

				for (var i = 0; i < operators.Length; i++)
				{
					if (tokens.TryReadExact(operators[i]))
					{
						tokens.ConsumeWhitespace();

						if (TryParseExpression(tokens, false, out Expression right))
						{
							result = new BinaryExpression(left, operators[i], right);
							return true;
						}
						else
						{
							tokens.Restore(tokenState);
							return false;
						}
					}
				}

				tokens.Restore(tokenState);
				return false;
			}
		}

		private static bool TryParseExpressionList(TokenStream tokens, out Expression[] result)
		{
			result = null;
			var tokenState = tokens.Save();
			var expressions = new List<Expression>();

			tokens.ConsumeWhitespace();

			if (!TryParseExpression(tokens, false, out Expression expression))
			{
				result = expressions.ToArray();
				return true;
			}

			expressions.Add(expression);

			tokens.ConsumeWhitespace();

			if (tokens.PeekChar() != ',')
			{
				result = expressions.ToArray();
				return true;
			}

			while (tokens.TryReadExact(","))
			{
				tokens.ConsumeWhitespace();

				if (!TryParseExpression(tokens, false, out expression))
				{
					tokens.Restore(tokenState);
					return false;
				}

				expressions.Add(expression);

				tokens.ConsumeWhitespace();
			}

			result = expressions.ToArray();
			return true;
		}

		private static bool TryParseLiteral(TokenStream tokens, out Literal result)
		{
			result = null;
			var tokenState = tokens.Save();

			tokens.ConsumeWhitespace();

			if (TryParseString(tokens, out string strValue))
			{
				result = new Literal(strValue);
				return true;
			}
			else if (tokens.TryReadNumber(out string number))
			{
				if (number.Contains(".") && TryParseFloat(number, out result)) return true;
				else if (number.StartsWith("0x") && TryParseHexadecimalInt(number.Substring(2), out result)) return true;
				else if (number.StartsWith("0b") && TryParseBinaryInt(number.Substring(2), out result)) return true;
				else if (TryParseDecimalInt(number, out result)) return true;
				else
				{
					tokens.Restore(tokenState);
					return false;
				}
			}
			else
			{
				tokens.Restore(tokenState);
				return false;
			}
		}

		private static bool TryParseString(TokenStream tokens, out string value)
		{
			value = null;
			var tokenState = tokens.Save();
			var buffer = new List<char>();

			tokens.ConsumeWhitespace();

			if (!tokens.TryReadExact("\""))
			{
				tokens.Restore(tokenState);
				return false;
			}

			var isEscaped = false;
			while (isEscaped || !tokens.TryReadExact("\""))
			{
				var c = tokens.ReadChar();
				var wasEscaped = isEscaped;
				isEscaped = false;

				if (wasEscaped)
				{
					switch (c)
					{
						case 'a':
							buffer.Add('\a');
							break;
						case 'b':
							buffer.Add('\b');
							break;
						case 'f':
							buffer.Add('\f');
							break;
						case 'n':
							buffer.Add('\n');
							break;
						case 'r':
							buffer.Add('\r');
							break;
						case 't':
							buffer.Add('\t');
							break;
						case 'v':
							buffer.Add('\v');
							break;
						case '0':
							buffer.Add('\0');
							break;
						default:
							buffer.Add(c);
							break;
					}
				}
				else
				{
					if (c == '\\')
					{
						isEscaped = true;
					}
					else if (c == '"')
					{
						break;
					}
					else if (c == '\0')
					{
						tokens.Restore(tokenState);
						return false;
					}
					else
					{
						buffer.Add(c);
					}
				}
			}

			value = new string(buffer.ToArray());
			return true;
		}

		private static bool TryParseFloat(string number, out Literal result)
		{
			if (float.TryParse(number, out float value32))
			{
				result = new Literal(value32);
				return true;
			}
			else if (double.TryParse(number, out double value64))
			{
				result = new Literal(value64);
				return true;
			}
			else
			{
				result = null;
				return false;
			}
		}

		private static bool TryParseDecimalInt(string number, out Literal result)
		{
			result = null;
			ulong value = 0;

			for (var i = 0; i < number.Length; i++)
			{
				var c = number[i];

				if (c >= '0' && c <= '9')
				{
					try
					{
						checked
						{
							value *= 10;
							value += (ulong)(c - '0');
						}
					}
					catch
					{
						return false;
					}
				}
				else if ((c == 'U' || c == 'u') && (i == (number.Length - 1)))
				{
					result = new Literal(value, true);
					return true;
				}
				else
				{
					return false;
				}
			}

			result = new Literal(value, false);
			return true;
		}

		private static bool TryParseBinaryInt(string number, out Literal result)
		{
			result = null;
			ulong value = 0;

			for (var i = 0; i < number.Length; i++)
			{
				var c = number[i];

				if (c >= '0' && c <= '1')
				{
					try
					{
						checked
						{
							value <<= 1;
							value += (ulong)(c - '0');
						}
					}
					catch
					{
						return false;
					}
				}
				else if ((c == 'U' || c == 'u') && (i == (number.Length - 1)))
				{
					result = new Literal(value, true);
					return true;
				}
				else
				{
					return false;
				}
			}

			result = new Literal(value, false);
			return true;
		}

		private static bool TryParseHexadecimalInt(string number, out Literal result)
		{
			result = null;
			ulong value = 0;

			for (var i = 0; i < number.Length; i++)
			{
				var c = number[i];

				if (c >= '0' && c <= '9')
				{
					try
					{
						checked
						{
							value <<= 4;
							value += (ulong)(c - '0');
						}
					}
					catch
					{
						return false;
					}
				}
				else if (c >= 'A' && c <= 'F')
				{
					try
					{
						checked
						{
							value <<= 4;
							value += (ulong)((c - 'A') + 10);
						}
					}
					catch
					{
						return false;
					}
				}
				else if (c >= 'a' && c <= 'f')
				{
					try
					{
						checked
						{
							value <<= 4;
							value += (ulong)((c - 'a') + 10);
						}
					}
					catch
					{
						return false;
					}
				}
				else if ((c == 'U' || c == 'u') && (i == (number.Length - 1)))
				{
					result = new Literal(value, true);
					return true;
				}
				else
				{
					return false;
				}
			}

			result = new Literal(value, false);
			return true;
		}

		private static bool TryParseDottedName(TokenStream tokens, out string[] result)
		{
			result = null;
			var tokenState = tokens.Save();

			tokens.ConsumeWhitespace();

			if (!tokens.TryReadIdentifier(out string name))
			{
				tokens.Restore(tokenState);
				return false;
			}

			tokens.ConsumeWhitespace();

			var path = new List<string> { name };

			while (tokens.TryReadExact("."))
			{
				tokens.ConsumeWhitespace();

				if (!tokens.TryReadIdentifier(out name))
				{
					tokens.Restore(tokenState);
					return false;
				}

				path.Add(name);

				tokens.ConsumeWhitespace();
			}

			result = path.ToArray();
			return true;
		}
	}
}
