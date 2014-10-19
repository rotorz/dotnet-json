// Copyright (c) 2014 Rotorz Limited. All rights reserved.

using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rotorz.Json.Tests {

	[TestClass]
	public class JsonBooleanNodeTests {

		#region Value

		[TestMethod]
		public void Value_set_False() {
			// Arrange
			var booleanNode = new JsonBooleanNode();

			// Act
			booleanNode.Value = false;

			// Assert
			Assert.AreEqual(false, booleanNode.Value);
		}

		[TestMethod]
		public void Value_set_True() {
			// Arrange
			var booleanNode = new JsonBooleanNode();

			// Act
			booleanNode.Value = true;

			// Assert
			Assert.AreEqual(true, booleanNode.Value);
		}

		#endregion

		#region Clone()

		[TestMethod]
		public void Clone_False() {
			// Arrange
			var booleanNode = new JsonBooleanNode(false);

			// Act
			var cloneNode = booleanNode.Clone() as JsonBooleanNode;

			// Assert
			Assert.AreNotSame(booleanNode, cloneNode);
			Assert.AreEqual(booleanNode.Value, cloneNode.Value);
		}

		[TestMethod]
		public void Clone_True() {
			// Arrange
			var booleanNode = new JsonBooleanNode(true);

			// Act
			var cloneNode = booleanNode.Clone() as JsonBooleanNode;

			// Assert
			Assert.AreNotSame(booleanNode, cloneNode);
			Assert.AreEqual(booleanNode.Value, cloneNode.Value);
		}

		#endregion

		#region ToString()

		[TestMethod]
		public void ToString_False() {
			// Arrange
			var booleanNode = new JsonBooleanNode(false);

			// Act
			string result = booleanNode.ToString();

			// Assert
			Assert.AreEqual("false", result);
		}

		[TestMethod]
		public void ToString_True() {
			// Arrange
			var booleanNode = new JsonBooleanNode(true);

			// Act
			string result = booleanNode.ToString();

			// Assert
			Assert.AreEqual("true", result);
		}

		#endregion

		#region ToObject(Type)

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ToObject_Null() {
			// Arrange
			var booleanNode = new JsonBooleanNode();
			Type type = null;

			// Act
			booleanNode.ToObject(type);
		}

		[TestMethod]
		public void ToObject_String_False() {
			// Arrange
			var booleanNode = new JsonBooleanNode(false);

			// Act
			string result = booleanNode.ToObject<string>();

			// Assert
			Assert.AreEqual("False", result);
		}

		[TestMethod]
		public void ToObject_String_True() {
			// Arrange
			var booleanNode = new JsonBooleanNode(true);

			// Act
			string result = booleanNode.ToObject<string>();

			// Assert
			Assert.AreEqual("True", result);
		}

		[TestMethod]
		public void ToObject_Double_False() {
			// Arrange
			var booleanNode = new JsonBooleanNode(false);

			// Act
			double result = booleanNode.ToObject<double>();

			// Assert
			Assert.AreEqual(0.0, result);
		}

		[TestMethod]
		public void ToObject_Double_True() {
			// Arrange
			var booleanNode = new JsonBooleanNode(true);

			// Act
			double result = booleanNode.ToObject<double>();

			// Assert
			Assert.AreEqual(1.0, result);
		}

		[TestMethod]
		public void ToObject_Integer_False() {
			// Arrange
			var booleanNode = new JsonBooleanNode(false);

			// Act
			int result = booleanNode.ToObject<int>();

			// Assert
			Assert.AreEqual(0, result);
		}

		[TestMethod]
		public void ToObject_Integer_True() {
			// Arrange
			var booleanNode = new JsonBooleanNode(true);

			// Act
			int result = booleanNode.ToObject<int>();

			// Assert
			Assert.AreEqual(1, result);
		}

		#endregion

		#region WriteTo(JsonWriter)

		[TestMethod]
		public void WriteTo_False() {
			// Arrange
			var booleanNode = new JsonBooleanNode(false);
			var writer = JsonWriter.Create();

			// Act
			booleanNode.WriteTo(writer);

			// Assert
			Assert.AreEqual("false", writer.ToString());
		}

		[TestMethod]
		public void WriteTo_True() {
			// Arrange
			var booleanNode = new JsonBooleanNode(true);
			var writer = JsonWriter.Create();

			// Act
			booleanNode.WriteTo(writer);

			// Assert
			Assert.AreEqual("true", writer.ToString());
		}

		#endregion

	}

}
