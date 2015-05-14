// Copyright (c) Rotorz Limited. All rights reserved.

using System;
using System.Runtime.Serialization;

namespace Rotorz.Json.MessagePack {

	/// <summary>
	/// Thrown if error was encountered whilst parsing MessagePack encoded data. The most
	/// likely cause of this exception is a syntax error within the input data.
	/// </summary>
	[Serializable]
	public class MessagePackParserException : JsonException {
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MessagePackParserException"/> class.
		/// </summary>
		/// <param name="message">Additional information about error.</param>
		public MessagePackParserException(string message) : base(message) {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MessagePackParserException"/> class.
		/// </summary>
		/// <param name="info">Serialization information.</param>
		/// <param name="context">Streaming context.</param>
		protected MessagePackParserException(SerializationInfo info, StreamingContext context) : base(info, context) {
		}

	}

}
