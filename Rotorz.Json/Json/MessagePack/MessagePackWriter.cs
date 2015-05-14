// Copyright (c) Rotorz Limited. All rights reserved.

using System;
using System.IO;
using System.Text;

namespace Rotorz.Json.MessagePack {

	// MessagePack Specification:
	// https://github.com/msgpack/msgpack/blob/master/spec.md

	/// <summary>
	/// A <see cref="IJsonWriter"/> that can be used to write a MessagePack encoded
	/// representation of a <see cref="JsonNode"/>. This class can be used directly to
	/// manually write MessagePack data without the need to instantiate any <see cref="JsonNode"/>
	/// instances.
	/// </summary>
	/// <remarks>
	/// <para>The <see cref="MessagePackWriter"/> class aims to implement the MessagePack
	/// specification; commit <a href="https://github.com/msgpack/msgpack/blob/8fc1ab3efbece26890d16baa8e5bbc6867ba80b8/spec.md">8fc1ab3efbece26890d16baa8e5bbc6867ba80b8</a>.</para>
	/// </remarks>
	/// <example>
	/// <para>The following code demonstrates how to manually write MessagePack data:</para>
	/// <code language="csharp"><![CDATA[
	/// byte[] bytes;
	/// using (var memoryStream = new MemoryStream()) {
	///     var writer = MessagePackWriter.Create(memoryStream);
	///     writer.WriteStartObject(2);
	///     writer.WritePropertyKey("Name");
	///     writer.WriteString("Jessica");
	///     writer.WritePropertyKey("Age");
	///     writer.WriteInteger(24);
	///     writer.WriteEndObject();
	///     bytes = memoryStream.ToArray();
	/// }
	/// ]]></code>
	/// </example>
	public sealed class MessagePackWriter : IJsonWriter {

		#region Factory Methods

		/// <summary>
		/// Create new <see cref="MessagePackWriter"/> instance and write content to the
		/// provided <see cref="Stream"/>.
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
			if (stream == null)
				throw new ArgumentNullException("stream");
			if (!stream.CanWrite)
				throw new ArgumentException("Cannot write to stream.", "stream");

			return Create(new BinaryWriter(stream));
		}

		/// <summary>
		/// Create new <see cref="MessagePackWriter"/> instance and write content using
		/// the provided <see cref="BinaryWriter"/>.
		/// </summary>
		/// <param name="binaryWriter">Binary data writer.</param>
		/// <returns>
		/// New <see cref="MessagePackWriter"/> instance.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">
		/// If <paramref name="binaryWriter"/> is <c>null</c>.
		/// </exception>
		public static MessagePackWriter Create(BinaryWriter binaryWriter) {
			if (binaryWriter == null)
				throw new ArgumentNullException("binaryWriter");

			return new MessagePackWriter(binaryWriter);
		}

		#endregion

		private readonly BinaryWriter _mpacWriter;

		/// <summary>
		/// Initializes a new instance of the <see cref="MessagePackWriter"/> class.
		/// </summary>
		/// <param name="binaryWriter">Binary data writer.</param>
		/// <exception cref="System.ArgumentNullException">
		/// If <paramref name="binaryWriter"/> is <c>null</c>.
		/// </exception>
		private MessagePackWriter(BinaryWriter binaryWriter) {
			if (binaryWriter == null)
				throw new ArgumentNullException("binaryWriter");

			_mpacWriter = binaryWriter;
        }
		
		#region Low Level Writer

		private void WriteFormatCode(MessagePackFormatCode formatCode) {
			_mpacWriter.Write((byte)formatCode);
		}

		private void WriteByte(byte value) {
			_mpacWriter.Write(value);
		}

		private void WriteBytes(byte[] bytes) {
			if (BitConverter.IsLittleEndian)
				Array.Reverse(bytes);

			_mpacWriter.Write(bytes, 0, bytes.Length);
		}

		private void WriteUInt16(ushort value) {
			WriteBytes(BitConverter.GetBytes(value));
        }

		private void WriteUInt32(uint value) {
			WriteBytes(BitConverter.GetBytes(value));
		}

		private void WriteUInt64(ulong value) {
			WriteBytes(BitConverter.GetBytes(value));
		}

		private void WriteInt16(short value) {
			WriteBytes(BitConverter.GetBytes(value));
		}

		private void WriteInt32(int value) {
			WriteBytes(BitConverter.GetBytes(value));
		}

		private void WriteInt64(long value) {
			WriteBytes(BitConverter.GetBytes(value));
		}

		private void WriteStringBytes(string value) {
			var bytes = Encoding.UTF8.GetBytes(value);
			_mpacWriter.Write(bytes, 0, bytes.Length);
		}

		#endregion

		/// <inheritdoc/>
		public void WriteStartObject(int propertyCount) {
			if (propertyCount < 0)
				throw new ArgumentOutOfRangeException("propertyCount", "Cannot be a negative value.");

			if (propertyCount <= 0x0F) {
				long b = (long)MessagePackFormatCode.FixMap | propertyCount;
				WriteByte((byte)b);
			}
			else if (propertyCount <= ushort.MaxValue) {
				WriteFormatCode(MessagePackFormatCode.Map16);
				WriteUInt16((ushort)propertyCount);
			}
			else {
				WriteFormatCode(MessagePackFormatCode.Map32);
				WriteInt32(propertyCount);
			}
		}

		/// <inheritdoc/>
		public void WritePropertyKey(string key) {
			WriteString(key);
		}

		/// <inheritdoc/>
		public void WriteEndObject() {
		}

		/// <inheritdoc/>
		public void WriteStartArray(int arrayLength) {
			if (arrayLength < 0)
				throw new ArgumentOutOfRangeException("arrayLength", "Cannot be a negative value.");

			if (arrayLength <= 0x0F) {
				long b = (long)MessagePackFormatCode.FixArray | arrayLength;
				WriteByte((byte)b);
			}
			else if (arrayLength <= ushort.MaxValue) {
				WriteFormatCode(MessagePackFormatCode.Array16);
				WriteUInt16((ushort)arrayLength);
			}
			else {
				WriteFormatCode(MessagePackFormatCode.Array32);
				WriteInt32(arrayLength);
			}
		}

		/// <inheritdoc/>
		public void WriteEndArray() {
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
			if (double.IsNaN(value)) {
				WriteString("NaN");
			}
			else if (double.IsNegativeInfinity(value)) {
				WriteString("-Infinity");
			}
			else if (double.IsPositiveInfinity(value)) {
				WriteString("Infinity");
			}
			else {
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
				_mpacWriter.Write(bytes, 0, bytes.Length);
			}
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

			_mpacWriter.Write(value, 0, value.Length);
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

			_mpacWriter.Write(value, 0, value.Length);
		}

	}

}
