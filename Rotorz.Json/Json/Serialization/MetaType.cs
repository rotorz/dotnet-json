// Copyright (c) Rotorz Limited. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.Serialization;

namespace Rotorz.Json.Serialization {

	/// <summary>
	/// Provides additional meta information for a <c>System.Type</c> which helps to
	/// deduce the nature of JSON encoded data by examining graph of the object which
	/// is being deserialized.
	/// </summary>
	/// <remarks>
	/// <para>Meta type information is cached once generated which helps to improve
	/// performance by greatly reducing the amount of reflection required each time
	/// an object of the same type is deserialized.</para>
	/// </remarks>
	/// <seealso cref="MetaType.FromType(Type)"/>
	internal sealed class MetaType {

		private static Dictionary<Type, MetaType> s_MetaTypeCache = new Dictionary<Type, MetaType>();

		/// <summary>
		/// Lookup meta information from the specified type.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <returns>
		/// The <see cref="MetaType"/> instance that is associated with the specified
		/// type. Meta type information is cached; thus accessing this method multiple
		/// times with the same <c>System.Type</c> will return the exact same <see cref="MetaType"/>
		/// instance.
		/// </returns>
		public static MetaType FromType(Type type) {
			lock (s_MetaTypeCache) {
				MetaType metaType;
				if (!s_MetaTypeCache.TryGetValue(type, out metaType)) {
					metaType = new MetaType(type);
					s_MetaTypeCache[type] = metaType;
				}
				return metaType;
			}
		}

		private MetaType(Type type) {
			Type = type;

			ScanForDictionaryStyleCollection();
			TargetNodeType = DetermineTargetNodeType();

			SearchForSerializableMembers();
			SearchForSerializationCallbacks();
		}

		/// <summary>
		/// Gets the associated type.
		/// </summary>
		public Type Type { get; private set; }

		#region Enumerable Key/Value Pairs

		/// <summary>
		/// Gets property information for <c>Key</c> when associated type represents
		/// a collection of <c>KeyValuePair&lt;T&gt;</c> entries. This is useful when
		/// converting JSON objects into dictionary-like collections.
		/// </summary>
		/// <seealso cref="IsDictionaryStyleCollection"/>
		/// <seealso cref="ValuePropertyInfo"/>
		public PropertyInfo KeyPropertyInfo { get; private set; }
		/// <summary>
		/// Gets property information for <c>Value</c> when associated type represents
		/// a collection of <c>KeyValuePair&lt;T&gt;</c> entries. This is useful when
		/// converting JSON objects into dictionary-like collections.
		/// </summary>
		/// <seealso cref="IsDictionaryStyleCollection"/>
		/// <seealso cref="KeyPropertyInfo"/>
		public PropertyInfo ValuePropertyInfo { get; private set; }

		/// <summary>
		/// Gets a value indicating whether associated type represents a dictionary-like
		/// collection of <c>KeyValuePair&lt;T&gt;</c> entries. This is useful when
		/// converting JSON objects into dictionary-like collections.
		/// </summary>
		/// <seealso cref="ValuePropertyInfo"/>
		/// <seealso cref="KeyPropertyInfo"/>
		public bool IsDictionaryStyleCollection {
			get { return KeyPropertyInfo != null; }
		}

		private void ScanForDictionaryStyleCollection() {
			var genericType = Type.GetInterface("ICollection`1");
			if (genericType == null)
				return;

			// Determine whether this is a collection of KeyValuePair<string,>
			var elementType = genericType.GetGenericArguments()[0];
			if (!elementType.IsGenericType || typeof(KeyValuePair<,>) != elementType.GetGenericTypeDefinition())
				return;

			Type[] keyValueTypes = elementType.GetGenericArguments();
			if (keyValueTypes[0] == typeof(string)) {
				KeyPropertyInfo = elementType.GetProperty("Key", BindingFlags.Public | BindingFlags.Instance);
				ValuePropertyInfo = elementType.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
			}
		}

		#endregion

		#region Target Node Type

