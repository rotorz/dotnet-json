// Copyright (c) Rotorz Limited. All rights reserved.

using System;

namespace Rotorz.Json {

	/// <summary>
	/// Node holding a boolean value of <c>true</c> or <c>false</c>.
	/// </summary>
	public sealed class JsonBooleanNode : JsonNode {

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonBooleanNode"/> class with a
		/// value of <c>false</c>.
		/// </summary>
		public JsonBooleanNode() {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonBooleanNode"/> class with
		/// the specified value of <c>true</c> or <c>false</c>.
		/// </summary>
		/// <param name="value">Initial value of node.</param>
		public JsonBooleanNode(bool value) {
			Value = value;
		}

		/// <summary>
		/// Gets or sets value of the node.
		/// </summary>
		public bool Value { get; set; }

		/// <inheritdoc/>
		public override JsonNode Clone() {
			return new JsonBooleanNode(Value);
		}

		/// <inheritdoc/>
		public override string ToString() {
			return Value ? "true" : "false";
		}

		/// <inheritdoc/>
		public override object ToObject(Type type) {
			if (type == null)
				throw new ArgumentNullException("type");

			return Convert.ChangeType(Value, type);
		}

		/// <inheritdoc/>
		public override void Write(IJsonWriter writer) {
			writer.WriteBoolean(Value);
		}

	}

}
