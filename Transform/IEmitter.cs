using System;
using System.IO;

namespace Transform
{
	public interface IEmitter
	{
		IEmitterContext GetContext();

		ILabel CreateLabel(string name, object key);
		ILabel NullLabel();
		bool TryGetLabelByKey(object key, out ILabel result);
		void MarkLabel(ILabel label);

		void Section(int index);

		void BeginData(ILabel label);
		void EndData();

		void BeginFunction(ILabel label, ulong returnType, ulong[] parameters, ILocal[] locals);
		void EndFunction();

		void ExternalFunction(ILabel label, string name, ulong returnType, ulong[] parameters);

		void QueueAction(Action action);

		void PushUnassigned(ulong size);
		void Push(bool value);
		void Push(sbyte value);
		void Push(byte value);
		void Push(short value);
		void Push(ushort value);
		void Push(int value);
		void Push(uint value);
		void Push(long value);
		void Push(ulong value);
		void Push(float value);
		void Push(double value);
		void PushAddress(ILabel label);
		void PushNullAddress();
		void PushStackAddress();
		void PushArgumentAddress(int index);
		void PushLocalAddress(int index);
		void Load();
		void Store();
		void Pop();

		void Add();
		void AddF();
		void Subtract();
		void SubtractF();
		void Multiply();
		void MultiplyF();
		void DivideI();
		void DivideU();
		void DivideF();
		void RemainderI();
		void RemainderU();
		void ShiftLeft();
		void ShiftRight();
		void BitwiseAnd();
		void BitwiseOr();
		void BitwiseXor();
		void BitwiseNot();
		void Negate();

		void BooleanAnd();
		void BooleanOr();
		void BooleanXor();
		void BooleanNot();
		void IsEqual();
		void IsEqualF();
		void IsNotEqual();
		void IsNotEqualF();
		void IsLessThanI();
		void IsLessThanU();
		void IsLessThanF();
		void IsGreaterThanI();
		void IsGreaterThanU();
		void IsGreaterThanF();
		void IsLessThanOrEqualI();
		void IsLessThanOrEqualU();
		void IsLessThanOrEqualF();
		void IsGreaterThanOrEqualI();
		void IsGreaterThanOrEqualU();
		void IsGreaterThanOrEqualF();

		void Jump(ILabel label);
		void BranchIf(ILabel label);
		void BranchIfNot(ILabel label);
		void Call();
		void Return();

		void EnterTry(ILabel handler);
		void ExitTry();
		void Throw();

		void SetParameter(string name, string value);
		int GetOutputCount();
		void WriteOutput(int index, Stream stream, ref string name);
	}
}
