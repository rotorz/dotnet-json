// Copyright (c) Rotorz Limited. All rights reserved.

using System;

namespace Rotorz.Json {

	/// <summary>
	/// Node holding double precision floating point value.
	/// </summary>
	public sealed class JsonDoubleNode : JsonNode {

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonDoubleNode"/> class with a
		/// value of zero.
		/// </summary>
		public JsonDoubleNode() {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonDoubleNode"/> class with the
		/// specified double precision numeric value.
		/// </summary>
		/// <param name="value">Initial value of node.</param>
		public JsonDoubleNode(double value) {
			Value = value;
		}

		/// <summary>
		/// Gets or sets value of the node.
		/// </summary>
		public double Value { get; set; }

		/// <inheritdoc/>
		public override JsonNode Clone() {
			return new JsonDoubleNode(Value);
		}

		/// <inheritdoc/>
		public override string ToString() {
			return JsonFormattingUtility.DoubleToString(Value);
		}

		/// <inheritdoc/>
		public override object ConvertTo(Type type) {
			if (type == null)
				throw new ArgumentNullException("type");

			return Convert.ChangeType(Value, type);
		}

		/// <inheritdoc/>
		public override void Write(IJsonWriter writer) {
			writer.WriteDouble(Value);
		}

	}

}
