// Copyright (c) Rotorz Limited. All rights reserved.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rotorz.Json.MessagePack;
using System;
using System.IO;

namespace Rotorz.Json.Tests {

	[TestClass]
	public class MessagePackWriterTests {

		private static byte[] WriteJsonNodeToBytes(JsonNode node) {
			byte[] bytes;
			using (var stream = new MemoryStream()) {
				var writer = MessagePackWriter.Create(stream);
				node.WriteTo(writer);
				bytes = stream.ToArray();
			}
			return bytes;
		}

		#region Writing Objects

		[TestMethod]
		public void WriteObject_Empty() {
			// Arrange
			var node = new JsonObjectNode();

			// Act
			var data = WriteJsonNodeToBytes(node);

			// Assert
			CollectionAssert.AreEqual(new byte[] { 0x80 }, data);
		}

		[TestMethod]
		public void WriteObject_SingleProperty_Null() {
			// Arrange
			var node = new JsonObjectNode();
			node["ABC"] = null;

			// Act
			var data = WriteJsonNodeToBytes(node);

			// Assert
			var expectedData = new byte[] {
				0x81, // {
				0xA3, 0x41, 0x42, 0x43, // "ABC"
				0xC0 // null
			};
            CollectionAssert.AreEqual(expectedData, data);
		}

		[TestMethod]
		public void WriteObject_NestedObjects() {
			// Arrange
			var node = JsonObjectNode.FromJson(@"
				{
					""Genre"": ""Horror"",
					""Items"": [
						{
							""Name"": ""ABC"",
							""Certification"": 15
						},
						{
							""Name"": ""DEF"",
							""Certification"": 12
						}
					]
				}
			");
			
			// Act
			var data = WriteJsonNodeToBytes(node);

			// Assert
			var expectedData = new byte[] {
				0x82, // {
					0xA5, 0x47, 0x65, 0x6E, 0x72, 0x65, // "Genre"
					0xA6, 0x48, 0x6F, 0x72, 0x72, 0x6F, 0x72, // "Horror"
					0xA5, 0x49, 0x74, 0x65, 0x6D, 0x73, // "Items"
					0x92, // [
					0x82, // {
							0xA4, 0x4E, 0x61, 0x6D, 0x65, // "Name"
							0xA3, 0x41, 0x42, 0x43, // "ABC"
							0xAD, 0x43, 0x65, 0x72, 0x74, 0x69, 0x66, 0x69, 0x63, 0x61, 0x74, 0x69, 0x6F, 0x6E, // "Certification"
							0x0F, // 15
					//,
					0x82, // {
						0xA4, 0x4E, 0x61, 0x6D, 0x65, // "Name"
						0xA3, 0x44, 0x45, 0x46, // "DEF"
						0xAD, 0x43, 0x65, 0x72, 0x74, 0x69, 0x66, 0x69, 0x63, 0x61, 0x74, 0x69, 0x6F, 0x6E, // "Certification"
						0x0C, // 12

			};
			CollectionAssert.AreEqual(expectedData, data);
		}

		#endregion

		#region WriteNull()

		[TestMethod]
		public void WriteNull() {
			// Arrange

			// Act
			byte[] bytes;
			using (var stream = new MemoryStream()) {
				var writer = MessagePackWriter.Create(stream);
				writer.WriteNull();
				bytes = stream.ToArray();
			}

			// Assert
			CollectionAssert.AreEqual(new byte[] { 0xC0 }, bytes);
		}

		#endregion

		#region Writing Arrays

		[TestMethod]
		public void WriteArray_Empty() {
			// Arrange

			// Act

			// Assert
			Assert.Fail();
		}

		[TestMethod]
		public void WriteArray_SingleElement_Null() {
			// Arrange

			// Act

			// Assert
			Assert.Fail();
		}

		[TestMethod]
		public void WriteArray_SingleElement_String() {
			// Arrange

			// Act

			// Assert
			Assert.Fail();
		}

		[TestMethod]
		public void WriteArray_MultipleElements_String() {
			// Arrange

			// Act

			// Assert
			Assert.Fail();
		}

		#endregion

		#region WriteInteger(long)

		private void WriteInteger_Parameterized(long value, string expectedResult) {
			// Arrange
			var node = new JsonIntegerNode(value);

			// Act
			var data = WriteJsonNodeToBytes(node);

			// Assert
			Assert.AreEqual(expectedResult, BitConverter.ToString(data));
		}

		[TestMethod]
		public void WriteInteger_Zero() {
			WriteInteger_Parameterized(0, "00");
		}

		[TestMethod]
		public void WriteInteger_One() {
			WriteInteger_Parameterized(1, "01");
		}

		[TestMethod]
		public void WriteInteger_NegativeOne() {
			WriteInteger_Parameterized(-1, "E1");
		}

		[TestMethod]
		public void WriteInteger_Min32() {
			WriteInteger_Parameterized(int.MinValue, "D2-80-00-00-00");
		}

		[TestMethod]
		public void WriteInteger_Max32() {
			WriteInteger_Parameterized(int.MaxValue, "CE-7F-FF-FF-FF");
		}

		[TestMethod]
		public void WriteInteger_Min64() {
			WriteInteger_Parameterized(long.MinValue, "D3-80-00-00-00-00-00-00-00");
		}

		[TestMethod]
		public void WriteInteger_Max64() {
			WriteInteger_Parameterized(long.MaxValue, "CF-7F-FF-FF-FF-FF-FF-FF-FF");
		}

		#endregion

		#region WriteDouble(double)

		private void WriteDouble_Parameterized(double value, string expectedResult) {
			// Arrange

			// Act

			// Assert
			Assert.Fail();
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

			// Act

			// Assert
			Assert.Fail();
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
			var node = new JsonBooleanNode(true);

			// Act
			var bytes = WriteJsonNodeToBytes(node);

			// Assert
			CollectionAssert.AreEqual(new byte[] { 0xC3 }, bytes);
		}

		[TestMethod]
		private void WriteBoolean_False() {
			// Arrange
			var node = new JsonBooleanNode(false);

			// Act
			var bytes = WriteJsonNodeToBytes(node);

			// Assert
			CollectionAssert.AreEqual(new byte[] { 0xC2 }, bytes);
		}

		#endregion

	}

}
