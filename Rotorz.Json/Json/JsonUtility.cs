// Copyright (c) Rotorz Limited. All rights reserved.

using System;
using System.IO;
using System.Text;

namespace Rotorz.Json {

	/// <summary>
	/// Utility methods for interacting with <see cref="JsonNode"/> instances.
	/// </summary>
	public static class JsonUtility {

		/// <inheritdoc cref="JsonNode.ReadFrom(string)"/>
		public static JsonNode ReadFrom(string json) {
			return JsonNode.ReadFrom(json);
		}

		/// <inheritdoc cref="JsonNode.ReadFrom(Stream)"/>
		public static JsonNode ReadFrom(Stream stream) {
			return JsonNode.ReadFrom(stream);
		}

		/// <inheritdoc cref="JsonNode.ReadFrom(TextReader)"/>
		public static JsonNode ReadFrom(TextReader reader) {
			return JsonNode.ReadFrom(reader);
		}

		/// <inheritdoc cref="JsonNode.ConvertFrom(object)"/>
		public static JsonNode ConvertFrom(object value) {
			return JsonNode.ConvertFrom(value);
		}

		/// <summary>
		/// Writes a <see cref="JsonNode"/> instance to the provided <see cref="StringBuilder"/>
		/// with the default formatting.
		/// </summary>
		/// <param name="node">A <see cref="JsonNode"/> instance.</param>
		/// <param name="builder">String builder.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <list type="bullet">
		/// <item>If <paramref name="node"/> is <c>null</c>.</item>
		/// <item>If <paramref name="builder"/> is <c>null</c>.</item>
		/// </list>
		/// </exception>
		/// <seealso cref="JsonWriter"/>
		/// <seealso cref="JsonNode.WriteTo(IJsonWriter)"/>
		public static void WriteTo(this JsonNode node, StringBuilder builder) {
			WriteTo(node, builder, JsonWriterSettings.DefaultSettings);
		}

		/// <summary>
		/// Writes a <see cref="JsonNode"/> instance to the provided <see cref="StringBuilder"/>
		/// with custom formatting.
		/// </summary>
		/// <param name="node">A <see cref="JsonNode"/> instance.</param>
		/// <param name="builder">String builder.</param>
		/// <param name="settings">Custom settings.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <list type="bullet">
		/// <item>If <paramref name="node"/> is <c>null</c>.</item>
		/// <item>If <paramref name="builder"/> is <c>null</c>.</item>
		/// <item>If <paramref name="settings"/> is <c>null</c>.</item>
		/// </list>
		/// </exception>
		/// <seealso cref="JsonWriter"/>
		/// <seealso cref="JsonNode.WriteTo(IJsonWriter)"/>
		public static void WriteTo(this JsonNode node, StringBuilder builder, JsonWriterSettings settings) {
			if (node == null)
				throw new ArgumentNullException("node");

			var writer = JsonWriter.Create(builder, settings);
			node.WriteTo(writer);
		}

		/// <summary>
		/// Creates a new <see cref="JsonWriter"/> instance and write content to the
		/// provided <see cref="Stream"/> with custom settings.
		/// </summary>
		/// <param name="node">A <see cref="JsonNode"/> instance or <c>null</c>.</param>
		/// <param name="stream">Stream that data will be written to.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <list type="bullet">
		/// <item>If <paramref name="node"/> is <c>null</c>.</item>
		/// <item>If <paramref name="stream"/> is <c>null</c>.</item>
		/// </list>
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// If <paramref name="stream"/> is not writable.
		/// </exception>
		/// <seealso cref="JsonWriter"/>
		/// <seealso cref="JsonNode.WriteTo(IJsonWriter)"/>
		public static void WriteTo(this JsonNode node, Stream stream) {
			WriteTo(node, stream, JsonWriterSettings.DefaultSettings);
		}

		/// <summary>
		/// Creates a new <see cref="JsonWriter"/> instance and write content to the
		/// provided <see cref="Stream"/> with custom settings.
		/// </summary>
		/// <param name="node">A <see cref="JsonNode"/> instance.</param>
		/// <param name="stream">Stream that data will be written to.</param>
		/// <param name="settings">Custom settings.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <list type="bullet">
		/// <item>If <paramref name="node"/> is <c>null</c>.</item>
		/// <item>If <paramref name="stream"/> is <c>null</c>.</item>
		/// <item>If <paramref name="settings"/> is <c>null</c>.</item>
		/// </list>
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// If <paramref name="stream"/> is not writable.
		/// </exception>
		/// <seealso cref="JsonWriter"/>
		/// <seealso cref="JsonNode.WriteTo(IJsonWriter)"/>
		public static void WriteTo(this JsonNode node, Stream stream, JsonWriterSettings settings) {
			if (node == null)
				throw new ArgumentNullException("node");

			var writer = JsonWriter.Create(stream, settings);
			node.WriteTo(writer);
		}

		/// <summary>
		/// Creates a new <see cref="JsonWriter"/> instance and write content to the
		/// provided <see cref="StringBuilder"/> with custom settings.
		/// </summary>
		/// <param name="node">A <see cref="JsonNode"/> instance.</param>
		/// <param name="textWriter">Text writer.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <list type="bullet">
		/// <item>If <paramref name="node"/> is <c>null</c>.</item>
		/// <item>If <paramref name="textWriter"/> is <c>null</c>.</item>
		/// </list>
		/// </exception>
		/// <seealso cref="JsonWriter"/>
		/// <seealso cref="JsonNode.WriteTo(IJsonWriter)"/>
		public static void WriteTo(this JsonNode node, TextWriter textWriter) {
			WriteTo(node, textWriter, JsonWriterSettings.DefaultSettings);
		}

		/// <summary>
		/// Creates a new <see cref="JsonWriter"/> instance and write content to the
		/// provided <see cref="StringBuilder"/> with custom settings.
		/// </summary>
		/// <param name="node">A <see cref="JsonNode"/> instance.</param>
		/// <param name="textWriter">Text writer.</param>
		/// <param name="settings">Custom settings.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <list type="bullet">
		/// <item>If <paramref name="node"/> is <c>null</c>.</item>
		/// <item>If <paramref name="textWriter"/> is <c>null</c>.</item>
		/// <item>If <paramref name="settings"/> is <c>null</c>.</item>
		/// </list>
		/// </exception>
		/// <seealso cref="JsonWriter"/>
		/// <seealso cref="JsonNode.WriteTo(IJsonWriter)"/>
		public static void WriteTo(this JsonNode node, TextWriter textWriter, JsonWriterSettings settings) {
			if (node == null)
				throw new ArgumentNullException("node");

			var writer = JsonWriter.Create(textWriter, settings);
			node.WriteTo(writer);
		}

	}

}
