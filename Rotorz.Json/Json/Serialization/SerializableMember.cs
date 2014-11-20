// Copyright (c) Rotorz Limited. All rights reserved.

using System.Reflection;

namespace Rotorz.Json.Serialization {

	/// <summary>
	/// Represents a serializable field or property in an object or structure instance.
	/// </summary>
	internal struct SerializableMember {

		/// <summary>
		/// Information about field or property.
		/// </summary>
		public MemberInfo info;

		/// <summary>
		/// Resolved name of field or property.
		/// </summary>
		public string resolvedName;

	}

}
