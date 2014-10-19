// Copyright (c) 2014 Rotorz Limited. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Rotorz.Json {

	/// <summary>
	/// Node representing a JSON array which can contain zero or more JSON nodes.
	/// </summary>
	/// <example>
	/// <para>Manual construction of an array node:</para>
	/// <code language="csharp"><![CDATA[
	/// var arrayNode = new JsonArrayNode();
	/// arrayNode.Add(new JsonIntegerNode(1));
	/// arrayNode.Add(new JsonIntegerNode(2));
	/// arrayNode.Add(new JsonIntegerNode(3));
	/// ]]></code>
	/// </example>
	public sealed class JsonArrayNode : JsonNode, IList<JsonNode> {

		#region Factory

		/// <summary>
		/// Create array node and populate from a native array of values.
		/// </summary>
		/// <remarks>
		/// <para>Array elements are cloned if they are already <see cref="JsonNode"/> instances.</para>
		/// </remarks>
		/// <typeparam name="T">Type of array elements.</typeparam>
		/// <param name="array">Native array of objects.</param>
		/// <returns>
		/// New <see cref="JsonArrayNode"/> instance containing zero or more nodes.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown when <paramref name="array"/> is a value of <c>null</c>.
		/// </exception>
		public static JsonArrayNode FromArray<T>(T[] array) {
			if (array == null)
				throw new ArgumentNullException("array");

			var node = new JsonArrayNode();
			foreach (T element in array)
				node.Add(FromObject(element));
			return node;
		}

		/// <summary>
		/// Create array node and populate from a collection of values.
		/// </summary>
		/// <remarks>
		/// <para>Collection entries are cloned if they are already <see cref="JsonNode"/> instances.</para>
		/// </remarks>
		/// <param name="collection">The collection.</param>
		/// <returns>
		/// New <see cref="JsonArrayNode"/> instance containing zero or more nodes.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown when <paramref name="collection"/> is a value of <c>null</c>.
		/// </exception>
		public static JsonArrayNode FromCollection(ICollection collection) {
			if (collection == null)
				throw new ArgumentNullException("collection");

			var node = new JsonArrayNode();
			foreach (var element in collection)
				node.Add(FromObject(element));
			return node;
		}

		#endregion

		/// <summary>
		/// Initialize new array node.
		/// </summary>
		public JsonArrayNode() {
		}

		/// <summary>
		/// Initialize new array node and populate from collection of nodes.
		/// </summary>
		/// <param name="nodes">Native array of nodes.</param>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown when <paramref name="nodes"/> is a value of <c>null</c>.
		/// </exception>
		public JsonArrayNode(JsonNode[] nodes) {
			if (nodes == null)
				throw new ArgumentNullException("nodes");

			foreach (var node in nodes)
				_nodes.Add(node);
		}

		/// <summary>
		/// Initialize new array node and populate from collection of nodes.
		/// </summary>
		/// <param name="collection">Collection of nodes.</param>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown when <paramref name="collection"/> is a value of <c>null</c>.
		/// </exception>
		public JsonArrayNode(ICollection<JsonNode> collection) {
			if (collection == null)
				throw new ArgumentNullException("collection");

			_nodes.AddRange(collection);
		}

		private List<JsonNode> _nodes = new List<JsonNode>();

		#region IList<JsonNode> Members

		/// <summary>
		/// Find index of specific node within array.
		/// </summary>
		/// <param name="item">Node instance or a value of <c>null</c>.</param>
		/// <returns>
		/// Zero-based index of first occurrence of input node within array if found;
		/// otherwise, a value of -1.
		/// </returns>
		public int IndexOf(JsonNode item) {
			return _nodes.IndexOf(item);
		}

		/// <summary>
		/// Insert node at specific position within array.
		/// </summary>
		/// <param name="index">Zero-based index of node within array.</param>
		/// <param name="item">New node instance or a value of <c>null</c>.</param>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is out of range.
		/// </exception>
		public void Insert(int index, JsonNode item) {
			_nodes.Insert(index, item);
		}

		/// <summary>
		/// Remove node at specific position within array.
		/// </summary>
		/// <param name="index">Zero-based index of node within array.</param>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is out of range.
		/// </exception>
		public void RemoveAt(int index) {
			_nodes.RemoveAt(index);
		}

		/// <summary>
		/// Lookup node from array at specified index.
		/// </summary>
		/// <param name="index">Zero-based index of node within array.</param>
		/// <returns>
		/// Node instance or a value of <c>null</c>.
		/// </returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is out of range.
		/// </exception>
		public JsonNode this[int index] {
			get { return _nodes[index]; }
			set { _nodes[index] = value; }
		}

		#endregion

		#region ICollection<JsonNode> Members

		/// <summary>
		/// Add node to array.
		/// </summary>
		/// <param name="item">Node or a value of <c>null</c>.</param>
		public void Add(JsonNode item) {
			_nodes.Add(item);
		}

		/// <summary>
		/// Clear all nodes from array.
		/// </summary>
		public void Clear() {
			_nodes.Clear();
		}

		/// <summary>
		/// Determines whether array contains the specified node.
		/// </summary>
		/// <param name="item">Node of which to search for.</param>
		/// <returns>
		/// A value of <c>true</c> if array contains the specified node; otherwise a
		/// value of <c>false</c>.
		/// </returns>
		public bool Contains(JsonNode item) {
			return _nodes.Contains(item);
		}

		/// <summary>
		/// Copy nodes to specified native array.
		/// </summary>
		/// <param name="array">Native array that should be populated with array nodes from this array.</param>
		/// <param name="arrayIndex">Zero-based index in <paramref name="array"/> at which copying begins.</param>
		public void CopyTo(JsonNode[] array, int arrayIndex) {
			_nodes.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Gets count of nodes in array.
		/// </summary>
		public int Count {
			get { return _nodes.Count; }
		}

		bool ICollection<JsonNode>.IsReadOnly {
			get { return false; }
		}

		/// <summary>
		/// Remove node from array if found.
		/// </summary>
		/// <param name="item">Node which is to be removed.</param>
		/// <returns>
		/// A value of <c>true</c> if node was removed; otherwise, a value of <c>false</c>
		/// if array does not actually contain specified node.
		/// </returns>
		public bool Remove(JsonNode item) {
			return _nodes.Remove(item);
		}

		#endregion

		#region IEnumerable<JsonNode> Members

		/// <summary>
		/// Returns an enumerator which iterates through collection of object nodes.
		/// </summary>
		/// <returns>
		/// Enumerator instance which can be used to iterate through array.
		/// </returns>
		public IEnumerator<JsonNode> GetEnumerator() {
			return _nodes.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator() {
			return _nodes.GetEnumerator();
		}

		#endregion

		/// <inheritdoc/>
		public override JsonNode Clone() {
			var clone = new JsonArrayNode();
			foreach (var node in _nodes) {
				if (node != null)
					clone.Add(node.Clone());
				else
					clone.Add(null);
			}
			return clone;
		}
		
		/// <inheritdoc/>
		public override object ToObject(Type type) {
			if (type == null)
				throw new ArgumentNullException("type");

			if (type.IsArray) {
				var elementType = type.GetElementType();

				var array = Array.CreateInstance(elementType, Count);
				for (int i = 0; i < _nodes.Count; ++i)
					array.SetValue(_nodes[i].ToObject(elementType), i);

				return array;
			}
			else {
				var collectionType = type.GetInterface("ICollection`1");
				if (collectionType != null) {
					var elementType = collectionType.GetGenericArguments()[0];

					var collection = Activator.CreateInstance(type);
					var addMethod = collectionType.GetMethod("Add", new Type[] { elementType });

					var paramBuffer = new object[1];

					foreach (var node in _nodes) {
						paramBuffer[0] = node.ToObject(elementType);
						addMethod.Invoke(collection, paramBuffer);
					}

					return collection;
				}
			}
			
			throw new InvalidOperationException("Was unable to convert array to type '" + type.FullName + "'.");
		}

		/// <inheritdoc/>
		public override string ToString() {
			if (Count == 0)
				return "[]";
			return base.ToString();
		}

		/// <inheritdoc/>
		public override void WriteTo(JsonWriter writer) {
			writer.WriteStartArray();

			foreach (var node in _nodes)
				if (node != null)
					node.WriteTo(writer);
				else
					writer.WriteValueRaw("null");

			writer.WriteEndArray();
		}

	}

}
