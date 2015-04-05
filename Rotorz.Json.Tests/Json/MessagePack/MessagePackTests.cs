// Copyright (c) Rotorz Limited. All rights reserved.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rotorz.Json.MessagePack;
using System.IO;

namespace Rotorz.Json.Serialization.Tests {

	[TestClass]
	public class MessagePackTests {

		[TestMethod]
		[DeploymentItem("Json/TestObjects/Files/MessagePack/cases.json")]
		[DeploymentItem("Json/TestObjects/Files/MessagePack/cases.mpac")]
		public void Parse_Stream_MessagePack_Cases() {
			Parse_Stream_MessagePack_Parameterized("cases.mpac");
		}

		[TestMethod]
		[DeploymentItem("Json/TestObjects/Files/MessagePack/cases.json")]
		[DeploymentItem("Json/TestObjects/Files/MessagePack/cases_compact.mpac")]
		public void Parse_Stream_MessagePack_CasesCompact() {
			Parse_Stream_MessagePack_Parameterized("cases_compact.mpac");
		}

		private void Parse_Stream_MessagePack_Parameterized(string mpacFileName) {
			// Arrange
			using (var casesJsonStream = new FileStream("cases.json", FileMode.Open, FileAccess.Read))
			using (var casesMpacStream = new FileStream(mpacFileName, FileMode.Open, FileAccess.Read)) {
				// Arrange
				var mpacReader = MessagePackReader.Create(casesMpacStream);

				// Act
				var result = mpacReader.Parse();

				// Assert
				var expectedResult = JsonObjectNode.FromStream(casesJsonStream);
				Assert.IsNotNull(result);
				AssertAreEqual(expectedResult, result);
			}
		}

		private static void AssertAreEqual(JsonNode expected, JsonNode actual) {
			if (expected == null) {
				Assert.IsNull(actual);
				return;
			}

			Assert.AreEqual(expected.GetType(), actual.GetType());

			var booleanNode = actual as JsonBooleanNode;
			if (booleanNode != null) {
				Assert.AreEqual((expected as JsonBooleanNode).Value, booleanNode.Value);
				return;
			}
			var integerNode = actual as JsonIntegerNode;
			if (integerNode != null) {
				Assert.AreEqual((expected as JsonIntegerNode).Value, integerNode.Value);
				return;
			}
			var doubleNode = actual as JsonDoubleNode;
			if (doubleNode != null) {
				Assert.AreEqual((expected as JsonDoubleNode).Value, doubleNode.Value);
				return;
			}
			var stringNode = actual as JsonStringNode;
			if (stringNode != null) {
				Assert.AreEqual((expected as JsonStringNode).Value, stringNode.Value);
				return;
			}
			var arrayNode = actual as JsonArrayNode;
			if (arrayNode != null) {
				var expectedArrayNode = expected as JsonArrayNode;
				Assert.AreEqual(expectedArrayNode.Count, arrayNode.Count);
				for (int i = 0; i < arrayNode.Count; ++i)
					AssertAreEqual(expectedArrayNode[i], arrayNode[i]);
				return;
			}
			var objectNode = actual as JsonObjectNode;
			if (objectNode != null) {
				var expectedObjectNode = expected as JsonObjectNode;
				Assert.AreEqual(expectedObjectNode.Count, objectNode.Count);
				foreach (var pair in objectNode) {
					Assert.IsTrue(expectedObjectNode.ContainsKey(pair.Key));
					AssertAreEqual(expectedObjectNode[pair.Key], pair.Value);
				}
				return;
			}

			throw new InternalTestFailureException();
		}

	}

}
