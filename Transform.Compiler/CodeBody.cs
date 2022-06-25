using System;
using System.Collections.Generic;

namespace Transform.Compiler
{
	public class CodeBody
	{
		public bool IsExternal { get; set; }
		public Function OwningFunction { get; set; }
		public CodeBody ParentBody { get; set; }

		public List<InitializedVariable> Locals { get; } = new List<InitializedVariable>();
		public List<Statement> Statements { get; } = new List<Statement>();

		public CodeBody(Function owningFunction, CodeBody parentBody, bool isExternal = false)
		{
			OwningFunction = owningFunction;
			ParentBody = parentBody;
			IsExternal = isExternal;
		}

		public InitializedVariable[] GetLocals()
		{
			var buffer = new List<InitializedVariable>();
			buffer.AddRange(Locals);
			for (var i = 0; i < Statements.Count; i++) buffer.AddRange(Statements[i].GetLocals());
			return buffer.ToArray();
		}

		public int GetLocalIndex(InitializedVariable local)
		{
			if (ParentBody == null)
			{
				var locals = GetLocals();
				return Array.IndexOf(locals, local);
			}
			else
			{
				return ParentBody.GetLocalIndex(local);
			}
		}
	}
}
