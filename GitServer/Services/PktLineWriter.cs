using System;
using System.IO;
using System.Text;

namespace GitServer.Services
{
	public class PktLineWriter : StreamWriter
	{
		bool _leaveOpen;
		StringBuilder _currLine = new StringBuilder();

		public PktLineWriter(Stream stream) : base(stream) { _leaveOpen = false; }

		public PktLineWriter(Stream stream, Encoding encoding) : base(stream, encoding) { _leaveOpen = false; }

		public PktLineWriter(Stream stream, Encoding encoding, int bufferSize) : base(stream, encoding, bufferSize) { _leaveOpen = false; }

		public PktLineWriter(Stream stream, Encoding encoding, int bufferSize, bool leaveOpen) : base(stream, encoding, bufferSize, leaveOpen) { _leaveOpen = leaveOpen; }

		protected override void Dispose(bool disposing)
		{
			if(!_leaveOpen)
				base.Write("0000");

			base.Flush();
			base.Dispose(disposing);
		}

		public override void Write(bool value) => _currLine.Append(value);

		public override void Write(char value)
		{
			if(value == '\n')
				WriteLine();
			else
				_currLine.Append(value);
		}

		public override void Write(char[] buffer)
		{
			foreach(char c in buffer)
				Write(c);
		}

		public override void Write(char[] buffer, int index, int count)
		{
			for (int i = index; i < count; i++)
				Write(buffer[i]);
		}

		public override void Write(decimal value) => _currLine.Append(value);

		public override void Write(double value) => _currLine.Append(value);

		public override void Write(float value) => _currLine.Append(value);

		public override void Write(int value) => _currLine.Append(value);

		public override void Write(long value) => _currLine.Append(value);

		public override void Write(object value) => _currLine.Append(value);

		public override void Write(string format, object arg0) => Write(String.Format(format, arg0));

		public override void Write(string format, object arg0, object arg1) => Write(String.Format(format, arg0, arg1));

		public override void Write(string format, object arg0, object arg1, object arg2) => Write(String.Format(format, arg0, arg1, arg2));

		public override void Write(string format, params object[] arg) => Write(String.Format(format, arg));

		public override void Write(string value)
		{
			foreach (char c in value)
				Write(c);
		}

		public override void Write(uint value) => _currLine.Append(value);

		public override void Write(ulong value) => _currLine.Append(value);

		public override void WriteLine()
		{
			base.Write($"{_currLine.Length + 5:x4}{_currLine}\n");
			base.Flush();
			_currLine.Clear();
		}

		public override void WriteLine(bool value)
		{
			Write(value);
			WriteLine();
		}

		public override void WriteLine(char value)
		{
			Write(value);
			WriteLine();
		}

		public override void WriteLine(char[] buffer)
		{
			Write(buffer);
			WriteLine();
		}

		public override void WriteLine(char[] buffer, int index, int count)
		{
			Write(buffer, index, count);
			WriteLine();
		}

		public override void WriteLine(decimal value)
		{
			Write(value);
			WriteLine();
		}

		public override void WriteLine(double value)
		{
			Write(value);
			WriteLine();
		}

		public override void WriteLine(float value)
		{
			Write(value);
			WriteLine();
		}

		public override void WriteLine(int value)
		{
			Write(value);
			WriteLine();
		}

		public override void WriteLine(long value)
		{
			Write(value);
			WriteLine();
		}

		public override void WriteLine(string format, object arg0)
		{
			Write(format, arg0);
			WriteLine();
		}

		public override void WriteLine(string format, object arg0, object arg1)
		{
			Write(format, arg0, arg1);
			WriteLine();
		}

		public override void WriteLine(string format, object arg0, object arg1, object arg2)
		{
			Write(format, arg0, arg1, arg2);
			WriteLine();
		}

		public override void WriteLine(string format, params object[] arg)
		{
			Write(format, arg);
			WriteLine();
		}

		public override void WriteLine(string value)
		{
			Write(value);
			WriteLine();
		}

		public override void WriteLine(uint value)
		{
			Write(value);
			WriteLine();
		}

		public override void WriteLine(ulong value)
		{
			Write(value);
			WriteLine();
		}
	}
}
