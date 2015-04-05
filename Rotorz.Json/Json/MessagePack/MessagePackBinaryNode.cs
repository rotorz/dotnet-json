// Copyright (c) Rotorz Limited. All rights reserved.

using System;

namespace Rotorz.Json.MessagePack {

	/// <summary>
	/// Node holding a boolean value of <c>true</c> or <c>false</c>.
	/// </summary>
	public class MessagePackBinaryNode : JsonNode {

		private static readonly byte[] EmptyByteArray = { };

		/// <summary>
		/// Initializes a new instance of the <see cref="MessagePackBinaryNode"/> class with an
		/// empty byte array.
		/// </summary>
		public MessagePackBinaryNode() {
			Value = EmptyByteArray;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MessagePackBinaryNode"/> class with
		/// the specified value of <c>true</c> or <c>false</c>.
		/// </summary>
		/// <param name="value">Initial value of node.</param>
		public MessagePackBinaryNode(byte[] data) {
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
