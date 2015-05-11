// Copyright (c) Rotorz Limited. All rights reserved.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Rotorz.Json.Tests {

	[TestClass]
	public class JsonWriterTests {

		#region Writing Objects

		[TestMethod]
		public void WriteObject_Empty() {
			// Arrange
			var writer = JsonWriter.Create();

			// Act
			writer.WriteStartObject();
			writer.WriteEndObject();

			// Assert
			Assert.AreEqual("{}", writer.ToString());
		}

		[TestMethod]
		[DeploymentItem("Json/TestObjects/Files/JsonWriter/WriteObject_SingleProperty_Null.json")]
		public void WriteObject_SingleProperty_Null() {
			// Arrange
			var writer = JsonWriter.Create();

			// Act
			writer.WriteStartObject();
			writer.WritePropertyKey("ABC");
			writer.WriteNull();
			writer.WriteEndObject();

			// Assert
			string result = writer.ToString();
			string expectedResult = File.ReadAllText("WriteObject_SingleProperty_Null.json");
			Assert.AreEqual(expectedResult, result);
		}

		[TestMethod]
		[DeploymentItem("Json/TestObjects/Files/JsonWriter/WriteObject_NestedObjects.json")]
		public void WriteObject_NestedObjects() {
			// Arrange
			var writer = JsonWriter.Create();

			// Act
			writer.WriteStartObject();
			writer.WritePropertyKey("Genre");
			writer.WriteString("Horror");
			writer.WritePropertyKey("Items");
			writer.WriteStartArray();
			{
				writer.WriteStartObject();
				writer.WritePropertyKey("Name");
				writer.WriteString("ABC");
				writer.WritePropertyKey("Certification");
				writer.WriteInteger(15);
				writer.WriteEndObject();

				writer.WriteStartObject();
				writer.WritePropertyKey("Name");
				writer.WriteString("DEF");
				writer.WritePropertyKey("Certification");
				writer.WriteInteger(15);
				writer.WriteEndObject();
			}
			writer.WriteEndArray();
			writer.WriteEndObject();

			// Assert
			string result = writer.ToString();
			string expectedResult = File.ReadAllText("WriteObject_NestedObjects.json");
			Assert.AreEqual(expectedResult, result);
		}

		#endregion

		#region WriteNull()

		[TestMethod]
		public void WriteNull() {
			// Arrange
			var writer = JsonWriter.Create();

			// Act
			writer.WriteNull();

			// Assert
			Assert.AreEqual("null", writer.ToString());
		}

		#endregion

		#region Writing Arrays

		[TestMethod]
		public void WriteArray_Empty() {
			// Arrange
			var writer = JsonWriter.Create();

			// Act
			writer.WriteStartArray();
			writer.WriteEndArray();

			// Assert
			Assert.AreEqual("[]", writer.ToString());
		}

		[TestMethod]
		[DeploymentItem("Json/TestObjects/Files/JsonWriter/WriteArray_SingleElement_Null.json")]
		public void WriteArray_SingleElement_Null() {
			// Arrange
			var writer = JsonWriter.Create();

			// Act
			writer.WriteStartArray();
			writer.WriteNull();
			writer.WriteEndArray();

			// Assert
			string result = writer.ToString();
			string expectedResult = File.ReadAllText("WriteArray_SingleElement_Null.json");
			Assert.AreEqual(expectedResult, result);
		}

		[TestMethod]
		[DeploymentItem("Json/TestObjects/Files/JsonWriter/WriteArray_SingleElement_String.json")]
		public void WriteArray_SingleElement_String() {
			// Arrange
			var writer = JsonWriter.Create();

			// Act
			writer.WriteStartArray();
			writer.WriteString("Hello World!");
			writer.WriteEndArray();

			// Assert
			string result = writer.ToString();
			string expectedResult = File.ReadAllText("WriteArray_SingleElement_String.json");
			Assert.AreEqual(expectedResult, result);
		}

		[TestMethod]
		[DeploymentItem("Json/TestObjects/Files/JsonWriter/WriteArray_MultipleElements_String.json")]
		public void WriteArray_MultipleElements_String() {
			// Arrange
			var writer = JsonWriter.Create();

			// Act
			writer.WriteStartArray();
			writer.WriteString("A");
			writer.WriteString("B");
			writer.WriteString("C");
			writer.WriteEndArray();

			// Assert
			string result = writer.ToString();
			string expectedResult = File.ReadAllText("WriteArray_MultipleElements_String.json");
			Assert.AreEqual(expectedResult, result);
		}

		#endregion

		#region WriteInteger(long)

		private void WriteInteger_Parameterized(long value, string expectedResult) {
			// Arrange
			var writer = JsonWriter.Create();

			// Act
			writer.WriteInteger(value);

			// Assert
			Assert.AreEqual(expectedResult, writer.ToString());
		}

		[TestMethod]
		public void WriteInteger_Zero() {
			WriteInteger_Parameterized(0, "0");
		}

		[TestMethod]
		public void WriteInteger_One() {
			WriteInteger_Parameterized(1, "1");
		}

		[TestMethod]
		public void WriteInteger_NegativeOne() {
			WriteInteger_Parameterized(-1, "-1");
		}

		[TestMethod]
		public void WriteInteger_Min32() {
			WriteInteger_Parameterized(int.MinValue, "-2147483648");
		}

		[TestMethod]
		public void WriteInteger_Max32() {
			WriteInteger_Parameterized(int.MaxValue, "2147483647");
		}

		[TestMethod]
		public void WriteInteger_Min64() {
			WriteInteger_Parameterized(long.MinValue, "-9223372036854775808");
		}

		[TestMethod]
		public void WriteInteger_Max64() {
			WriteInteger_Parameterized(long.MaxValue, "9223372036854775807");
		}

		#endregion

		#region WriteDouble(double)

		private void WriteDouble_Parameterized(double value, string expectedResult) {
			// Arrange
			var writer = JsonWriter.Create();

			// Act
			writer.WriteDouble(value);

			// Assert
			Assert.AreEqual(expectedResult, writer.ToString());
		}

		[TestMethod]
		public void WriteDouble_Zero() {
			WriteDouble_Parameterized(0.0, "0.0");
		}

		[TestMethod]
		public void WriteDouble_One() {
			WriteDouble_Parameterized(1.0, "1.0");
		}

		[TestMethod]
		public void WriteDouble_NegativeOne() {
			WriteDouble_Parameterized(-1.0, "-1.0");
		}

		[TestMethod]
		public void WriteDouble_LargeValue() {
			WriteDouble_Parameterized(1000000000000000.0, "1e+15");
		}

		[TestMethod]
		public void WriteDouble_AlmostLargeValue() {
			WriteDouble_Parameterized(100000000000000.0, "100000000000000.0");
		}

		[TestMethod]
		public void WriteDouble_SmallValue() {
			WriteDouble_Parameterized(0.00001, "1e-05");
		}

		[TestMethod]
		public void WriteDouble_AlmostSmallValue() {
			WriteDouble_Parameterized(0.0001, "0.0001");
		}

		[TestMethod]
		public void WriteDouble_NaN() {
			WriteDouble_Parameterized(double.NaN, "\"NaN\"");
		}

		[TestMethod]
		public void WriteDouble_NegativeInfinity() {
			WriteDouble_Parameterized(double.NegativeInfinity, "\"-Infinity\"");
		}

		[TestMethod]
		public void WriteDouble_PositiveInfinity() {
			WriteDouble_Parameterized(double.PositiveInfinity, "\"Infinity\"");
		}

		#endregion

		#region WriteString(string)

		private void WriteString_Parameterized(string value, string expectedResult) {
			// Arrange
			var writer = JsonWriter.Create();

			// Act
			writer.WriteString(value);

			// Assert
			Assert.AreEqual(expectedResult, writer.ToString());
		}

		[TestMethod]
		public void WriteString_Null() {
			WriteString_Parameterized(null, "\"\"");
		}

		[TestMethod]
		public void WriteString_Empty() {
			WriteString_Parameterized("", "\"\"");
		}

		[TestMethod]
		public void WriteString_SimpleCharacters() {
			WriteString_Parameterized("Hello World!", "\"Hello World!\"");
		}

		[TestMethod]
		public void WriteString_EscapeSequences() {
			WriteString_Parameterized("Hello\r\n\tWorld!", "\"Hello\\r\\n\\tWorld!\"");
		}

		#endregion

		#region WriteBoolean(boolean)

		[TestMethod]
		private void WriteBoolean_True() {
			// Arrange
			var writer = JsonWriter.Create();

			// Act
			writer.WriteBoolean(true);

			// Assert
			Assert.AreEqual("true", writer.ToString());
		}

		[TestMethod]
		private void WriteBoolean_False() {
			// Arrange
			var writer = JsonWriter.Create();

			// Act
			writer.WriteBoolean(false);

			// Assert
			Assert.AreEqual("false", writer.ToString());
		}

		#endregion

	}

}
