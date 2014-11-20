// Copyright (c) Rotorz Limited. All rights reserved.

using System;

namespace Rotorz.Json {

	/// <summary>
	/// Node holding a string value.
	/// </summary>
	/// <remarks>
	/// <para>String node cannot contain a value of <c>null</c>; instead an empty
	/// string is automatically assumed if a value of <c>null</c> is provided.</para>
	/// </remarks>
	public sealed class JsonStringNode : JsonNode {

		/// <summary>
		/// Initialize new string node with empty string.
		/// </summary>
		public JsonStringNode() : this("") {
		}

		/// <summary>
		/// Initialize new string node.
		/// </summary>
		/// <param name="value">Initial value of node.</param>
		public JsonStringNode(string value) {
			_value = value;
		}

		private string _value;

		/// <summary>
		/// Gets or sets value of node.
		/// </summary>
		/// <remarks>
		/// <para>It is not possible to assign a value of <c>null</c> to a string
		/// node; instead an empty string is automatically assumed.</para>
		/// </remarks>
		public string Value {
			get { return _value; }
			set { _value = value != null ? value : ""; }
		}

		/// <inheritdoc/>
		public override JsonNode Clone() {
			return new JsonStringNode(Value);
		}

		/// <inheritdoc/>
		public override string ToString() {
			return Value == ""
				? "\"\""
				: base.ToString();
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
