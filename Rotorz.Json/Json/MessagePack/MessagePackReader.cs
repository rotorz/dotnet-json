// Copyright (c) Rotorz Limited. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Rotorz.Json.MessagePack {

	// MessagePack Specification:
	// https://github.com/msgpack/msgpack/blob/master/spec.md

	internal sealed class MessagePackReader {

		#region Factory Methods

		/// <summary>
		/// Creates a new <see cref="MessagePackReader"/> instance from a stream.
		/// </summary>
		/// <remarks>
		/// <para>Remember to close the provided <see cref="Stream"/> when it is no
		/// longer required after invoking this method.</para>
		/// </remarks>
		/// <param name="stream">Stream.</param>
		/// <returns>
		/// The new <see cref="MessagePackReader"/> instance.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">
		/// If <paramref name="stream"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// If <paramref name="stream"/> is not readable.
		/// </exception>
		/// <seealso cref="Read()"/>
		public static MessagePackReader Create(Stream stream) {
			if (stream == null)
				throw new ArgumentNullException("stream");
			if (!stream.CanRead)
				throw new ArgumentException("Cannot read from stream.", "stream");

			stream = new BufferedStream(stream, 2048);

			return Create(new BinaryReader(stream));
		}

		/// <summary>
		/// Creates a new <see cref="MessagePackReader"/> instance from a text reader. This
		/// allows JSON encoded text to be parsed from a variety of sources including
		/// strings, files, etc.
		/// </summary>
		/// <remarks>
		/// <para>Remember to dispose the provided <see cref="BinaryReader"/> when it is no
		/// longer required after invoking this method.</para>
		/// </remarks>
		/// <param name="reader">Binary reader.</param>
		/// <returns>
		/// The new <see cref="MessagePackReader"/> instance.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">
		/// If <paramref name="reader"/> is <c>null</c>.
		/// </exception>
		/// <seealso cref="Read()"/>
		public static MessagePackReader Create(BinaryReader reader) {
			if (reader == null)
				throw new ArgumentNullException("reader");

			return new MessagePackReader(reader);
		}

		#endregion

		private readonly BinaryReader _mpackReader;

		/// <summary>
		/// Initialize new <see cref="MessagePackReader"/> instance.
		/// </summary>
		/// <param name="reader">Binary reader.</param>
		/// <exception cref="System.ArgumentNullException">
		/// If <paramref name="reader"/> is <c>null</c>.
		/// </exception>
		private MessagePackReader(BinaryReader reader) {
			if (reader == null)
				throw new ArgumentNullException("reader");

			_mpackReader = reader;
		}

		#region Low Value Reader

		private byte[] _valueBuffer = new byte[8];

		private void ReadValueBufferBytes(int count) {
			for (int i = 0; i < count; ++i)
				_valueBuffer[i] = _mpackReader.ReadByte();

			if (BitConverter.IsLittleEndian)
				Array.Reverse(_valueBuffer, 0, count);
		}

		private byte ReadByte() {
			return _mpackReader.ReadByte();
		}

		private float ReadFloat32() {
			ReadValueBufferBytes(4);
			return BitConverter.ToSingle(_valueBuffer, 0);
		}

		private double ReadFloat64() {
			ReadValueBufferBytes(8);
			return BitConverter.ToDouble(_valueBuffer, 0);
		}

		private ushort ReadUInt16() {
			ReadValueBufferBytes(2);
			return BitConverter.ToUInt16(_valueBuffer, 0);
		}

		private uint ReadUInt32() {
			ReadValueBufferBytes(4);
			return BitConverter.ToUInt32(_valueBuffer, 0);
		}

		private ulong ReadUInt64() {
			ReadValueBufferBytes(8);
			return BitConverter.ToUInt64(_valueBuffer, 0);
		}

		private short ReadInt16() {
			ReadValueBufferBytes(2);
			return BitConverter.ToInt16(_valueBuffer, 0);
		}

		private int ReadInt32() {
			ReadValueBufferBytes(4);
			return BitConverter.ToInt32(_valueBuffer, 0);
		}

		private long ReadInt64() {
			ReadValueBufferBytes(8);
			return BitConverter.ToInt64(_valueBuffer, 0);
		}

		private JsonStringNode ReadString(int length) {
			byte[] bytes = _mpackReader.ReadBytes(length);
			string str = Encoding.UTF8.GetString(bytes);
			return new JsonStringNode(str);
		}

		#endregion

		/// <summary>
		/// Reads MessagePack encoded data from the underlying stream.
		/// </summary>
		/// <returns>
		/// A <see cref="JsonNode"/> instance of the applicable type; otherwise, a value
		/// of <c>null</c> if input content was either empty or consisted entirely of
		/// whitespace.
		/// </returns>
		/// <exception cref="JsonParserException">
		/// If a syntax error was encountered whilst attempting to parse input content.
		/// Exception contains identifies the source of the error by providing the line
		/// number and position.
		/// </exception>
		public JsonNode Read() {
			var nodes = new List<JsonNode>();
			try {
				while (true)
					nodes.Add(ReadValue());
			}
			catch (EndOfStreamException) {
			}

			if (nodes.Count > 1)
				return new JsonArrayNode(nodes);
			else if (nodes.Count == 1)
				return nodes[0];
			else
				return null;
		}

		/// <summary>
		/// Parse value node (null, integer, double, boolean, string, array or object).
		/// </summary>
		/// <returns>
		/// New <see cref="JsonNode"/> holding value; returns a value of <c>null</c> when
		/// <c>null</c> is detected in input.
		/// </returns>
		/// <exception cref="JsonParserException">
		/// If a syntax error was encountered whilst attempting to parse input content.
		/// Exception contains identifies the source of the error by providing the line
		/// number and position.
		/// </exception>
		private JsonNode ReadValue() {
			byte b = ReadByte();

			MessagePackFormatCode formatCode;
			if ((b & 0x80) == 0x00) {
				formatCode = MessagePackFormatCode.PositiveFixInt;
				b &= 0x7F;
			}
			else if ((b & 0xF0) == 0x80) {
				formatCode = MessagePackFormatCode.FixMap;
				b &= 0x0F;
			}
			else if ((b & 0xF0) == 0x90) {
				formatCode = MessagePackFormatCode.FixArray;
				b &= 0x0F;
			}
			else if ((b & 0xE0) == 0xA0) {
				formatCode = MessagePackFormatCode.FixStr;
				b &= 0x1F;
			}
			else if ((b & 0xE0) == 0xE0) {
				formatCode = MessagePackFormatCode.NegativeFixInt;
			}
			else {
				formatCode = (MessagePackFormatCode)b;
			}

			int length;

			switch (formatCode) {
				case MessagePackFormatCode.PositiveFixInt:
					return new JsonIntegerNode(b);
				case MessagePackFormatCode.FixMap:
					return ReadMap(b);
				case MessagePackFormatCode.FixArray:
					return ReadArray(b);
				case MessagePackFormatCode.FixStr:
					return ReadString(b);

				case MessagePackFormatCode.Nil:
					return null;
				case MessagePackFormatCode.False:
					return new JsonBooleanNode(false);
				case MessagePackFormatCode.True:
					return new JsonBooleanNode(true);
					
				case MessagePackFormatCode.Bin8:
					return new MessagePackBinaryNode(_mpackReader.ReadBytes(ReadByte()));
				case MessagePackFormatCode.Bin16:
					return new MessagePackBinaryNode(_mpackReader.ReadBytes(ReadInt16()));
				case MessagePackFormatCode.Bin32:
					return new MessagePackBinaryNode(_mpackReader.ReadBytes(ReadInt32()));
				case MessagePackFormatCode.Ext8:
					length = ReadByte();
					return new MessagePackExtendedNode((sbyte)ReadByte(), _mpackReader.ReadBytes(length));
				case MessagePackFormatCode.Ext16:
					length = ReadInt16();
					return new MessagePackExtendedNode((sbyte)ReadByte(), _mpackReader.ReadBytes(length));
				case MessagePackFormatCode.Ext32:
					length = ReadInt32();
					return new MessagePackExtendedNode((sbyte)ReadByte(), _mpackReader.ReadBytes(length));

				case MessagePackFormatCode.Float32:
					return new JsonDoubleNode(ReadFloat32());
				case MessagePackFormatCode.Float64:
					return new JsonDoubleNode(ReadFloat64());

				case MessagePackFormatCode.UInt8:
					return new JsonIntegerNode(ReadByte());
				case MessagePackFormatCode.UInt16:
					return new JsonIntegerNode(ReadUInt16());
				case MessagePackFormatCode.UInt32:
					return new JsonIntegerNode(ReadUInt32());
				case MessagePackFormatCode.UInt64:
					// Can't store UInt64 in JsonIntegerNode; losing precision...
					//!TODO: Handle this better!
					return new JsonIntegerNode((long)ReadUInt64());
					
				case MessagePackFormatCode.Int8:
					return new JsonIntegerNode((sbyte)ReadByte());
				case MessagePackFormatCode.Int16:
					return new JsonIntegerNode(ReadInt16());
				case MessagePackFormatCode.Int32:
					return new JsonIntegerNode(ReadInt32());
				case MessagePackFormatCode.Int64:
					return new JsonIntegerNode(ReadInt64());

				case MessagePackFormatCode.FixExt1:
					return new MessagePackExtendedNode((sbyte)ReadByte(), _mpackReader.ReadBytes(1));
				case MessagePackFormatCode.FixExt2:
					return new MessagePackExtendedNode((sbyte)ReadByte(), _mpackReader.ReadBytes(2));
				case MessagePackFormatCode.FixExt4:
					return new MessagePackExtendedNode((sbyte)ReadByte(), _mpackReader.ReadBytes(4));
				case MessagePackFormatCode.FixExt8:
					return new MessagePackExtendedNode((sbyte)ReadByte(), _mpackReader.ReadBytes(8));
				case MessagePackFormatCode.FixExt16:
					return new MessagePackExtendedNode((sbyte)ReadByte(), _mpackReader.ReadBytes(16));
					
				case MessagePackFormatCode.Str8:
					return ReadString(ReadByte());
				case MessagePackFormatCode.Str16:
					return ReadString(ReadUInt16());
				case MessagePackFormatCode.Str32:
					return ReadString(ReadInt32());
				case MessagePackFormatCode.Array16:
					return ReadArray(ReadUInt16());
				case MessagePackFormatCode.Array32:
					return ReadArray(ReadInt32());
				case MessagePackFormatCode.Map16:
					return ReadMap(ReadUInt16());
				case MessagePackFormatCode.Map32:
					return ReadMap(ReadInt32());

				case MessagePackFormatCode.NegativeFixInt:
					return new JsonIntegerNode((sbyte)b);

				default:
					throw new IOException(string.Format("Encountered unexpected format code '{0:X}' in MessagePack format.", (byte)formatCode));
			}
		}

		private JsonNode ReadArray(int length) {
			var arrayNode = new JsonArrayNode();
			while (length-- > 0)
				arrayNode.Add(ReadValue());
			return arrayNode;
		}

		private JsonNode ReadMap(int length) {
			var objectNode = new JsonObjectNode();
			while (length-- > 0) {
				var node = ReadValue();

				var keyNode = node as JsonStringNode;
				if (keyNode == null)
					throw new IOException(string.Format("Expected string for key but encountered '{0}'.", node.GetType()));

				objectNode[keyNode.Value] = ReadValue();
			}
			return objectNode;
		}

	}

}
