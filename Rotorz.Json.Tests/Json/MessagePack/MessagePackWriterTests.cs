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
				node.Write(writer);
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
			var bytes = WriteJsonNodeToBytes(node);

			// Assert
			CollectionAssert.AreEqual(new byte[] { 0x80 }, bytes);
		}

		[TestMethod]
		public void WriteObject_SingleProperty_Null() {
			// Arrange
			var node = new JsonObjectNode();
			node["ABC"] = null;

			// Act
			var bytes = WriteJsonNodeToBytes(node);

			// Assert
			var expectedData = new byte[] {
				0x81, // {
				0xA3, 0x41, 0x42, 0x43, // "ABC"
				0xC0 // null
			};
            CollectionAssert.AreEqual(expectedData, bytes);
		}

		[TestMethod]
		public void WriteObject_NestedObjects() {
			// Arrange
			var node = JsonObjectNode.ReadFrom(@"
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
			var bytes = WriteJsonNodeToBytes(node);

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
			CollectionAssert.AreEqual(expectedData, bytes);
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
			var node = new JsonArrayNode();

			// Act
			var bytes = WriteJsonNodeToBytes(node);

			// Assert
			CollectionAssert.AreEqual(new byte[] { 0x90 }, bytes);
		}

		[TestMethod]
		public void WriteArray_SingleElement_Null() {
			// Arrange
			var node = new JsonArrayNode();
			node.Add(null);

			// Act
			var bytes = WriteJsonNodeToBytes(node);

			// Assert
			var expectedData = new byte[] { 0x91, 0xC0 };
			CollectionAssert.AreEqual(expectedData, bytes);
		}

		[TestMethod]
		public void WriteArray_SingleElement_String() {
			// Arrange
			var node = new JsonArrayNode();
			node.Add(new JsonStringNode("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur."));

			// Act
			var bytes = WriteJsonNodeToBytes(node);

			// Assert
			var expectedData = "91-DA-01-4E-4C-6F-72-65-6D-20-69-70-73-75-6D-20-64-6F-6C-6F-72-20-73-69-74-20-61-6D-65-74-2C-20-63-6F-6E-73-65-63-74-65-74-75-72-20-61-64-69-70-69-73-63-69-6E-67-20-65-6C-69-74-2C-20-73-65-64-20-64-6F-20-65-69-75-73-6D-6F-64-20-74-65-6D-70-6F-72-20-69-6E-63-69-64-69-64-75-6E-74-20-75-74-20-6C-61-62-6F-72-65-20-65-74-20-64-6F-6C-6F-72-65-20-6D-61-67-6E-61-20-61-6C-69-71-75-61-2E-20-55-74-20-65-6E-69-6D-20-61-64-20-6D-69-6E-69-6D-20-76-65-6E-69-61-6D-2C-20-71-75-69-73-20-6E-6F-73-74-72-75-64-20-65-78-65-72-63-69-74-61-74-69-6F-6E-20-75-6C-6C-61-6D-63-6F-20-6C-61-62-6F-72-69-73-20-6E-69-73-69-20-75-74-20-61-6C-69-71-75-69-70-20-65-78-20-65-61-20-63-6F-6D-6D-6F-64-6F-20-63-6F-6E-73-65-71-75-61-74-2E-20-44-75-69-73-20-61-75-74-65-20-69-72-75-72-65-20-64-6F-6C-6F-72-20-69-6E-20-72-65-70-72-65-68-65-6E-64-65-72-69-74-20-69-6E-20-76-6F-6C-75-70-74-61-74-65-20-76-65-6C-69-74-20-65-73-73-65-20-63-69-6C-6C-75-6D-20-64-6F-6C-6F-72-65-20-65-75-20-66-75-67-69-61-74-20-6E-75-6C-6C-61-20-70-61-72-69-61-74-75-72-2E";
			Assert.AreEqual(expectedData, BitConverter.ToString(bytes));
		}

		[TestMethod]
		public void WriteArray_MultipleElements_String() {
			// Arrange
			var node = new JsonArrayNode();
			node.Add(new JsonStringNode("A"));
			node.Add(new JsonStringNode("B"));
			node.Add(new JsonStringNode("C"));

			// Act
			var bytes = WriteJsonNodeToBytes(node);

			// Assert
			var expectedData = "93-A1-41-A1-42-A1-43";
			Assert.AreEqual(expectedData, BitConverter.ToString(bytes));
		}

		#endregion

		#region WriteInteger(long)

		private void WriteInteger_Parameterized(long value, string expectedResult) {
			// Arrange
			var node = new JsonIntegerNode(value);

			// Act
			var bytes = WriteJsonNodeToBytes(node);

			// Assert
			Assert.AreEqual(expectedResult, BitConverter.ToString(bytes));
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
			var node = new JsonDoubleNode(value);

			// Act
			var bytes = WriteJsonNodeToBytes(node);

			// Assert
			Assert.AreEqual(expectedResult, BitConverter.ToString(bytes));
		}

		[TestMethod]
		public void WriteDouble_Zero() {
			WriteDouble_Parameterized(0.0, "CA-00-00-00-00");
		}

		[TestMethod]
		public void WriteDouble_One() {
			WriteDouble_Parameterized(1.0, "CA-3F-80-00-00");
		}

		[TestMethod]
		public void WriteDouble_NegativeOne() {
			WriteDouble_Parameterized(-1.0, "CA-BF-80-00-00");
		}

		[TestMethod]
		public void WriteDouble_LargeValue() {
			WriteDouble_Parameterized(1000000000000000.0, "CA-58-63-5F-A9");
		}

		[TestMethod]
		public void WriteDouble_AlmostLargeValue() {
			WriteDouble_Parameterized(100000000000000.0, "CA-56-B5-E6-21");
		}

		[TestMethod]
		public void WriteDouble_SmallValue() {
			WriteDouble_Parameterized(0.00001, "CA-37-27-C5-AC");
		}

		[TestMethod]
		public void WriteDouble_AlmostSmallValue() {
			WriteDouble_Parameterized(0.0001, "CA-38-D1-B7-17");
		}

		[TestMethod]
		public void WriteDouble_NaN() {
			WriteDouble_Parameterized(double.NaN, "A3-4E-61-4E");
		}

		[TestMethod]
		public void WriteDouble_NegativeInfinity() {
			WriteDouble_Parameterized(double.NegativeInfinity, "A9-2D-49-6E-66-69-6E-69-74-79");
		}

		[TestMethod]
		public void WriteDouble_PositiveInfinity() {
			WriteDouble_Parameterized(double.PositiveInfinity, "A8-49-6E-66-69-6E-69-74-79");
		}

		#endregion

		#region WriteString(string)

		[TestMethod]
		public void WriteString_Null() {
			// Arrange
			string value = null;

			// Act
			byte[] bytes;
			using (var stream = new MemoryStream()) {
				var writer = MessagePackWriter.Create(stream);
				writer.WriteString(value);
				bytes = stream.ToArray();
			}

			// Assert
			Assert.AreEqual("A0", BitConverter.ToString(bytes));
		}

		private void WriteString_Parameterized(string value, string expectedResult) {
			// Arrange
			var node = new JsonStringNode(value);

			// Act
			var bytes = WriteJsonNodeToBytes(node);

			// Assert
			Assert.AreEqual(expectedResult, BitConverter.ToString(bytes));
		}

		[TestMethod]
		public void WriteString_Empty() {
			WriteString_Parameterized("", "A0");
		}

		[TestMethod]
		public void WriteString_SimpleCharacters() {
			WriteString_Parameterized("Hello World!", "AC-48-65-6C-6C-6F-20-57-6F-72-6C-64-21");
		}

		[TestMethod]
		public void WriteString_EscapeSequences() {
			WriteString_Parameterized("Hello\r\n\tWorld!", "AE-48-65-6C-6C-6F-0D-0A-09-57-6F-72-6C-64-21");
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
