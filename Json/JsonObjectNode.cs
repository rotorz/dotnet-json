// Copyright (c) 2014 Rotorz Limited. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

using Rotorz.Json.Serialization;

namespace Rotorz.Json {

	/// <summary>
	/// Node representing a JSON object with named properties.
	/// </summary>
	/// <example>
	/// <para>Manual construction of an object node:</para>
	/// <code language="csharp"><![CDATA[
	/// var objectNode = new JsonObjectNode();
	/// objectNode["A"] = new JsonIntegerNode(1);
	/// objectNode["B"] = new JsonIntegerNode(2);
	/// objectNode["C"] = new JsonIntegerNode(3);
	/// ]]></code>
	/// </example>
	public sealed class JsonObjectNode : JsonNode, IDictionary<string, JsonNode> {

		#region Factory

		/// <summary>
		/// Attempt to create object node from a .NET object.
		/// </summary>
		/// <param name="instance">Input object.</param>
		/// <returns>
		/// The new <see cref="JsonObjectNode"/> instance; otherwise a value of <c>null</c>
		/// if input was <c>null</c>.
		/// </returns>
		/// <exception cref="System.Exception">
		/// Thrown if error is encountered whilst creating object node. Errors are likely
		/// to occur when unable to convert property values into JSON nodes.
		/// </exception>
		internal static JsonObjectNode FromInstance(object instance) {
			if (instance == null)
				return null;

			var node = new JsonObjectNode();
			var metaType = MetaType.FromType(instance.GetType());
			
			metaType.InvokeOnSerializing(instance, default(StreamingContext));

			foreach (var member in metaType.SerializableMembers) {
				object value;
				if (member.info.MemberType == MemberTypes.Field) {
					var mi = (FieldInfo)member.info;
					value = mi.GetValue(instance);
				}
				else {
					var pi = (PropertyInfo)member.info;
					value = pi.GetValue(instance, null);
				}
				node[member.resolvedName] = FromObject(value);
			}

			metaType.InvokeOnSerialized(instance, default(StreamingContext));

			return node;
		}

		/// <summary>
		/// Create object node from a generic dictionary.
		/// </summary>
		/// <remarks>
		/// <para>Property values are cloned if they are already <see cref="JsonNode"/> instances.</para>
		/// </remarks>
		/// <typeparam name="TValue">Type of property value.</typeparam>
		/// <param name="dictionary">Input dictionary.</param>
		/// <returns>
		/// The new <see cref="JsonObjectNode"/> instance.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown when <paramref name="dictionary"/> is a value of <c>null</c>.
		/// </exception>
		/// <exception cref="System.Exception">
		/// Thrown if error is encountered whilst creating object node. Errors are likely
		/// to occur when unable to convert property values into JSON nodes.
		/// </exception>
		public static JsonObjectNode FromDictionary<TValue>(IDictionary<string, TValue> dictionary) {
			if (dictionary == null)
				throw new ArgumentNullException("dictionary");

			var node = new JsonObjectNode();

			foreach (var property in dictionary)
				node[property.Key] = FromObject(property.Value);

			return node;
		}

		#endregion

		/// <summary>
		/// Initialize new object node.
		/// </summary>
		public JsonObjectNode() {
		}

		private Dictionary<string, JsonNode> _properties = new Dictionary<string, JsonNode>();

		#region IDictionary<string, JsonNode> Members

		/// <summary>
		/// Add property to object.
		/// </summary>
		/// <param name="key">Unique property key.</param>
		/// <param name="value">Property value node or a value of <c>null</c>.</param>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown if <paramref name="key"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// Thrown when a property with the same key already exists within object.
		/// </exception>
		public void Add(string key, JsonNode value) {
			_properties.Add(key, value);
		}

		/// <summary>
		/// Gets a value indicating whether object node contains a property with
		/// the specified key.
		/// </summary>
		/// <param name="key">Property key.</param>
		/// <returns>
		/// A value of <c>true</c> if property exists with the specified key; otherwise,
		/// a value of <c>false</c>.
		/// </returns>
		public bool ContainsKey(string key) {
			return _properties.ContainsKey(key);
		}

		/// <summary>
		/// Remove property with the specified key if found.
		/// </summary>
		/// <param name="key">Key of property which is to be removed.</param>
		/// <returns>
		/// A value of <c>true</c> if property was removed; otherwise, a value of <c>false</c>
		/// of object does not actually contain the specified property.
		/// </returns>
		public bool Remove(string key) {
			return _properties.Remove(key);
		}

		/// <summary>
		/// Try to get value of property with the specified key.
		/// </summary>
		/// <param name="key">Property key.</param>
		/// <param name="value">Method outputs property value when property exists with
		/// the specified key; otherwise outputs a value of <c>null</c>.</param>
		/// <returns>
		/// A value of <c>true</c> if property value was retrieved; otherwise, a value
		/// of <c>false</c> indicating that property does not exist.
		/// </returns>
		public bool TryGetValue(string key, out JsonNode value) {
			return _properties.TryGetValue(key, out value);
		}

