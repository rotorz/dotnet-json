﻿// Copyright (c) 2014 Rotorz Limited. All rights reserved.

using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Rotorz.Json {

	/// <summary>
	/// Parse input stream of JSON encoded text and generate sequence of zero or more
	/// <see cref="JsonNode"/> instances.
	/// </summary>
	/// <remarks>
	/// <para>This class was implemented from the specification presented on the
	/// <a href="http://json.org">http://json.org</a> website.</para>
	/// <para>One interesting thing about this specification is that it doesn't
	/// identify a standardized way to represent floating-point values of NaN,
	/// -Infinity and Infinity.</para>
	/// <para>According to various forum and Q&amp;A postings on the Internet, a
	/// number of security issues arise when using JavaScript compatible constant
	/// values because in some cases hackers are able to inject malicious code
	/// into the application.</para>
	/// <para>To avoid this vulnerability such values are encoded as simple strings
	/// which can then be detected when deserializing JSON encoded data. Fortunately
	/// <c>System.Convert.ChangeType</c> can be used to deal with this implementation
	/// specific.</para>
	/// </remarks>
	internal sealed class JsonParser {

		/// <summary>
		/// Create new <see cref="JsonParser"/> instance from a stream.
		/// </summary>
		/// <remarks>
		/// <para>User code should close provided stream when no longer required
		/// after JSON encoded content has been parsed; this can be accomplished with
		/// the <c>using</c> construct:</para>
		/// <code language="csharp"><![CDATA[
		/// JsonNode result;
		/// using (var fs = new FileStream(@"C:\TestFile.json", FileMode.Open, FileAccess.Read)) {
		///     var parser = JsonParser.Create(fs);
		///     result = parser.Parse();
		/// }
		/// ]]></code>
		/// <para>This function is provided for convenience since it's implementation
		/// is merely as follows:</para>
		/// <code language="csharp"><![CDATA[
		/// public static JsonParser Create(Stream stream) {
		///     return new JsonParser(new StreamReader(stream));
		/// }
		/// ]]></code>
		/// </remarks>
		/// <param name="stream">Stream.</param>
		/// <returns>
		/// New <see cref="JsonParser"/> instance.
		/// </returns>
		/// <seealso cref="Parse()"/>
		public static JsonParser Create(Stream stream) {
			return new JsonParser(new StreamReader(stream));
		}

		/// <summary>
		/// Create new <see cref="JsonParser"/> instance from a text reader. This allows
		/// JSON encoded text to be parsed from a variety of sources including strings,
		/// files, etc.
		/// </summary>
		/// <remarks>
		/// <para>User code should close provided stream when no longer required
		/// after JSON encoded content has been parsed; this can be accomplished with
		/// the <c>using</c> construct:</para>
		/// <code language="csharp"><![CDATA[
		/// JsonNode result;
		/// using (var fs = new FileStream(@"C:\TestFile.json", FileMode.Open, FileAccess.Read)) {
		///     var reader = new StreamReader(fs);
		///     var parser = JsonParser.Create(reader);
		///     result = parser.Parse();
		/// }
		/// ]]></code>
		/// </remarks>
		/// <param name="reader">Text reader.</param>
		/// <returns>
		/// New <see cref="JsonParser"/> instance.
		/// </returns>
		/// <seealso cref="Parse()"/>
		public static JsonParser Create(TextReader reader) {
			return new JsonParser(reader);
		}

		/// <summary>
		/// Initialize new <see cref="JsonParser"/> instance.
		/// </summary>
		/// <param name="reader">Text reader.</param>
		private JsonParser(TextReader reader) {
			_jsonReader = reader;

			ReadBuffer();
			SkipWhitespace();
		}

		#region Text Reader

		private TextReader _jsonReader;

		/// <summary>
		/// Buffer holds a maximum of 2048 characters from input stream.
		/// </summary>
		private char[] _buffer;
		/// <summary>
		/// Indicates current size of buffer (used size).
		/// </summary>
		private int _bufferSize;
		/// <summary>
		/// Current position in <see cref="_buffer"/>.
		/// </summary>
		private int _bufferPos;

		/// <summary>
		/// Indicates phase of reading JSON encoded text.
		/// </summary>
		private enum ReadPhase {
			/// <summary>
			/// Reading from input stream using text reader.
			/// </summary>
			ReadingStream,
			/// <summary>
			/// Has reached end of input stream using text reader but buffered input
			/// still needs to be parsed.
			/// </summary>
			HasReachedEndOfStream,
			/// <summary>
			/// Has reached end of input; any further requests will yield '\0'.
			/// </summary>
			HasReachedEndOfInput
		}

		/// <summary>
		/// Current reading phase of parser.
		/// </summary>
		private ReadPhase _phase = ReadPhase.ReadingStream;

		private void ReadBuffer() {
			if (_phase == ReadPhase.HasReachedEndOfInput)
				throw new InvalidOperationException("Attempting to read buffer after end of input has been reached.");

			char[] buffer = _buffer;
			int trailingCount;

			if (buffer == null) {
				// Create buffer for reading input.
				buffer = _buffer = new char[2048];
				trailingCount = 0;
			}
			else {
				// Move any trailing unused characters to start of buffer.
				trailingCount = _bufferSize - _bufferPos;
				for (int i = 0; i < trailingCount; ++i)
					buffer[i] = buffer[_bufferPos + i];

				// Zero-out any excess characters.
				if (_phase == ReadPhase.HasReachedEndOfStream) {
					for (int i = trailingCount; i < buffer.Length; ++i)
						buffer[i] = default(char);

					_phase = ReadPhase.HasReachedEndOfInput;
				}
			}

			if (_phase == ReadPhase.ReadingStream) {
				_bufferSize = trailingCount + _jsonReader.ReadBlock(buffer, trailingCount, buffer.Length - trailingCount);

				// Has end of input stream been reached?
				if (_bufferSize < buffer.Length) {
					_phase = ReadPhase.HasReachedEndOfStream;

					// Zero-out any excess characters.
					for (int i = _bufferSize; i < buffer.Length; ++i)
						buffer[i] = default(char);
				}
			}

			if (!_initLineEnding) {
				// Use either '\r' or '\n' to quickly detect line endings for
				// maintaining line number and position. Initially we do not know
				// which type of line ending characters are being used. If a file
				// contains mixed styles then line number and position feedback
				// will be inaccurate when syntax errors are encountered.
				for (int i = 0; i < buffer.Length - 1; ++i) {
					char c = buffer[i];
					if (c == '\r' || c == '\n') {
						_initLineEnding = true;
						_lineEnding = c;
						if (c == '\r' && buffer[i + 1] == '\n')
							_lineEndingLength = 2;
						break;
					}
				}
			}

			_bufferPos = 0;
		}

		/// <summary>
		/// Peeks at next character in buffer but does not advance buffer position.
		/// </summary>
		/// <returns>
		/// The next character if further input remains; otherwise, a value of '\0'
		/// is returned.
		/// </returns>
		/// <seealso cref="Peek(int)"/>
		/// <seealso cref="Accept(int)"/>
		public char Peek() {
			return _buffer[_bufferPos];
		}

		/// <summary>
		/// Peeks at nth next character in buffer but does not advance buffer position.
		/// </summary>
		/// <param name="offset">Zero-based offset at which to peek. Specifying a value
		/// of 1 is no different to using <see cref="Peek()"/> instead.</param>
		/// <returns>
		/// The nth next character if further input remains; otherwise, a value of '\0'
		/// is returned.
		/// </returns>
		/// <seealso cref="Peek()"/>
		/// <seealso cref="Accept(int)"/>
		public char Peek(int offset) {
			return _buffer[_bufferPos + offset];
		}

		// Maximum lookahead must be large enough to hold the largest value which can
		// be matched when parsing JSON. At the moment this is a value of 6 since
		// the unicode character escape sequence is "\u####" (6 characters).
		private const int MaximumLookahead = 6;

		private bool _hasReachedEnd;

		/// <summary>
		/// Gets a value indicating whether end of input has been reached.
		/// </summary>
		private bool HasReachedEnd {
			get { return _hasReachedEnd || Peek() == '\0'; }
		}

		/// <summary>
		/// Current line number in original input.
		/// </summary>
		private int _lineNumber = 1;
		/// <summary>
		/// Current position in line of original input.
		/// </summary>
		private int _linePosition = 0;

		private bool _initLineEnding;
		private char _lineEnding;
		private int _lineEndingLength = 1;

		private char ReadChar() {
			int remainingCharsInBuffer = _bufferSize - _bufferPos;
			if (remainingCharsInBuffer < MaximumLookahead && _phase != ReadPhase.HasReachedEndOfInput) {
				// Insufficient characters for lookahead, continue reading input buffer.
				ReadBuffer();
			}

			// Has end of input been reached?
			if (_bufferPos >= _bufferSize) {
				_hasReachedEnd = true;
				return default(char);
			}

			char c = Peek();
			++_bufferPos;
			++_linePosition;

			if (_lineEnding == c) {
				++_lineNumber;
				_linePosition = 1 - _lineEndingLength;
			}

			return c;
		}

		/// <summary>
		/// Accept one or more characters from input and advanced to next
		/// position in buffer.
		/// </summary>
		/// <param name="count">Number of input characters to accept.</param>
		/// <seealso cref="Peek()"/>
		/// <seealso cref="Peek(int)"/>
		private void Accept(int count = 1) {
			while (count-- > 0)
				ReadChar();
		}
		
		#endregion

		/// <summary>
		/// Skip whitespace by accepting all input characters which contain
		/// spaces, new lines and indentation type characters.
		/// </summary>
		private void SkipWhitespace() {
			while (char.IsWhiteSpace(Peek()))
				Accept();
		}

		/// <summary>
		/// Match string in input by peeking at characters ahead of the current
		/// buffer position. This is useful for detecting special keywords such
		/// as "true", "false" and "null".
		/// </summary>
		/// <remarks>
		/// <para>This method does not advanced current position within buffer;
		/// instead <see cref="Accept(int)"/> should be called by passing in the
		/// length of the matched string.</para>
		/// </remarks>
		/// <param name="match">String which is to be matched.</param>
		/// <returns>
		/// A value of <c>true</c> if string was matched; otherwise, a value of <c>false</c>.
		/// </returns>
		/// <seealso cref="Accept(int)"/>
		private bool MatchString(string match) {
			int length = Math.Min(MaximumLookahead, match.Length);
			for (int i = 0; i < length; ++i)
				if (Peek(i) != match[i])
					return false;
			return true;
		}

		/// <summary>
		/// Parse input JSON encoded content.
		/// </summary>
		/// <returns>
		/// A <see cref="JsonNode"/> instance of the applicable type; otherwise, a
		/// value of <c>null</c> if input content was either empty or consisted
		/// entirely of whitespace.
		/// </returns>
		/// <exception cref="JsonParserException">
		/// Thrown if a syntax error was encountered whilst attempting to parse
		/// input content. Exception contains identifies the source of the error
		/// by providing the line number and position.
		/// </exception>
		public JsonNode Parse() {
			SkipWhitespace();
			if (!HasReachedEnd)
				return ParseValue();
			return null;
		}

		/// <summary>
		/// Parse value node (null, integer, double, boolean, string, array or object).
		/// </summary>
		/// <returns>
		/// New <see cref="JsonNode"/> holding value; returns a value of <c>null</c>
		/// when <c>null</c> is detected in input.
		/// </returns>
		/// <exception cref="JsonParserException">
		/// Thrown if a syntax error was encountered whilst attempting to parse
		/// input content. Exception contains identifies the source of the error
		/// by providing the line number and position.
		/// </exception>
		private JsonNode ParseValue() {
			if (MatchString("true")) {
				Accept(4);
				return new JsonBooleanNode(true);
			}
			else if (MatchString("false")) {
				Accept(5);
				return new JsonBooleanNode(false);
			}
			else if (MatchString("null")) {
				Accept(4);
				return null;
			}
			else {
				char c = Peek();

				if (c == '-' || (c >= '0' && c <= '9'))
					return ParseNumeric();

				switch (c) {
					case '"':
						return new JsonStringNode(ParseStringLiteral("String"));
					case '[':
						return ParseArray();
					case '{':
						return ParseObject();
					default:
						if (HasReachedEnd)
							throw new JsonParserException("Unexpected end of input; expected value.", _lineNumber, _linePosition);
						else
							throw new JsonParserException("Encountered unexpected input '" + c + "'", _lineNumber, _linePosition);
				}
			}
		}

		private StringBuilder _stringLiteral = new StringBuilder();
		private StringBuilder _unicodeSequence = new StringBuilder();

		/// <summary>
		/// Parse string literal for value or property key.
		/// </summary>
		/// <remarks>
		/// <para>Character escape sequences are automatically evaluated whilst parsing
		/// string literal.</para>
		/// </remarks>
		/// <param name="context">Context of literal; indicates whether literal is being
		/// parsed for a value or for a property key. This argument helps to provide more
		/// meaningful syntax error messages.</param>
		/// <returns>
		/// The resulting string.
		/// </returns>
		/// <exception cref="JsonParserException">
		/// Thrown if a syntax error was encountered whilst attempting to parse
		/// input content. Exception contains identifies the source of the error
		/// by providing the line number and position.
		/// </exception>
		private string ParseStringLiteral(string context) {
			_stringLiteral.Length = 0;

			Accept();

			char c;

			while (!HasReachedEnd) {
				switch (Peek()) {
					case '"':
						Accept();
						return _stringLiteral.ToString();

					case '\\':
						Accept();
						if (HasReachedEnd)
							break;

						switch (Peek()) {
							case '"':
							case '\\':
							case '/':
								_stringLiteral.Append(ReadChar());
								break;
							case 'b':
								_stringLiteral.Append('\b');
								Accept();
								break;
							case 'f':
								_stringLiteral.Append('\f');
								Accept();
								break;
							case 'n':
								_stringLiteral.Append('\n');
								Accept();
								break;
							case 'r':
								_stringLiteral.Append('\r');
								Accept();
								break;
							case 't':
								_stringLiteral.Append('\t');
								Accept();
								break;
							case 'u':
								Accept();

								_unicodeSequence.Length = 0;
								for (int i = 0; i < 4; ++i) {
									if (HasReachedEnd)
										throw new JsonParserException("Encountered unicode character escape sequence.", _lineNumber, _linePosition);

									c = ReadChar();
									if (c >= '0' && c <= '9' || c >= 'A' && c <= 'F' || c >= 'a' && c <= 'f')
										_unicodeSequence.Append(c);
									else
										throw new JsonParserException("Encountered illegal character '" + c + "' in unicode escape sequence.", _lineNumber, _linePosition);
								}

								_stringLiteral.Append((char)int.Parse(_unicodeSequence.ToString(), NumberStyles.HexNumber));
								break;

							default:
								throw new JsonParserException("Encountered illegal escape sequence '\\" + Peek() + "'.", _lineNumber, _linePosition);
						}
						break;

					default:
						c = ReadChar();
						CheckStringCharacter(c, context);
						_stringLiteral.Append(c);
						break;
				}
			}

			throw new JsonParserException("Expected '\"' but reached end of input.", _lineNumber, _linePosition);
		}

		/// <summary>
		/// Check input character to determine whether it is permitted within a string
		/// literal. For instance, control characters are not permitted in JSON encoded
		/// string literals, instead an escape sequence must be used '\n'.
		/// </summary>
		/// <param name="c">Candidate character.</param>
		/// <param name="context">Context of literal; indicates whether literal is being
		/// parsed for a value or for a property key. This argument helps to provide more
		/// meaningful syntax error messages.</param>
		/// <exception cref="JsonParserException">
		/// Thrown if a syntax error was encountered whilst attempting to parse
		/// input content. Exception contains identifies the source of the error
		/// by providing the line number and position.
		/// </exception>
		private void CheckStringCharacter(char c, string context) {
			if (char.IsControl(c)) {
				if (c == '\r' || c == '\n')
					throw new JsonParserException(context + " cannot span multiple lines. Consider using escape sequence '\n' instead.", _lineNumber, _linePosition);
				else if (c == '\t')
					throw new JsonParserException(context + " cannot include tab character. Consider using escape sequence '\t' instead.", _lineNumber, _linePosition);
				else
					throw new JsonParserException(context + " cannot contain control character. Consider using escape sequence instead", _lineNumber, _linePosition);
			}
		}

		/// <summary>
		/// Parse JSON array containing zero-or-more values.
		/// </summary>
		/// <returns>
		/// The new <see cref="JsonNode"/> instance.
		/// </returns>
		/// <exception cref="JsonParserException">
		/// Thrown if a syntax error was encountered whilst attempting to parse
		/// input content. Exception contains identifies the source of the error
		/// by providing the line number and position.
		/// </exception>
		private JsonNode ParseArray() {
			Accept();

			SkipWhitespace();

			var node = new JsonArrayNode();
			while (!HasReachedEnd) {
				if (Peek() == ']') {
					Accept();
					return node;
				}
				else if (Peek() == ',' && node.Count != 0) {
					Accept();

					SkipWhitespace();
					if (HasReachedEnd)
						break;
				}

				node.Add(ParseValue());
				SkipWhitespace();
			}

			throw new JsonParserException("Expected ']' but reached end of input.", _lineNumber, _linePosition);
		}

		/// <summary>
		/// Parse JSON object which comprises of zero-or-more named properties.
		/// </summary>
		/// <returns>
		/// The new <see cref="JsonNode"/> instance.
		/// </returns>
		/// <exception cref="JsonParserException">
		/// Thrown if a syntax error was encountered whilst attempting to parse
		/// input content. Exception contains identifies the source of the error
		/// by providing the line number and position.
		/// </exception>
		private JsonNode ParseObject() {
			Accept();

			SkipWhitespace();
			var node = new JsonObjectNode();
			while (!HasReachedEnd) {
				if (Peek() == '}') {
					Accept();
					return node;
				}
				else if (Peek() == ',' && node.Count != 0) {
					Accept();

					SkipWhitespace();
					if (HasReachedEnd)
						break;
				}

				string key = ParseStringLiteral("Key");
				SkipWhitespace();
				if (HasReachedEnd)
					break;

				if (Peek() != ':')
					throw new JsonParserException("Found '" + Peek() + "' but expected ':'", _lineNumber, _linePosition);
				Accept();
				SkipWhitespace();
				if (HasReachedEnd)
					break;

				node[key] = ParseValue();
				SkipWhitespace();
			}

			throw new JsonParserException("Expected '}' but reached end of input.", _lineNumber, _linePosition);
		}

		/// <summary>
		/// Parse numeric value and determines whether to create a new <see cref="JsonIntegerNode"/>
		/// or <see cref="JsonDoubleNode"/> based upon formatting of input value.
		/// </summary>
		/// <remarks>
		/// <para>Please note that this method is unable to read values of NaN, -Infinity
		/// or Infinity since those are interpretted as string literals which can be
		/// processed at a later stage when deserializing nodes to generate object graphs.</para>
		/// </remarks>
		/// <returns>
		/// The new <see cref="JsonNode"/> instance.
		/// </returns>
		/// <exception cref="JsonParserException">
		/// Thrown if a syntax error was encountered whilst attempting to parse
		/// input content. Exception contains identifies the source of the error
		/// by providing the line number and position.
		/// </exception>
		private JsonNode ParseNumeric() {
			_stringLiteral.Length = 0;

			bool integral = true;

			if (Peek() == '-') {
				_stringLiteral.Append('-');
				Accept();
			}
			if (HasReachedEnd)
				throw new JsonParserException("Expected numeric value but reached end of input.", _lineNumber, _linePosition);

			while (char.IsDigit(Peek()))
				_stringLiteral.Append(ReadChar());

			if (!HasReachedEnd && Peek() == '.') {
				integral = false;

				_stringLiteral.Append('.');
				Accept();
				if (HasReachedEnd)
					throw new JsonParserException("Expected numeric value but reached end of input.", _lineNumber, _linePosition);

				while (char.IsDigit(Peek()))
					_stringLiteral.Append(ReadChar());
			}

			char c = Peek();
			if (!HasReachedEnd && (c == 'e' || c == 'E')) {
				integral = false;

				_stringLiteral.Append('e');
				Accept();
				if (HasReachedEnd)
					throw new JsonParserException("Expected numeric value but reached end of input.", _lineNumber, _linePosition);

				c = Peek();
				if (c == '+' || c == '-') {
					_stringLiteral.Append(c);
					Accept();

					if (HasReachedEnd)
						throw new JsonParserException("Expected numeric value but reached end of input.", _lineNumber, _linePosition);
				}

				while (char.IsDigit(Peek()))
					_stringLiteral.Append(ReadChar());
			}

			if (integral)
				return new JsonIntegerNode(long.Parse(_stringLiteral.ToString(), CultureInfo.InvariantCulture));
			else
				return new JsonDoubleNode(double.Parse(_stringLiteral.ToString(), CultureInfo.InvariantCulture));
		}

	}

}