		/// <summary>
		/// Identifies type of <see cref="JsonNode"/> which would best represent
		/// the associated type.
		/// </summary>
		public enum NodeType {
			/// <summary>
			/// Was unable to determine an appropriate node type.
			/// </summary>
			Unknown,
			/// <summary>
			/// Indicates that <see cref="JsonIntegerNode"/> would best fit.
			/// </summary>
			Integer,
			/// <summary>
			/// Indicates that <see cref="JsonDoubleNode"/> would best fit.
			/// </summary>
			Double,
			/// <summary>
			/// Indicates that <see cref="JsonBooleanNode"/> would best fit.
			/// </summary>
			Boolean,
			/// <summary>
			/// Indicates that <see cref="JsonStringNode"/> would best fit.
			/// </summary>
			String,
			/// <summary>
			/// Indicates that <see cref="JsonArrayNode"/> would best fit.
			/// </summary>
			Array,
			/// <summary>
			/// Indicates that <see cref="JsonObjectNode"/> would best fit.
			/// </summary>
			Object
		}

		/// <summary>
		/// Gets the type of <see cref="JsonNode"/> which would best accomodate an
		/// instance/value of the associated type.
		/// </summary>
		public NodeType TargetNodeType { get; private set; }

		private NodeType DetermineTargetNodeType() {
			var type = Type;

			if (type.IsPrimitive) {
				if (ReflectionUtility.IsNumericType(type)) {
					if (ReflectionUtility.IsIntegralType(type))
						return NodeType.Integer;
					else
						return NodeType.Double;
				}
				else if (ReflectionUtility.IsBooleanType(type)) {
					return NodeType.Boolean;
				}
			}
			else if (type.IsEnum) {
				return NodeType.Integer;
			}
			else if (Type.GetTypeCode(type) == TypeCode.String) {
				return NodeType.String;
			}
			else if (type.IsArray || (typeof(ICollection).IsAssignableFrom(type) && !IsDictionaryStyleCollection)) {
				return NodeType.Array;
			}
			else {
				return NodeType.Object;
			}

			return NodeType.Unknown;
		}

		#endregion

		#region Serializable Members

		/// <summary>
		/// Gets list of field and property members which can be serialized or
		/// deserialized from the associated type.
		/// </summary>
		public IList<SerializableMember> SerializableMembers { get; private set; }

		private void SearchForSerializableMembers() {
			var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

			var members = new List<SerializableMember>();
			SerializableMembers = new ReadOnlyCollection<SerializableMember>(members);

			SerializableMember member;

			// Search for serializable fields.
			foreach (var field in Type.GetFields(bindingFlags))
				if (field.IsPublic || field.IsDefined(typeof(JsonPropertyAttribute), false)) {
					member.info = field;
					member.resolvedName = ResolvePropertyName(field);
					members.Add(member);
				}

			// Search for serializable properties.
			foreach (var property in Type.GetProperties(bindingFlags))
				if (property.IsDefined(typeof(JsonPropertyAttribute), false)) {
					member.info = property;
					member.resolvedName = ResolvePropertyName(property);
					members.Add(member);
				}
		}

		private static string ResolvePropertyName(MemberInfo memberInfo) {
			string name = memberInfo.Name;

			var attributes = memberInfo.GetCustomAttributes(typeof(JsonPropertyAttribute), false);
			if (attributes.Length == 1) {
				var settingPropertyAttribute = (JsonPropertyAttribute)attributes[0];
				if (!string.IsNullOrEmpty(settingPropertyAttribute.Name))
					name = settingPropertyAttribute.Name;
			}

			return name;
		}

		#endregion

		#region Serialization Callbacks

		private void SearchForSerializationCallbacks() {
			var methods = Type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (MethodInfo method in methods) {
				// Only consider methods which have the correct signature.
				if (method.ReturnType != typeof(void))
					continue;
				ParameterInfo[] parameters = method.GetParameters();
				if (parameters.Length != 1 || parameters[0].ParameterType != typeof(StreamingContext))
					continue;

				// Test for serialization callback attributes.
				foreach (var attribute in method.GetCustomAttributes(true)) {
					if (attribute is OnSerializingAttribute) {
						if (_onSerializingMethods == null)
							_onSerializingMethods = new List<MethodInfo>();
						_onSerializingMethods.Add(method);
					}
					if (attribute is OnSerializedAttribute) {
						if (_onSerializedMethods == null)
							_onSerializedMethods = new List<MethodInfo>();
						_onSerializedMethods.Add(method);
					}
					if (attribute is OnDeserializingAttribute) {
						if (_onDeserializingMethods == null)
							_onDeserializingMethods = new List<MethodInfo>();
						_onDeserializingMethods.Add(method);
					}
					if (attribute is OnDeserializedAttribute) {
						if (_onDeserializedMethods == null)
							_onDeserializedMethods = new List<MethodInfo>();
						_onDeserializedMethods.Add(method);
					}
				}
			}
		}

