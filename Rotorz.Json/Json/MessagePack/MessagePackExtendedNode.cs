// Copyright (c) Rotorz Limited. All rights reserved.

using System;

namespace Rotorz.Json.MessagePack {

	/// <summary>
	/// Node holding an extended binary value in the form of a byte array with an
	/// accompanying <see xref="ExtendedType"/> value.
	/// </summary>
	public sealed class MessagePackExtendedNode : MessagePackBinaryNode {

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
		public MessagePackExtendedNode(sbyte type, byte[] data)
			: base(data)
		{
			ExtendedType = type;
		}

		/// <summary>
		/// Gets or sets the type of the extended value.
		/// </summary>
		public sbyte ExtendedType { get; set; }

		/// <inheritdoc/>
		public override JsonNode Clone() {
			return new MessagePackExtendedNode(ExtendedType, Value);
		}

		/// <inheritdoc/>
		public override object ToObject(Type type) {
			if (type == null)
				throw new ArgumentNullException("type");

			return Convert.ChangeType(Value, type);
		}

		/// <inheritdoc/>
		public override void WriteTo(JsonWriter writer) {
			writer.WriteStartObject();
			writer.WritePropertyKey("type");
			writer.WriteValue(ExtendedType);
			writer.WritePropertyKey("data");
			base.WriteTo(writer);
			writer.WriteEndObject();
		}

	}

}
