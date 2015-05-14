// Copyright (c) Rotorz Limited. All rights reserved.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rotorz.Json.Tests.TestObjects;
using System.IO;

namespace Rotorz.Json.Tests {

	[TestClass]
	public class JsonUtilityTests {

		#region ToString(JsonWriterSettings)

		[TestMethod]
		[DeploymentItem("Json/TestObjects/Files/JsonObjectGraphs/SimpleObject.json")]
		public void ToString_IndentWithTabs() {
			// Arrange
			JsonObjectNode simpleObjectNode = JsonObjectGraphs.CreateSimpleObject();

			// Act
			string result = JsonUtility.ToJsonString(simpleObjectNode, new JsonWriterSettings());

			// Assert
			string expectedResult = File.ReadAllText("SimpleObject.json");
			Assert.AreEqual(expectedResult, result);
		}

		[TestMethod]
		[DeploymentItem("Json/TestObjects/Files/JsonObjectGraphs/SimpleObject_WithoutTabs.json")]
		public void ToString_IndentWithoutTabs() {
			// Arrange
			JsonObjectNode simpleObjectNode = JsonObjectGraphs.CreateSimpleObject();

			// Act
			var settings = new JsonWriterSettings();
			settings.Indent = false;

			string result = JsonUtility.ToJsonString(simpleObjectNode, settings);

			// Assert
			string expectedResult = File.ReadAllText("SimpleObject_WithoutTabs.json");
			Assert.AreEqual(expectedResult, result);
		}

		[TestMethod]
		[DeploymentItem("Json/TestObjects/Files/JsonObjectGraphs/SimpleObject_WithSpaces.json")]
		public void ToString_IndentWithSpaces() {
			// Arrange
			JsonObjectNode simpleObjectNode = JsonObjectGraphs.CreateSimpleObject();

			// Act
			var settings = new JsonWriterSettings();
			settings.IndentChars = "   ";
			string result = JsonUtility.ToJsonString(simpleObjectNode, settings);

			// Assert
			string expectedResult = File.ReadAllText("SimpleObject_WithSpaces.json");
			Assert.AreEqual(expectedResult, result);
		}

		#endregion

	}

}
