// Copyright (c) Rotorz Limited. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Rotorz.Json.MessagePack {

	// https://github.com/msgpack/msgpack/blob/master/spec.md
	// 2fb4eaa9688888b74bdabb2222f0e0f42712b6b1
	internal sealed class MessagePackReader {

		/// <summary>
		/// Create a new <see cref="MessagePackReader"/> instance from a stream.
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
		/// <seealso cref="Parse()"/>
		public static MessagePackReader Create(Stream stream) {
			if (stream == null)
				throw new ArgumentNullException("stream");

			stream = new BufferedStream(stream, 2048);

			return new MessagePackReader(new BinaryReader(stream));
		}

		/// <summary>
		/// Create a new <see cref="MessagePackReader"/> instance from a text reader. This
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
		/// <seealso cref="Parse()"/>
		public static MessagePackReader Create(BinaryReader reader) {
			if (reader == null)
				throw new ArgumentNullException("reader");

			return new MessagePackReader(reader);
		}

		private BinaryReader _mpackReader;

		/// <summary>
		/// Initialize new <see cref="MessagePackReader"/> instance.
		/// </summary>
		/// <param name="reader">Binary reader.</param>
		private MessagePackReader(BinaryReader reader) {
			_mpackReader = reader;
		}

		#region Basic Value Reader

		private byte[] _valueBuffer = new byte[8];

		private void ReadValueBufferBytes(int count) {
			for (int i = 0; i < count; ++i)
				_valueBuffer[i] = _mpackReader.ReadByte();
		}

		private byte ReadByte() {
			return _mpackReader.ReadByte();
		}

		private float ReadFloat32() {
			ReadValueBufferBytes(4);
			Array.Reverse(_valueBuffer, 0, 4);
			return BitConverter.ToSingle(_valueBuffer, 0);
		}

		private double ReadFloat64() {
			ReadValueBufferBytes(8);
			Array.Reverse(_valueBuffer, 0, 8);
			return BitConverter.ToDouble(_valueBuffer, 0);
		}

		private ushort ReadUInt16() {
			ReadValueBufferBytes(2);
			Array.Reverse(_valueBuffer, 0, 2);
			return BitConverter.ToUInt16(_valueBuffer, 0);
		}

		private uint ReadUInt32() {
			ReadValueBufferBytes(4);
			Array.Reverse(_valueBuffer, 0, 4);
			return BitConverter.ToUInt32(_valueBuffer, 0);
		}

		private ulong ReadUInt64() {
			ReadValueBufferBytes(8);
			Array.Reverse(_valueBuffer, 0, 8);
			return BitConverter.ToUInt64(_valueBuffer, 0);
		}

		private short ReadInt16() {
			ReadValueBufferBytes(2);
			Array.Reverse(_valueBuffer, 0, 2);
			return BitConverter.ToInt16(_valueBuffer, 0);
		}

		private int ReadInt32() {
			ReadValueBufferBytes(4);
			Array.Reverse(_valueBuffer, 0, 4);
			return BitConverter.ToInt32(_valueBuffer, 0);
		}

		private long ReadInt64() {
			ReadValueBufferBytes(8);
			Array.Reverse(_valueBuffer, 0, 8);
			return BitConverter.ToInt64(_valueBuffer, 0);
		}

		private JsonStringNode ReadString(int length) {
			byte[] bytes = _mpackReader.ReadBytes(length);
			string str = Encoding.UTF8.GetString(bytes);
			return new JsonStringNode(str);
		}

		#endregion

		/// <summary>
		/// Parse input JSON encoded content.
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
		public JsonNode Parse() {
			var nodes = new List<JsonNode>();
			try {
				while (true)
					nodes.Add(ReadValue());
			}
			catch (EndOfStreamException) {
			}
			return nodes.Count > 1
				? new JsonArrayNode(nodes)
				: nodes[0];
		}

		private enum FormatCode {
			PositiveFixInt	= 0x00,///
			FixMap			= 0x80,///
			FixArray		= 0x90,///
			FixStr			= 0xA0,///
			Nil				= 0xC0,///
			Unused			= 0xC1,///
			False			= 0xC2,///
			True			= 0xC3,///
			Bin8			= 0xC4,
			Bin16			= 0xC5,
			Bin32			= 0xC6,
			Ext8			= 0xC7,
			Ext16			= 0xC8,
			Ext32			= 0xC9,
			Float32			= 0xCA,///
			Float64			= 0xCB,///
			UInt8			= 0xCC,///
			UInt16			= 0xCD,///
			UInt32			= 0xCE,///
			UInt64			= 0xCF,///!
			Int8			= 0xD0,///
			Int16			= 0xD1,///
			Int32			= 0xD2,///
			Int64			= 0xD3,///
			FixExt1			= 0xD4,
			FixExt2			= 0xD5,
			FixExt4			= 0xD6,
			FixExt8			= 0xD7,
			FixExt16		= 0xD8,
			Str8			= 0xD9,///
			Str16			= 0xDA,///
			Str32			= 0xDB,///
			Array16			= 0xDC,///
			Array32			= 0xDD,///
			Map16			= 0xDE,///
			Map32			= 0xDF,///
			NegativeFixInt	= 0xE0,///
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

			FormatCode formatCode;
			if ((b & 0x80) == 0x00) {
				formatCode = FormatCode.PositiveFixInt;
				b &= 0x7F;
			}
			else if ((b & 0xF0) == 0x80) {
				formatCode = FormatCode.FixMap;
				b &= 0x0F;
			}
			else if ((b & 0xF0) == 0x90) {
				formatCode = FormatCode.FixArray;
				b &= 0x0F;
			}
			else if ((b & 0xE0) == 0xA0) {
				formatCode = FormatCode.FixStr;
				b &= 0x1F;
			}
			else if ((b & 0xE0) == 0xE0) {
				formatCode = FormatCode.NegativeFixInt;
			}
			else {
				formatCode = (FormatCode)b;
			}

			switch (formatCode) {
				case FormatCode.PositiveFixInt:
					return new JsonIntegerNode(b);
				case FormatCode.FixMap:
					return ReadMap(b);
				case FormatCode.FixArray:
					return ReadArray(b);
				case FormatCode.FixStr:
					return ReadString(b);

				case FormatCode.Nil:
					return null;
				case FormatCode.False:
					return new JsonBooleanNode(false);
				case FormatCode.True:
					return new JsonBooleanNode(true);
					
				case FormatCode.Bin8:
					_mpackReader.BaseStream.Seek(ReadByte(), SeekOrigin.Current);
					return null;
				case FormatCode.Bin16:
					_mpackReader.BaseStream.Seek(ReadInt16(), SeekOrigin.Current);
					return null;
				case FormatCode.Bin32:
					_mpackReader.BaseStream.Seek(ReadInt32(), SeekOrigin.Current);
					return null;
				case FormatCode.Ext8:
					_mpackReader.BaseStream.Seek(ReadByte() + 1, SeekOrigin.Current);
					return null;
				case FormatCode.Ext16:
					_mpackReader.BaseStream.Seek(ReadInt16() + 1, SeekOrigin.Current);
					return null;
				case FormatCode.Ext32:
					_mpackReader.BaseStream.Seek(ReadInt32() + 1, SeekOrigin.Current);
					return null;

				case FormatCode.Float32:
					return new JsonDoubleNode(ReadFloat32());
				case FormatCode.Float64:
					return new JsonDoubleNode(ReadFloat64());

				case FormatCode.UInt8:
					return new JsonIntegerNode(ReadByte());
				case FormatCode.UInt16:
					return new JsonIntegerNode(ReadUInt16());
				case FormatCode.UInt32:
					return new JsonIntegerNode(ReadUInt32());
				case FormatCode.UInt64:
					// Can't store UInt64 in JsonIntegerNode; losing precision...
					//!TODO: Handle this better!
					return new JsonIntegerNode((long)ReadUInt64());
					
				case FormatCode.Int8:
					return new JsonIntegerNode((sbyte)ReadByte());
				case FormatCode.Int16:
					return new JsonIntegerNode(ReadInt16());
				case FormatCode.Int32:
					return new JsonIntegerNode(ReadInt32());
				case FormatCode.Int64:
					return new JsonIntegerNode(ReadInt64());

				case FormatCode.FixExt1:
					_mpackReader.BaseStream.Seek(1 + 1, SeekOrigin.Current);
					return null;
				case FormatCode.FixExt2:
					_mpackReader.BaseStream.Seek(1 + 2, SeekOrigin.Current);
					return null;
				case FormatCode.FixExt4:
					_mpackReader.BaseStream.Seek(1 + 4, SeekOrigin.Current);
					return null;
				case FormatCode.FixExt8:
					_mpackReader.BaseStream.Seek(1 + 8, SeekOrigin.Current);
					return null;
				case FormatCode.FixExt16:
					_mpackReader.BaseStream.Seek(1 + 16, SeekOrigin.Current);
					return null;
					
				case FormatCode.Str8:
					return ReadString(ReadByte());
				case FormatCode.Str16:
					return ReadString(ReadInt16());
				case FormatCode.Str32:
					return ReadString(ReadInt32());
				case FormatCode.Array16:
					return ReadArray(ReadInt16());
				case FormatCode.Array32:
					return ReadArray(ReadInt32());
				case FormatCode.Map16:
					return ReadMap(ReadInt16());
				case FormatCode.Map32:
					return ReadMap(ReadInt32());

				case FormatCode.NegativeFixInt:
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