		/// <summary>
		/// Gets collection of property keys.
		/// </summary>
		public ICollection<string> Keys {
			get { return _properties.Keys; }
		}

		/// <summary>
		/// Gets collection of property values.
		/// </summary>
		public ICollection<JsonNode> Values {
			get { return _properties.Values; }
		}

		/// <summary>
		/// Lookup value of property with the specified key.
		/// </summary>
		/// <param name="key">Property key.</param>
		/// <returns>
		/// Value of property when property exists; otherwise, a value of <c>null</c>
		/// indicating that property does not exist.
		/// </returns>
		public JsonNode this[string key] {
			get {
				JsonNode node;
				_properties.TryGetValue(key, out node);
				return node;
			}
			set {
				_properties[key] = value;
			}
		}

		#endregion

		#region ICollection<KeyValuePair<string, JsonNode>> Members

		void ICollection<KeyValuePair<string, JsonNode>>.Add(KeyValuePair<string, JsonNode> item) {
			_properties.Add(item.Key, item.Value);
		}

		/// <summary>
		/// Clear all properties from object.
		/// </summary>
		public void Clear() {
			_properties.Clear();
		}

		bool ICollection<KeyValuePair<string, JsonNode>>.Contains(KeyValuePair<string, JsonNode> item) {
			return _properties.ContainsKey(item.Key);
		}

		void ICollection<KeyValuePair<string, JsonNode>>.CopyTo(KeyValuePair<string, JsonNode>[] array, int arrayIndex) {
			var collection = (ICollection<KeyValuePair<string, JsonNode>>)_properties;
			collection.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Gets count of properties in object.
		/// </summary>
		public int Count {
			get { return _properties.Count; }
		}

		bool ICollection<KeyValuePair<string, JsonNode>>.IsReadOnly {
			get {
				var collection = (ICollection<KeyValuePair<string, JsonNode>>)_properties;
				return collection.IsReadOnly;
			}
		}

		bool ICollection<KeyValuePair<string, JsonNode>>.Remove(KeyValuePair<string, JsonNode> item) {
			var collection = (ICollection<KeyValuePair<string, JsonNode>>)_properties;
			return collection.Remove(item);
		}

		#endregion

		#region IEnumerable<KeyValuePair<string,JsonNode>> Members

		/// <summary>
		/// Returns an enumerator which iterates through collection of object property
		/// key and value pairs.
		/// </summary>
		/// <returns>
		/// Enumerator instance which can be used to iterate through collection.
		/// </returns>
		public IEnumerator<KeyValuePair<string, JsonNode>> GetEnumerator() {
			return _properties.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator() {
			return _properties.GetEnumerator();
		}

		#endregion

		/// <inheritdoc/>
		public override JsonNode Clone() {
			var node = new JsonObjectNode();
			foreach (var property in _properties) {
				node[property.Key] = property.Value != null
					? property.Value.Clone()
					: null;
			}
			return node;
		}

		/// <inheritdoc/>
		public override object ToObject(Type type) {
			if (type == null)
				throw new ArgumentNullException("type");

			var obj = Activator.CreateInstance(type);

			IDictionary dictionary = obj as IDictionary;
			if (dictionary != null) {
				// Read properties into dictionary.
				var valueType = type.GetGenericArguments()[1];
				foreach (var property in _properties) {
					if (property.Value != null)
						dictionary[property.Key] = property.Value.ToObject(valueType);
					else
						dictionary[property.Key] = null;
				}
			}
			else {
				var metaType = MetaType.FromType(type);
				metaType.InvokeOnDeserializing(obj, default(StreamingContext));

				// Read properties into object instance.
				foreach (var member in metaType.SerializableMembers) {
					var valueNode = this[member.resolvedName];
					if (valueNode == null)
						continue;

					if (member.info.MemberType == MemberTypes.Field) {
						var fi = (FieldInfo)member.info;
						fi.SetValue(obj, valueNode.ToObject(fi.FieldType));
					}
					else {
						var pi = (PropertyInfo)member.info;
						pi.SetValue(obj, valueNode.ToObject(pi.PropertyType), null);
					}
				}

				metaType.InvokeOnDeserialized(obj, default(StreamingContext));
			}

			return obj;
		}

		/// <inheritdoc/>
		public override string ToString() {
			if (Count == 0)
				return "{}";
			return base.ToString();
		}

		/// <inheritdoc/>
		public override void WriteTo(JsonWriter writer) {
			writer.WriteStartObject();

			foreach (var property in _properties) {
				writer.WritePropertyKey(property.Key);

				if (property.Value != null)
					property.Value.WriteTo(writer);
				else
					writer.WriteValueRaw("null");
			}

			writer.WriteEndObject();
		}

	}

}
