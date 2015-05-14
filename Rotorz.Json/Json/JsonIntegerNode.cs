// Copyright (c) Rotorz Limited. All rights reserved.

using System;
using System.Globalization;

namespace Rotorz.Json {

	/// <summary>
	/// Node holding a 64-bit integer value.
	/// </summary>
	public sealed class JsonIntegerNode : JsonNode {

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonIntegerNode"/> class with a
		/// value of zero.
		/// </summary>
		public JsonIntegerNode() {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonIntegerNode"/> class with
		/// the specified integer value.
		/// </summary>
		/// <param name="value">Initial value of node.</param>
		public JsonIntegerNode(long value) {
			Value = value;
		}

		/// <summary>
		/// Gets or sets value of the node.
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
		public override object ConvertTo(Type type) {
			if (type == null)
				throw new ArgumentNullException("type");

			if (type.IsEnum)
				return Convert.ChangeType(Value, Enum.GetUnderlyingType(type));
			else
				return Convert.ChangeType(Value, type);
		}

		/// <inheritdoc/>
		public override void Write(IJsonWriter writer) {
			writer.WriteInteger(Value);
		}

	}

}