		private List<MethodInfo> _onSerializingMethods;
		private List<MethodInfo> _onSerializedMethods;
		private List<MethodInfo> _onDeserializingMethods;
		private List<MethodInfo> _onDeserializedMethods;

		private void Invoke(List<MethodInfo> callbacks, object obj, StreamingContext context) {
			if (obj == null)
				throw new ArgumentNullException("obj");
			if (obj.GetType() != Type)
				throw new ArgumentException("Object is not an instance of the type '" + Type.FullName + "'.", "obj");

			if (callbacks == null)
				return;

			object[] callbackParams = { context };

			foreach (var callback in callbacks)
				callback.Invoke(obj, callbackParams);
		}

		/// <summary>
		/// Invoke callback methods which are annotated with <c>OnSerializingAttribute</c>
		/// for specified instance of the associated type.
		/// </summary>
		/// <remarks>
		/// <para><c>OnSerializing</c> callbacks should be invoked before serialization
		/// begins for the specified instance.</para>
		/// </remarks>
		/// <param name="obj">Instance of associated type.</param>
		/// <param name="context">Additional context information.</param>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown if <paramref name="obj"/> was a value of <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// Thrown if <paramref name="obj"/> was not an instance of the type which is
		/// associated with thie <see cref="MetaType"/>.
		/// </exception>
		public void InvokeOnSerializing(object obj, StreamingContext context) {
			Invoke(_onSerializingMethods, obj, context);
		}
		/// <summary>
		/// Invoke callback methods which are annotated with <c>OnSerializedAttribute</c>
		/// for specified instance of the associated type.
		/// </summary>
		/// <remarks>
		/// <para><c>OnSerialized</c> callbacks should be invoked after the specified
		/// instance has been serialized.</para>
		/// </remarks>
		/// <param name="obj">Instance of associated type.</param>
		/// <param name="context">Additional context information.</param>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown if <paramref name="obj"/> was a value of <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// Thrown if <paramref name="obj"/> was not an instance of the type which is
		/// associated with thie <see cref="MetaType"/>.
		/// </exception>
		public void InvokeOnSerialized(object obj, StreamingContext context) {
			Invoke(_onSerializedMethods, obj, context);
		}
		/// <summary>
		/// Invoke callback methods which are annotated with <c>OnDeserializingAttribute</c>
		/// for specified instance of the associated type.
		/// </summary>
		/// <remarks>
		/// <para><c>OnDeserializing</c> callbacks should be invoked before beginning
		/// to deserialize the specified instance.</para>
		/// </remarks>
		/// <param name="obj">Instance of associated type.</param>
		/// <param name="context">Additional context information.</param>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown if <paramref name="obj"/> was a value of <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// Thrown if <paramref name="obj"/> was not an instance of the type which is
		/// associated with thie <see cref="MetaType"/>.
		/// </exception>
		public void InvokeOnDeserializing(object obj, StreamingContext context) {
			Invoke(_onDeserializingMethods, obj, context);
		}
		/// <summary>
		/// Invoke callback methods which are annotated with <c>OnDeserializedAttribute</c>
		/// for specified instance of the associated type.
		/// </summary>
		/// <remarks>
		/// <para><c>OnDeserialized</c> callbacks should be invoked after the specified
		/// instance has been deserialized.</para>
		/// </remarks>
		/// <param name="obj">Instance of associated type.</param>
		/// <param name="context">Additional context information.</param>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown if <paramref name="obj"/> was a value of <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// Thrown if <paramref name="obj"/> was not an instance of the type which is
		/// associated with thie <see cref="MetaType"/>.
		/// </exception>
		public void InvokeOnDeserialized(object obj, StreamingContext context) {
			Invoke(_onDeserializedMethods, obj, context);
		}

		#endregion

	}

}
