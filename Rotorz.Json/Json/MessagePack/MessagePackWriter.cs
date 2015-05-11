// Copyright (c) Rotorz Limited. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Rotorz.Json.MessagePack {

	// https://github.com/msgpack/msgpack/blob/master/spec.md
	// 2fb4eaa9688888b74bdabb2222f0e0f42712b6b1
	internal sealed class MessagePackWriter : IJsonWriter {

		#region Factory Methods

		/// <summary>
		/// Create new <see cref="MessagePackWriter"/> instance and write content to the
		/// provided <see cref="Stream"/> instance with custom settings.
		/// </summary>
		/// <param name="stream">Stream that data will be written to.</param>
		/// <returns>
		/// New <see cref="MessagePackWriter"/> instance.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">
		/// If <paramref name="stream"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// If <paramref name="stream"/> is not writable.
		/// </exception>
		public static MessagePackWriter Create(Stream stream) {
            return new MessagePackWriter(stream);
		}

		#endregion

		private Stream _mpacStream;

		#region Low Level Writer

		private void WriteFormatCode(MessagePackFormatCode formatCode) {
			WriteByte((byte)formatCode);
		}

		private void WriteByte(byte value) {
			_mpacStream.WriteByte(value);
		}

		private void WriteUInt16(ushort value) {
			var bytes = BitConverter.GetBytes(value);
			Array.Reverse(bytes);
			_mpacStream.Write(bytes, 0, bytes.Length);
        }

		private void WriteUInt32(uint value) {
			var bytes = BitConverter.GetBytes(value);
			Array.Reverse(bytes);
			_mpacStream.Write(bytes, 0, bytes.Length);
		}

		private void WriteUInt64(ulong value) {
			var bytes = BitConverter.GetBytes(value);
			Array.Reverse(bytes);
			_mpacStream.Write(bytes, 0, bytes.Length);
		}

		private void WriteInt16(short value) {
			var bytes = BitConverter.GetBytes(value);
			Array.Reverse(bytes);
			_mpacStream.Write(bytes, 0, bytes.Length);
		}

		private void WriteInt32(int value) {
			var bytes = BitConverter.GetBytes(value);
			Array.Reverse(bytes);
			_mpacStream.Write(bytes, 0, bytes.Length);
		}

		private void WriteInt64(long value) {
			var bytes = BitConverter.GetBytes(value);
			Array.Reverse(bytes);
			_mpacStream.Write(bytes, 0, bytes.Length);
		}

		private void WriteStringBytes(string value) {
			var bytes = Encoding.UTF8.GetBytes(value);
			_mpacStream.Write(bytes, 0, bytes.Length);
		}

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="MessagePackWriter"/> class.
		/// </summary>
		/// <param name="stream">Stream that data will be written to.</param>
		private MessagePackWriter(Stream stream) {
			if (stream == null)
				throw new ArgumentNullException("stream");
			if (!stream.CanWrite)
				throw new ArgumentException("Not writable.", "stream");

			_mpacStream = stream;
        }
		
		/// <inheritdoc/>
		public void WriteObject(IDictionary<string, JsonNode> collection) {
			int length = collection.Count;

			if (length <= 0x0F) {
				long b = (long)MessagePackFormatCode.FixMap | length;
				WriteByte((byte)b);
			}
			else if (length <= ushort.MaxValue) {
				WriteFormatCode(MessagePackFormatCode.Map16);
				WriteUInt16((ushort)length);
			}
			else {
				WriteFormatCode(MessagePackFormatCode.Map32);
				WriteInt32(length);
			}

			foreach (var entry in collection) {
				WriteString(entry.Key);
				if (entry.Value != null)
					entry.Value.WriteTo(this);
				else
					WriteNull();
			}
		}

		/// <inheritdoc/>
		public void WriteArray(IList<JsonNode> collection) {
			int length = collection.Count;

			if (length <= 0x0F) {
				long b = (long)MessagePackFormatCode.FixArray | length;
				WriteByte((byte)b);
			}
			else if (length <= ushort.MaxValue) {
				WriteFormatCode(MessagePackFormatCode.Array16);
				WriteUInt16((ushort)length);
			}
			else {
				WriteFormatCode(MessagePackFormatCode.Array32);
				WriteInt32(length);
			}

			for (int i = 0; i < length; ++i) {
				var value = collection[i];
				if (value != null)
					value.WriteTo(this);
				else
					WriteNull();
			}
		}

		/// <inheritdoc/>
		public void WriteNull() {
			WriteFormatCode(MessagePackFormatCode.Nil);
		}

		/// <inheritdoc/>
		public void WriteInteger(long value) {
			if (value >= 0) {
				if (value <= 0x7F) {
					long b = (long)MessagePackFormatCode.PositiveFixInt | value;
					WriteByte((byte)b);
				}
				else if (value <= byte.MaxValue) {
					WriteFormatCode(MessagePackFormatCode.UInt8);
					WriteByte((byte)value);
				}
				else if (value <= ushort.MaxValue) {
					WriteFormatCode(MessagePackFormatCode.UInt16);
					WriteUInt16((ushort)value);
				}
				else if (value <= uint.MaxValue) {
					WriteFormatCode(MessagePackFormatCode.UInt32);
					WriteUInt32((uint)value);
				}
				else {
					WriteFormatCode(MessagePackFormatCode.UInt64);
					WriteUInt64((ulong)value);
				}
			}
			else {
				if (-value <= 0x1F && value != long.MinValue) {
					long b = (long)MessagePackFormatCode.NegativeFixInt | -value;
					WriteByte((byte)b);
				}
				else if (value >= sbyte.MinValue) {
					WriteFormatCode(MessagePackFormatCode.Int8);
					WriteByte((byte)(sbyte)value);
				}
				else if (value >= short.MinValue) {
					WriteFormatCode(MessagePackFormatCode.Int16);
					WriteInt16((short)value);
				}
				else if (value >= int.MinValue) {
					WriteFormatCode(MessagePackFormatCode.Int32);
					WriteInt32((int)value);
				}
				else {
					WriteFormatCode(MessagePackFormatCode.Int64);
					WriteInt64(value);
				}
			}
		}

		/// <inheritdoc/>
		public void WriteDouble(double value) {
			byte[] bytes;

			if (value >= float.MinValue && value <= float.MaxValue) {
				WriteFormatCode(MessagePackFormatCode.Float32);
				bytes = BitConverter.GetBytes((float)value);
			}
			else {
				WriteFormatCode(MessagePackFormatCode.Float64);
				bytes = BitConverter.GetBytes(value);
			}

			Array.Reverse(bytes);
			_mpacStream.Write(bytes, 0, bytes.Length);
		}

		/// <inheritdoc/>
		public void WriteString(string value) {
			if (value == null)
				value = "";

			if (value.Length <= 0x1F) {
				long b = (long)MessagePackFormatCode.FixStr | value.Length;
				WriteByte((byte)b);
			}
			else if (value.Length <= byte.MaxValue) {
				WriteFormatCode(MessagePackFormatCode.Str8);
				WriteByte((byte)value.Length);
			}
			else if (value.Length <= ushort.MaxValue) {
				WriteFormatCode(MessagePackFormatCode.Str16);
				WriteUInt16((ushort)value.Length);
			}
			else {
				WriteFormatCode(MessagePackFormatCode.Str32);
				WriteInt32(value.Length);
			}

			WriteStringBytes(value);
		}

		/// <inheritdoc/>
		public void WriteBoolean(bool value) {
			WriteFormatCode(value ? MessagePackFormatCode.True : MessagePackFormatCode.False);
		}

		/// <inheritdoc/>
		public void WriteBinary(byte[] value) {
			if (value == null)
				throw new ArgumentNullException("value");

			if (value.Length <= byte.MaxValue) {
				WriteFormatCode(MessagePackFormatCode.Bin8);
				WriteByte((byte)value.Length);
			}
			else if (value.Length <= ushort.MaxValue) {
				WriteFormatCode(MessagePackFormatCode.Bin16);
				WriteUInt16((ushort)value.Length);
			}
			else {
				WriteFormatCode(MessagePackFormatCode.Bin32);
				WriteInt32(value.Length);
			}

			_mpacStream.Write(value, 0, value.Length);
		}

		/// <summary>
		/// Writes extended data to the underlying <see cref="Stream"/>.
		/// </summary>
		/// <param name="type">Some user defined type code.</param>
		/// <param name="value">Array of zero or more bytes.</param>
		/// <exception cref="System.ArgumentNullException">
		/// If <paramref name="value"/> is <c>null</c>.
		/// </exception>
		public void WriteExtended(sbyte type, byte[] value) {
			if (value == null)
				throw new ArgumentNullException("value");

			if (value.Length <= byte.MaxValue) {
				WriteFormatCode(MessagePackFormatCode.Ext8);
				WriteByte((byte)value.Length);
			}
			else if (value.Length <= ushort.MaxValue) {
				WriteFormatCode(MessagePackFormatCode.Ext16);
				WriteUInt16((ushort)value.Length);
			}
			else {
				WriteFormatCode(MessagePackFormatCode.Ext32);
				WriteInt32(value.Length);
			}

			WriteByte((byte)type);

			_mpacStream.Write(value, 0, value.Length);
		}

	}

}
