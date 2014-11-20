// Copyright (c) Rotorz Limited. All rights reserved.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rotorz.Json.Tests.TestObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Rotorz.Json.Tests {

	[TestClass]
	public class JsonArrayNodeTests {

		#region Factory: FromArray(object[])

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void FromArray_Null() {
			// Arrange
			object[] array = null;

			// Act
			JsonArrayNode.FromArray(array);
		}

		[TestMethod]
		public void FromArray_Empty() {
			// Arrange
			object[] array = { };

			// Act
			var arrayNode = JsonArrayNode.FromArray(array);

			// Assert
			Assert.IsNotNull(arrayNode);
			Assert.AreEqual(0, arrayNode.Count);
		}

		[TestMethod]
		public void FromArray_NumericElements() {
			// Arrange
			int[] numbers = { 2, 4, 6, 8, 10 };

			// Act
			var arrayNode = JsonArrayNode.FromArray(numbers);

			// Assert
			Assert.IsNotNull(arrayNode);
			Assert.AreEqual(numbers.Length, arrayNode.Count);

			for (int i = 0; i < numbers.Length; ++i) {
				var elementNode = arrayNode[i] as JsonIntegerNode;
				Assert.IsNotNull(elementNode);
				Assert.AreEqual(numbers[i], (int)elementNode.Value);
			}
		}

		[TestMethod]
		public void FromArray_StringElements() {
			// Arrange
			string[] strings = { "A", "B", "C", "D", "E" };

			// Act
			var arrayNode = JsonArrayNode.FromArray(strings);

			// Assert
			Assert.IsNotNull(arrayNode);
			Assert.AreEqual(strings.Length, arrayNode.Count);

			for (int i = 0; i < strings.Length; ++i) {
				var elementNode = arrayNode[i] as JsonStringNode;
				Assert.IsNotNull(elementNode);
				Assert.AreEqual(strings[i], elementNode.Value);
			}
		}

		[TestMethod]
		public void FromArray_MixedElements() {
			// Arrange
			object[] mixed = { "A", 1, "C", 2, "E", 3 };

			// Act
			var arrayNode = JsonArrayNode.FromArray(mixed);

			// Assert
			Assert.IsNotNull(arrayNode);
			Assert.AreEqual(mixed.Length, arrayNode.Count);

			for (int i = 0; i < mixed.Length; ++i) {
				var elementNode = arrayNode[i];

				if (elementNode is JsonIntegerNode) {
					var integerNode = elementNode as JsonIntegerNode;
					Assert.AreEqual(mixed[i], (int)integerNode.Value);
				}
				else if (elementNode is JsonStringNode) {
					var stringNode = elementNode as JsonStringNode;
					Assert.AreEqual(mixed[i], stringNode.Value);
				}
				else {
					Assert.Fail("Invalid node type.");
				}
			}
		}

		#endregion

		#region Factory: FromCollection(ICollection)

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void FromCollection_Null() {
			// Arrange
			ICollection collection = null;

			// Act
			JsonArrayNode.FromCollection(collection);
		}

		[TestMethod]
		public void FromCollection_List_Empty() {
			// Arrange
			var list = new List<int>();

			// Act
			var arrayNode = JsonArrayNode.FromCollection(list);

			// Assert
			Assert.IsNotNull(arrayNode);
			Assert.AreEqual(0, arrayNode.Count);
		}

		[TestMethod]
		public void FromCollection_List_Numbers() {
			// Arrange
			var numbers = new List<int>(new int[] { 2, 4, 6, 8, 10 });

			// Act
			var arrayNode = JsonArrayNode.FromCollection(numbers);

			// Assert
			Assert.IsNotNull(arrayNode);
			Assert.AreEqual(5, arrayNode.Count);

			for (int i = 0; i < numbers.Count; ++i) {
				var elementNode = arrayNode[i] as JsonIntegerNode;
				Assert.IsNotNull(elementNode);
				Assert.AreEqual(numbers[i], (int)elementNode.Value);
			}
		}

		[TestMethod]
		public void FromCollection_List_Strings() {
			// Arrange
			var strings = new List<string>(new string[] { "A", "B", "C", "D", "E" });

			// Act
			var arrayNode = JsonArrayNode.FromCollection(strings);

			// Assert
			Assert.IsNotNull(arrayNode);
			Assert.AreEqual(5, arrayNode.Count);

			for (int i = 0; i < strings.Count; ++i) {
				var elementNode = arrayNode[i] as JsonStringNode;
				Assert.IsNotNull(elementNode);
				Assert.AreEqual(strings[i], elementNode.Value);
			}
		}

		[TestMethod]
		public void FromCollection_ArrayList_Mixed() {
			// Arrange
			var mixed = new ArrayList(new object[] { "A", 1, "C", 2, "E", 3 });

			// Act
			var arrayNode = JsonArrayNode.FromCollection(mixed);

			// Assert
			Assert.IsNotNull(arrayNode);
			Assert.AreEqual(6, arrayNode.Count);

			for (int i = 0; i < mixed.Count; ++i) {
				var elementNode = arrayNode[i];

				if (elementNode is JsonIntegerNode) {
					var integerNode = elementNode as JsonIntegerNode;
					Assert.AreEqual(mixed[i], (int)integerNode.Value);
				}
				else if (elementNode is JsonStringNode) {
					var stringNode = elementNode as JsonStringNode;
					Assert.AreEqual(mixed[i], stringNode.Value);
				}
				else {
					Assert.Fail("Invalid node type.");
				}
			}
		}

		#endregion

		#region JsonArrayNode()

		[TestMethod]
		public void Create() {
			// Arrange

			// Act
			var arrayNode = new JsonArrayNode();

			// Assert
			Assert.AreEqual(0, arrayNode.Count);
		}

		#endregion

		#region JsonArrayNode(JsonNodes[])

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_Array_Null() {
			// Arrange
			JsonNode[] array = null;

			// Act
			new JsonArrayNode(array);
		}

		[TestMethod]
		public void Create_Array_Empty() {
			// Arrange
			JsonNode[] array = { };

			// Act
			var arrayNode = new JsonArrayNode(new JsonNode[0]);

			// Assert
			Assert.AreEqual(0, arrayNode.Count);
		}

		[TestMethod]
		public void Create_Array_WithIntegerNodes() {
			// Arrange
			JsonNode[] nodes = {
				new JsonIntegerNode(1),
				new JsonIntegerNode(2),
				new JsonIntegerNode(3)
			};

			// Act
			var arrayNode = new JsonArrayNode(nodes);

			// Assert
			Assert.AreEqual(3, arrayNode.Count);
			Assert.AreEqual(1, (int)(arrayNode[0] as JsonIntegerNode).Value);
			Assert.AreEqual(2, (int)(arrayNode[1] as JsonIntegerNode).Value);
			Assert.AreEqual(3, (int)(arrayNode[2] as JsonIntegerNode).Value);
		}

		#endregion

		#region JsonArrayNode(ICollection<JsonNode>)

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_Collection_Null() {
			// Arrange
			ICollection<JsonNode> collection = null;

			// Act
			new JsonArrayNode(collection);
		}

		[TestMethod]
		public void Create_List_Empty() {
			// Arrange
			var list = new List<JsonNode>();

			// Act
			var arrayNode = new JsonArrayNode(list);

			// Assert
			Assert.AreEqual(0, arrayNode.Count);
		}

		[TestMethod]
		public void Create_List_WitherIntegerNodes() {
			// Arrange
			var list = new List<JsonNode>();
			list.Add(new JsonIntegerNode(1));
			list.Add(new JsonIntegerNode(2));
			list.Add(new JsonIntegerNode(3));

			// Act
			var arrayNode = new JsonArrayNode(list);

			// Assert
			Assert.AreEqual(3, arrayNode.Count);
			Assert.AreEqual(1, (int)(arrayNode[0] as JsonIntegerNode).Value);
			Assert.AreEqual(2, (int)(arrayNode[1] as JsonIntegerNode).Value);
			Assert.AreEqual(3, (int)(arrayNode[2] as JsonIntegerNode).Value);
		}

		#endregion

		#region Clone()

		[TestMethod]
		public void Clone_Empty() {
			// Arrange
			var arrayNode = new JsonArrayNode();

			// Act
			var cloneNode = arrayNode.Clone() as JsonArrayNode;

			// Assert
			Assert.IsNotNull(cloneNode);
			Assert.AreNotSame(arrayNode, cloneNode);
			Assert.AreEqual(0, cloneNode.Count);
		}

		[TestMethod]
		public void Clone_NumberElements() {
			// Arrange
			var arrayNode = JsonArrayNode.FromArray(new int[] { 1, 2, 3, 4, 5 });

			// Act
			var cloneNode = arrayNode.Clone() as JsonArrayNode;

			// Assert
			Assert.IsNotNull(cloneNode);
			Assert.AreNotSame(arrayNode, cloneNode);
			Assert.AreEqual(5, cloneNode.Count);

			Assert.AreEqual(1, (int)(cloneNode[0] as JsonIntegerNode).Value);
			Assert.AreEqual(2, (int)(cloneNode[1] as JsonIntegerNode).Value);
			Assert.AreEqual(3, (int)(cloneNode[2] as JsonIntegerNode).Value);
			Assert.AreEqual(4, (int)(cloneNode[3] as JsonIntegerNode).Value);
			Assert.AreEqual(5, (int)(cloneNode[4] as JsonIntegerNode).Value);
		}

		[TestMethod]
		public void Clone_NumberElementsWithNullElement() {
			// Arrange
			var arrayNode = JsonArrayNode.FromArray(new object[] { 1, null, 5 });

			// Act
			var cloneNode = arrayNode.Clone() as JsonArrayNode;

			// Assert
			Assert.IsNotNull(cloneNode);
			Assert.AreNotSame(arrayNode, cloneNode);
			Assert.AreEqual(3, cloneNode.Count);

			Assert.AreEqual(1, (int)(cloneNode[0] as JsonIntegerNode).Value);
			Assert.IsNull(cloneNode[1]);
			Assert.AreEqual(5, (int)(cloneNode[2] as JsonIntegerNode).Value);
		}

		#endregion

		#region ToObject(Type)

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ToObject_Null() {
			// Arrange
			var arrayNode = new JsonArrayNode();
			Type type = null;

			// Act
			arrayNode.ToObject(type);
		}

		[TestMethod]
		public void ToObject_Array_IdentityCardArray() {
			// Arrange
			var arrayNode = JsonObjectGraphs.CreateIdentityCardArray();

			// Act
			PersonCard[] cards = arrayNode.ToObject<PersonCard[]>();

			// Assert
			Assert.IsNotNull(cards);
			Assert.AreEqual(2, cards.Length);
			Assert.AreEqual("Bob", cards[0].Name);
			Assert.AreEqual(22, cards[0].Age);
			Assert.AreEqual("Jessica", cards[1].Name);
			Assert.AreEqual(22, cards[1].Age);
		}

		[TestMethod]
		public void ToObject_List_IdentityCardArray() {
			// Arrange
			var arrayNode = JsonObjectGraphs.CreateIdentityCardArray();

			// Act
			List<PersonCard> cards = arrayNode.ToObject<List<PersonCard>>();

			// Assert
			Assert.IsNotNull(cards);
			Assert.AreEqual(2, cards.Count);
			Assert.AreEqual("Bob", cards[0].Name);
			Assert.AreEqual(22, cards[0].Age);
			Assert.AreEqual("Jessica", cards[1].Name);
			Assert.AreEqual(22, cards[1].Age);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ToObject_Int_IdentityCardArray() {
			// Arrange
			var arrayNode = JsonObjectGraphs.CreateIdentityCardArray();

			// Act
			int cards = arrayNode.ToObject<int>();
		}

		#endregion

		#region ToString()

		[TestMethod]
		public void ToString_Empty() {
			// Arrange
			var arrayNode = new JsonArrayNode();

			// Act
			string result = arrayNode.ToString();
			string expectedResult = "[]";

			// Assert
			Assert.AreEqual(expectedResult, result);
		}

		[TestMethod]
		[DeploymentItem("Json/TestObjects/Files/JsonArrayNode/ToString_StringArray.json")]
		public void ToString_StringArray() {
			// Arrange
			var arrayNode = JsonArrayNode.FromArray(new string[] { "A", "B" });

			// Act
			string result = arrayNode.ToString();
			string expectedResult = File.ReadAllText("ToString_StringArray.json");

			// Assert
			Assert.AreEqual(expectedResult, result);
		}

		#endregion

		#region WriteTo(JsonWriter)

		[TestMethod]
		public void WriteTo_Empty() {
			// Arrange
			var arrayNode = new JsonArrayNode();
			var writer = JsonWriter.Create();

			// Act
			arrayNode.WriteTo(writer);

			// Assert
			string result = writer.ToString();
			string expectedResult = "[]";

			Assert.AreEqual(expectedResult, result);
		}

		[TestMethod]
		[DeploymentItem("Json/TestObjects/Files/JsonArrayNode/ToString_StringArray.json")]
		public void WriteTo_StringArray() {
			// Arrange
			var arrayNode = JsonArrayNode.FromArray(new string[] { "A", "B" });
			var writer = JsonWriter.Create();

			// Act
			arrayNode.WriteTo(writer);

			// Assert
			string result = writer.ToString();
			string expectedResult = File.ReadAllText("ToString_StringArray.json");

			Assert.AreEqual(expectedResult, result);
		}

		#endregion

	}

}
