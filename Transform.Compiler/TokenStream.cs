using System;
using System.IO;

namespace Transform.Compiler
{
	public class TokenStream
	{
		public bool EOF => PeekChar() == '\0';

		private readonly Stream Source;

		public TokenStream(Stream source)
		{
			if (!source.CanSeek) throw new ArgumentException("Stream must support seeking.", nameof(source));
			Source = source;
		}

		public object Save()
		{
			return Source.Position;
		}

		public void Restore(object save)
		{
			Source.Position = (long)save;
		}

		public char ReadChar()
		{
			var c = Source.ReadByte();
			var additionalLength = 0;

			if (c < 0)
			{
				return '\0';
			}
			else if ((c & 0b11100000) == 0b11000000)
			{
				additionalLength = 1;
				c &= 0b00011111;
			}
			else if ((c & 0b11110000) == 0b11100000)
			{
				additionalLength = 2;
				c &= 0b00001111;
			}
			else if ((c & 0b11111000) == 0b11110000)
			{
				additionalLength = 3;
				c &= 0b00000111;
			}

			for (var i = 0; i < additionalLength; i++)
			{
				var newC = Source.ReadByte();
				if (newC < 0) return '\0';

				c <<= 6;
				c |= newC & 0b00111111;
			}

			return (char)c;
		}

		public char PeekChar()
		{
			var position = Source.Position;
			var result = ReadChar();
			Source.Position = position;
			return result;
		}

		public bool ConsumeWhitespace()
		{
			var c = PeekChar();

			if (!char.IsWhiteSpace(c)) return false;

			while (char.IsWhiteSpace(c))
			{
				ReadChar();
				c = PeekChar();
			}

			return true;
		}

		public string ReadChars(int count)
		{
			var buffer = new char[count];
			for (var i = 0; i < count; i++) buffer[i] = ReadChar();
			return new string(buffer);
		}

		public string PeekChars(int count)
		{
			var position = Source.Position;
			var result = ReadChars(count);
			Source.Position = position;
			return result;
		}

		public bool TryReadExact(string exact)
		{
			var buffer = PeekChars(exact.Length);
			if (buffer != exact) return false;
			ReadChars(exact.Length);
			return true;
		}

		public bool TryReadIdentifier(out string value)
		{
			var c = PeekChar();
			value = "";

			if (!IsIdentifierInitialChar(c)) return false;

			while (IsIdentifierChar(c))
			{
				value += ReadChar();
				c = PeekChar();
			}

			return true;
		}

		public bool TryReadNumber(out string value)
		{
			var c = PeekChar();
			value = "";

			if (!IsNumericInitialChar(c)) return false;

			while (IsNumericChar(c))
			{
				value += ReadChar();
				c = PeekChar();
			}

			return true;
		}

		public bool TryReadOperator(out string value)
		{
			var c = PeekChar();
			value = "";

			if (!IsOperatorChar(c)) return false;

			while (IsOperatorChar(c))
			{
				value += ReadChar();
				c = PeekChar();
			}

			return true;
		}

		public static bool IsIdentifierInitialChar(char c)
		{
			return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_';
		}

		public static bool IsIdentifierChar(char c)
		{
			return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || c == '_';
		}

		public static bool IsNumericInitialChar(char c)
		{
			return c >= '0' && c <= '9';
		}

		public static bool IsNumericChar(char c)
		{
			return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || c == '.';
		}

		public static bool IsOperatorChar(char c)
		{
			switch (c)
			{
				case '~':
				case '!':
				case '%':
				case '^':
				case '&':
				case '*':
				case '-':
				case '+':
				case '=':
				case '|':
				case '<':
				case '>':
				case '.':
				case '?':
				case '/':
					return true;
				default:
					return false;
			}
		}
	}
}
