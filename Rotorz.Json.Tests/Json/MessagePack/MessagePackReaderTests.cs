// Copyright (c) Rotorz Limited. All rights reserved.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rotorz.Json.MessagePack.Tests {

	[TestClass]
	public class MessagePackReaderTests {

		[TestMethod]
		[DeploymentItem("Json/TestObjects/Files/MessagePack/cases.json")]
		[DeploymentItem("Json/TestObjects/Files/MessagePack/cases.mpac")]
		public void Read_Stream_MessagePack_Cases() {
			Read_Stream_MessagePack_Parameterized("cases.json", "cases.mpac");
		}

		[TestMethod]
		[DeploymentItem("Json/TestObjects/Files/MessagePack/cases.json")]
		[DeploymentItem("Json/TestObjects/Files/MessagePack/cases_compact.mpac")]
		public void Read_Stream_MessagePack_CasesCompact() {
			Read_Stream_MessagePack_Parameterized("cases.json", "cases_compact.mpac");
		}

		[TestMethod]
		[DeploymentItem("Json/TestObjects/Files/MessagePack/binary_cases.json")]
		[DeploymentItem("Json/TestObjects/Files/MessagePack/binary_cases.mpac")]
		public void Read_Stream_MessagePack_BinaryCases() {
			Read_Stream_MessagePack_Parameterized("binary_cases.json", "binary_cases.mpac");
		}

		private void Read_Stream_MessagePack_Parameterized(string jsonFileName, string mpacFileName) {
			// Arrange
			using (var casesJsonStream = new FileStream(jsonFileName, FileMode.Open, FileAccess.Read))
			using (var casesMpacStream = new FileStream(mpacFileName, FileMode.Open, FileAccess.Read)) {
				var messagePackReader = MessagePackReader.Create(casesMpacStream);

				// Act
				var result = new JsonArrayNode();
				while (true) {
					try {
						result.Add(messagePackReader.ReadNext());
					}
					catch (EndOfStreamException) {
						break;
					}
				}

				// Assert
				var expectedResult = JsonObjectNode.ReadFrom(casesJsonStream);
				Assert.IsNotNull(result);
				AssertAreEqual(expectedResult, result);
			}
		}

		private static void AssertAreEqual(JsonNode expected, JsonNode actual) {
			if (expected == null) {
				Assert.IsNull(actual);
				return;
			}

			if (!(actual is MessagePackBinaryNode || actual is MessagePackExtendedNode))
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
			
			var binaryNode = actual as MessagePackBinaryNode;
			if (binaryNode != null) {
				var expectedNodeData = (expected as JsonArrayNode)
					.Select(e => (byte)(e as JsonIntegerNode).Value)
					.ToArray();
				AssertAreEqual(expectedNodeData, binaryNode.Value);
				return;
			}
			var extendedNode = actual as MessagePackExtendedNode;
			if (extendedNode != null) {
				var expectedExtendedNode = expected as JsonObjectNode;
				var expectedNodeData = (expectedExtendedNode["data"] as JsonArrayNode)
					.Select(e => (byte)(e as JsonIntegerNode).Value)
					.ToArray();
				Assert.AreEqual((expectedExtendedNode["type"] as JsonIntegerNode).Value, (int)extendedNode.ExtendedType);
				AssertAreEqual(expectedNodeData, extendedNode.Value);
				return;
			}

			throw new InternalTestFailureException();
		}

		private static void AssertAreEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual) {
			Assert.AreEqual(expected.Count(), actual.Count());

			var expectedEnumerator = expected.GetEnumerator();
			var actualEnumerator = actual.GetEnumerator();
			
			while (expectedEnumerator.MoveNext() && actualEnumerator.MoveNext())
				Assert.AreEqual(expectedEnumerator.Current, actualEnumerator.Current);
		}

	}

}
