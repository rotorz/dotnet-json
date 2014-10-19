// Copyright (c) 2014 Rotorz Limited. All rights reserved.

using System;
using System.Runtime.Serialization;

namespace Rotorz.Json {

	/// <summary>
	/// General purpose exception which is thrown when working with JSON.
	/// </summary>
	[Serializable]
	public class JsonException : Exception {
		
		/// <summary>
		/// Initialize new <see cref="JsonException"/> instance.
		/// </summary>
		public JsonException() : base() {
		}
		
		/// <summary>
		/// Initialize new <see cref="JsonException"/> instance.
		/// </summary>
		/// <param name="message">Exception message.</param>
		public JsonException(string message) : base(message) {
		}

		/// <summary>
		/// Initialize new <see cref="JsonException"/> instance.
		/// </summary>
		/// <param name="message">Exception message.</param>
		/// <param name="innerException">Inner exception which was thrown
		/// leading to this exception being thrown.</param>
		public JsonException(string message, Exception innerException) : base(message, innerException) {
		}

		/// <summary>
		/// Initialize new <see cref="JsonParserException"/> instance.
		/// </summary>
		/// <param name="info">Serialization information.</param>
		/// <param name="context">Streaming context.</param>
		protected JsonException(SerializationInfo info, StreamingContext context) : base(info, context) {
		}

	}

}
