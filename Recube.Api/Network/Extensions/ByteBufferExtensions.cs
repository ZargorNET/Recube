using System;
using System.Collections.Generic;
using System.Text;
using DotNetty.Buffers;

namespace Recube.Api.Network.Extensions
{
	public static class ByteBufferExtensions
	{
		public static void WriteVarInt(this IByteBuffer buffer, int number)
		{
			VarInt.WriteVarInt(number, out var bytes);
			buffer.WriteBytes(bytes);
		}

		public static void WriteVarLong(this IByteBuffer buffer, long number)
		{
			VarInt.WriteVarLong(number, out var bytes);
			buffer.WriteBytes(bytes);
		}

		public static int ReadVarInt(this IByteBuffer buffer)
		{
			VarInt.ReadVarInt(buffer, out var varInt);
			if (!varInt.HasValue) throw new InvalidOperationException("Could not parse VarInt");
			return varInt.Value;
		}

		public static long ReadVarLong(this IByteBuffer buffer)
		{
			VarInt.ReadVarLong(buffer, out var varLong);
			if (!varLong.HasValue) throw new InvalidOperationException("Could not parse VarLong");
			return varLong.Value;
		}

		public static void WriteStringWithLength(this IByteBuffer buffer, string s)
		{
			if (s.Length > 32767) throw new InvalidOperationException("String is longer than 32767 characters");

			buffer.WriteVarInt(s.Length);
			buffer.WriteString(s, Encoding.UTF8);
		}

		public static string ReadStringWithLength(this IByteBuffer buffer)
		{
			var length = buffer.ReadVarInt();
			return buffer.ReadString(length, Encoding.UTF8);
		}

		public static void WriteULongArray(this IByteBuffer buffer, ulong[] data)
		{
			foreach (var value in data)
			{
				buffer.WriteLong((long) value);
			}
		}

		public static void WriteLongArray(this IByteBuffer buffer, IEnumerable<long> data)
		{
			foreach (var value in data)
			{
				buffer.WriteLong(value);
			}
		}

		public static void WriteIntArray(this IByteBuffer buffer, IEnumerable<int> data)
		{
			foreach (var value in data)
			{
				buffer.WriteInt(value);
			}
		}

		public static void WriteVarIntArray(this IByteBuffer buffer, IEnumerable<int> data)
		{
			foreach (var value in data)
			{
				buffer.WriteVarInt(value);
			}
		}
	}
}