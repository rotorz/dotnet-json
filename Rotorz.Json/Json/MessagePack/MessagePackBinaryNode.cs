// Copyright (c) Rotorz Limited. All rights reserved.

using System;

namespace Rotorz.Json.MessagePack {

	/// <summary>
	/// A MessagePack node that holds binary data as an array of bytes.
	/// </summary>
	public sealed class MessagePackBinaryNode : JsonNode {

		private static readonly byte[] EmptyByteArray = { };

		/// <summary>
		/// Initializes a new instance of the <see cref="MessagePackBinaryNode"/> class
		/// with an empty byte array.
		/// </summary>
		public MessagePackBinaryNode() {
			Value = EmptyByteArray;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MessagePackBinaryNode"/> class
		/// with the specified array of bytes.
		/// </summary>
		/// <param name="data">An array of zero or more bytes of binary data.</param>
		/// <exception cref="System.ArgumentNullException">
		/// If <paramref name="data"/> is <c>null</c>.
		/// </exception>
		public MessagePackBinaryNode(byte[] data) {
			if (data == null)
				throw new ArgumentNullException("data");

			Value = data;
		}

		/// <summary>
		/// Gets or sets the value of the node.
		/// </summary>
		public byte[] Value { get; set; }

		/// <inheritdoc/>
		public override JsonNode Clone() {
			return new MessagePackBinaryNode(Value);
		}

		/// <inheritdoc/>
		public override string ToString() {
			return "(binary)";
		}

		/// <inheritdoc/>
		public override object ConvertTo(Type type) {
			if (type == null)
				throw new ArgumentNullException("type");

			return Convert.ChangeType(Value, type);
		}

		/// <inheritdoc/>
		public override void Write(IJsonWriter writer) {
			writer.WriteBinary(Value);
		}

	}

}
