// Copyright (c) Rotorz Limited. All rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Rotorz.Json {

	/// <summary>
	/// Writes JSON encoded string and accepts several formatting settings. This class is
	/// particularly useful when manually writing JSON content.
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
	/// card.WriteTo(writer);
	/// var json = writer.ToString();
	/// ]]></code>
	/// </remarks>
	public sealed class JsonWriter : IJsonWriter {

		#region Factory Methods

		/// <summary>
		/// Creates a new <see cref="JsonWriter"/> instance with custom settings.
		/// </summary>
		/// <returns>
		/// New <see cref="JsonWriter"/> instance.
		/// </returns>
		public static JsonWriter Create() {
			return Create(new StringWriter(), JsonWriterSettings.DefaultSettings);
		}

		/// <summary>
		/// Creates a new <see cref="JsonWriter"/> instance with custom settings.
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
		/// provided <see cref="StringBuilder"/> instance with custom settings.
		/// </summary>
		/// <param name="builder">String builder.</param>
		/// <returns>
		/// New <see cref="JsonWriter"/> instance.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">
		/// If <paramref name="builder"/> is <c>null</c>.
		/// </exception>
		public static JsonWriter Create(StringBuilder builder) {
			if (builder == null)
				throw new ArgumentNullException("builder");

			return Create(new StringWriter(builder), JsonWriterSettings.DefaultSettings);
		}

		/// <summary>
		/// Creates a new <see cref="JsonWriter"/> instance and write content to the
		/// provided <see cref="StringBuilder"/> instance with custom settings.
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
		/// provided <see cref="StringBuilder"/> instance with custom settings.
		/// </summary>
		/// <param name="writer">Text writer.</param>
		/// <returns>
		/// New <see cref="JsonWriter"/> instance.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">
		/// If <paramref name="writer"/> is <c>null</c>.
		/// </exception>
		public static JsonWriter Create(TextWriter writer) {
			if (writer == null)
				throw new ArgumentNullException("writer");

			return new JsonWriter(writer, JsonWriterSettings.DefaultSettings);
		}

		/// <summary>
		/// Creates a new <see cref="JsonWriter"/> instance and write content to the
		/// provided <see cref="StringBuilder"/> instance with custom settings.
		/// </summary>
		/// <param name="writer">Text writer.</param>
		/// <param name="settings">Custom settings.</param>
		/// <returns>
		/// New <see cref="JsonWriter"/> instance.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">
		/// <list type="bullet">
		/// <item>If <paramref name="writer"/> is <c>null</c>.</item>
		/// <item>If <paramref name="settings"/> is <c>null</c>.</item>
		/// </list>
		/// </exception>
		public static JsonWriter Create(TextWriter writer, JsonWriterSettings settings) {
			if (writer == null)
				throw new ArgumentNullException("writer");
			if (settings == null)
				throw new ArgumentNullException("settings");

			return new JsonWriter(writer, settings);
		}

		#endregion

		private readonly TextWriter _writer;

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonWriter"/> class.
		/// </summary>
		/// <param name="writer">Text writer.</param>
		/// <param name="settings">Custom settings.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <list type="bullet">
		/// <item>If <paramref name="writer"/> is <c>null</c>.</item>
		/// <item>If <paramref name="settings"/> is <c>null</c>.</item>
		/// </list>
		/// </exception>
		private JsonWriter(TextWriter writer, JsonWriterSettings settings) {
			if (writer == null)
				throw new ArgumentNullException("writer");
			if (settings == null)
				throw new ArgumentNullException("settings");

			_writer = writer;
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

		private class ContextStack {

			private List<Context> _stack = new List<Context>();

			public void Push(Context context) {
				_stack.Add(context);
			}

			public Context Pop() {
				var top = Peek();
				_stack.RemoveAt(_stack.Count - 1);
				return top;
			}

			public Context Peek() {
				if (_stack.Count == 0)
					throw new InvalidOperationException("Cannot access next context when stack is empty.");
				return _stack[_stack.Count - 1];
			}

			public int Count {
				get { return _stack.Count; }
			}

		}

		private ContextStack _contextStack = new ContextStack();
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
		/// Write start of object '{'.
		/// </summary>
		/// <example>
		/// <para>This method is useful when outputting object notation:</para>
		/// <code language="csharp"><![CDATA[
		/// writer.WriteStartObject();
		/// writer.WritePropertyKey("Name");
		/// writer.WriteValue("Bob");
		/// writer.WriteEndObject();
		/// ]]></code>
		/// <para>Which generates the following JSON nodes:</para>
		/// <code><![CDATA[
		/// {
		///     "Name": "Bob"
		/// }
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

		/// <summary>
		/// Write end of object '}'.
		/// </summary>
		/// <seealso cref="WriteStartObject()"/>
		/// <seealso cref="WritePropertyKey(string)"/>
		public void WriteEndObject() {
			_contextStack.Pop();

			if (!_empty) {
				WriteLine();
				WriteIndent();
			}

			_writer.Write('}');

			DoEndValue();
		}

		/// <summary>
		/// Write property key; special characters are automatically escaped.
		/// </summary>
		/// <example>
		/// <para>This method is useful when outputting object notation:</para>
		/// <code language="csharp"><![CDATA[
		/// writer.WriteStartObject();
		/// writer.WritePropertyKey("Name");
		/// writer.WriteValue("Bob");
		/// writer.WriteEndObject();
		/// ]]></code>
		/// <para>Which generates the following JSON nodes:</para>
		/// <code><![CDATA[
		/// {
		///     "Name": "Bob"
		/// }
		/// ]]></code>
		/// </example>
		/// <param name="key">Key value.</param>
		/// <seealso cref="WriteStartObject()"/>
		/// <seealso cref="WritePropertyKey(string)"/>
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

		/// <summary>
		/// Write start of array marker '['.
		/// </summary>
		/// <example>
		/// <para>This method is useful when outputting arrays:</para>
		/// <code language="csharp"><![CDATA[
		/// writer.WriteStartArray();
		/// writer.WriteValue("Bob");
		/// writer.WriteValue("Jessica");
		/// writer.WriteValue("Sandra");
		/// writer.WriteEndArray();
		/// ]]></code>
		/// <para>Which generates the following JSON nodes:</para>
		/// <code><![CDATA[
		/// [
		///     "Bob",
		///     "Jessica",
		///     "Sandra"
		/// ]
		/// ]]></code>
		/// </example>
		/// <seealso cref="WriteEndArray"/>
		public void WriteStartArray() {
			DoBeginValue();

			_writer.Write('[');

			_contextStack.Push(Context.Array);
			_empty = true;
		}

		/// <summary>
		/// Write end of array marker ']'.
		/// </summary>
		/// <seealso cref="WriteStartArray"/>
		public void WriteEndArray() {
			_contextStack.Pop();
			
			if (!_empty) {
				WriteLine();
				WriteIndent();
			}

			_writer.Write(']');

			DoEndValue();
		}

		#endregion
		
		/// <inheritdoc/>
		public void WriteObject(IDictionary<string, JsonNode> collection) {
			if (collection == null)
				throw new ArgumentNullException("collection");

			WriteStartObject();

			foreach (var property in collection) {
				WritePropertyKey(property.Key);

				if (property.Value != null)
					property.Value.WriteTo(this);
				else
					WriteNull();
			}

			WriteEndObject();
		}

		/// <inheritdoc/>
		public void WriteArray(IList<JsonNode> collection) {
			if (collection == null)
				throw new ArgumentNullException("collection");

			WriteStartArray();

			foreach (var node in collection)
				if (node != null)
					node.WriteTo(this);
				else
					WriteNull();

			WriteEndArray();
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

			WriteStartArray();
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
				return sb.ToString();
			}

			return base.ToString();
		}

	}

}
