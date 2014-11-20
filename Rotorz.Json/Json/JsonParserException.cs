// Copyright (c) Rotorz Limited. All rights reserved.

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Rotorz.Json {

	/// <summary>
	/// Thrown if error was encountered whilst parsing a JSON encoded string. The most
	/// likely cause of this exception is a syntax error within the input string.
	/// </summary>
	[Serializable]
	public class JsonParserException : JsonException {
		
		/// <summary>
		/// Initializes a new instance of the <see cref="JsonParserException"/> class.
		/// </summary>
		/// <param name="message">Additional information about error.</param>
		/// <param name="lineNumber">Indicates number of line in input where error was encountered.</param>
		/// <param name="linePosition">Indicates position in line where error was encountered.</param>
		public JsonParserException(string message, int lineNumber, int linePosition) : base(message) {
			LineNumber = lineNumber;
			LinePosition = linePosition;
		}

		/// <summary>
		/// Initialize new <see cref="JsonParserException"/> instance.
		/// </summary>
		/// <param name="info">Serialization information.</param>
		/// <param name="context">Streaming context.</param>
		protected JsonParserException(SerializationInfo info, StreamingContext context) : base(info, context) {
			LineNumber = info.GetInt32("lineNumber");
			LinePosition = info.GetInt32("linePosition");
		}

		private string _message;

		/// <summary>
		/// Gets exception message.
		/// </summary>
		public override string Message {
			get {
				if (_message == null) {
					_message = LineNumber != 0
						? string.Format("({1},{2}): {0}", base.Message, LineNumber, LinePosition)
						: base.Message;
				}
				return _message;
			}
		}

		/// <summary>
		/// Gets number of line in input at which error was encountered.
		/// </summary>
		public int LineNumber { get; private set; }
		/// <summary>
		/// Gets zero-based position in line at which error was encountered.
		/// </summary>
		public int LinePosition { get; private set; }

		/// <exclude/>
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);

			info.AddValue("lineNumber", LineNumber);
			info.AddValue("linePosition", LinePosition);
		}

	}

}
