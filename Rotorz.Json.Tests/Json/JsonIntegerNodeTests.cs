// Copyright (c) Rotorz Limited. All rights reserved.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rotorz.Json.Tests.TestObjects;
using Rotorz.Tests;
using System;

namespace Rotorz.Json.Tests {

	[TestClass]
	public class JsonIntegerNodeTests {

		#region Value

		private void Value_set_Parameterized(long value) {
			// Arrange
			var integerNode = new JsonIntegerNode();

			// Act
			integerNode.Value = value;

			// Assert
			Assert.AreEqual(value, integerNode.Value);
		}

		[TestMethod]
		public void Value_set_Zero() {
			Value_set_Parameterized(0);
		}

		[TestMethod]
		public void Value_set_NegativeSmallValue() {
			Value_set_Parameterized(-1);
		}

		[TestMethod]
		public void Value_set_PositiveSmallValue() {
			Value_set_Parameterized(1);
		}

		[TestMethod]
		public void Value_set_MinimumValue() {
			Value_set_Parameterized(long.MinValue);
		}

		[TestMethod]
		public void Value_set_MaximumValue() {
			Value_set_Parameterized(long.MaxValue);
		}

		#endregion

		#region UnsignedValue

		private void UnsignedValue_set_Parameterized(ulong value) {
			// Arrange
			var integerNode = new JsonIntegerNode();

			// Act
			integerNode.UnsignedValue = value;

			// Assert
			Assert.AreEqual(value, integerNode.UnsignedValue);
		}

		[TestMethod]
		public void UnsignedValue_set_Zero() {
			UnsignedValue_set_Parameterized(0);
		}

		[TestMethod]
		public void UnsignedValue_set_PositiveSmallValue() {
			UnsignedValue_set_Parameterized(1);
		}

		[TestMethod]
		public void UnsignedValue_set_MinimumValue() {
			UnsignedValue_set_Parameterized(ulong.MinValue);
		}

		[TestMethod]
		public void UnsignedValue_set_MaximumValue() {
			UnsignedValue_set_Parameterized(ulong.MaxValue);
		}

		#endregion

		#region Clone()

		[TestMethod]
		public void Clone_Zero() {
			// Arrange
			var integerNode = new JsonIntegerNode(0);

			// Act
			var cloneNode = integerNode.Clone() as JsonIntegerNode;

			// Assert
			Assert.AreNotSame(integerNode, cloneNode);
			Assert.AreEqual(integerNode.Value, cloneNode.Value);
		}

		[TestMethod]
		public void Clone_SmallValue() {
			// Arrange
			var integerNode = new JsonIntegerNode(1);

			// Act
			var cloneNode = integerNode.Clone() as JsonIntegerNode;

			// Assert
			Assert.AreNotSame(integerNode, cloneNode);
			Assert.AreEqual(integerNode.Value, cloneNode.Value);
		}

		[TestMethod]
		public void Clone_MaximumValue() {
			// Arrange
			var integerNode = new JsonIntegerNode(long.MaxValue);

			// Act
			var cloneNode = integerNode.Clone() as JsonIntegerNode;

			// Assert
			Assert.AreNotSame(integerNode, cloneNode);
			Assert.AreEqual(integerNode.Value, cloneNode.Value);
		}

		#endregion

		#region ToString()

		private void ToString_Parameterized(long value, string expectedResult) {
			// Arrange
			var integerNode = new JsonIntegerNode(value);

			// Act
			string result = integerNode.ToString();

			// Assert
			Assert.AreEqual(expectedResult, result);
		}

		[TestMethod]
		public void ToString_Zero() {
			ToString_Parameterized(0, "0");
		}

		[TestMethod]
		public void ToString_One() {
			ToString_Parameterized(1, "1");
		}

		[TestMethod]
		public void ToString_MinimumValue() {
			ToString_Parameterized(long.MinValue, "-9223372036854775808");
		}

		[TestMethod]
		public void ToString_MaximumValue() {
			ToString_Parameterized(long.MaxValue, "9223372036854775807");
		}

		[TestMethod]
		public void ToString_DecimalValue_DefaultCulture() {
			// Arrange
			var node = new JsonIntegerNode(1234567);

			// Act
			string json = node.ToString();

			// Assert
			Assert.IsNotNull(node);
			Assert.AreEqual("1234567", json);
		}

		[TestMethod]
		public void ToString_DecimalValue_CultureWithDifferentDecimalSeparator() {
			CultureTestUtility.ExecuteInCulture("fr-FR", () => {
				// Arrange
				var node = new JsonIntegerNode(1234567);

				// Act
				string json = node.ToString();

				// Assert
				Assert.IsNotNull(node);
				Assert.AreEqual("1234567", json);
			});
		}

		#endregion

		#region ToObject(Type)

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ToObject_Null() {
			// Arrange
			var integerNode = new JsonIntegerNode();
			Type type = null;

			// Act
			integerNode.ConvertTo(type);
		}

		private void ToObject_Parameterized<T>(long value, T expectedResult) {
			// Arrange
			var integerNode = new JsonIntegerNode(value);

			// Act
			T result = integerNode.ConvertTo<T>();

			// Assert
			Assert.AreEqual(expectedResult, result);
		}

		[TestMethod]
		public void ToObject_String() {
			ToObject_Parameterized(42, expectedResult: "42");
		}

		[TestMethod]
		public void ToObject_Double() {
			ToObject_Parameterized(42, expectedResult: 42.0);
		}

		[TestMethod]
		public void ToObject_Boolean_False() {
			ToObject_Parameterized(0, expectedResult: false);
		}

		[TestMethod]
		public void ToObject_Boolean_True() {
			ToObject_Parameterized(1, expectedResult: true);
		}

		[TestMethod]
		public void ToObject_EnumForTesting_Test1() {
			ToObject_Parameterized(0, expectedResult: EnumForTesting.Test1);
		}

		[TestMethod]
		public void ToObject_EnumForTesting_Test2() {
			ToObject_Parameterized(1, expectedResult: EnumForTesting.Test2);
		}

		#endregion

		#region WriteTo(JsonWriter)

		private void WriteTo_Parameterized(long value, string expectedResult) {
			// Arrange
			var integerNode = new JsonIntegerNode(value);
			var writer = JsonWriter.Create();

			// Act
			integerNode.Write(writer);

			// Assert
			Assert.AreEqual(expectedResult, writer.ToString());
		}

		[TestMethod]
		public void WriteTo_Zero() {
			WriteTo_Parameterized(0, "0");
		}

		[TestMethod]
		public void WriteTo_One() {
			WriteTo_Parameterized(1, "1");
		}

		[TestMethod]
		public void WriteTo_MinimumValue() {
			WriteTo_Parameterized(long.MinValue, "-9223372036854775808");
		}

		[TestMethod]
		public void WriteTo_MaximumValue() {
			WriteTo_Parameterized(long.MaxValue, "9223372036854775807");
		}

		#endregion

	}

}
