// Copyright (c) 2014 Rotorz Limited. All rights reserved.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rotorz.Json.Tests.TestObjects;
using Rotorz.Tests;
using System;

namespace Rotorz.Json.Tests {

	[TestClass]
	public class JsonDoubleNodeTests {

		#region Value

		private void Value_set_Parameterized(double value) {
			// Arrange
			var doubleNode = new JsonDoubleNode();

			// Act
			doubleNode.Value = value;

			// Assert
			Assert.AreEqual(value, doubleNode.Value);
		}

		[TestMethod]
		public void Value_set_Zero() {
			Value_set_Parameterized(0.0);
		}

		[TestMethod]
		public void Value_set_NegativeSmallValue() {
			Value_set_Parameterized(-0.00000001);
		}

		[TestMethod]
		public void Value_set_PositiveSmallValue() {
			Value_set_Parameterized(0.00000001);
		}

		[TestMethod]
		public void Value_set_MinimumValue() {
			Value_set_Parameterized(double.MinValue);
		}

		[TestMethod]
		public void Value_set_MaximumValue() {
			Value_set_Parameterized(double.MaxValue);
		}

		#endregion

		#region Clone()

		[TestMethod]
		public void Clone_Zero() {
			// Arrange
			var doubleNode = new JsonDoubleNode(0);

			// Act
			var cloneNode = doubleNode.Clone() as JsonDoubleNode;

			// Assert
			Assert.AreNotSame(doubleNode, cloneNode);
			Assert.AreEqual(doubleNode.Value, cloneNode.Value);
		}

		[TestMethod]
		public void Clone_SmallValue() {
			// Arrange
			var doubleNode = new JsonDoubleNode(0.00000001);

			// Act
			var cloneNode = doubleNode.Clone() as JsonDoubleNode;

			// Assert
			Assert.AreNotSame(doubleNode, cloneNode);
			Assert.AreEqual(doubleNode.Value, cloneNode.Value);
		}

		[TestMethod]
		public void Clone_MaximumValue() {
			// Arrange
			var doubleNode = new JsonDoubleNode(double.MaxValue);

			// Act
			var cloneNode = doubleNode.Clone() as JsonDoubleNode;

			// Assert
			Assert.AreNotSame(doubleNode, cloneNode);
			Assert.AreEqual(doubleNode.Value, cloneNode.Value);
		}

		#endregion

		#region ToString()

		private void ToString_Parameterized(double value, string expectedResult) {
			// Arrange
			var doubleNode = new JsonDoubleNode(value);

			// Act
			string result = doubleNode.ToString();

			// Assert
			Assert.AreEqual(expectedResult, result);
		}

		[TestMethod]
		public void ToString_Zero() {
			ToString_Parameterized(0.0, expectedResult: "0.0");
		}

		[TestMethod]
		public void ToString_One() {
			ToString_Parameterized(1.0, expectedResult: "1.0");
		}

		[TestMethod]
		public void ToString_NaN() {
			ToString_Parameterized(double.NaN, expectedResult: "\"NaN\"");
		}

		[TestMethod]
		public void ToString_NegativeInfinity() {
			ToString_Parameterized(double.NegativeInfinity, expectedResult: "\"-Infinity\"");
		}

		[TestMethod]
		public void ToString_PositiveInfinity() {
			ToString_Parameterized(double.PositiveInfinity, expectedResult: "\"Infinity\"");
		}

		[TestMethod]
		public void ToString_DecimalValue_DefaultCulture() {
			// Arrange
			var node = new JsonDoubleNode(0.1234567);

			// Act
			string json = node.ToString();

			// Assert
			Assert.IsNotNull(node);
			Assert.AreEqual("0.1234567", json);
		}

		[TestMethod]
		public void ToString_DecimalValue_CultureWithDifferentDecimalSeparator() {
			CultureTestUtility.ExecuteInCulture("fr-FR", () => {
				// Arrange
				var node = new JsonDoubleNode(0.1234567);

				// Act
				string json = node.ToString();

				// Assert
				Assert.IsNotNull(node);
				Assert.AreEqual("0.1234567", json);
			});
		}

		#endregion

		#region ToObject(Type)

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ToObject_Null() {
			// Arrange
			var doubleNode = new JsonDoubleNode();
			Type type = null;

			// Act
			doubleNode.ToObject(type);
		}

		private void ToObject_Parameterized<T>(double value, T expectedResult) {
			// Arrange
			var integerNode = new JsonDoubleNode(value);

			// Act
			T result = integerNode.ToObject<T>();

			// Assert
			Assert.AreEqual(expectedResult, result);
		}

		[TestMethod]
		public void ToObject_String() {
			ToObject_Parameterized(42.0, expectedResult: "42");
		}

		[TestMethod]
		public void ToObject_Integer() {
			ToObject_Parameterized(42.0, expectedResult: 42);
		}

		[TestMethod]
		public void ToObject_Boolean_False() {
			ToObject_Parameterized(0.0, expectedResult: false);
		}

		[TestMethod]
		public void ToObject_Boolean_True() {
			ToObject_Parameterized(1.0, expectedResult: true);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidCastException))]
		public void ToObject_ZeroToEnum_ShouldFail() {
			ToObject_Parameterized(0.0, expectedResult: EnumForTesting.Test1);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidCastException))]
		public void ToObject_OneToEnum_ShouldFail() {
			ToObject_Parameterized(1.0, expectedResult: EnumForTesting.Test2);
		}

		#endregion

		#region WriteTo(JsonWriter)

		private void WriteTo_Parameterized(double value, string expectedResult) {
			// Arrange
			var doubleNode = new JsonDoubleNode(value);
			var writer = JsonWriter.Create();

			// Act
			doubleNode.WriteTo(writer);

			// Assert
			Assert.AreEqual(expectedResult, writer.ToString());
		}

		[TestMethod]
		public void WriteTo_Zero() {
			WriteTo_Parameterized(0.0, expectedResult: "0.0");
		}

		[TestMethod]
		public void WriteTo_One() {
			WriteTo_Parameterized(1.0, expectedResult: "1.0");
		}

		[TestMethod]
		public void WriteTo_NaN() {
			WriteTo_Parameterized(double.NaN, expectedResult: "\"NaN\"");
		}

		[TestMethod]
		public void WriteTo_NegativeInfinity() {
			WriteTo_Parameterized(double.NegativeInfinity, expectedResult: "\"-Infinity\"");
		}

		[TestMethod]
		public void WriteTo_PositiveInfinity() {
			WriteTo_Parameterized(double.PositiveInfinity, expectedResult: "\"Infinity\"");
		}

		#endregion

	}

}
