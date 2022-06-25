using System;
using System.IO;
using System.Text;

namespace TestBed
{
	public class ConsoleStream : Stream
	{
		public override bool CanRead => true;
		public override bool CanSeek => false;
		public override bool CanWrite => true;
		public override long Length => throw new NotSupportedException();
		public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

		public override void Flush()
		{
			Console.Out.Flush();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			var charBuffer = new char[count];
			var result = Console.In.Read(charBuffer, 0, count);
			for (var i = 0; i < result; i++) buffer[offset + i] = (byte)charBuffer[i];
			return result;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			Console.Out.Write(Encoding.UTF8.GetString(buffer, offset, count));
		}
	}
}
