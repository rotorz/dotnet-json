// Copyright (c) Rotorz Limited. All rights reserved.

using Rotorz.Json.Internal;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Rotorz.Json {

	/// <summary>
	/// A <see cref="IJsonWriter"/> that can be used to write JSON encoded data which
	/// accepts formatting settings. This class can be used to manually write JSON
	/// encoded data without the need to instantiate any <see cref="JsonNode"/> instances.
	/// </summary>
	/// <remarks>
	/// <para>Each <see cref="JsonNode"/> has as custom implementation of <see cref="JsonNode.ToString()"/>
	/// and <see cref="JsonNode.ToString(JsonWriterSettings)"/> which produce a JSON encoded strings:</para>
	/// <code language="csharp"><![CDATA[
	/// var card = new JsonObjectNode();
	/// card["Name"] = "Jessica";
	/// card["Age"] = 24;
	/// string json = card.ToString();
	/// ]]></code>
	/// <para>Alternative the more verbose implementation would be the following:</para>
	/// <code language="csharp"><![CDATA[
	/// var writer = JsonWriter.Create();
	/// card.Write(writer);
	/// var json = writer.ToString();
	/// ]]></code>
	/// </remarks>
	/// <example>
	/// <para>The following code demonstrates how to manually write JSON data:</para>
	/// <code language="csharp"><![CDATA[
	/// var writer = JsonWriter.Create();
	/// writer.WriteStartObject();
	/// writer.WritePropertyKey("Name");
	/// writer.WriteString("Jessica");
	/// writer.WritePropertyKey("Age");
	/// writer.WriteInteger(24);
	/// writer.WriteEndObject();
	/// ]]></code>
	/// </example>
	public sealed class JsonWriter : IJsonWriter {

		#region Factory Methods

		/// <summary>
		/// Creates a new <see cref="JsonWriter"/> instance with the default formatting.
		/// </summary>
		/// <returns>
		/// New <see cref="JsonWriter"/> instance.
		/// </returns>
		public static JsonWriter Create() {
			return Create(JsonWriterSettings.DefaultSettings);
		}

		/// <summary>
		/// Creates a new <see cref="JsonWriter"/> instance with custom formatting.
		/// </summary>
		/// <param name="settings">Custom settings.</param>
		/// <returns>
		/// New <see cref="JsonWriter"/> instance.
		/// </returns>
		public static JsonWriter Create(JsonWriterSettings settings) {
			return Create(new StringWriter(), settings);
		}

		/// <summary>
		/// Creates a new <see cref="JsonWriter"/> instance and write content to the
		/// provided <see cref="StringBuilder"/> with the default formatting.
		/// </summary>
		/// <param name="builder">String builder.</param>
		/// <returns>
		/// New <see cref="JsonWriter"/> instance.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">
		/// If <paramref name="builder"/> is <c>null</c>.
		/// </exception>
		public static JsonWriter Create(StringBuilder builder) {
			return Create(builder, JsonWriterSettings.DefaultSettings);
		}

		/// <summary>
		/// Creates a new <see cref="JsonWriter"/> instance and write content to the
		/// provided <see cref="StringBuilder"/> with custom formatting.
		/// </summary>
		/// <param name="builder">String builder.</param>
		/// <param name="settings">Custom settings.</param>
		/// <returns>
		/// New <see cref="JsonWriter"/> instance.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">
		/// <list type="bullet">
		/// <item>If <paramref name="builder"/> is <c>null</c>.</item>
		/// <item>If <paramref name="settings"/> is <c>null</c>.</item>
		/// </list>
		/// </exception>
		public static JsonWriter Create(StringBuilder builder, JsonWriterSettings settings) {
			if (builder == null)
				throw new ArgumentNullException("builder");
			if (settings == null)
				throw new ArgumentNullException("settings");

			return Create(new StringWriter(builder), settings);
		}

		/// <summary>
		/// Creates a new <see cref="JsonWriter"/> instance and write content to the
		/// provided <see cref="Stream"/> with the default formatting.
		/// </summary>
		/// <param name="stream">Stream that data will be written to.</param>
		/// <returns>
		/// New <see cref="JsonWriter"/> instance.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">
		/// If <paramref name="stream"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// If <paramref name="stream"/> is not writable.
		/// </exception>
		public static JsonWriter Create(Stream stream) {
			return Create(stream, JsonWriterSettings.DefaultSettings);
		}

		/// <summary>
		/// Creates a new <see cref="JsonWriter"/> instance and write content to the
		/// provided <see cref="Stream"/> with custom formatting.
		/// </summary>
		/// <param name="stream">Stream that data will be written to.</param>
		/// <param name="settings">Custom settings.</param>
		/// <returns>
		/// New <see cref="JsonWriter"/> instance.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">
		/// <list type="bullet">
		/// <item>If <paramref name="stream"/> is <c>null</c>.</item>
		/// <item>If <paramref name="settings"/> is <c>null</c>.</item>
		/// </list>
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// If <paramref name="stream"/> is not writable.
		/// </exception>
		public static JsonWriter Create(Stream stream, JsonWriterSettings settings) {
			if (stream == null)
				throw new ArgumentNullException("stream");
			if (!stream.CanWrite)
				throw new ArgumentException("Cannot write to stream.", "stream");
			if (settings == null)
				throw new ArgumentNullException("settings");

			return new JsonWriter(new StreamWriter(stream), settings);
		}

		/// <summary>
		/// Creates a new <see cref="JsonWriter"/> instance and write content to the
		/// provided <see cref="StringBuilder"/> with the default formatting.
		/// </summary>
		/// <param name="textWriter">Text writer.</param>
		/// <returns>
		/// New <see cref="JsonWriter"/> instance.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">
		/// If <paramref name="textWriter"/> is <c>null</c>.
		/// </exception>
		public static JsonWriter Create(TextWriter textWriter) {
			return Create(textWriter, JsonWriterSettings.DefaultSettings);
		}

		/// <summary>
		/// Creates a new <see cref="JsonWriter"/> instance and write content to the
		/// provided <see cref="StringBuilder"/> with custom formatting.
		/// </summary>
		/// <param name="textWriter">Text writer.</param>
		/// <param name="settings">Custom settings.</param>
		/// <returns>
		/// New <see cref="JsonWriter"/> instance.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">
		/// <list type="bullet">
		/// <item>If <paramref name="textWriter"/> is <c>null</c>.</item>
		/// <item>If <paramref name="settings"/> is <c>null</c>.</item>
		/// </list>
		/// </exception>
		public static JsonWriter Create(TextWriter textWriter, JsonWriterSettings settings) {
			if (textWriter == null)
				throw new ArgumentNullException("textWriter");
			if (settings == null)
				throw new ArgumentNullException("settings");

			return new JsonWriter(textWriter, settings);
		}

		#endregion

		private readonly TextWriter _writer;

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonWriter"/> class.
		/// </summary>
		/// <param name="textWriter">Text writer.</param>
		/// <param name="settings">Custom settings.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <list type="bullet">
		/// <item>If <paramref name="textWriter"/> is <c>null</c>.</item>
		/// <item>If <paramref name="settings"/> is <c>null</c>.</item>
		/// </list>
		/// </exception>
		private JsonWriter(TextWriter textWriter, JsonWriterSettings settings) {
			if (textWriter == null)
				throw new ArgumentNullException("textWriter");
			if (settings == null)
				throw new ArgumentNullException("settings");

			_writer = textWriter;
			Settings = settings;

			settings.IsReadOnly = true;

			_contextStack.Push(Context.Root);
		}

		#region Low Level Writing

		private enum Context {
			Root,
			Object,
			Array,
		}

		private SimpleStack<Context> _contextStack = new SimpleStack<Context>();
		private bool _empty = true;

		/// <summary>
		/// Gets writer settings which are used to control formatting of output. Setting
		/// properties become read-only once assigned to a <see cref="JsonWriter"/>
		/// instance.
		/// </summary>
		public JsonWriterSettings Settings { get; private set; }

		private void WriteIndent() {
			if (Settings.Indent == true) {
				int count = _contextStack.Count;
				while (--count > 0)
					_writer.Write(Settings.IndentChars);
			}
		}

		private void WriteLine() {
			if (Settings.Indent == true)
				_writer.Write(Settings.NewLineChars);
		}

		private void WriteSpace() {
			if (Settings.Indent == true)
				_writer.Write(" ");
		}

		private void WriteEscapedLiteral(string value) {
			if (value == null)
				return;

			for (int i = 0; i < value.Length; ++i) {
				char c = value[i];
				switch (c) {
					case '\"':
						_writer.Write("\\\"");
						break;
					case '\\':
						_writer.Write("\\\\");
						break;
					case '/':
						_writer.Write("\\/");
						break;
					case '\b':
						_writer.Write("\\b");
						break;
					case '\f':
						_writer.Write("\\f");
						break;
					case '\n':
						_writer.Write("\\n");
						break;
					case '\r':
						_writer.Write("\\r");
						break;
					case '\t':
						_writer.Write("\\t");
						break;
					default:
						_writer.Write(c);
						break;
				}
			}
		}

		private void DoBeginValue() {
			if (!_empty)
				_writer.Write(',');

			if (_contextStack.Peek() == Context.Array) {
				WriteLine();
				WriteIndent();
			}
		}

		private void DoEndValue() {
			_empty = false;
		}

		/// <summary>
		/// Write raw JSON value.
		/// </summary>
		/// <remarks>
		/// <para>Whitespace is still automatically added when specified; for instance,
		/// value will be indented if <see cref="JsonWriterSettings.Indent"/> is set to a
		/// value of <c>true</c>.</para>
		/// <para>If <paramref name="content"/> is a value of <c>null</c> then the value
		/// "null" is written to output.</para>
		/// </remarks>
		/// <param name="content">String to be written verbatim.</param>
		private void WriteValueRaw(string content) {
			DoBeginValue();

			_writer.Write(content ?? "null");

			DoEndValue();
		}

		#endregion

		/// <inheritdoc/>
		public void WriteStartObject(int propertyCount) {
			if (propertyCount < 0)
				throw new ArgumentOutOfRangeException("propertyCount", "Cannot be a negative value.");

			WriteStartObject();
		}

		/// <summary>
		/// Writes the start of an object literal.
		/// </summary>
		/// <example>
		/// <code><![CDATA[
		/// {
		///     "Name": "Bob"
		/// }
		/// ]]></code>
		/// <para>The above object literal is represented by the following writer logic:</para>
		/// <code language="csharp"><![CDATA[
		/// writer.WriteStartObject();
		/// writer.WritePropertyKey("Name");
		/// writer.WriteValue("Bob");
		/// writer.WriteEndObject();
		/// ]]></code>
		/// </example>
		/// <seealso cref="WritePropertyKey(string)"/>
		/// <seealso cref="WriteEndObject()"/>
		public void WriteStartObject() {
			DoBeginValue();

			_writer.Write('{');

			_contextStack.Push(Context.Object);
			_empty = true;
		}

		/// <inheritdoc/>
		public void WritePropertyKey(string key) {
			DoBeginValue();

			WriteLine();
			WriteIndent();

			_writer.Write("\"");
			WriteEscapedLiteral(key);
			_writer.Write("\":");

			WriteSpace();

			_empty = true;
		}

		/// <inheritdoc/>
		public void WriteEndObject() {
			_contextStack.Pop();

			if (!_empty) {
				WriteLine();
				WriteIndent();
			}

			_writer.Write('}');

			DoEndValue();
		}

		/// <inheritdoc/>
		public void WriteStartArray(int arrayLength) {
			if (arrayLength < 0)
				throw new ArgumentOutOfRangeException("arrayLength", "Cannot be a negative value.");

			WriteStartArray();
		}

		/// <summary>
		/// Writes the start of an array literal.
		/// </summary>
		/// <example>
		/// <code><![CDATA[
		/// [
		///     "Bob",
		///     "Jessica",
		///     "Sandra"
		/// ]
		/// ]]></code>
		/// <para>The above array literal is represented by the following writer logic:</para>
		/// <code language="csharp"><![CDATA[
		/// writer.WriteStartArray();
		/// writer.WriteValue("Bob");
		/// writer.WriteValue("Jessica");
		/// writer.WriteValue("Sandra");
		/// writer.WriteEndArray();
		/// ]]></code>
		/// </example>
		/// <seealso cref="WriteEndArray()"/>
		public void WriteStartArray() {
			DoBeginValue();

			_writer.Write('[');

			_contextStack.Push(Context.Array);
			_empty = true;
		}

		/// <inheritdoc/>
		public void WriteEndArray() {
			_contextStack.Pop();
			
			if (!_empty) {
				WriteLine();
				WriteIndent();
			}

			_writer.Write(']');

			DoEndValue();
		}

		/// <inheritdoc/>
		public void WriteNull() {
			WriteValueRaw("null");
		}

		/// <inheritdoc/>
		public void WriteInteger(long value) {
			WriteValueRaw(value.ToString(CultureInfo.InvariantCulture));
		}

		/// <inheritdoc/>
		public void WriteDouble(double value) {
			WriteValueRaw(JsonFormattingUtility.DoubleToString(value));
		}

		/// <inheritdoc/>
		public void WriteString(string value) {
			DoBeginValue();

			_writer.Write('"');
			WriteEscapedLiteral(value);
			_writer.Write('"');

			DoEndValue();
		}

		/// <inheritdoc/>
		public void WriteBoolean(bool value) {
			WriteValueRaw(value ? "true" : "false");
		}

		/// <inheritdoc/>
		public void WriteBinary(byte[] value) {
			if (value == null)
				throw new ArgumentNullException("value");

			WriteStartArray(value.Length);
			for (int i = 0; i < value.Length; ++i)
				WriteInteger(value[i]);
			WriteEndArray();
		}

		/// <summary>
		/// Get current state of JSON encoded string which is being written.
		/// </summary>
		/// <returns>
		/// The JSON encoded string when writing using a <see cref="StringWriter"/>.
		/// </returns>
		public override string ToString() {
			var stringWriter = _writer as StringWriter;
			if (stringWriter != null) {
				var sb = stringWriter.GetStringBuilder();
				stringWriter.Flush();

				// Return a value of "null" if output was empty.
				var result = sb.ToString();
				return !string.IsNullOrEmpty(result) ? result : "null";
			}

			return base.ToString();
		}

	}

}
