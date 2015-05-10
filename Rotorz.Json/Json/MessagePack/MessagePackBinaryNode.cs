// Copyright (c) Rotorz Limited. All rights reserved.

using System;

namespace Rotorz.Json.MessagePack {

	/// <summary>
	/// Node holding binary data in the form of a byte array.
	/// </summary>
	public class MessagePackBinaryNode : JsonNode {

		private static readonly byte[] EmptyByteArray = { };

		/// <summary>
		/// Initializes a new instance of the <see cref="MessagePackBinaryNode"/>
		/// class with an empty byte array.
		/// </summary>
		public MessagePackBinaryNode() {
			Value = EmptyByteArray;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MessagePackBinaryNode"/>
		/// class with the specified binary data.
		/// </summary>
		/// <param name="data">Initial value of node.</param>
		/// <exception cref="System.ArgumentNullException">
		/// If <paramref name="data"/> is <c>null</c>.
		/// </exception>
		public MessagePackBinaryNode(byte[] data) {
			if (data == null)
				throw new ArgumentNullException("data");
			Value = data;
		}

		/// <summary>
		/// Gets or sets value of the node.
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
		public override object ToObject(Type type) {
			if (type == null)
				throw new ArgumentNullException("type");

			return Convert.ChangeType(Value, type);
		}

		/// <inheritdoc/>
		public override void WriteTo(JsonWriter writer) {
			writer.WriteStartArray();
			foreach (var b in Value)
				writer.WriteValue(b);
			writer.WriteEndArray();
		}

	}

}
