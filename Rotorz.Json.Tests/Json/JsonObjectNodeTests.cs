// Copyright (c) 2014 Rotorz Limited. All rights reserved.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace Rotorz.Json.Tests {

	[TestClass]
	public class JsonObjectNodeTests {

		#region Factory: FromDictionary(IDictionary<string, TValue>)

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void FromDictionary_Null() {
			// Arrange
			Dictionary<string, int> dictionary = null;

			// Act
			JsonObjectNode.FromDictionary(dictionary);
		}

		[TestMethod]
		public void FromDictionary_Empty() {
			// Arrange
			var dictionary = new Dictionary<string, int>();

			// Act
			var objectNode = JsonObjectNode.FromDictionary(dictionary);

			// Assert
			Assert.IsNotNull(objectNode);
			Assert.AreEqual(0, objectNode.Count);
		}

		[TestMethod]
		public void FromDictionary_Properties() {
			// Arrange
			var dictionary = new Dictionary<string, int>();
			dictionary["A"] = 1;
			dictionary["B"] = 2;
			dictionary["C"] = 4;

			// Act
			var objectNode = JsonObjectNode.FromDictionary(dictionary);

			// Assert
			Assert.IsNotNull(objectNode);
			Assert.AreEqual(3, objectNode.Count);

			var nodeA = objectNode["A"] as JsonIntegerNode;
			var nodeB = objectNode["B"] as JsonIntegerNode;
			var nodeC = objectNode["C"] as JsonIntegerNode;

			Assert.AreEqual(1, (int)nodeA.Value);
			Assert.AreEqual(2, (int)nodeB.Value);
			Assert.AreEqual(4, (int)nodeC.Value);
		}

		#endregion

		#region ToString()

		[TestMethod]
		[DeploymentItem("Json/TestObjects/Files/JsonObjectNode/ToString_KeysWithEscapeSequences.json")]
		public void ToString_KeysWithEscapeSequences() {
			// Arrange
			var node = new JsonObjectNode();
			node["Simple Key"] = null;
			node["Hello\nWorld!"] = null;

			// Act
			string result = node.ToString();

			// Assert
			string expectedResult = File.ReadAllText("ToString_KeysWithEscapeSequences.json");
			Assert.AreEqual(expectedResult, result);
		}

		#endregion

	}

}
