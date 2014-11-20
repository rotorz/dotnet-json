// Copyright (c) Rotorz Limited. All rights reserved.

using System;

namespace Rotorz.Json {

	/// <summary>
	/// Node holding double precision floating point value.
	/// </summary>
	public sealed class JsonDoubleNode : JsonNode {

		/// <summary>
		/// Initialize new double precision numeric node with a value of zero.
		/// </summary>
		public JsonDoubleNode() {
		}

		/// <summary>
		/// Initialize new double precision numeric node.
		/// </summary>
		/// <param name="value">Initial value of node.</param>
		public JsonDoubleNode(double value) {
			Value = value;
		}

		/// <summary>
		/// Gets or sets value of node.
		/// </summary>
		public double Value { get; set; }

		/// <inheritdoc/>
		public override JsonNode Clone() {
			return new JsonDoubleNode(Value);
		}

		/// <inheritdoc/>
		public override string ToString() {
			return JsonWriter.DoubleToString(Value);
		}

		/// <inheritdoc/>
		public override object ToObject(Type type) {
			if (type == null)
				throw new ArgumentNullException("type");

			return Convert.ChangeType(Value, type);
		}

		/// <inheritdoc/>
		public override void WriteTo(JsonWriter writer) {
			writer.WriteValue(Value);
		}

	}

}
