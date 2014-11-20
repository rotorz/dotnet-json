// Copyright (c) Rotorz Limited. All rights reserved.

using System;

namespace Rotorz.Json {

	/// <summary>
	/// Behaviour and output of <see cref="JsonWriter"/> can be customized by with custom
	/// settings by providing an instance of this class.
	/// </summary>
	public sealed class JsonWriterSettings {

		internal static JsonWriterSettings DefaultSettings { get; private set; }

		static JsonWriterSettings() {
			DefaultSettings = new JsonWriterSettings();
			DefaultSettings.IsReadOnly = true;
		}

		/// <summary>
		/// Initialize new <see cref="JsonWriterSettings"/> instance with default values.
		/// </summary>
		public JsonWriterSettings() {
			Reset();
		}

		private bool _indent;
		private string _indentChars;
		private string _newlineChars;

		internal bool IsReadOnly { get; set; }

		private void CheckReadOnly() {
			if (IsReadOnly)
				throw new JsonException("Cannot modify property because settings are now read-only.");
		}

		/// <summary>
		/// Gets or sets whether nested values should be indented within output.
		/// </summary>
		/// <exception cref="JsonException">
		/// If attempting to modify property after settings object has already been
		/// provided to a <see cref="JsonWriter"/> for usage.
		/// </exception>
		/// <seealso cref="IndentChars"/>
		public bool Indent {
			get { return _indent; }
			set {
				CheckReadOnly();
				_indent = value;
			}
		}

		/// <summary>
		/// Gets or sets string of characters which to used when indenting nested values
		/// within output.
		/// </summary>
		/// <remarks>
		/// <para>This property is only applicable when <see cref="Indent"/> is set to a
		/// value of <c>true</c>.</para>
		/// </remarks>
		/// <exception cref="JsonException">
		/// If attempting to modify property after settings object has already been
		/// provided to a <see cref="JsonWriter"/> for usage.
		/// </exception>
		/// <seealso cref="Indent"/>
		public string IndentChars {
			get { return _indentChars; }
			set {
				CheckReadOnly();
				if (value == null)
					throw new ArgumentNullException("value");
				_indentChars = value;
			}
		}

		/// <summary>
		/// Gets or sets string of characters to use when adding new lines.
		/// </summary>
		/// <exception cref="JsonException">
		/// If attempting to modify property after settings object has already been
		/// provided to a <see cref="JsonWriter"/> for usage.
		/// </exception>
		public string NewLineChars {
			get { return _newlineChars; }
			set {
				CheckReadOnly();
				if (value == null)
					throw new ArgumentNullException("value");
				_newlineChars = value;
			}
		}

		/// <summary>
		/// Reset to default setting values.
		/// </summary>
		/// <exception cref="JsonException">
		/// If attempting to modify property after settings object has already been
		/// provided to a <see cref="JsonWriter"/> for usage.
		/// </exception>
		public void Reset() {
			CheckReadOnly();

			_indent = true;
			_indentChars = "\t";
			_newlineChars = "\r\n";
		}

	}

}
