// Copyright (c) Rotorz Limited. All rights reserved.

using System;
using System.IO;

namespace Rotorz.Json.MessagePack {
	
	/// <summary>
	/// Utility methods for reading and writing <see cref="JsonNode"/> instances to
	/// MessagePack encoded binary data.
	/// </summary>
	public static class MessagePackUtility {

		/// <summary>
		/// Reads a <see cref="JsonNode"/> from an array of MessagePack encoded bytes.
		/// </summary>
		/// <param name="bytes">An array of one or more MessagePack encoded bytes.</param>
		/// <returns>
		/// A <see cref="JsonNode"/> instance or a value of <c>null</c>.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">
		/// If <paramref name="bytes"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// If <paramref name="bytes"/> contains zero bytes.
		/// </exception>
		/// <exception cref="MessagePackParserException">
		/// If an error was encountered whilst attempting to parse MessagePack encoded
		/// data. This exception typical indicates that input contains one or more errors.
		/// </exception>
		public static JsonNode ReadNodeFromBytes(byte[] bytes) {
			if (bytes == null)
				throw new ArgumentNullException("bytes");
			if (bytes.Length == 0)
				throw new ArgumentException("Must contain at least one byte.", "bytes");
			
			using (var stream = new MemoryStream(bytes))
				return ReadNodeFrom(stream);
		}

		/// <summary>
		/// Writes a <see cref="JsonNode"/> to an array of MessagePack encoded bytes.
		/// </summary>
		/// <param name="node">A <see cref="JsonNode"/> instance or <c>null</c>.</param>
		public static byte[] WriteNodeToBytes(JsonNode node) {
			using (var stream = new MemoryStream()) {
				WriteNodeTo(node, stream);
				return stream.ToArray();
			}
		}

		/// <summary>
		/// Reads a <see cref="JsonNode"/> from a stream of MessagePack encoded bytes.
		/// </summary>
		/// <remarks>
		/// <para>User code should close the provided stream when it is no longer required
		/// after data has been read; this can be accomplished with the <c>using</c> construct:</para>
		/// <code language="csharp"><![CDATA[
		/// JsonNode result;
		/// using (var fs = new FileStream(@"C:\TestFile.mpac", FileMode.Open, FileAccess.Read)) {
		///     result = MessagePackUtility.ReadJsonNodeFrom(fs);
		/// }
		/// ]]></code>
		/// </remarks>
		/// <param name="stream">Input stream.</param>
		/// <returns>
		/// A <see cref="JsonNode"/> instance or a value of <c>null</c>.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">
		/// If <paramref name="stream"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// If <paramref name="stream"/> is not readable.
		/// </exception>
		/// <exception cref="MessagePackParserException">
		/// If an error was encountered whilst attempting to parse MessagePack encoded
		/// data. This exception typical indicates that input string contains one or more
		/// syntax errors.
		/// </exception>
		public static JsonNode ReadNodeFrom(Stream stream) {
			return MessagePackReader.Create(stream).Read();
		}

		/// <summary>
		/// Reads a <see cref="JsonNode"/> from MessagePack encoded bytes using a <see cref="BinaryReader"/>.
		/// </summary>
		/// <remarks>
		/// <para>User code should close the provided reader when it is no longer required
		/// after data has been read; this can be accomplished with the <c>using</c> construct.</para>
		/// </remarks>
		/// <param name="reader">Binary reader.</param>
		/// <returns>
		/// A <see cref="JsonNode"/> instance or a value of <c>null</c>.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">
		/// If <paramref name="reader"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="MessagePackParserException">
		/// If an error was encountered whilst attempting to parse MessagePack encoded
		/// data. This exception typical indicates that input string contains one or more
		/// syntax errors.
		/// </exception>
		public static JsonNode ReadNodeFrom(BinaryReader reader) {
			return MessagePackReader.Create(reader).Read();
		}

		/// <summary>
		/// Writes a <see cref="JsonNode"/> to a stream of MessagePack encoded bytes.
		/// </summary>
		/// <param name="node">A <see cref="JsonNode"/> instance or <c>null</c>.</param>
		/// <param name="stream">Stream that data will be written to.</param>
		/// <exception cref="System.ArgumentNullException">
		/// If <paramref name="stream"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// If <paramref name="stream"/> is not writable.
		/// </exception>
		public static void WriteNodeTo(JsonNode node, Stream stream) {
			var writer = MessagePackWriter.Create(stream);
			if (node != null)
				node.Write(writer);
			else
				writer.WriteNull();
		}

		/// <summary>
		/// Writes a <see cref="JsonNode"/> to MessagePack encoded bytes using a <see cref="BinaryWriter"/>.
		/// </summary>
		/// <param name="node">A <see cref="JsonNode"/> instance or <c>null</c>.</param>
		/// <param name="writer">Binary data writer.</param>
		/// <exception cref="System.ArgumentNullException">
		/// If <paramref name="writer"/> is <c>null</c>.
		/// </exception>
		public static void WriteNodeTo(JsonNode node, BinaryWriter binaryWriter) {
			var writer = MessagePackWriter.Create(binaryWriter);
			if (node != null)
				node.Write(writer);
			else
				writer.WriteNull();
		}

	}

}
