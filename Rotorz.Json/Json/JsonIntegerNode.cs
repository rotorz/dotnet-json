﻿// Copyright (c) Rotorz Limited. All rights reserved.

using System;
using System.Globalization;

namespace Rotorz.Json {

	/// <summary>
	/// Node holding a 64-bit integer value.
	/// </summary>
	public sealed class JsonIntegerNode : JsonNode {

		/// <summary>
		/// Initialize new integer node with an initial value of zero.
		/// </summary>
		public JsonIntegerNode() {
		}

		/// <summary>
		/// Initialize new integer node.
		/// </summary>
		/// <param name="value">Initial value of node.</param>
		public JsonIntegerNode(long value) {
			Value = value;
		}

		/// <summary>
		/// Gets or sets value of node.
		/// </summary>
		/// <seealso cref="UnsignedValue"/>
		public long Value { get; set; }

		/// <summary>
		/// Gets or sets unsigned value of node. This property is only useful
		/// when using the unsigned long (ulong) data type.
		/// </summary>
		/// <seealso cref="Value"/>
		public ulong UnsignedValue {
			get { return (ulong)Value; }
			set { Value = (long)value; }
		}

		/// <inheritdoc/>
		public override JsonNode Clone() {
			return new JsonIntegerNode(Value);
		}

		/// <inheritdoc/>
		public override string ToString() {
			return Value.ToString(CultureInfo.InvariantCulture);
		}

		/// <inheritdoc/>
		public override object ToObject(Type type) {
			if (type == null)
				throw new ArgumentNullException("type");

			if (type.IsEnum)
				return Convert.ChangeType(Value, Enum.GetUnderlyingType(type));
			else
				return Convert.ChangeType(Value, type);
		}

		/// <inheritdoc/>
		public override void WriteTo(JsonWriter writer) {
			writer.WriteValue(Value);
		}

	}

}
