﻿// Copyright (c) 2014 Rotorz Limited. All rights reserved.

using System;

namespace Rotorz.Json {

	/// <summary>
	/// Node holding a boolean value of <c>true</c> or <c>false</c>.
	/// </summary>
	public sealed class JsonBooleanNode : JsonNode {

		/// <summary>
		/// Initialize new boolean node with a value of <c>false</c>.
		/// </summary>
		public JsonBooleanNode() {
		}

		/// <summary>
		/// Initialize new boolean node.
		/// </summary>
		/// <param name="value">Initial value of node.</param>
		public JsonBooleanNode(bool value) {
			Value = value;
		}

		/// <summary>
		/// Gets or sets value of node.
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
		public override void WriteTo(JsonWriter writer) {
			writer.WriteValue(Value);
		}

	}

}