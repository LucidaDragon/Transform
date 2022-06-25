using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Transform.Emitters
{
	public class TIL : IEmitter
	{
		private class Label : ILabel
		{
			public ulong Id { get; }
			public string Name { get; }
			public object Key { get; }

			public Label(ulong id, string name, object key)
			{
				Id = id;
				Name = name;
				Key = key;
			}
		}

		private class Context : IEmitterContext
		{
			public ulong MemoryCharSize => 1;
			public ulong RegisterWordSize => 8;
			public ulong AddressWordSize => 8;
			public ulong IEEEFloat32Size => 4;
			public ulong IEEEFloat64Size => 8;

			public ulong SizeOfBits(ulong bits)
			{
				if (bits == 0) return 0;
				var result = bits / 8;
				if (result == 0) return 1;
				else return result;
			}
		}

		private readonly Dictionary<object, Label> Labels = new Dictionary<object, Label>();
		private readonly Queue<Action> ActionQueue = new Queue<Action>();
		private readonly List<string> Log = new List<string>();
		private ulong NextId = 0;

		public IEmitterContext GetContext()
		{
			return new Context();
		}

		public ILabel CreateLabel(string name, object key)
		{
			var result = new Label(NextId++, name, key);
			if (key != null) Labels.Add(key, result);
			return result;
		}

		public ILabel NullLabel() => null;

		public bool TryGetLabelByKey(object key, out ILabel result)
		{
			result = null;
			if (key == null) return false;
			var success = Labels.TryGetValue(key, out Label label);
			result = label;
			return success;
		}

		public void QueueAction(Action action)
		{
			ActionQueue.Enqueue(action);
		}

		public void WriteOutput(int index, Stream stream, ref string name)
		{
			if (index != 0) throw new IndexOutOfRangeException();

			using (var writer = new StreamWriter(stream, Encoding.ASCII, 512, true))
			{
				for (var i = 0; i < Log.Count; i++) writer.WriteLine(Log[i]);
			}
		}

		private void WriteLog(params object[] values)
		{
			var frame = new StackTrace().GetFrame(1);
			var valueStrings = new string[values.Length];
			for (var i = 0; i < values.Length; i++) valueStrings[i] = ObjectString(values[i]);
			Log.Add($"{frame.GetMethod().Name} {string.Join(", ", valueStrings)}");
		}

		private static string ObjectString(object obj)
		{
			if (obj == null) return $"<null>";
			else if (obj is string str) return $"\"{str.Replace("\\", "\\\\").Replace("\"", "\\\"")}\"";
			else if (obj is IEnumerable e) return $"{{{string.Join(", ", e.Cast<object>().Select(o => ObjectString(o)))}}}";
			else if (obj is Label label) return $"Label(0x{label.Id:X8}, {ObjectString(label.Name)}, {ObjectString(label.Key)})";
			else return obj.ToString();
		}

		public void Add() => WriteLog();

		public void AddF() => WriteLog();

		public void BeginData(ILabel label) => WriteLog(label);

		public void BeginFunction(ILabel label, ulong returnType, ulong[] parameters, ILocal[] locals) => WriteLog(label, returnType, parameters, locals);

		public void BitwiseAnd() => WriteLog();

		public void BitwiseNot() => WriteLog();

		public void BitwiseOr() => WriteLog();

		public void BitwiseXor() => WriteLog();

		public void BooleanAnd() => WriteLog();

		public void BooleanNot() => WriteLog();

		public void BooleanOr() => WriteLog();

		public void BooleanXor() => WriteLog();

		public void BranchIf(ILabel label) => WriteLog(label);

		public void BranchIfNot(ILabel label) => WriteLog(label);

		public void Call() => WriteLog();

		public void DivideF() => WriteLog();

		public void DivideI() => WriteLog();

		public void DivideU() => WriteLog();

		public void EndData()
		{
			WriteLog();
			while (ActionQueue.Count > 0) ActionQueue.Dequeue()();
		}

		public void EndFunction()
		{
			WriteLog();
			while (ActionQueue.Count > 0) ActionQueue.Dequeue()();
		}

		public void EnterTry(ILabel handler) => WriteLog(handler);

		public void ExitTry() => WriteLog();

		public void ExternalFunction(ILabel label, string name, ulong returnType, ulong[] parameters) => WriteLog(label, name, returnType, parameters);

		public int GetOutputCount() => 1;

		public void IsEqual() => WriteLog();

		public void IsEqualF() => WriteLog();

		public void IsGreaterThanF() => WriteLog();

		public void IsGreaterThanI() => WriteLog();

		public void IsGreaterThanOrEqualF() => WriteLog();

		public void IsGreaterThanOrEqualI() => WriteLog();

		public void IsGreaterThanOrEqualU() => WriteLog();

		public void IsGreaterThanU() => WriteLog();

		public void IsLessThanF() => WriteLog();

		public void IsLessThanI() => WriteLog();

		public void IsLessThanOrEqualF() => WriteLog();

		public void IsLessThanOrEqualI() => WriteLog();

		public void IsLessThanOrEqualU() => WriteLog();

		public void IsLessThanU() => WriteLog();

		public void IsNotEqual() => WriteLog();

		public void IsNotEqualF() => WriteLog();

		public void Jump(ILabel label) => WriteLog(label);

		public void Load() => WriteLog();

		public void MarkLabel(ILabel label) => WriteLog(label);

		public void Multiply() => WriteLog();

		public void MultiplyF() => WriteLog();

		public void Negate() => WriteLog();

		public void Pop() => WriteLog();

		public void Push(bool value) => WriteLog(value);

		public void Push(sbyte value) => WriteLog(value);

		public void Push(byte value) => WriteLog(value);

		public void Push(short value) => WriteLog(value);

		public void Push(ushort value) => WriteLog(value);

		public void Push(int value) => WriteLog(value);

		public void Push(uint value) => WriteLog(value);

		public void Push(long value) => WriteLog(value);

		public void Push(ulong value) => WriteLog(value);

		public void Push(float value) => WriteLog(value);

		public void Push(double value) => WriteLog(value);

		public void PushAddress(ILabel label) => WriteLog(label);

		public void PushArgumentAddress(int index) => WriteLog(index);

		public void PushLocalAddress(int index) => WriteLog(index);

		public void PushNullAddress() => PushAddress(null);

		public void PushStackAddress() => WriteLog();

		public void PushUnassigned(ulong size) => WriteLog(size);

		public void RemainderI() => WriteLog();

		public void RemainderU() => WriteLog();

		public void Return() => WriteLog();

		public void Section(int index) => WriteLog(index);

		public void SetParameter(string name, string value) => WriteLog(name, value);

		public void ShiftLeft() => WriteLog();

		public void ShiftRight() => WriteLog();

		public void Store() => WriteLog();

		public void Subtract() => WriteLog();

		public void SubtractF() => WriteLog();

		public void Throw() => WriteLog();
	}
}
