namespace Transform
{
	public interface ILocal
	{
		ulong Size { get; }
		void PushInitialValue(IEmitter emit);
	}
}
