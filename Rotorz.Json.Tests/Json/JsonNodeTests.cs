// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rotorz.Json.Tests.TestObjects;
using Rotorz.Tests;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Rotorz.Json.Tests
{
    [TestClass]
    public class JsonNodeTests
    {
        #region From(string)

        [TestMethod]
        public void From_EmptyString()
        {
            // Arrange
            string json = "";

            // Act
            var node = JsonNode.ReadFrom(json);

            // Assert
            Assert.IsNull(node);
        }

        [TestMethod]
        public void From_Null()
        {
            // Arrange
            string json = null;

            // Act
            var node = JsonNode.ReadFrom(json);

            // Assert
            Assert.IsNull(node);
        }

        [TestMethod]
        public void From_DecimalValue_DefaultCulture()
        {
            // Arrange
            string json = @"{""value"":1.23}";

            // Act
            var node = JsonNode.ReadFrom(json) as JsonObjectNode;

            // Assert
            Assert.IsNotNull(node);
            Assert.AreEqual(1.23, node["value"].ConvertTo<double>());
        }

        [TestMethod]
        public void From_DecimalValue_CultureWithDifferentDecimalSeparator()
        {
            CultureTestUtility.ExecuteInCulture("fr-FR", () => {
                // Arrange
                string json = @"{""value"":1.23}";

                // Act
                var node = JsonNode.ReadFrom(json) as JsonObjectNode;

                // Assert
                Assert.IsNotNull(node);
                Assert.AreEqual(1.23, node["value"].ConvertTo<double>());
            });
        }

        #endregion


        #region ConvertFrom(object)

        [TestMethod]
        public void ConvertFrom_int()
        {
            // Arrange
            int value = 42;

            // Act
            var node = JsonNode.ConvertFrom(value);

            // Assert
            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(JsonIntegerNode));
            Assert.AreEqual(value, (node as JsonIntegerNode).Value);
        }

        [TestMethod]
        public void ConvertFrom_uint()
        {
            // Arrange
            uint value = 42;

            // Act
            var node = JsonNode.ConvertFrom(value);

            // Assert
            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(JsonIntegerNode));
            Assert.AreEqual(value, (node as JsonIntegerNode).Value);
        }

        [TestMethod]
        public void ConvertFrom_long()
        {
            // Arrange
            long value = long.MaxValue;

            // Act
            var node = JsonNode.ConvertFrom(value);

            // Assert
            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(JsonIntegerNode));
            Assert.AreEqual(value, (node as JsonIntegerNode).Value);
        }

        [TestMethod]
        public void ConvertFrom_ulong()
        {
            // Arrange
            ulong value = ulong.MaxValue;

            // Act
            var node = JsonNode.ConvertFrom(value);

            // Assert
            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(JsonIntegerNode));
            Assert.AreEqual(value, (node as JsonIntegerNode).UnsignedValue);
        }

        [TestMethod]
        public void ConvertFrom_short()
        {
            // Arrange
            short value = short.MaxValue;

            // Act
            var node = JsonNode.ConvertFrom(value);

            // Assert
            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(JsonIntegerNode));
            Assert.AreEqual(value, (node as JsonIntegerNode).Value);
        }

        [TestMethod]
        public void ConvertFrom_ushort()
        {
            // Arrange
            ushort value = ushort.MaxValue;

            // Act
            var node = JsonNode.ConvertFrom(value);

            // Assert
            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(JsonIntegerNode));
            Assert.AreEqual(value, (node as JsonIntegerNode).Value);
        }

        [TestMethod]
        public void ConvertFrom_byte()
        {
            // Arrange
            byte value = byte.MaxValue;

            // Act
            var node = JsonNode.ConvertFrom(value);

            // Assert
            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(JsonIntegerNode));
            Assert.AreEqual(value, (node as JsonIntegerNode).Value);
        }

        [TestMethod]
        public void ConvertFrom_sbyte()
        {
            // Arrange
            sbyte value = sbyte.MaxValue;

            // Act
            var node = JsonNode.ConvertFrom(value);

            // Assert
            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(JsonIntegerNode));
            Assert.AreEqual(value, (node as JsonIntegerNode).Value);
        }

        [TestMethod]
        public void ConvertFrom_char()
        {
            // Arrange
            char value = char.MaxValue;

            // Act
            var node = JsonNode.ConvertFrom(value);

            // Assert
            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(JsonIntegerNode));
            Assert.AreEqual(value, (node as JsonIntegerNode).Value);
        }

        [TestMethod]
        public void ConvertFrom_string()
        {
            // Arrange
            string value = "Hello World!";

            // Act
            var node = JsonNode.ConvertFrom(value);

            // Assert
            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(JsonStringNode));
            Assert.AreEqual(value, (node as JsonStringNode).Value);
        }

        [TestMethod]
        public void ConvertFrom_string_array()
        {
            // Arrange
            string[] value = { "A", "B", "C" };

            // Act
            var arrayNode = JsonNode.ConvertFrom(value) as JsonArrayNode;

            // Assert
            Assert.IsNotNull(arrayNode);
            Assert.AreEqual(3, arrayNode.Count);
            Assert.AreEqual("A", (arrayNode[0] as JsonStringNode).Value);
            Assert.AreEqual("B", (arrayNode[1] as JsonStringNode).Value);
            Assert.AreEqual("C", (arrayNode[2] as JsonStringNode).Value);
        }

        [TestMethod]
        public void ConvertFrom_string_list()
        {
            // Arrange
            var value = new List<string>(new string[] { "A", "B", "C" });

            // Act
            var arrayNode = JsonNode.ConvertFrom(value) as JsonArrayNode;
            Assert.IsNotNull(arrayNode);
            Assert.AreEqual(3, arrayNode.Count);
            Assert.AreEqual("A", (arrayNode[0] as JsonStringNode).Value);
            Assert.AreEqual("B", (arrayNode[1] as JsonStringNode).Value);
            Assert.AreEqual("C", (arrayNode[2] as JsonStringNode).Value);
        }

        [TestMethod]
        public void ConvertFrom_string_dictionary()
        {
            // Arrange
            var value = new Dictionary<string, int>();
            value["A"] = 3;
            value["B"] = 2;
            value["C"] = 1;

            // Act
            var objectNode = JsonNode.ConvertFrom(value) as JsonObjectNode;

            // Assert
            Assert.IsNotNull(objectNode);
            Assert.AreEqual(3, objectNode.Count);
            Assert.AreEqual(3, (objectNode["A"] as JsonIntegerNode).Value);
            Assert.AreEqual(2, (objectNode["B"] as JsonIntegerNode).Value);
            Assert.AreEqual(1, (objectNode["C"] as JsonIntegerNode).Value);
        }

        [TestMethod]
        public void ConvertFrom_string_object()
        {
            // Arrange
            var value = new PersonCard() {
                Name = "Sabrina Styles",
                Age = 42,
                Friends = 872
            };

            // Act
            var objectNode = JsonNode.ConvertFrom(value) as JsonObjectNode;

            // Assert
            Assert.IsNotNull(objectNode);
            Assert.AreEqual(3, objectNode.Count);
            Assert.AreEqual("Sabrina Styles", (objectNode["Name"] as JsonStringNode).Value);
            Assert.AreEqual(42, (objectNode["Age"] as JsonIntegerNode).Value);
            Assert.AreEqual(872, (objectNode["Friends"] as JsonIntegerNode).Value);
        }

        #endregion


        #region WriteTo(JsonWriter)

        [TestMethod]
        [DeploymentItem("Json/TestObjects/Files/JsonObjectGraphs/SimpleObject_WithSpaces.json")]
        public void WriteTo_ManualSetupViaStringBuilder()
        {
            // Arrange
            JsonObjectNode simpleObjectNode = JsonObjectGraphs.CreateSimpleObject();

            // Act
            var stringWriter = new StringWriter(new StringBuilder());
            var settings = new JsonWriterSettings();
            settings.IndentChars = "   ";
            var writer = JsonWriter.Create(stringWriter, settings);

            simpleObjectNode.Write(writer);
            string result = writer.ToString();

            // Assert
            string expectedResult = File.ReadAllText("SimpleObject_WithSpaces.json");
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        [DeploymentItem("Json/TestObjects/Files/JsonObjectGraphs/SimpleObject_WithSpaces.json")]
        public void WriteTo_ManualSetup()
        {
            // Arrange
            JsonObjectNode simpleObjectNode = JsonObjectGraphs.CreateSimpleObject();

            // Act
            var settings = new JsonWriterSettings();
            settings.IndentChars = "   ";
            var writer = JsonWriter.Create(settings);

            simpleObjectNode.Write(writer);
            string result = writer.ToString();

            // Assert
            string expectedResult = File.ReadAllText("SimpleObject_WithSpaces.json");
            Assert.AreEqual(expectedResult, result);
        }

        #endregion
    }
}
