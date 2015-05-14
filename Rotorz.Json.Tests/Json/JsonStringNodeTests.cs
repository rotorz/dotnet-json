// Copyright (c) Rotorz Limited. All rights reserved.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Rotorz.Json.Tests {

	[TestClass]
	public class JsonStringNodeTests {

		#region Value

		[TestMethod]
		public void Value_set_Null() {
			// Arrange
			var stringNode = new JsonStringNode();

			// Act
			stringNode.Value = null;

			// Assert
			Assert.AreEqual("", stringNode.Value);
		}

		[TestMethod]
		public void Value_set_Empty() {
			// Arrange
			var stringNode = new JsonStringNode();

			// Act
			stringNode.Value = "";

			// Assert
			Assert.AreEqual("", stringNode.Value);
		}

		[TestMethod]
		public void Value_set_SimpleCharacters() {
			// Arrange
			var stringNode = new JsonStringNode();

			// Act
			stringNode.Value = "Hello World!";

			// Assert
			Assert.AreEqual("Hello World!", stringNode.Value);
		}

		#endregion

		#region Clone()

		[TestMethod]
		public void Clone_Null() {
			// Arrange
			var stringNode = new JsonStringNode(null);

			// Act
			var cloneNode = stringNode.Clone() as JsonStringNode;

			// Assert
			Assert.AreNotSame(stringNode, cloneNode);
			Assert.AreEqual(stringNode.Value, cloneNode.Value);
		}

		[TestMethod]
		public void Clone_SimpleCharacters() {
			// Arrange
			var stringNode = new JsonStringNode("Hello World!");

			// Act
			var cloneNode = stringNode.Clone() as JsonStringNode;

			// Assert
			Assert.AreNotSame(stringNode, cloneNode);
			Assert.AreEqual(stringNode.Value, cloneNode.Value);
		}

		#endregion

		#region ToString()

		[TestMethod]
		public void ToString_Null() {
			// Arrange
			var stringNode = new JsonStringNode(null);

			// Act
			string result = stringNode.ToString();

			// Assert
			Assert.AreEqual("\"\"", result);
		}

		[TestMethod]
		public void ToString_Empty() {
			// Arrange
			var stringNode = new JsonStringNode("");

			// Act
			string result = stringNode.ToString();

			// Assert
			Assert.AreEqual("\"\"", result);
		}

		[TestMethod]
		public void ToString_SimpleCharacters() {
			// Arrange
			var stringNode = new JsonStringNode("Hello World!");

			// Act
			string result = stringNode.ToString();

			// Assert
			Assert.AreEqual("\"Hello World!\"", result);
		}

		[TestMethod]
		public void ToString_EscapeSequences() {
			// Arrange
			var stringNode = new JsonStringNode("Hello\r\n\tWorld!");

			// Act
			string result = stringNode.ToString();

			// Assert
			Assert.AreEqual("\"Hello\\r\\n\\tWorld!\"", result);
		}

		#endregion

		#region ToObject(Type)

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ToObject_Null() {
			// Arrange
			var stringNode = new JsonStringNode();
			Type type = null;

			// Act
			stringNode.ConvertTo(type);
		}

		[TestMethod]
		public void ToObject_NumberStringToString() {
			// Arrange
			var stringNode = new JsonStringNode("42");

			// Act
			string value = stringNode.ConvertTo<string>();

			// Assert
			Assert.AreEqual("42", value);
		}

		[TestMethod]
		public void ToObject_NumberStringToInt32() {
			// Arrange
			var stringNode = new JsonStringNode("42");

			// Act
			int value = stringNode.ConvertTo<int>();

			// Assert
			Assert.AreEqual(42, value);
		}

		[TestMethod]
		public void ToObject_FalseToBoolean() {
			// Arrange
			var stringNode = new JsonStringNode("false");

			// Act
			bool value = stringNode.ConvertTo<bool>();

			// Assert
			Assert.AreEqual(false, value);
		}

		[TestMethod]
		public void ToObject_TrueToBoolean() {
			// Arrange
			var stringNode = new JsonStringNode("true");

			// Act
			bool value = stringNode.ConvertTo<bool>();

			// Assert
			Assert.AreEqual(true, value);
		}

		#endregion

		#region WriteTo(JsonWriter)

		[TestMethod]
		public void WriteTo_Null() {
			// Arrange
			var stringNode = new JsonStringNode(null);
			var writer = JsonWriter.Create();

			// Act
			stringNode.Write(writer);

			// Assert
			Assert.AreEqual("\"\"", writer.ToString());
		}

		[TestMethod]
		public void WriteTo_Empty() {
			// Arrange
			var stringNode = new JsonStringNode("");
			var writer = JsonWriter.Create();

			// Act
			stringNode.Write(writer);

			// Assert
			Assert.AreEqual("\"\"", writer.ToString());
		}

		[TestMethod]
		public void WriteTo_SimpleCharacters() {
			// Arrange
			var stringNode = new JsonStringNode("Hello World!");
			var writer = JsonWriter.Create();

			// Act
			stringNode.Write(writer);

			// Assert
			Assert.AreEqual("\"Hello World!\"", writer.ToString());
		}

		[TestMethod]
		public void WriteTo_EscapeSequences() {
			// Arrange
			var stringNode = new JsonStringNode("Hello\r\n\tWorld!");
			var writer = JsonWriter.Create();

			// Act
			stringNode.Write(writer);
			
			// Assert
			Assert.AreEqual("\"Hello\\r\\n\\tWorld!\"", writer.ToString());
		}

		#endregion

	}

}
