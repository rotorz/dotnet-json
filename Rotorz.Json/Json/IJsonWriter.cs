// Copyright (c) Rotorz Limited. All rights reserved.

using System.Collections.Generic;

namespace Rotorz.Json {

	/// <summary>
	/// Interface for writing <see cref="JsonNode"/> instances into some persisted state
	/// such as a JSON encoded file.
	/// </summary>
	public interface IJsonWriter {

		/// <summary>
		/// Writes a key/value collection of <see cref="JsonNode"/> instances.
		/// </summary>
		/// <param name="collection">The key/value collection of <see cref="JsonNode"/> instances.</param>
		/// <exception cref="System.ArgumentNullException">
		/// If <paramref name="collection"/> is <c>null</c>.
		/// </exception>
		void WriteObject(IDictionary<string, JsonNode> collection);

		/// <summary>
		/// Writes an ordered array of <see cref="JsonNode"/> instances.
		/// </summary>
		/// <param name="collection">The collection of <see cref="JsonNode"/> instances.</param>
		/// <exception cref="System.ArgumentNullException">
		/// If <paramref name="collection"/> is <c>null</c>.
		/// </exception>
		void WriteArray(IList<JsonNode> collection);

		/// <summary>
		/// Writes a value of <c>null</c>.
		/// </summary>
		void WriteNull();

		/// <summary>
		/// Writes an integer value.
		/// </summary>
		/// <param name="value">Value.</param>
		void WriteInteger(long value);

		/// <summary>
		/// Writes a double-precision floating point value.
		/// </summary>
		/// <param name="value">Value.</param>
		void WriteDouble(double value);

		/// <summary>
		/// Writes a string literal; special characters are automatically escaped.
		/// </summary>
		/// <remarks>
		/// <para>Should write an empty string if <paramref name="value"/> is <c>null</c>.</para>
		/// </remarks>
		/// <param name="value">Content for string.</param>
		void WriteString(string value);

		/// <summary>
		/// Writes a boolean value of <c>true</c> or <c>false</c>.
		/// </summary>
		/// <param name="value">Content for string.</param>
		void WriteBoolean(bool value);

		/// <summary>
		/// Writes an array of binary data.
		/// </summary>
		/// <param name="value">Array of zero or more bytes.</param>
		/// <exception cref="System.ArgumentNullException">
		/// If <paramref name="value"/> is <c>null</c>.
		/// </exception>
		void WriteBinary(byte[] value);

	}

}
