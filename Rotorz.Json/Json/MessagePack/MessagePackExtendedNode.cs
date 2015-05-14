// Copyright (c) Rotorz Limited. All rights reserved.

using System;

namespace Rotorz.Json.MessagePack {

	/// <summary>
	/// Node holding an extended binary value in the form of a byte array with an
	/// accompanying <see xref="ExtendedType"/> value.
	/// </summary>
	public sealed class MessagePackExtendedNode : JsonNode {

		/// <summary>
		/// Initializes a new instance of the <see cref="MessagePackExtendedNode"/>
		/// class with an empty byte array.
		/// </summary>
		public MessagePackExtendedNode() {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MessagePackExtendedNode"/>
		/// class with the specified type and binary data.
		/// </summary>
		/// <param name="type">The type of extended value.</param>
		/// <param name="data">Initial value of node.</param>
		/// <exception cref="System.ArgumentNullException">
		/// If <paramref name="data"/> is <c>null</c>.
		/// </exception>
		public MessagePackExtendedNode(sbyte type, byte[] data) {
			if (data == null)
				throw new ArgumentNullException("data");

			ExtendedType = type;
			Value = data;
		}

		/// <summary>
		/// Gets or sets the type of the extended value.
		/// </summary>
		public sbyte ExtendedType { get; set; }

		/// <summary>
		/// Gets or sets value of the node.
		/// </summary>
		public byte[] Value { get; set; }

		/// <inheritdoc/>
		public override JsonNode Clone() {
			return new MessagePackExtendedNode(ExtendedType, Value);
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
			writer.WriteStartObject(2);
			writer.WritePropertyKey("type");
			writer.WriteInteger(ExtendedType);
			writer.WritePropertyKey("data");
			writer.WriteBinary(Value);
			writer.WriteEndObject();
		}

	}

}
