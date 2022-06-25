namespace Transform
{
	public interface IEmitterContext
	{
		ulong MemoryCharSize { get; }
		ulong RegisterWordSize { get; }
		ulong AddressWordSize { get; }
		ulong IEEEFloat32Size { get; }
		ulong IEEEFloat64Size { get; }

		ulong SizeOfBits(ulong bits);
	}
}
