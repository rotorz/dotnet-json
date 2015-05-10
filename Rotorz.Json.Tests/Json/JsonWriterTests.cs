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
			writer.WriteValue("Horror");
			writer.WritePropertyKey("Items");
			writer.WriteStartArray();
			{
				writer.WriteStartObject();
				writer.WritePropertyKey("Name");
				writer.WriteValue("ABC");
				writer.WritePropertyKey("Certification");
				writer.WriteValue(15);
				writer.WriteEndObject();

				writer.WriteStartObject();
				writer.WritePropertyKey("Name");
				writer.WriteValue("DEF");
				writer.WritePropertyKey("Certification");
				writer.WriteValue(15);
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

		#region WriteValueRaw(string)

		[TestMethod]
		public void WriteValueRaw_Null() {
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
			writer.WriteValue("Hello World!");
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
			writer.WriteValue("A");
			writer.WriteValue("B");
			writer.WriteValue("C");
			writer.WriteEndArray();

			// Assert
			string result = writer.ToString();
			string expectedResult = File.ReadAllText("WriteArray_MultipleElements_String.json");
			Assert.AreEqual(expectedResult, result);
		}

		#endregion

		#region WriteValue(long)

		private void WriteValue_Integer_Parameterized(long value, string expectedResult) {
			// Arrange
			var writer = JsonWriter.Create();

			// Act
			writer.WriteValue(value);

			// Assert
			Assert.AreEqual(expectedResult, writer.ToString());
		}

		[TestMethod]
		public void WriteValue_Integer_Zero() {
			WriteValue_Integer_Parameterized(0, "0");
		}

		[TestMethod]
		public void WriteValue_Integer_One() {
			WriteValue_Integer_Parameterized(1, "1");
		}

		[TestMethod]
		public void WriteValue_Integer_NegativeOne() {
			WriteValue_Integer_Parameterized(-1, "-1");
		}

		[TestMethod]
		public void WriteValue_Integer_Min32() {
			WriteValue_Integer_Parameterized(int.MinValue, "-2147483648");
		}

		[TestMethod]
		public void WriteValue_Integer_Max32() {
			WriteValue_Integer_Parameterized(int.MaxValue, "2147483647");
		}

		[TestMethod]
		public void WriteValue_Integer_Min64() {
			WriteValue_Integer_Parameterized(long.MinValue, "-9223372036854775808");
		}

		[TestMethod]
		public void WriteValue_Integer_Max64() {
			WriteValue_Integer_Parameterized(long.MaxValue, "9223372036854775807");
		}

		#endregion

		#region WriteValue(double)

		private void WriteValue_Double_Parameterized(double value, string expectedResult) {
			// Arrange
			var writer = JsonWriter.Create();

			// Act
			writer.WriteValue(value);

			// Assert
			Assert.AreEqual(expectedResult, writer.ToString());
		}

		[TestMethod]
		public void WriteValue_Double_Zero() {
			WriteValue_Double_Parameterized(0.0, "0.0");
		}

		[TestMethod]
		public void WriteValue_Double_One() {
			WriteValue_Double_Parameterized(1.0, "1.0");
		}

		[TestMethod]
		public void WriteValue_Double_NegativeOne() {
			WriteValue_Double_Parameterized(-1.0, "-1.0");
		}

		[TestMethod]
		public void WriteValue_Double_LargeValue() {
			WriteValue_Double_Parameterized(1000000000000000.0, "1e+15");
		}

		[TestMethod]
		public void WriteValue_Double_AlmostLargeValue() {
			WriteValue_Double_Parameterized(100000000000000.0, "100000000000000.0");
		}

		[TestMethod]
		public void WriteValue_Double_SmallValue() {
			WriteValue_Double_Parameterized(0.00001, "1e-05");
		}

		[TestMethod]
		public void WriteValue_Double_AlmostSmallValue() {
			WriteValue_Double_Parameterized(0.0001, "0.0001");
		}

		[TestMethod]
		public void WriteValue_Double_NaN() {
			WriteValue_Double_Parameterized(double.NaN, "\"NaN\"");
		}

		[TestMethod]
		public void WriteValue_Double_NegativeInfinity() {
			WriteValue_Double_Parameterized(double.NegativeInfinity, "\"-Infinity\"");
		}

		[TestMethod]
		public void WriteValue_Double_PositiveInfinity() {
			WriteValue_Double_Parameterized(double.PositiveInfinity, "\"Infinity\"");
		}

		#endregion

		#region WriteValue(string)

		private void WriteValue_String_Parameterized(string value, string expectedResult) {
			// Arrange
			var writer = JsonWriter.Create();

			// Act
			writer.WriteValue(value);

			// Assert
			Assert.AreEqual(expectedResult, writer.ToString());
		}

		[TestMethod]
		public void WriteValue_String_Null() {
			WriteValue_String_Parameterized(null, "\"\"");
		}

		[TestMethod]
		public void WriteValue_String_Empty() {
			WriteValue_String_Parameterized("", "\"\"");
		}

		[TestMethod]
		public void WriteValue_String_SimpleCharacters() {
			WriteValue_String_Parameterized("Hello World!", "\"Hello World!\"");
		}

		[TestMethod]
		public void WriteValue_String_EscapeSequences() {
			WriteValue_String_Parameterized("Hello\r\n\tWorld!", "\"Hello\\r\\n\\tWorld!\"");
		}

		#endregion

		#region WriteValue(boolean)

		private void WriteValue_Bool_True() {
			// Arrange
			var writer = JsonWriter.Create();

			// Act
			writer.WriteValue(true);

			// Assert
			Assert.AreEqual("true", writer.ToString());
		}

		private void WriteValue_Bool_False() {
			// Arrange
			var writer = JsonWriter.Create();

			// Act
			writer.WriteValue(false);

			// Assert
			Assert.AreEqual("false", writer.ToString());
		}

		#endregion

	}

}
